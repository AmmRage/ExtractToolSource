namespace Ravioli.ArchivePlugins
{
    using Ravioli.ArchiveInterface;
    using Ravioli.PluginHelpers;
    using System;
    using System.Collections.Generic;
    using System.IO;

    public class SGFSPlugin : AbstractorArchive
    {
        protected override AbstractorDefinition[] CreateDefinitions()
        {
            Type pluginType = typeof(DltFile);
            base.DuplicatesPolicy = AbstractorDuplicatesPolicy.Replace;
            List<AbstractorDefinition> list = new List<AbstractorDefinition>();
            string[] files = Directory.GetFiles(Path.GetDirectoryName(base.FileName), "stargun?.dlt");
            Array.Sort<string>(files);
            foreach (string str2 in files)
            {
                list.Add(new SubArchiveAbstractorDefinition(str2, pluginType));
            }
            return list.ToArray();
        }

        protected override bool IsValidFormat(Stream stream, BinaryReader reader)
        {
            string directoryName = Path.GetDirectoryName(base.FileName);
            return (File.Exists(Path.Combine(directoryName, "stargun.exe")) && File.Exists(Path.Combine(directoryName, "stargun.dlt")));
        }

        public override string[] DefaultDirectories
        {
            get
            {
                return SystemSearcher.GetCommonMsDosGameDirs(new string[] { "Stargun", "Stargunner" });
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
                return "Stargunner";
            }
        }
    }
}

