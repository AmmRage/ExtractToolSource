namespace Ravioli.ArchivePlugins
{
    using Ravioli.ArchiveInterface;
    using Ravioli.PluginHelpers;
    using Ravioli.PluginHelpers.Reading;
    using System;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Text;

    public class GfsFile : GenericArchive
    {
        private static readonly byte[] gfsSignature = Encoding.GetEncoding(0x4e4).GetBytes("Reverge Package File");
        private static readonly byte[] gfsVersion = Encoding.GetEncoding(0x4e4).GetBytes("1.1");

        protected override bool IsValidFormat(Stream stream, BinaryReader reader)
        {
            bool littleEndian = false;
            EndianBinaryReader enReader = new EndianBinaryReader(stream, littleEndian);
            return (ReadGfsHeader(stream, enReader) != null);
        }

        protected override GenericDirectoryEntry[] ReadGenericDirectory(Stream stream, BinaryReader reader)
        {
            bool littleEndian = false;
            EndianBinaryReader enReader = new EndianBinaryReader(stream, littleEndian);
            GfsHeader header = ReadGfsHeader(stream, enReader);
            if (header == null)
            {
                throw new InvalidDataException("This is not a valid GFS file.");
            }
            GenericDirectoryEntry[] entryArray = new GenericDirectoryEntry[header.ItemCount];
            for (int i = 0; i < header.ItemCount; i++)
            {
                GenericDirectoryEntry entry = new GenericDirectoryEntry();
                enReader.ReadUInt32();
                uint num2 = enReader.ReadUInt32();
                byte[] bytes = enReader.ReadBytes((int) num2);
                entry.Name = Encoding.GetEncoding(0x4e4).GetString(bytes);
                enReader.ReadUInt32();
                if (i == 0)
                {
                    entry.Offset = header.DirectoryLength;
                }
                else
                {
                    GenericDirectoryEntry entry2 = entryArray[i - 1];
                    entry.Offset = entry2.Offset + entry2.Length;
                }
                entry.Length = enReader.ReadUInt32();
                enReader.ReadUInt32();
                entryArray[i] = entry;
            }
            return entryArray;
        }

        private static GfsHeader ReadGfsHeader(Stream stream, EndianBinaryReader enReader)
        {
            GfsHeader header = new GfsHeader {
                DirectoryLength = enReader.ReadUInt32(),
                Unknown = enReader.ReadUInt32()
            };
            bool flag = false;
            bool flag2 = false;
            uint num = enReader.ReadUInt32();
            if (num == gfsSignature.Length)
            {
                byte[] buffer = enReader.ReadBytes((int) num);
                flag = ByteCalc.AreArraysEqual(buffer, gfsSignature);
                if (flag)
                {
                    header.FormatName = Encoding.GetEncoding(0x4e4).GetString(buffer);
                    header.Unknown2 = enReader.ReadUInt32();
                    uint num2 = enReader.ReadUInt32();
                    byte[] buffer2 = enReader.ReadBytes((int) num2);
                    flag2 = ByteCalc.AreArraysEqual(buffer2, gfsVersion);
                    if (flag2)
                    {
                        header.FormatVersion = Encoding.GetEncoding(0x4e4).GetString(buffer2);
                        header.Unknown3 = enReader.ReadUInt32();
                        header.ItemCount = enReader.ReadUInt32();
                    }
                }
            }
            if (flag && flag2)
            {
                return header;
            }
            return null;
        }

        public override string[] Extensions
        {
            get
            {
                return new string[] { ".gfs" };
            }
        }

        public override string TypeName
        {
            get
            {
                return "Reverge Package File";
            }
        }

        private class GfsHeader
        {
            public uint DirectoryLength { get; set; }

            public string FormatName { get; set; }

            public string FormatVersion { get; set; }

            public uint ItemCount { get; set; }

            public uint Unknown { get; set; }

            public uint Unknown2 { get; set; }

            public uint Unknown3 { get; set; }
        }
    }
}

