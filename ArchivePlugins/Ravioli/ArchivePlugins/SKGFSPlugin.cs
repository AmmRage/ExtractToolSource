namespace Ravioli.ArchivePlugins
{
    using Ravioli.ArchiveInterface;
    using Ravioli.PluginHelpers;
    using System;
    using System.Collections.Generic;
    using System.IO;

    public class SKGFSPlugin : AbstractorArchive
    {
        protected override AbstractorDefinition[] CreateDefinitions()
        {
            Type pluginType = typeof(GfsFile);
            base.DuplicatesPolicy = AbstractorDuplicatesPolicy.Replace;
            List<AbstractorDefinition> list = new List<AbstractorDefinition>();
            foreach (string str3 in Directory.GetFiles(Path.Combine(Path.GetDirectoryName(base.FileName), "data01"), "*.gfs"))
            {
                list.Add(new SubArchiveAbstractorDefinition(str3, pluginType));
            }
            return list.ToArray();
        }

        protected override bool IsValidFormat(Stream stream, BinaryReader reader)
        {
            bool flag = false;
            string directoryName = Path.GetDirectoryName(base.FileName);
            if (File.Exists(Path.Combine(directoryName, "Skullgirls.exe")))
            {
                string path = Path.Combine(directoryName, "data01");
                if (Directory.Exists(path))
                {
                    flag = Directory.GetFiles(path, "*.gfs").Length > 0;
                }
            }
            return flag;
        }

        public override string[] DefaultDirectories
        {
            get
            {
                return new string[] { Path.Combine(SystemSearcher.SteamAppsCommonDirectory, "Skullgirls") };
            }
        }

        public override string DisplayFileName
        {
            get
            {
                return this.TypeName;
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
                return "Skullgirls";
            }
        }
    }
}

