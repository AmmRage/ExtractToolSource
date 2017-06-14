namespace Ravioli.ArchivePlugins
{
    using Ravioli.ArchiveInterface;
    using Ravioli.PluginHelpers;
    using System;
    using System.Collections.Generic;
    using System.IO;

    public class EM4FSPlugin : AbstractorArchive
    {
        protected override AbstractorDefinition[] CreateDefinitions()
        {
            string str2 = Path.Combine(Path.GetDirectoryName(base.FileName), "Data");
            return new List<AbstractorDefinition> { new AbstractorDefinition(str2 + @"\*.*", "", 1) }.ToArray();
        }

        protected override bool IsValidFormat(Stream stream, BinaryReader reader)
        {
            string directoryName = Path.GetDirectoryName(base.FileName);
            return (File.Exists(Path.Combine(directoryName, "Em4.exe")) && Directory.Exists(Path.Combine(directoryName, "Data")));
        }

        public override string[] DefaultDirectories
        {
            get
            {
                return SystemSearcher.Combine3264ProgramFiles(@"sixteen tons entertainment\Emergency4");
            }
        }

        public override string DisplayFileName
        {
            get
            {
                return "Emergency 4";
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
                return "Emergency 4";
            }
        }
    }
}

