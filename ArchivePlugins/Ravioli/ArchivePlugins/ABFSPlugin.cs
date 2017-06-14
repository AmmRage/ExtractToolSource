namespace Ravioli.ArchivePlugins
{
    using Ravioli.ArchiveInterface;
    using Ravioli.PluginHelpers;
    using System;
    using System.Collections.Generic;
    using System.IO;

    public class ABFSPlugin : AbstractorArchive
    {
        protected override AbstractorDefinition[] CreateDefinitions()
        {
            Path.GetDirectoryName(base.FileName);
            return new List<AbstractorDefinition> { new AbstractorDefinition(@"data\*.*", "", 1) }.ToArray();
        }

        protected override bool IsValidFormat(Stream stream, BinaryReader reader)
        {
            return File.Exists(Path.Combine(Path.GetDirectoryName(base.FileName), "AngryBirds.exe"));
        }

        public override string[] DefaultDirectories
        {
            get
            {
                List<string> list = new List<string>();
                list.AddRange(SystemSearcher.Combine3264ProgramFiles(@"Rovio\Angry Birds"));
                list.AddRange(SystemSearcher.Combine3264ProgramFiles(@"Rovio Entertainment Ltd\Angry Birds"));
                return list.ToArray();
            }
        }

        public override string DisplayFileName
        {
            get
            {
                return "Angry Birds";
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
                return "Angry Birds";
            }
        }
    }
}

