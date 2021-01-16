namespace Post_to_API_Tool
{
    using Post_to_API_Tool.Configuration;
    using System.Windows.Forms;

    public partial class ResponseViewerFrm : Form
    {
        private readonly Config config;


        public ResponseViewerFrm(Config config, string response)
        {
            this.config = config;

            InitializeComponent();

            if(this.config.ResponseViewerFrmWidth >= this.MinimumSize.Width)
            {
                this.Width = this.config.ResponseViewerFrmWidth;
            }
            if (this.config.ResponseViewerFrmHeight >= this.MinimumSize.Height)
            {
                this.Height = this.config.ResponseViewerFrmHeight;
            }

            this.ResponseTxt.Text = response;
        }

        private void CloseBtn_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void ResponseViewerFrm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.config.ResponseViewerFrmWidth = this.Width;
            this.config.ResponseViewerFrmHeight = this.Height;
        }

        private void ResponseViewerFrm_Resize(object sender, System.EventArgs e)
        {

        }
    }
}
