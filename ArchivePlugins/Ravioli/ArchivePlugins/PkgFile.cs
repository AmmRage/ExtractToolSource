namespace Ravioli.ArchivePlugins
{
    using Ravioli.ArchiveInterface;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;

    public class PkgFile : GenericArchive
    {
        private GenericDirectoryEntry[] CreateFlatFileList(PkgDirectoryListEntry[] directoryList, PkgFileListEntry[] fileList, string[] fileListNames)
        {
            List<GenericDirectoryEntry> list = new List<GenericDirectoryEntry>();
            foreach (PkgDirectoryListEntry entry in directoryList)
            {
                for (uint i = entry.StartIndex; i < entry.EndIndex; i++)
                {
                    PkgFileListEntry entry2 = fileList[i];
                    GenericDirectoryEntry item = new GenericDirectoryEntry {
                        Name = entry.Name + "/" + fileListNames[i],
                        Offset = (long) entry2.Offset,
                        Length = (long) entry2.Size
                    };
                    list.Add(item);
                }
            }
            return list.ToArray();
        }

        private string GetFileName(Stream stream, BinaryReader reader, PkgHeader header, PkgFileListEntry entry)
        {
            stream.Seek((long) (header.FileNameListOffset + entry.NameOffset), SeekOrigin.Begin);
            string str = this.ReadZeroTerminatedString(reader);
            stream.Seek((long) (header.ExtensionListOffset + entry.ExtensionOffset), SeekOrigin.Begin);
            string str2 = this.ReadZeroTerminatedString(reader);
            return (str + "." + str2);
        }

        protected override bool IsValidFormat(Stream stream, BinaryReader reader)
        {
            stream.Seek(0L, SeekOrigin.Begin);
            return (reader.ReadUInt32() == 0x474b505a);
        }

        private PkgDirectoryListEntry[] ReadDirectoryList(Stream stream, BinaryReader reader, PkgHeader header)
        {
            stream.Seek((long) header.DirectoryListOffset, SeekOrigin.Begin);
            List<PkgDirectoryListEntry> list = new List<PkgDirectoryListEntry>();
            string str = string.Empty;
            string str2 = string.Empty;
            uint num = 0;
            uint num2 = 0;
            while (num2 < header.DirectoryCharCount)
            {
                byte num3 = reader.ReadByte();
                reader.ReadByte();
                reader.ReadUInt16();
                reader.ReadUInt16();
                reader.ReadUInt16();
                char ch = Convert.ToChar(num3);
                str = str + ch;
                num2++;
                ushort num4 = reader.ReadUInt16();
                ushort num5 = reader.ReadUInt16();
                if ((num4 != 0) || (num5 != 0))
                {
                    num++;
                    if (str.StartsWith("/") || str.StartsWith(@"\"))
                    {
                        str = str2 + str;
                    }
                    string.Format("{0,4} {1,-30} startIndex=0x{2:X}, endIndex=0x{3:X}", new object[] { num, str, num4, num5 });
                    PkgDirectoryListEntry item = new PkgDirectoryListEntry {
                        Name = str,
                        StartIndex = num4,
                        EndIndex = num5
                    };
                    list.Add(item);
                    str2 = str;
                    str = string.Empty;
                }
            }
            return list.ToArray();
        }

        private void ReadFileList(Stream stream, BinaryReader reader, PkgHeader header, out PkgFileListEntry[] fileList, out string[] fileListNames)
        {
            stream.Seek(0x200L, SeekOrigin.Begin);
            fileList = new PkgFileListEntry[header.FileCount];
            for (uint i = 0; i < header.FileCount; i++)
            {
                PkgFileListEntry entry = new PkgFileListEntry();
                reader.ReadByte();
                entry.ExtensionOffset = reader.ReadUInt16();
                reader.ReadByte();
                entry.NameOffset = reader.ReadUInt32();
                entry.Offset = reader.ReadUInt32();
                entry.Size = reader.ReadUInt32();
                fileList[i] = entry;
            }
            fileListNames = new string[fileList.Length];
            for (uint j = 0; j < fileList.Length; j++)
            {
                fileListNames[j] = this.GetFileName(stream, reader, header, fileList[j]);
            }
        }

        protected override GenericDirectoryEntry[] ReadGenericDirectory(Stream stream, BinaryReader reader)
        {
            PkgFileListEntry[] entryArray;
            string[] strArray;
            if (!this.IsValidFormat(stream, reader))
            {
                throw new InvalidDataException("This is not a valid PKG file.");
            }
            stream.Seek(0L, SeekOrigin.Begin);
            PkgHeader header = new PkgHeader {
                Signature = reader.ReadUInt32(),
                Version = reader.ReadUInt32(),
                DataStartOffset = reader.ReadUInt32(),
                FileCount = reader.ReadUInt32(),
                DirectoryListOffset = reader.ReadUInt32(),
                DirectoryCharCount = reader.ReadUInt32(),
                FileNameListOffset = reader.ReadUInt32(),
                ExtensionListOffset = reader.ReadUInt32()
            };
            this.ReadFileList(stream, reader, header, out entryArray, out strArray);
            PkgDirectoryListEntry[] directoryList = this.ReadDirectoryList(stream, reader, header);
            return this.CreateFlatFileList(directoryList, entryArray, strArray);
        }

        private string ReadZeroTerminatedString(BinaryReader reader)
        {
            StringBuilder builder = new StringBuilder();
            for (byte i = reader.ReadByte(); i != 0; i = reader.ReadByte())
            {
                builder.Append(Convert.ToChar(i));
            }
            return builder.ToString();
        }

        public override string[] Extensions
        {
            get
            {
                return new string[] { ".pkg" };
            }
        }

        public override string TypeName
        {
            get
            {
                return "Psychonauts PKG File";
            }
        }
    }
}

