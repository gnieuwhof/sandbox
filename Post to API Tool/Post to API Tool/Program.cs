namespace Post_to_API_Tool
{
    using Newtonsoft.Json;
    using Post_to_API_Tool.Configuration;
    using System;
    using System.IO;
    using System.Windows.Forms;

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
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainFrm(args[0]));
        }
    }
}
