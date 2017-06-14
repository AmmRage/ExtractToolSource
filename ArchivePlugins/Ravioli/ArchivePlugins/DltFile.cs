namespace Ravioli.ArchivePlugins
{
    using Ravioli.ArchiveInterface;
    using Ravioli.PluginHelpers.Reading;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;

    public class DltFile : ArchiveBase
    {
        private IList<CompressedDirectoryEntry> directory;

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
                long num2;
                stream.Position = entry.Offset;
                uint num = StaticReader.ReadUInt32(stream);
                compressed = num == 0x50424750;
                stream.Position = entry.Offset;
                if (compressed)
                {
                    stream2 = new MemoryStream();
                    DltDecompressor.Decompress(stream, stream2);
                    stream2.Position = 0L;
                    num2 = ((byteCount >= 0L) && (byteCount < stream2.Length)) ? byteCount : stream2.Length;
                }
                else
                {
                    stream2 = stream;
                    num2 = ((byteCount >= 0L) && (byteCount < entry.Length)) ? byteCount : entry.Length;
                }
                byte[] buffer = new byte[0x2000];
                while (num2 > 0L)
                {
                    int count = (num2 > 0x2000L) ? 0x2000 : ((int) num2);
                    int num4 = stream2.Read(buffer, 0, count);
                    outputStream.Write(buffer, 0, num4);
                    num2 -= num4;
                    if (num4 == 0)
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
            stream.Seek(0L, SeekOrigin.Begin);
            return (reader.ReadUInt32() == 0x45564144);
        }

        protected override void OnClose()
        {
            this.directory = null;
        }

        protected override void ReadDirectory(Stream stream, BinaryReader reader)
        {
            if (!this.IsValidFormat(stream, reader))
            {
                throw new InvalidDataException("This is not a valid DLT file.");
            }
            reader.ReadUInt16();
            uint num = reader.ReadUInt16();
            CompressedDirectoryEntry[] entryArray = new CompressedDirectoryEntry[num];
            for (int i = 0; i < num; i++)
            {
                byte[] bytes = reader.ReadBytes(0x20);
                reader.ReadUInt32();
                uint num3 = reader.ReadUInt32();
                for (int j = 1; j < 0x20; j++)
                {
                    bytes[j] = (byte) ((bytes[j - 1] + j) ^ bytes[j]);
                }
                string name = Encoding.GetEncoding(0x4e4).GetString(bytes).TrimEnd(new char[1]);
                entryArray[i] = new CompressedDirectoryEntry(name, stream.Position, (long) num3, false, (long) num3);
                stream.Position += num3;
            }
            foreach (CompressedDirectoryEntry entry in entryArray)
            {
                MemoryStream outputStream = new MemoryStream();
                try
                {
                    bool flag;
                    this.CopyData(entry, stream, outputStream, -1L, out flag);
                    if (flag)
                    {
                        entry.Length = outputStream.Length;
                        entry.Compressed = true;
                    }
                }
                finally
                {
                    outputStream.Close();
                }
            }
            this.directory = entryArray;
        }

        public override string[] Extensions
        {
            get
            {
                return new string[] { ".dlt" };
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
                return "Stargunner DLT File";
            }
        }
    }
}

