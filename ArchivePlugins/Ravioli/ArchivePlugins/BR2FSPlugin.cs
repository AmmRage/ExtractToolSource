namespace Ravioli.ArchivePlugins
{
    using Ravioli.ArchiveInterface;
    using Ravioli.PluginHelpers;
    using System;
    using System.Collections.Generic;
    using System.IO;

    public class BR2FSPlugin : AbstractorArchive
    {
        protected override AbstractorDefinition[] CreateDefinitions()
        {
            Path.GetDirectoryName(base.FileName);
            List<AbstractorDefinition> list = new List<AbstractorDefinition>();
            Type pluginType = typeof(PodFile);
            list.Add(new SubArchiveAbstractorDefinition("*.pod", pluginType));
            list.Add(new AbstractorDefinition(@"video\*.mpg", "video"));
            return list.ToArray();
        }

        protected override bool IsValidFormat(Stream stream, BinaryReader reader)
        {
            string directoryName = Path.GetDirectoryName(base.FileName);
            if (Directory.GetFiles(directoryName, "*.pod").Length <= 0)
            {
                return false;
            }
            if (!File.Exists(Path.Combine(directoryName, "rayne2.exe")))
            {
                return File.Exists(Path.Combine(directoryName, "rayne2demo.exe"));
            }
            return true;
        }

        public override string[] DefaultDirectories
        {
            get
            {
                List<string> list = new List<string>();
                list.AddRange(SystemSearcher.Combine3264ProgramFiles(@"Majesco Entertainment\BloodRayne 2"));
                list.Add(Path.Combine(SystemSearcher.SteamAppsCommonDirectory, "bloodrayne 2"));
                return list.ToArray();
            }
        }

        public override string DisplayFileName
        {
            get
            {
                return "BloodRayne 2";
            }
        }

        public override string[] Extensions
        {
            get
            {
                return null;
            }
        }

        public override string TypeName
        {
            get
            {
                return "BloodRayne 2";
            }
        }
    }
}

