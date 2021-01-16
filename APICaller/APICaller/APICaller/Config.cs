using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APICaller
{
    public class Controller
    {
        public string Path { get; set; }
        public string[] Endpoints { get; set; }
    }

    public class Config
    {
        public string ApiUrl { get; set; }
        public string TenantID { get; set; }
        public string ClientID { get; set; }
        public string Scope { get; set; }
        public string Secret { get; set; }
        public Controller[] Controllers { get; set; }
    }
}
