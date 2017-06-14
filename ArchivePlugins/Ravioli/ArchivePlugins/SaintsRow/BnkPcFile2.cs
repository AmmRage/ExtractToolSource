namespace Ravioli.ArchivePlugins.SaintsRow
{
    using Ravioli.ArchiveInterface;
    using Ravioli.Xbox360Plugins;
    using System;
    using System.IO;

    public class BnkPcFile2 : GenericArchive
    {
        private string idString;

        protected override bool IsValidFormat(Stream stream, BinaryReader reader)
        {
            stream.Seek(0L, SeekOrigin.Begin);
            return BnkReader.IsValidFormat(stream, reader);
        }

        protected override void OnClose()
        {
            this.idString = null;
        }

        protected override GenericDirectoryEntry[] ReadGenericDirectory(Stream stream, BinaryReader reader)
        {
            if (!this.IsValidFormat(stream, reader))
            {
                throw new InvalidDataException("This is not a valid BNK_PC file.");
            }
            stream.Seek(0L, SeekOrigin.Begin);
            return BnkReader.ReadBnkDirectory(stream, reader, out this.idString);
        }

        public override string Comment
        {
            get
            {
                return this.idString;
            }
        }

        public override string[] Extensions
        {
            get
            {
                return new string[] { ".bnk_pc" };
            }
        }

        public override string TypeName
        {
            get
            {
                return "Saints Row 3/4 BNK_PC (WWISE) File";
            }
        }
    }
}

