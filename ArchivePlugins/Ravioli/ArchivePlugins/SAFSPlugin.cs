namespace Ravioli.ArchivePlugins
{
    using Ravioli.ArchiveInterface;
    using Ravioli.PluginHelpers;
    using System;
    using System.Collections.Generic;
    using System.IO;

    public class SAFSPlugin : AbstractorArchive
    {
        protected override AbstractorDefinition[] CreateDefinitions()
        {
            List<AbstractorDefinition> list = new List<AbstractorDefinition>();
            Type pluginType = typeof(SAPakFile);
            list.Add(new SubArchiveAbstractorDefinition("sg08.pak", pluginType));
            return list.ToArray();
        }

        protected override bool IsValidFormat(Stream stream, BinaryReader reader)
        {
            string directoryName = Path.GetDirectoryName(base.FileName);
            return (File.Exists(Path.Combine(directoryName, "SummerAthletics.exe")) && (Directory.GetFiles(directoryName, "sg08.pak").Length > 0));
        }

        public override string[] DefaultDirectories
        {
            get
            {
                List<string> list = new List<string>();
                list.AddRange(SystemSearcher.Combine3264ProgramFiles("Summer Athletics"));
                return list.ToArray();
            }
        }

        public override string DisplayFileName
        {
            get
            {
                return "Summer Athletics";
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
                return "Summer Athletics";
            }
        }
    }
}

