﻿namespace Post_to_API_Tool
{
    using Newtonsoft.Json;
    using Post_to_API_Tool.Configuration;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
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

            bool cont = this.LoadConfigFile(this.configFilePath);
            if (!cont)
            {
                return;
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

                if(this.config.FontSize > 0)
                {
                    this.PayloadTxt.Font = new Font(
                        this.PayloadTxt.Font.FontFamily, this.config.FontSize);
                }
            }

            this.httpClient = new HttpClient();

            _ = this.UpdateTokenHeader(true);
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
                string message = "Deserializing config failed." +
                    Environment.NewLine + Environment.NewLine + ex.Message;

                MessageBox.Show(message, Program.PROGRAM_TITLE,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                var uri = new Uri(new Uri(this.config.ApiUrl), path);
                string message = txt;

                if (DateTime.Now.Subtract(this.tokenAquiredTime) > TimeSpan.FromMinutes(42))
                {
                    await this.UpdateTokenHeader(false);
                }

                this.SetStatus($"Calling: {uri}");
                HttpResponseMessage response = await Post(message, uri, this.tokenHeader);
                this.SetStatus($"Response received from: {uri}");

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

            Controller controller = this.config.Controllers
                .FirstOrDefault(c => c.Path == selectedControler);

            if (controller != null)
            {
                this.EndpointCmb.Items.Clear();
                this.EndpointCmb.Items.AddRange(controller.Endpoints.ToArray());
                this.EndpointCmb.SelectedIndex = 0;
            }
        }

        private async Task UpdateTokenHeader(bool setControlsEnabled)
        {
            if (config == null)
            {
                return;
            }

            string clientId = this.config.ClientID;
            string secret = this.config.Secret;
            string tenantId = this.config.TenantID;
            string scope = $"{this.config.Scope}/.default";

            await this.GetAuthenticationHeader(
                clientId, secret, tenantId, scope);

            if(setControlsEnabled)
            {
                this.EnableControls(true);
            }
        }

        private async Task GetAuthenticationHeader(
            string clientId, string secret, string tenantId, string scope)
        {
            this.SetStatus("Aquiring token...");
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
            this.SetStatus("Token aquired");
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
            if (this.config != null)
            {
                this.config.SelectedController = $"{this.ControllerCmb.SelectedItem}";
                this.config.SelectedEndpoint = $"{this.EndpointCmb.SelectedItem}";

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
    }
}