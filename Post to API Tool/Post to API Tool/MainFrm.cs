namespace Post_to_API_Tool
{
    using Newtonsoft.Json;
    using Post_to_API_Tool.Configuration;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    public partial class MainFrm : Form
    {
        private const string BEGIN_PAYLOAD_MARK = "--BEGIN_PAYLOAD";
        private const string END_PAYLOAD_MARK = "--END_PAYLOAD";
        private const string STATUS_CODE_PREFIX = "Status Code:";

        private readonly HttpClient httpClient;
        private readonly string configFilePath;
        private Config config;
        private AuthenticationHeaderValue tokenHeader;
        private DateTime tokenAquiredTime;


        public MainFrm(string configFilePath)
        {
            this.configFilePath = configFilePath;

            InitializeComponent();

            this.EnableControls(false);

            try
            {
                bool cont = this.LoadConfigFile(configFilePath);
                if (!cont)
                {
                    this.Close();
                }

                if (config != null)
                {
                    if (this.config.MainFrmWidth >= this.MinimumSize.Width)
                    {
                        this.Width = this.config.MainFrmWidth;
                    }
                    if (this.config.MainFrmHeight >= this.MinimumSize.Height)
                    {
                        this.Height = this.config.MainFrmHeight;
                    }

                    var controllers = config.Controllers.Select(c => c.Path);
                    this.ControllerCmb.Items.AddRange(controllers.ToArray());
                    this.ControllerCmb.SelectedIndex = 0;

                    if (this.ControllerCmb.Items.Contains(this.config.SelectedController ?? ""))
                    {
                        this.ControllerCmb.SelectedItem = this.config.SelectedController;
                    }
                    if(this.EndpointCmb.Items.Contains(this.config.SelectedEndpoint ?? ""))
                    {
                        this.EndpointCmb.SelectedItem = this.config.SelectedEndpoint;
                    }
                }
                this.httpClient = new HttpClient();

                _ = this.UpdateTokenHeader();
            }
            finally 
            {
                this.EnableControls(true);
            }
        }

        private bool LoadConfigFile(string path)
        {
            try
            {
                using (StreamReader file = File.OpenText(path))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    this.config = (Config)serializer.Deserialize(file, typeof(Config));
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Deserializing config failed {ex.Message}",
                    "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return false;
        }

        private void EnableControls(bool enable)
        {
            this.ControllerCmb.Enabled = enable;
            this.EndpointCmb.Enabled = enable;
            this.PayloadTxt.Enabled = enable;
            this.CallAPIBtn.Enabled = enable;
        }

        private async void CallAPIBtn_Click(object sender, EventArgs e)
        {
            string content;
            try
            {
                this.EnableControls(false);
                this.StatusCodeLbl.Text = STATUS_CODE_PREFIX;


                string txt = this.PayloadTxt.Text;

                int start = txt.IndexOf(BEGIN_PAYLOAD_MARK);
                int end = txt.IndexOf(END_PAYLOAD_MARK);

                if ((start != -1) && (end != -1) && (start < end))
                {
                    int from = start + BEGIN_PAYLOAD_MARK.Length;
                    int length = end - from;

                    txt = txt.Substring(from, length);

                    txt = txt.Trim('\n', '\r');

                    this.PayloadTxt.Text = txt;
                }

                string path = $"{this.ControllerCmb.SelectedItem}/{this.EndpointCmb.SelectedItem}";
                var notificationUrl = new Uri(new Uri(this.config.ApiUrl), path);
                string message = txt;

                if (DateTime.Now.Subtract(this.tokenAquiredTime) > TimeSpan.FromMinutes(42))
                {
                    await this.UpdateTokenHeader();
                }

                HttpResponseMessage response = await Post(message, notificationUrl, this.tokenHeader);

                this.StatusCodeLbl.Text = $"{STATUS_CODE_PREFIX} {response.StatusCode}";

                content = await response.Content.ReadAsStringAsync();
            }
            finally
            {
                this.EnableControls(true);
            }

            if (!string.IsNullOrEmpty(content))
            {
                using (var pop = new ResponseViewerFrm(this.config, content))
                {
                    pop.ShowDialog();
                }
            }
        }

        private void ControllerCmb_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedControler = (string)this.ControllerCmb.SelectedItem;

            Controller controller = this.config.Controllers.FirstOrDefault(c => c.Path == selectedControler);

            if (controller != null)
            {
                this.EndpointCmb.Items.Clear();
                this.EndpointCmb.Items.AddRange(controller.Endpoints.ToArray());
                this.EndpointCmb.SelectedIndex = 0;
            }
        }

        private Task UpdateTokenHeader()
        {
            string clientId = this.config.ClientID;
            string secret = this.config.Secret;
            string tenantId = this.config.TenantID;
            string scope = $"{this.config.Scope}/.default";

            return this.GetAuthenticationHeader(
                clientId, secret, tenantId, scope);
        }

        private async Task GetAuthenticationHeader(
            string clientId, string secret, string tenantId, string scope)
        {
            this.ToolStripStatusLabel.Text = "Aquiring token...";
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
            this.tokenAquiredTime = DateTime.Now;
            this.ToolStripStatusLabel.Text = $"{this.tokenAquiredTime:HH:mm:ss} Token aquired";
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
            this.config.SelectedController = $"{this.ControllerCmb.SelectedItem}";
            this.config.SelectedEndpoint = $"{this.EndpointCmb.SelectedItem}";
            this.config.MainFrmWidth = this.Width;
            this.config.MainFrmHeight = this.Height;

            this.SaveConfig();
        }

        private void SaveConfig()
        {
            string json = JsonConvert.SerializeObject(
                this.config, Formatting.Indented);

            File.WriteAllText(this.configFilePath, json);
        }
    }
}
