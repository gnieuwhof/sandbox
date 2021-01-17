namespace Post_to_API_Tool
{
    using Newtonsoft.Json;
    using Post_to_API_Tool.Configuration;
    using System.Drawing;
    using System.Windows.Forms;

    public partial class TextViewerFrm : Form
    {
        private readonly Config config;


        public TextViewerFrm(string title, Config config, string response)
        {
            this.config = config;

            InitializeComponent();

            this.Text = title;

            if (this.config.ResponseViewerFrmWidth >= this.MinimumSize.Width)
            {
                this.Width = this.config.ResponseViewerFrmWidth;
            }
            if (this.config.ResponseViewerFrmHeight >= this.MinimumSize.Height)
            {
                this.Height = this.config.ResponseViewerFrmHeight;
            }

            if (this.config.FontSize > 0)
            {
                this.ResponseTxt.Font = new Font(
                    this.ResponseTxt.Font.FontFamily, this.config.FontSize);
            }

            try
            {
                object obj = JsonConvert.DeserializeObject(response);
                string json = JsonConvert.SerializeObject(obj, Formatting.Indented);
                this.ResponseTxt.Text = json;
            }
            catch
            {
                this.ResponseTxt.Text = response;
            }
        }

        private void CloseBtn_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void ResponseViewerFrm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.WindowState != FormWindowState.Maximized)
            {
                this.config.ResponseViewerFrmWidth = this.Width;
                this.config.ResponseViewerFrmHeight = this.Height;
            }
        }

        private void ResponseViewerFrm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }
    }
}
