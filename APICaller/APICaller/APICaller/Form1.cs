using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace APICaller
{
    public partial class Form1 : Form
    {
        private readonly Config config;
        private readonly HttpClient httpClient;


        public Form1(Config config)
        {
            this.config = config;

            InitializeComponent();

            var controllers = config.Controllers.Select(c => c.Path);
            this.comboBox1.Items.AddRange(controllers.ToArray());
            this.comboBox1.SelectedIndex = 0;


            this.httpClient = new HttpClient();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            string txt = this.textBox1.Text;

            int start = txt.IndexOf("--BEGIN_PAYLOAD");
            int end = txt.IndexOf("--END_PAYLOAD");

            if ((start != -1) && (end != -1) && (start < end))
            {
                int from = start + "--BEGIN_PAYLOAD".Length;
                int length = end - from;

                txt = txt.Substring(from, length);

                txt = txt.Trim('\n', '\r');

                this.textBox1.Text = txt;
            }

            string clientId = this.config.ClientID;
            string secret = this.config.Secret;
            string tenantId = this.config.TenantID;
            string scope = $"{this.config.Scope}/.default";

            string path = $"{this.comboBox1.SelectedItem}/{this.comboBox2.SelectedItem}";
            var notificationUrl = new Uri(new Uri(this.config.ApiUrl), path);
            string message = txt;

            AuthenticationHeaderValue authHeader = await this.GetAuthenticationHeader(
                clientId, secret, tenantId, scope);

            HttpResponseMessage response = await Post(message, notificationUrl, authHeader);

            string content = await response.Content.ReadAsStringAsync();

            if (!string.IsNullOrEmpty(content))
            {
                var pop = new Form2(content);
                pop.Show();
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedControler = (string)this.comboBox1.SelectedItem;

            Controller controller = this.config.Controllers.FirstOrDefault(c => c.Path == selectedControler);

            if (controller != null)
            {
                this.comboBox2.Items.Clear();
                this.comboBox2.Items.AddRange(controller.Endpoints.ToArray());
                this.comboBox2.SelectedIndex = 0;
            }
        }


        private async Task<AuthenticationHeaderValue> GetAuthenticationHeader(
            string clientId, string secret, string tenantId, string scope)
        {
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

            return header;
        }

        private async Task<HttpResponseMessage> Post(string json,
            Uri url, AuthenticationHeaderValue authHeader)
        {
            httpClient.DefaultRequestHeaders.Authorization = authHeader;

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage result = await this.httpClient.PostAsync(url, content);

            return result;
        }
    }
}
