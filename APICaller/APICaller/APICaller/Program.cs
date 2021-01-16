using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace APICaller
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                MessageBox.Show("No config arg found",
                    "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            Config config = null;

            try
            {
                using (StreamReader file = File.OpenText(args[0]))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    config = (Config)serializer.Deserialize(file, typeof(Config));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Deserializing config failed {ex.Message}",
                    "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1(config));
        }
    }
}
