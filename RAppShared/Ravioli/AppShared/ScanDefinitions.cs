using Ravioli.ArchivePlugins;

namespace Ravioli.AppShared
{
    using System;
    using System.Diagnostics;
    using System.IO;

    public class ScanDefinitions
    {
        public static readonly ArchivePluginMapping ScanArchiveMapping = new ArchivePluginMapping(new string[] { ".rsr" }, typeof(ScanArchive), "Scanned file", false, false);

        public static string GetTempResultsFileInNewDir(string fileName)
        {
            return Path.Combine(Path.Combine(Path.GetTempPath(), string.Concat(new object[] { "RScanner", Process.GetCurrentProcess().Id, "_", Environment.TickCount })), Path.ChangeExtension(Path.GetFileName(fileName), ".rsr"));
        }

        public static string GetTempResultsFileName(string fileName)
        {
            return Path.Combine(Path.GetTempPath(), string.Concat(new object[] { "RScanner", Process.GetCurrentProcess().Id, "_", Path.ChangeExtension(Path.GetFileName(fileName), ".rsr") }));
        }
    }
}

