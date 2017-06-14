namespace Ravioli.ArchivePlugins
{
    using Ravioli.ArchiveInterface;
    using System;
    using System.IO;

    public class WbmFile : GenericArchive
    {
        protected override bool IsValidFormat(Stream stream, BinaryReader reader)
        {
            stream.Seek(0L, SeekOrigin.Begin);
            return (reader.ReadUInt32() == 0x46464952);
        }

        protected override GenericDirectoryEntry[] ReadGenericDirectory(Stream stream, BinaryReader reader)
        {
            if (!this.IsValidFormat(stream, reader))
            {
                throw new InvalidDataException("This is not a valid WBM file.");
            }
            GenericDirectoryEntry entry = new GenericDirectoryEntry {
                Name = Path.ChangeExtension(Path.GetFileName(base.FileName), ".wav"),
                Offset = 0L,
                Length = stream.Length
            };
            return new GenericDirectoryEntry[] { entry };
        }

        public override string[] Extensions
        {
            get
            {
                return new string[] { ".wbm" };
            }
        }

        public override string TypeName
        {
            get
            {
                return "Snocka Watten WBM File";
            }
        }
    }
}

