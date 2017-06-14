namespace Ravioli.ArchivePlugins
{
    using Ravioli.ArchiveInterface;
    using Ravioli.PluginHelpers;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;

    public class ResFile : ArchiveBase, IPaletteProvider
    {
        private IList<CompressedDirectoryEntry> directory;
        private int fileVersion;
        private byte[] paletteBytes;

        private void CopyData(CompressedDirectoryEntry entry, Stream stream, Stream outputStream, long byteCount)
        {
            bool flag;
            this.CopyData(entry, stream, outputStream, byteCount, out flag);
        }

        private void CopyData(CompressedDirectoryEntry entry, Stream stream, Stream outputStream, long byteCount, out bool compressed)
        {
            Stream stream2 = null;
            compressed = false;
            try
            {
                long num;
                stream.Position = entry.Offset;
                compressed = this.fileVersion == 2;
                if (compressed)
                {
                    stream2 = new MemoryStream();
                    Compression.DecompressLZSSStream(stream, entry.CompressedLength, stream2);
                    stream2.Position = 0L;
                    num = ((byteCount >= 0L) && (byteCount < stream2.Length)) ? byteCount : stream2.Length;
                }
                else
                {
                    stream2 = stream;
                    num = ((byteCount >= 0L) && (byteCount < entry.Length)) ? byteCount : entry.Length;
                }
                byte[] buffer = new byte[0x2000];
                while (num > 0L)
                {
                    int count = (num > 0x2000L) ? 0x2000 : ((int) num);
                    int num3 = stream2.Read(buffer, 0, count);
                    outputStream.Write(buffer, 0, num3);
                    num -= num3;
                    if (num3 == 0)
                    {
                        throw new EndOfStreamException("End of stream reached too early while extracting file.");
                    }
                }
            }
            finally
            {
                if (compressed && (stream2 != null))
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
            CompressedDirectoryEntry entry = this.directory[(int) file.ID];
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
            bool flag = false;
            this.fileVersion = 0;
            stream.Seek(0L, SeekOrigin.Begin);
            byte[] buffer = new byte[0x1f];
            if (stream.Read(buffer, 0, 15) == 15)
            {
                if (Encoding.GetEncoding(0x4e4).GetString(buffer, 0, 15) == "Resource File\r\n")
                {
                    flag = true;
                    this.fileVersion = 1;
                    return flag;
                }
                if ((stream.Read(buffer, 15, 0x10) == 0x10) && (Encoding.GetEncoding(0x4e4).GetString(buffer, 0, 0x1f) == "Absolute Magic Resource File!\r\n"))
                {
                    flag = true;
                    this.fileVersion = 2;
                }
            }
            return flag;
        }

        protected override void OnClose()
        {
            this.directory = null;
            this.paletteBytes = null;
        }

        protected override void ReadDirectory(Stream stream, BinaryReader reader)
        {
            if (!this.IsValidFormat(stream, reader))
            {
                throw new InvalidDataException("This is not a valid RES file.");
            }
            List<CompressedDirectoryEntry> list = new List<CompressedDirectoryEntry>();
            CompressedDirectoryEntry entry = null;
            reader.ReadUInt32();
            uint num = reader.ReadUInt32();
            stream.Position = num;
            while (stream.Position < stream.Length)
            {
                byte count = reader.ReadByte();
                byte[] bytes = reader.ReadBytes(12);
                string name = Encoding.GetEncoding(0x4e4).GetString(bytes, 0, count);
                reader.ReadByte();
                reader.ReadUInt32();
                uint num3 = reader.ReadUInt32();
                uint num4 = reader.ReadUInt32();
                CompressedDirectoryEntry item = new CompressedDirectoryEntry(name, (long) num4, (long) num3, false, (long) num3);
                list.Add(item);
                if (name.EndsWith(".PAL", StringComparison.OrdinalIgnoreCase))
                {
                    entry = item;
                }
            }
            foreach (CompressedDirectoryEntry entry3 in list)
            {
                MemoryStream outputStream = new MemoryStream();
                try
                {
                    bool flag;
                    this.CopyData(entry3, stream, outputStream, -1L, out flag);
                    if (flag)
                    {
                        entry3.Length = outputStream.Length;
                        entry3.Compressed = true;
                    }
                }
                finally
                {
                    outputStream.Close();
                }
            }
            byte[] buffer2 = null;
            if (entry != null)
            {
                MemoryStream stream3 = new MemoryStream();
                try
                {
                    this.CopyData(entry, stream, stream3, -1L);
                    buffer2 = stream3.ToArray();
                }
                finally
                {
                    stream3.Close();
                }
            }
            this.directory = list;
            this.paletteBytes = buffer2;
        }

        public override string[] Extensions
        {
            get
            {
                return new string[] { ".res" };
            }
        }

        public override IFileInfo[] Files
        {
            get
            {
                ArchiveFileInfo[] infoArray = new ArchiveFileInfo[this.directory.Count];
                for (int i = 0; i < this.directory.Count; i++)
                {
                    CompressedDirectoryEntry entry = this.directory[i];
                    infoArray[i] = new CompressedFileInfo((long) i, entry.Name, entry.Length, entry.Compressed, entry.CompressedLength);
                }
                return infoArray;
            }
        }

        public byte[] Palette
        {
            get
            {
                return this.paletteBytes;
            }
        }

        public override string TypeName
        {
            get
            {
                return "Absolute Magic Resource File";
            }
        }
    }
}

