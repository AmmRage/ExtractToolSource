using Ravioli.AppShared;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace Ravioli.Explorer
{
    internal static class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            //var dll = @"C:\TEST\TestFolder_Misc\RavioliGameTools\Src\Explorer\bin\Debug\Plugins\ArchivePlugins.dll";
            //try
            //{
            //    var assembly = Assembly.ReflectionOnlyLoadFrom(dll);
            //    var exportedTypes = assembly.GetExportedTypes();
            //}
            //catch (Exception ex)
            //{
            //    Debug.WriteLine(ex);
            //}

            var archivePluginManager = new ArchivePluginManager();
            archivePluginManager.EnumeratePlugins();
            if (archivePluginManager.AvailableArchivePlugins.Length == 0)
            {
                Debug.WriteLine(
                    "No archive plug-ins were found. At least one plug-in must be installed for this program to work.");
            }

            //var str = string.Empty;
            //if (args.Length > 0)
            //{
            //    str = args[0];
            //}
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //var mainForm = new frmMain {
            //    StartupDocument = str
            //};
            //Application.Run(mainForm);
        }
    }
}

