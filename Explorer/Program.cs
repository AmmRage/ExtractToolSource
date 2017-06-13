namespace Ravioli.Explorer
{
    using System;
    using System.Windows.Forms;

    internal static class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            string str = string.Empty;
            if (args.Length > 0)
            {
                str = args[0];
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            frmMain mainForm = new frmMain {
                StartupDocument = str
            };
            Application.Run(mainForm);
        }
    }
}

