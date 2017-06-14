namespace Ravioli.ArchivePlugins
{
    using Ravioli.ArchiveInterface;
    using Ravioli.PluginHelpers;
    using System;
    using System.Collections.Generic;
    using System.IO;

    public class ITGFSPlugin : AbstractorArchive
    {
        private string version;

        protected override AbstractorDefinition[] CreateDefinitions()
        {
            Type pluginType = typeof(PckFile);
            base.DuplicatesPolicy = AbstractorDuplicatesPolicy.Replace;
            List<AbstractorDefinition> list = new List<AbstractorDefinition> {
                new SubArchiveAbstractorDefinition(@"pcks\D673D8A8.PCK", pluginType),
                new SubArchiveAbstractorDefinition(@"pcks\D82CA1FD.PCK", pluginType),
                new SubArchiveAbstractorDefinition(@"pcks\DBFEE9B0.PCK", pluginType),
                new SubArchiveAbstractorDefinition(@"pcks\DD70BA0B.PCK", pluginType)
            };
            string directoryName = Path.GetDirectoryName(base.FileName);
            if (File.Exists(Path.Combine(directoryName, @"pcks\D08D4AA6.PCK")))
            {
                list.Add(new SubArchiveAbstractorDefinition(@"pcks\D08D4AA6.PCK", pluginType));
            }
            if (Directory.Exists(Path.Combine(directoryName, @"pcks\packa")))
            {
                list.Add(new SubArchiveAbstractorDefinition(@"pcks\packa\D1513758.PCK", pluginType));
                list.Add(new SubArchiveAbstractorDefinition(@"pcks\packa\D4D500B4.PCK", pluginType));
                list.Add(new SubArchiveAbstractorDefinition(@"pcks\packa\DC37F75E.PCK", pluginType));
                list.Add(new SubArchiveAbstractorDefinition(@"pcks\basepatch\DDF0C003.PCK", pluginType));
                this.version = "R2";
            }
            return list.ToArray();
        }

        protected override bool IsValidFormat(Stream stream, BinaryReader reader)
        {
            return File.Exists(Path.Combine(Path.GetDirectoryName(base.FileName), @"pcks\D82CA1FD.PCK"));
        }

        public override string[] DefaultDirectories
        {
            get
            {
                return SystemSearcher.Combine3264ProgramFiles("In The Groove");
            }
        }

        public override string DisplayFileName
        {
            get
            {
                string str = "In The Groove";
                if (!string.IsNullOrEmpty(this.version))
                {
                    str = str + " (" + this.version + ")";
                }
                return str;
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
                return "In The Groove";
            }
        }
    }
}

