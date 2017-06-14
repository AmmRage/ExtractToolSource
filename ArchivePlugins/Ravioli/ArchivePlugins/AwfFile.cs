namespace Ravioli.ArchivePlugins
{
    using Ravioli.ArchiveInterface;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    public class AwfFile : GenericArchive
    {
        private long CalculateItemSize(IList<GenericDirectoryEntry> directory, long fileLength, int index)
        {
            GenericDirectoryEntry entry = directory[index];
            long offset = entry.Offset;
            if (index < (directory.Count - 1))
            {
                return ((directory[index + 1].Offset - 1L) - offset);
            }
            return (fileLength - offset);
        }

        protected override bool IsValidFormat(Stream stream, BinaryReader reader)
        {
            return true;
        }

        protected override GenericDirectoryEntry[] ReadGenericDirectory(Stream stream, BinaryReader reader)
        {
            stream.Seek(0L, SeekOrigin.Begin);
            int num = reader.ReadInt32();
            long length = stream.Length;
            GenericDirectoryEntry[] directory = new GenericDirectoryEntry[num];
            for (int i = 0; i < num; i++)
            {
                GenericDirectoryEntry entry = new GenericDirectoryEntry {
                    Offset = reader.ReadInt32()
                };
                byte[] bytes = reader.ReadBytes(260);
                entry.Name = Encoding.GetEncoding(0x4e4).GetString(bytes).Trim(new char[1]);
                directory[i] = entry;
            }
            for (int j = 0; j < directory.Length; j++)
            {
                GenericDirectoryEntry entry2 = directory[j];
                entry2.Length = this.CalculateItemSize(directory, length, j);
            }
            return directory;
        }

        public override string[] Extensions
        {
            get
            {
                return new string[] { ".awf" };
            }
        }

        public override string TypeName
        {
            get
            {
                return "WWTBAM AWF File";
            }
        }
    }
}

