namespace Ravioli.ArchivePlugins
{
    using Ravioli.ArchiveInterface;
    using System;
    using System.IO;
    using System.Text;

    public class ImgFile : GenericArchive
    {
        protected override bool IsValidFormat(Stream stream, BinaryReader reader)
        {
            stream.Seek(0L, SeekOrigin.Begin);
            byte[] bytes = reader.ReadBytes("GIF87a".Length);
            return (Encoding.GetEncoding(0x4e4).GetString(bytes) == "GIF87a");
        }

        protected override GenericDirectoryEntry[] ReadGenericDirectory(Stream stream, BinaryReader reader)
        {
            if (!this.IsValidFormat(stream, reader))
            {
                throw new InvalidDataException("This is not a valid IMG file.");
            }
            GenericDirectoryEntry entry = new GenericDirectoryEntry {
                Name = Path.ChangeExtension(Path.GetFileName(base.FileName), ".gif"),
                Offset = 0L,
                Length = stream.Length
            };
            return new GenericDirectoryEntry[] { entry };
        }

        public override string[] Extensions
        {
            get
            {
                return new string[] { ".img" };
            }
        }

        public override string TypeName
        {
            get
            {
                return "STTNG IMG File";
            }
        }
    }
}

