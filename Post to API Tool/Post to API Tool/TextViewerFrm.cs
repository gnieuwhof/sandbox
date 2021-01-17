namespace Post_to_API_Tool
{
    using Newtonsoft.Json;
    using Post_to_API_Tool.Configuration;
    using System.Drawing;
    using System.Windows.Forms;

    public partial class TextViewerFrm : Form
    {
        private readonly Config config;
        private bool keyDown;
        private Keys pressedKey;


        public TextViewerFrm(string title, Config config, string response)
        {
            this.config = config;

            InitializeComponent();

            this.TextTxt.MouseWheel += this.PayloadTxt_MouseWheel;

            this.Text = title;

            this.ApplyConfig();

            try
            {
                object obj = JsonConvert.DeserializeObject(response);
                string json = JsonConvert.SerializeObject(obj, Formatting.Indented);
                this.TextTxt.Text = json;
            }
            catch
            {
                this.TextTxt.Text = response;
            }

            this.CloseBtn.Focus();
        }

        private void ApplyConfig()
        {
            if (this.config.ResponseViewerFrmWidth >= this.MinimumSize.Width)
            {
                this.Width = this.config.ResponseViewerFrmWidth;
            }
            if (this.config.ResponseViewerFrmHeight >= this.MinimumSize.Height)
            {
                this.Height = this.config.ResponseViewerFrmHeight;
            }

            if (this.config.TextViewerFrmFontSize > 0)
            {
                this.SetFontSize(this.config.TextViewerFrmFontSize);
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

        private void ResponseTxt_KeyDown(object sender, KeyEventArgs e)
        {
            this.keyDown = true;

            this.pressedKey = e.KeyCode;
        }

        private void ResponseTxt_KeyUp(object sender, KeyEventArgs e)
        {
            this.keyDown = false;
        }

        private void PayloadTxt_MouseWheel(object sender, MouseEventArgs e)
        {
            if (this.keyDown && (this.pressedKey == Keys.ControlKey))
            {
                if (e.Delta < 0)
                {
                    if (this.config.TextViewerFrmFontSize > 1)
                    {
                        this.SetFontSize(--this.config.TextViewerFrmFontSize);
                    }
                }
                else
                {
                    if (this.config.TextViewerFrmFontSize < 25)
                    {
                        this.SetFontSize(++this.config.TextViewerFrmFontSize);
                    }
                }
            }
        }

        private void SetFontSize(int size)
        {
            if (size > 0)
            {
                this.config.TextViewerFrmFontSize = size;

                this.TextTxt.Font = new Font(
                    this.TextTxt.Font.FontFamily, this.config.TextViewerFrmFontSize);
            }
        }
    }
}
