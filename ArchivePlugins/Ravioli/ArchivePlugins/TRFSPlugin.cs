namespace Ravioli.ArchivePlugins
{
    using Ravioli.ArchiveInterface;
    using System;
    using System.IO;

    public abstract class TRFSPlugin : AbstractorArchive
    {
        protected TRFSPlugin()
        {
        }

        protected override AbstractorDefinition[] CreateDefinitions()
        {
            base.DuplicatesPolicy = AbstractorDuplicatesPolicy.Replace;
            string sourceFile = Path.Combine(Path.GetDirectoryName(base.FileName), "bigfile.000");
            return new AbstractorDefinition[] { new SubArchiveAbstractorDefinition(sourceFile, typeof(TRBigFile)) };
        }

        protected override bool IsValidFormat(Stream stream, BinaryReader reader)
        {
            return File.Exists(Path.Combine(Path.GetDirectoryName(base.FileName), "bigfile.000"));
        }

        public override string[] DefaultDirectories
        {
            get
            {
                return new string[0];
            }
        }

        public override string DisplayFileName
        {
            get
            {
                return "Tomb Raider";
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
                return "Tomb Raider (Legend and newer)";
            }
        }
    }
}

