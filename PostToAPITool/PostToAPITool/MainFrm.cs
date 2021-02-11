namespace PostToAPITool
{
    using Newtonsoft.Json;
    using PostToAPITool.Configuration;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    public partial class MainFrm : Form
    {
        private const string BEGIN_PAYLOAD_MARK = "--BEGIN_PAYLOAD";
        private const string END_PAYLOAD_MARK = "--END_PAYLOAD";
        private const string STATUS_CODE_PREFIX = "Status Code:";
        private const string TOKEN_ACQUIRED_TIME_PREFIX = "Token acquired time:";
        private const string BYTES_RECEIVED_PREFIX = "Bytes received:";

        private readonly HttpClient httpClient;
        private readonly string configFilePath;

        private Config config;
        private AuthenticationHeaderValue tokenHeader;
        private DateTime tokenAcquiredTime;
        private bool keyDown;
        private Keys pressedKey;

        private string response;


        public MainFrm(string configFilePath)
        {
            this.configFilePath = configFilePath;

            InitializeComponent();

            this.PayloadTxt.MouseWheel += this.PayloadTxt_MouseWheel;

            this.EnableControls(false);

            bool validConfig = this.LoadConfigFile(this.configFilePath);
            if (!validConfig)
            {
                return;
            }

            string configFile = Path.GetFileName(configFilePath);

            if (config != null)
            {
                this.ApplyConfig();

                this.httpClient = new HttpClient
                {
                    Timeout = Timeout.InfiniteTimeSpan
                };

                _ = this.UpdateTokenHeaderIfNecessary(
                    setControlsEnabled: true, focusPayloadTxtControl: true);
            }

            string grant = null;

            if(!string.IsNullOrWhiteSpace(config.ClientID))
            {
                grant = "Client/Secret";
            }
            else if(!string.IsNullOrWhiteSpace(config.Username))
            {
                grant = $"Username/Password ({config.Username})";
            }

            this.Text = $"{Program.PROGRAM_TITLE}    [config: {configFile}]    |    {config?.Scope}   |   {grant}";
        }

        private void ApplyConfig()
        {
            if (this.config.MainFrmWidth >= this.MinimumSize.Width)
            {
                this.Width = this.config.MainFrmWidth;
            }

            if (this.config.MainFrmHeight >= this.MinimumSize.Height)
            {
                this.Height = this.config.MainFrmHeight;
            }

            this.HostCmb.Items.AddRange(this.config.Hosts);
            this.HostCmb.SelectedIndex = 0;

            if (this.HostCmb.Items.Contains(this.config.SelectedHost ?? ""))
            {
                this.HostCmb.SelectedItem = this.config.SelectedHost;
            }

            IEnumerable<string> controllers =
                this.config.Controllers.Select(c => c.Path);
            this.ControllerCmb.Items.AddRange(controllers.ToArray());
            this.ControllerCmb.SelectedIndex = 0;

            if (this.ControllerCmb.Items.Contains(this.config.SelectedController ?? ""))
            {
                this.ControllerCmb.SelectedItem = this.config.SelectedController;
            }

            if (this.EndpointCmb.Items.Contains(this.config.SelectedEndpoint ?? ""))
            {
                this.EndpointCmb.SelectedItem = this.config.SelectedEndpoint;
            }

            this.AutoOpenResponseChk.Checked = this.config.AutoOpenResponseViewer;

            if (this.config.MainFrmFontSize > 0)
            {
                this.SetFontSize(this.config.MainFrmFontSize);
            }

            try
            {
                var backColor = (Color)new ColorConverter().ConvertFromString(this.config.BackColor);
                this.PayloadTxt.BackColor = backColor;
            }
            catch
            {
                this.config.BackColor = "#FFFFFF";
            }
            try
            {
                var foreColor = (Color)new ColorConverter().ConvertFromString(this.config.TextColor);
                this.PayloadTxt.ForeColor = foreColor;
            }
            catch
            {
                this.config.TextColor = "#000000";
            }

            this.AutoFormatChk.Checked = this.config.AutoFormat;
        }

        private bool LoadConfigFile(string path)
        {
            try
            {
                using (StreamReader file = File.OpenText(path))
                {
                    var serializer = new JsonSerializer();
                    this.config = (Config)serializer.Deserialize(file, typeof(Config));

                    bool configIsValid = this.ValidateConfig();

                    return configIsValid;
                }
            }
            catch (Exception ex)
            {
                string message = "Deserializing config failed." +
                    Environment.NewLine + Environment.NewLine + ex.Message;

                MessageBox.Show(message, Program.PROGRAM_TITLE,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return false;
        }

        private bool ValidateConfig()
        {
            if (string.IsNullOrWhiteSpace(this.config.TenantID))
            {
                return this.ShowErrorMessageAndReturnFalse(
                    "No TenantID found in config.");
            }

            if (string.IsNullOrWhiteSpace(this.config.ClientID) &&
                string.IsNullOrWhiteSpace(this.config.Username))
            {
                return this.ShowErrorMessageAndReturnFalse(
                    "No ClientID & no Username found in config.");
            }
            if (!string.IsNullOrWhiteSpace(this.config.ClientID) &&
                string.IsNullOrWhiteSpace(this.config.Secret))
            {
                return this.ShowErrorMessageAndReturnFalse(
                    "No Secret found in config.");
            }
            if (!string.IsNullOrWhiteSpace(this.config.Username) &&
                string.IsNullOrWhiteSpace(this.config.Password))
            {
                return this.ShowErrorMessageAndReturnFalse(
                    "No Password found in config.");
            }

            if (string.IsNullOrWhiteSpace(this.config.Scope))
            {
                return this.ShowErrorMessageAndReturnFalse(
                    "No Scope found in config.");
            }
            if (config.Hosts?.Any() != true)
            {
                return this.ShowErrorMessageAndReturnFalse(
                    "No hosts found in config.");
            }
            if (config.Controllers?.Any() != true)
            {
                return this.ShowErrorMessageAndReturnFalse(
                    "No controllers found in config.");
            }
            foreach (var controller in this.config.Controllers)
            {
                if (string.IsNullOrWhiteSpace(controller.Path))
                {
                    return this.ShowErrorMessageAndReturnFalse(
                        "Controller without path found in config.");
                }
                if (controller.Endpoints?.Any() != true)
                {
                    return this.ShowErrorMessageAndReturnFalse(
                        $"No endpoints for controller '{controller.Path}' found in config.");
                }
                foreach (var endpoint in controller.Endpoints)
                {
                    if (string.IsNullOrWhiteSpace(endpoint))
                    {
                        return this.ShowErrorMessageAndReturnFalse(
                            $"Blank endpoint for controller '{controller.Path}' found in config.");
                    }
                }
            }

            return true;
        }

        private bool ShowErrorMessageAndReturnFalse(string errorMessage)
        {
            MessageBox.Show(errorMessage, Program.PROGRAM_TITLE,
                MessageBoxButtons.OK, MessageBoxIcon.Error);

            return false;
        }

        private void EnableControls(bool enable)
        {
            this.HostCmb.Enabled = enable;
            this.ControllerCmb.Enabled = enable;
            this.EndpointCmb.Enabled = enable;
            this.PayloadTxt.Enabled = enable;
            this.CallAPIBtn.Enabled = enable;
        }

        private async void CallAPIBtn_Click(object sender, EventArgs e)
        {
            this.response = string.Empty;
            Uri uri = null;
            try
            {
                this.StatusStrip.BackColor = SystemColors.Control;
                this.EnableControls(false);
                this.StatusCodeLbl.Text = STATUS_CODE_PREFIX;
                this.BytesReceivedLbl.Text = BYTES_RECEIVED_PREFIX;
                this.ToolStripProgressBar.Style = ProgressBarStyle.Marquee;
                this.ToolStripProgressBar.MarqueeAnimationSpeed = 50;

                string txt = this.PreparePayload(this.PayloadTxt.Text);

                if (this.AutoFormatChk.Checked)
                {
                    txt = JsonHelper.FormatIfJson(txt);
                }

                this.PayloadTxt.Text = txt;

                string path = $"{this.ControllerCmb.SelectedItem}/{this.EndpointCmb.SelectedItem}";
                uri = new Uri(new Uri($"{this.HostCmb.SelectedItem}"), path);
                string message = txt;

                await this.UpdateTokenHeaderIfNecessary(false);

                this.SetStatus($"Calling: {uri}");
                HttpResponseMessage response = await Post(message, uri, this.tokenHeader);
                this.SetStatus($"Response received from: {uri}");

                this.StatusCodeLbl.Text = $"{STATUS_CODE_PREFIX} {response.StatusCode}";

                this.response = await response.Content.ReadAsStringAsync();

                this.BytesReceivedLbl.Text = $"{BYTES_RECEIVED_PREFIX} {this.response.Length}";

                this.StatusStrip.BackColor = response.IsSuccessStatusCode
                    ? Color.FromArgb(196, 255, 196)
                    : Color.FromArgb(255, 255, 196);
            }
            catch (Exception ex)
            {
                this.StatusStrip.BackColor = Color.LightPink;

                if (ex.InnerException is WebException webException)
                {
                    switch (webException.Status)
                    {
                        case WebExceptionStatus.ConnectFailure:
                        case WebExceptionStatus.ReceiveFailure:
                        case WebExceptionStatus.ConnectionClosed:
                        case WebExceptionStatus.KeepAliveFailure:
                            {
                                var inner = this.GetMostInnerException(ex);

                                this.SetStatus("EXCEPTION: " + inner.Message);

                                return;
                            }
                    }
                }

                using (var pop = new TextViewerFrm("Exception Viewer", this.config, ex.ToString()))
                {
                    pop.ShowDialog();
                }
                return;
            }
            finally
            {
                this.EnableControls(true);
                this.ToolStripProgressBar.Style = ProgressBarStyle.Blocks;
            }

            if (this.AutoOpenResponseChk.Checked && !string.IsNullOrEmpty(this.response))
            {
                using (var pop = new TextViewerFrm("Response Viewer", this.config, this.response))
                {
                    pop.ShowDialog();
                }
            }
        }

        private Exception GetMostInnerException(Exception ex)
        {
            Exception result = ex;

            if (ex.InnerException != null)
            {
                result = this.GetMostInnerException(ex.InnerException);
            }

            return result;
        }

        private string PreparePayload(string payload)
        {
            int start = payload.IndexOf(BEGIN_PAYLOAD_MARK);
            int end = payload.IndexOf(END_PAYLOAD_MARK);

            if ((start != -1) && (end != -1) && (start < end))
            {
                int from = start + BEGIN_PAYLOAD_MARK.Length;
                int length = end - from;

                payload = payload.Substring(from, length);

                payload = payload.Trim('\n', '\r');
            }

            return payload;
        }

        private void ControllerCmb_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedControler = (string)this.ControllerCmb.SelectedItem;

            Controller controller = this.config.Controllers
                .FirstOrDefault(c => c.Path == selectedControler);

            if (controller != null)
            {
                this.EndpointCmb.Items.Clear();
                this.EndpointCmb.Items.AddRange(controller.Endpoints.ToArray());
                this.EndpointCmb.SelectedIndex = 0;
            }
        }

        private async Task UpdateTokenHeaderIfNecessary(
            bool setControlsEnabled, bool focusPayloadTxtControl = false)
        {
            bool renewToken = true;

            if (this.tokenAcquiredTime != default)
            {
                TimeSpan age = DateTime.Now.Subtract(this.tokenAcquiredTime);
                renewToken = (age > TimeSpan.FromMinutes(42));
            }

            if ((config != null) && renewToken)
            {
                string clientId = this.config.ClientID;
                string secret = this.config.Secret;
                string tenantId = this.config.TenantID;
                string scope = $"{this.config.Scope}/.default";

                try
                {
                    if ((clientId != null) && (secret != null))
                    {
                        await this.GetAuthenticationHeader(
                            clientId, secret, tenantId, scope);
                    }
                    else
                    {
                        await this.GetAuthenticationHeader(
                            this.config.Scope,
                            "51f81489-12ee-4a9e-aaae-a2591f45987d",
                            this.config.Username,
                            this.config.Password,
                            tenantId
                            );
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An Exception was thrown while acquiring the token.", Program.PROGRAM_TITLE,
                        MessageBoxButtons.OK, MessageBoxIcon.Error);

                    using (var pop = new TextViewerFrm("Exception Viewer", this.config, ex.ToString()))
                    {
                        pop.ShowDialog();

                        return;
                    }
                }
            }

            if (setControlsEnabled)
            {
                this.EnableControls(true);
            }

            if (focusPayloadTxtControl)
            {
                this.PayloadTxt.Focus();
            }
        }

        private async Task GetAuthenticationHeader(
            string clientId, string secret, string tenantId, string scope)
        {
            this.SetStatus("Acquiring client/secret token...");
            string loginUrl = $"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/token";

            string postData = $"client_id={clientId}" +
                $"&client_info=1" +
                $"&client_secret={secret}" +
                $"&scope={scope}" +
                $"&grant_type=client_credentials";

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, loginUrl);
            request.Content = new StringContent(postData, Encoding.UTF8);
            request.Content.Headers.Remove("Content-Type");
            request.Content.Headers.TryAddWithoutValidation("Content-Type", $"application/x-www-form-urlencoded");

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            HttpResponseMessage responseMessage = await this.httpClient.SendAsync(request);
            string jsonResponse = await responseMessage.Content.ReadAsStringAsync();

            var jsonContent = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonResponse);

            string token = jsonContent["access_token"];

            var header = new AuthenticationHeaderValue("Bearer", token);

            this.tokenHeader = header;
            this.tokenAcquiredTime = DateTime.Now;
            this.SetStatus("Client/secret token acquired");
            this.TokenAcquireTimeLbl.Text = $"{TOKEN_ACQUIRED_TIME_PREFIX} {DateTime.Now:HH:mm:ss}";
        }

        private async Task GetAuthenticationHeader(
            string resource, string clientId, string username, string password, string tenantId)
        {
            this.SetStatus("Acquiring user/pass token...");
            string loginUrl = $"https://login.microsoftonline.com/{tenantId}/oauth2/token";

            string[] data = new[]
            {
                $"resource={resource}",
                $"client_id={clientId}",
                $"grant_type=password",
                $"username={username}",
                $"password={password}",
                $"scope=openid"
            };

            string postData = string.Join("&", data);

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, loginUrl);
            request.Content = new StringContent(postData, Encoding.UTF8);
            request.Content.Headers.Remove("Content-Type");
            request.Content.Headers.TryAddWithoutValidation("Content-Type", $"application/x-www-form-urlencoded");

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            HttpResponseMessage responseMessage = await this.httpClient.SendAsync(request);
            string jsonResponse = await responseMessage.Content.ReadAsStringAsync();

            var jsonContent = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonResponse);

            string token = jsonContent["access_token"];

            var header = new AuthenticationHeaderValue("Bearer", token);

            this.tokenHeader = header;
            this.tokenAcquiredTime = DateTime.Now;
            this.SetStatus("User/pass token acquired");
            this.TokenAcquireTimeLbl.Text = $"{TOKEN_ACQUIRED_TIME_PREFIX} {DateTime.Now:HH:mm:ss}";
        }

        private void SetStatus(string status)
        {
            this.ToolStripStatusLabel.Text = $"[{DateTime.Now:HH:mm:ss}] {status}";
        }

        private async Task<HttpResponseMessage> Post(string json,
            Uri url, AuthenticationHeaderValue authHeader)
        {
            httpClient.DefaultRequestHeaders.Authorization = authHeader;

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage result = await this.httpClient.PostAsync(url, content);

            return result;
        }

        private void MainFrm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.SaveConfig();
        }

        private void SaveConfig()
        {
            if (this.config != null)
            {
                this.config.SelectedHost = $"{this.HostCmb.SelectedItem}";
                this.config.SelectedController = $"{this.ControllerCmb.SelectedItem}";
                this.config.SelectedEndpoint = $"{this.EndpointCmb.SelectedItem}";
                this.config.AutoOpenResponseViewer = this.AutoOpenResponseChk.Checked;
                this.config.AutoFormat = this.AutoFormatChk.Checked;

                if (this.WindowState != FormWindowState.Maximized)
                {
                    this.config.MainFrmWidth = this.Width;
                    this.config.MainFrmHeight = this.Height;
                }

                string json = JsonConvert.SerializeObject(
                    this.config, Formatting.Indented);

                File.WriteAllText(this.configFilePath, json);
            }
        }

        private void ShowTokenLbl_Click(object sender, EventArgs e)
        {
            if (this.tokenHeader != null)
            {
                string token = this.tokenHeader.ToString();

                using (var pop = new TextViewerFrm("Token Viewer", this.config, token))
                {
                    pop.ShowDialog();
                }
            }
        }

        private void PayloadTxt_KeyDown(object sender, KeyEventArgs e)
        {
            this.keyDown = true;

            this.pressedKey = e.KeyCode;
        }

        private void PayloadTxt_KeyUp(object sender, KeyEventArgs e)
        {
            this.keyDown = false;
        }

        private void PayloadTxt_MouseWheel(object sender, MouseEventArgs e)
        {
            if (this.keyDown && (this.pressedKey == Keys.ControlKey))
            {
                if (e.Delta < 0)
                {
                    if (this.config.MainFrmFontSize > 1)
                    {
                        this.SetFontSize(--this.config.MainFrmFontSize);
                    }
                }
                else
                {
                    if (this.config.MainFrmFontSize < 25)
                    {
                        this.SetFontSize(++this.config.MainFrmFontSize);
                    }
                }
            }
        }

        private void SetFontSize(int size)
        {
            this.config.MainFrmFontSize = size;

            this.PayloadTxt.Font = new Font(
                this.PayloadTxt.Font.FontFamily, this.config.MainFrmFontSize);
        }

        private void ShowResponeLbl_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.response))
            {
                using (var pop = new TextViewerFrm("Response Viewer", this.config, this.response))
                {
                    pop.ShowDialog();
                }
            }
        }

        private void PayloadTxt_Enter(object sender, EventArgs e)
        {
            this.StatusStrip.BackColor = SystemColors.Control;
        }

        private void AutoFormatChk_CheckedChanged(object sender, EventArgs e)
        {
            this.SaveConfig();
        }
    }
}
