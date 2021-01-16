﻿namespace Post_to_API_Tool.Configuration
{
    public class Config
    {
        public string ApiUrl { get; set; }

        public string TenantID { get; set; }

        public string ClientID { get; set; }

        public string Scope { get; set; }

        public string Secret { get; set; }

        public Controller[] Controllers { get; set; }

        public string SelectedController { get; set; }

        public string SelectedEndpoint { get; set; }

        public int MainFrmWidth { get; set; }

        public int MainFrmHeight { get; set; }

        public int ResponseViewerFrmWidth { get; set; }

        public int ResponseViewerFrmHeight { get; set; }
    }
}
