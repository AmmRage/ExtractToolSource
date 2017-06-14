namespace Ravioli.ArchivePlugins
{
    using Ravioli.ArchiveInterface;
    using System;
    using System.IO;
    using System.Text;

    public class WadFile : GenericArchive
    {
        protected override bool IsValidFormat(Stream stream, BinaryReader reader)
        {
            stream.Seek(0L, SeekOrigin.Begin);
            return (reader.ReadUInt32() == 0x44415749);
        }

        protected override GenericDirectoryEntry[] ReadGenericDirectory(Stream stream, BinaryReader reader)
        {
            if (!this.IsValidFormat(stream, reader))
            {
                throw new InvalidDataException("This is not a valid WAD file.");
            }
            stream.Seek(0L, SeekOrigin.Begin);
            reader.ReadUInt32();
            uint num = reader.ReadUInt32();
            uint num2 = reader.ReadUInt32();
            stream.Seek((long) num2, SeekOrigin.Begin);
            GenericDirectoryEntry[] entryArray = new GenericDirectoryEntry[num];
            for (uint i = 0; i < num; i++)
            {
                GenericDirectoryEntry entry = new GenericDirectoryEntry {
                    Offset = (long) reader.ReadUInt32(),
                    Length = (long) reader.ReadUInt32()
                };
                byte[] bytes = reader.ReadBytes(8);
                entry.Name = Encoding.GetEncoding(0x4e4).GetString(bytes);
                if (entry.Name.Contains("\0"))
                {
                    int index = entry.Name.IndexOf('\0');
                    entry.Name = entry.Name.Substring(0, index);
                }
                entryArray[i] = entry;
            }
            return entryArray;
        }

        public override string[] Extensions
        {
            get
            {
                return new string[] { ".wad" };
            }
        }

        public override string TypeName
        {
            get
            {
                return "Doom WAD (IWAD) File";
            }
        }
    }
}

