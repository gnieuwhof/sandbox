namespace PostToAPITool
{
    using Newtonsoft.Json;
    using PostToAPITool.Configuration;
    using System;
    using System.IO;
    using System.Windows.Forms;

    static class Program
    {
        public const string PROGRAM_TITLE = "Post to API Tool";


        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                string message = "Program cannot start: No config argument found." +
                    Environment.NewLine +
                    "Pass in the path of the config file as command line argument.";

                MessageBox.Show(message, PROGRAM_TITLE,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }

            string configFilePath = args[0];

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainFrm(configFilePath));
        }
    }
}
