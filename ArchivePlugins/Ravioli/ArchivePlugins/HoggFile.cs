namespace Ravioli.ArchivePlugins
{
    using Ravioli.ArchiveInterface;
    using Ravioli.PluginHelpers;
    using Ravioli.PluginHelpers.Reading;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Text;

    public class HoggFile : ArchiveBase
    {
        private IList<HoggDirectoryEntry> directory;

        private static bool CopyBuffered(Stream stream, long length, Stream outputStream)
        {
            byte[] buffer = new byte[0x2000];
            long num = length;
            while (num > 0L)
            {
                int count = (num > 0x2000L) ? 0x2000 : ((int) num);
                int num3 = stream.Read(buffer, 0, count);
                outputStream.Write(buffer, 0, num3);
                num -= num3;
                if (num3 == 0)
                {
                    break;
                }
            }
            return (num == 0L);
        }

        private void CopyData(HoggDirectoryEntry entry, Stream stream, Stream outputStream, long byteCount)
        {
            Stream stream2 = null;
            bool flag = false;
            try
            {
                long num2;
                stream.Position = entry.Offset;
                ushort num = StaticReader.ReadUInt16(stream);
                flag = ((num == 0x5e78) || (num == 0x9c78)) || (num == 0xda78);
                stream.Position = entry.Offset;
                if (flag)
                {
                    stream2 = new MemoryStream();
                    long outputLength = ((byteCount >= 0L) && (byteCount < entry.Length)) ? byteCount : entry.Length;
                    Compression.DecompressZlibStream(stream, stream2, outputLength);
                    stream2.Position = 0L;
                    num2 = ((byteCount >= 0L) && (byteCount < stream2.Length)) ? byteCount : stream2.Length;
                }
                else
                {
                    stream2 = stream;
                    num2 = ((byteCount >= 0L) && (byteCount < entry.Length)) ? byteCount : entry.Length;
                }
                if (!CopyBuffered(stream2, num2, outputStream))
                {
                    throw new EndOfStreamException("End of stream reached too early while extracting file.");
                }
            }
            finally
            {
                if (flag && (stream2 != null))
                {
                    stream2.Close();
                    stream2 = null;
                }
            }
        }

        protected override void ExtractFile(IFileInfo file, Stream stream, Stream outputStream)
        {
            this.CopyData(this.directory[(int) file.ID], stream, outputStream, -1L);
        }

        protected override void ExtractFile(IFileInfo file, Stream stream, string outputFileName)
        {
            HoggDirectoryEntry entry = this.directory[(int) file.ID];
            FileStream outputStream = new FileStream(outputFileName, FileMode.Create, FileAccess.Write);
            try
            {
                this.CopyData(entry, stream, outputStream, -1L);
                outputStream.Close();
            }
            catch (Exception)
            {
                outputStream.Close();
                if (File.Exists(outputFileName))
                {
                    File.Delete(outputFileName);
                }
                throw;
            }
        }

        protected override void ExtractFile(IFileInfo file, Stream stream, Stream outputStream, long byteCount)
        {
            this.CopyData(this.directory[(int) file.ID], stream, outputStream, byteCount);
        }

        protected override bool IsValidFormat(Stream stream, BinaryReader reader)
        {
            stream.Seek(0L, SeekOrigin.Begin);
            return (reader.ReadUInt32() == 0xdeadf00d);
        }

        protected override void OnClose()
        {
            this.directory = null;
        }

        protected override void ReadDirectory(Stream stream, BinaryReader reader)
        {
            if (!this.IsValidFormat(stream, reader))
            {
                throw new InvalidDataException("This is not a valid HOGG file.");
            }
            stream.Seek(0L, SeekOrigin.Begin);
            reader.ReadUInt32();
            reader.ReadUInt16();
            uint num = reader.ReadUInt16();
            uint num2 = reader.ReadUInt32();
            uint num3 = reader.ReadUInt32();
            reader.ReadUInt32();
            uint num4 = reader.ReadUInt32();
            stream.Position += num;
            long position = stream.Position;
            stream.Position += num4;
            long num6 = stream.Position;
            List<HoggDirectoryEntry> list = new List<HoggDirectoryEntry>();
            while (stream.Position < (num6 + num2))
            {
                HoggDirectoryEntry item = new HoggDirectoryEntry {
                    Offset = (long) reader.ReadUInt32()
                };
                reader.ReadUInt32();
                item.Length = reader.ReadUInt32();
                item.CompressedLength = item.Length;
                reader.ReadUInt32();
                reader.ReadUInt32();
                reader.ReadUInt32();
                reader.ReadUInt32();
                item.Index = reader.ReadUInt32();
                item.Name = item.Index.ToString("D5");
                if ((item.Offset != 0L) && (item.Offset != 0xffffffffL))
                {
                    list.Add(item);
                }
            }
            long num7 = stream.Position;
            List<HoggLastPartEntry> list2 = new List<HoggLastPartEntry>();
            while (stream.Position < (num7 + num3))
            {
                HoggLastPartEntry entry2 = new HoggLastPartEntry {
                    Index = reader.ReadUInt32(),
                    Unknown8 = reader.ReadUInt32(),
                    UncompressedLength = reader.ReadUInt32(),
                    Unknown9 = reader.ReadUInt32()
                };
                if (entry2.Unknown8 != 0)
                {
                    list2.Add(entry2);
                }
            }
            for (int i = 0; i < list.Count; i++)
            {
                HoggLastPartEntry entry3 = list2[i];
                if ((entry3.Unknown8 != 0) && (entry3.UncompressedLength > 0))
                {
                    HoggDirectoryEntry entry4 = list[i];
                    entry4.Compressed = true;
                    entry4.Length = entry3.UncompressedLength;
                }
            }
            Dictionary<uint, byte[]> dictionary = new Dictionary<uint, byte[]>();
            stream.Position = position;
            reader.ReadUInt32();
            reader.ReadUInt32();
            uint num9 = reader.ReadUInt32();
            while (stream.Position < (position + num9))
            {
                byte num10 = reader.ReadByte();
                if (num10 >= 1)
                {
                    uint key = reader.ReadUInt32();
                    while (num10 > 1)
                    {
                        num10 = reader.ReadByte();
                        key = reader.ReadUInt32();
                    }
                    uint num12 = reader.ReadUInt32();
                    byte[] buffer = reader.ReadBytes((int) num12);
                    dictionary.Add(key, buffer);
                }
            }
            HoggDirectoryEntry entry5 = list[0];
            list.RemoveAt(0);
            list2.RemoveAt(0);
            MemoryStream outputStream = new MemoryStream();
            try
            {
                this.CopyData(entry5, stream, outputStream, -1L);
                outputStream.Position = 0L;
                BinaryReader reader2 = new BinaryReader(outputStream);
                try
                {
                    reader2.ReadUInt32();
                    uint num13 = reader2.ReadUInt32();
                    if (num13 > 0)
                    {
                        uint num14 = reader2.ReadUInt32();
                        byte[] bytes = reader2.ReadBytes((int) num14);
                        if (Encoding.GetEncoding(0x4e4).GetString(bytes, 0, ((int) num14) - 1) == "?DataList")
                        {
                            for (uint k = 1; k < num13; k++)
                            {
                                uint num16 = reader2.ReadUInt32();
                                byte[] buffer3 = reader2.ReadBytes((int) num16);
                                if (!dictionary.ContainsKey(k))
                                {
                                    dictionary.Add(k, buffer3);
                                }
                            }
                        }
                    }
                }
                finally
                {
                    reader2.Close();
                }
            }
            finally
            {
                outputStream.Close();
            }
            for (int j = 0; j < list.Count; j++)
            {
                HoggLastPartEntry entry6 = list2[j];
                byte[] buffer4 = dictionary[entry6.Index];
                HoggDirectoryEntry entry7 = list[j];
                entry7.Name = Encoding.GetEncoding(0x4e4).GetString(buffer4, 0, buffer4.Length - 1);
            }
            this.directory = list;
        }

        public override string[] Extensions
        {
            get
            {
                return new string[] { ".hogg" };
            }
        }

        public override IFileInfo[] Files
        {
            get
            {
                CompressedFileInfo[] infoArray = new CompressedFileInfo[this.directory.Count];
                for (int i = 0; i < this.directory.Count; i++)
                {
                    CompressedDirectoryEntry entry = this.directory[i];
                    infoArray[i] = new CompressedFileInfo((long) i, entry.Name, entry.Length, entry.Compressed, entry.CompressedLength);
                }
                return infoArray;
            }
        }

        public override string TypeName
        {
            get
            {
                return "Star Trek Online HOGG File";
            }
        }

        private class HoggDirectoryEntry : CompressedDirectoryEntry
        {
            public uint Index { get; set; }
        }

        private class HoggLastPartEntry
        {
            public uint Index { get; set; }

            public uint UncompressedLength { get; set; }

            public uint Unknown8 { get; set; }

            public uint Unknown9 { get; set; }
        }
    }
}

