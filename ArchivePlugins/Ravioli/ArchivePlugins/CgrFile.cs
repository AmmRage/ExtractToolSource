namespace Ravioli.ArchivePlugins
{
    using Ravioli.ArchiveInterface;
    using Ravioli.PluginHelpers.Reading;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;

    public class CgrFile : ArchiveBase
    {
        private long compressedLength;
        private long compressedOffset;
        private long decompressedLength;
        private List<GenericDirectoryEntry> directory;
        private bool isCompressed;

        private void CopyData(GenericDirectoryEntry entry, Stream stream, Stream outputStream, long byteCount)
        {
            Stream stream2 = null;
            try
            {
                if (this.isCompressed)
                {
                    stream2 = new MemoryStream();
                    stream.Position = this.compressedOffset;
                    if (DecompressZlibStream(stream, stream2, this.decompressedLength) != this.decompressedLength)
                    {
                        throw new IOException("Decompressed output length does not match expected length.");
                    }
                }
                else
                {
                    stream2 = stream;
                }
                stream2.Seek(entry.Offset, SeekOrigin.Begin);
                long num2 = ((byteCount >= 0L) && (byteCount < entry.Length)) ? byteCount : entry.Length;
                byte[] buffer = new byte[0x2000];
                while (num2 > 0L)
                {
                    int count = (num2 > 0x2000L) ? 0x2000 : ((int) num2);
                    int num4 = stream2.Read(buffer, 0, count);
                    outputStream.Write(buffer, 0, num4);
                    num2 -= num4;
                }
            }
            finally
            {
                if (this.isCompressed && (stream2 != null))
                {
                    stream2.Close();
                    stream2 = null;
                }
            }
        }

        private static long DecompressDeflateStream(Stream inputStream, Stream outputStream, long length)
        {
            long num = 0L;
            long num2 = length;
            DeflateStream stream = new DeflateStream(inputStream, CompressionMode.Decompress, true);
            try
            {
                while (num2 > 0L)
                {
                    int count = (num2 > 0x2000L) ? 0x2000 : ((int) num2);
                    byte[] buffer = new byte[count];
                    int num4 = stream.Read(buffer, 0, count);
                    if (num4 <= 0)
                    {
                        return num;
                    }
                    outputStream.Write(buffer, 0, num4);
                    num += num4;
                    num2 -= num4;
                }
            }
            finally
            {
                stream.Close();
            }
            return num;
        }

        private static long DecompressZlibStream(Stream inputStream, Stream outputStream, long outputLength)
        {
            int num = inputStream.ReadByte();
            inputStream.ReadByte();
            if ((num & 15) != 8)
            {
                throw new NotSupportedException("Unsupported compression method in data stream.");
            }
            return DecompressDeflateStream(inputStream, outputStream, outputLength);
        }

        protected override void ExtractFile(IFileInfo file, Stream stream, Stream outputStream)
        {
            this.CopyData(this.directory[(int) file.ID], stream, outputStream, -1L);
        }

        protected override void ExtractFile(IFileInfo file, Stream stream, string outputFileName)
        {
            GenericDirectoryEntry entry = this.directory[(int) file.ID];
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

        private void HandleDecompressedData(Stream data)
        {
            this.directory = new List<GenericDirectoryEntry>();
            data.Position = 0L;
            while (data.Position < data.Length)
            {
                uint num = StaticReader.ReadUInt32(data);
                uint num2 = StaticReader.ReadUInt32(data);
                switch (num)
                {
                    case 0x53525651:
                    {
                        data.Position += 8L;
                        continue;
                    }
                    case 0x56465542:
                    {
                        long position = data.Position;
                        string extension = string.Empty;
                        if (num2 >= 4)
                        {
                            uint num4 = StaticReader.ReadUInt32(data);
                            if (num4 == 0x5367674f)
                            {
                                extension = ".ogg";
                            }
                            else if ((num4 & 0x334449) == 0x334449)
                            {
                                extension = ".mp3";
                            }
                            data.Position += num2 - 4;
                        }
                        if (extension.Length == 0)
                        {
                            extension = ".dat";
                            data.Position += num2;
                        }
                        string name = Path.ChangeExtension(Path.GetFileName(base.FileName), extension);
                        this.directory.Add(new GenericDirectoryEntry(name, position, (long) num2));
                        return;
                    }
                }
                data.Position += num2;
            }
        }

        protected override bool IsValidFormat(Stream stream, BinaryReader reader)
        {
            stream.Seek(0L, SeekOrigin.Begin);
            uint num = reader.ReadUInt32();
            if (num != 0x46544341)
            {
                return (num == 0x53525651);
            }
            return true;
        }

        protected override void OnClose()
        {
            this.directory = null;
            this.isCompressed = false;
            this.compressedOffset = 0L;
            this.compressedLength = 0L;
            this.decompressedLength = 0L;
        }

        protected override void ReadDirectory(Stream stream, BinaryReader reader)
        {
            if (!this.IsValidFormat(stream, reader))
            {
                throw new InvalidDataException("This is not a valid CGR file.");
            }
            bool flag = false;
            uint num = 0;
            bool flag2 = false;
            stream.Seek(0L, SeekOrigin.Begin);
            while (stream.Position < stream.Length)
            {
                uint num2 = reader.ReadUInt32();
                uint num3 = reader.ReadUInt32();
                if (num2 == 0x53525651)
                {
                    this.isCompressed = false;
                    this.compressedLength = 0L;
                    this.compressedOffset = 0L;
                    this.decompressedLength = stream.Length;
                    this.HandleDecompressedData(stream);
                    flag2 = true;
                    break;
                }
                if (num2 == 0x534f495a)
                {
                    num = reader.ReadUInt32();
                    flag = true;
                }
                else
                {
                    if (num2 == 0x4243495a)
                    {
                        if (!flag)
                        {
                            throw new InvalidDataException("Invalid data - output size is unknown.");
                        }
                        this.isCompressed = true;
                        this.compressedOffset = stream.Position;
                        this.compressedLength = num3;
                        this.decompressedLength = num;
                        MemoryStream outputStream = new MemoryStream();
                        try
                        {
                            if (DecompressZlibStream(stream, outputStream, (long) num) != num)
                            {
                                throw new IOException("Decompressed output length does not match expected length.");
                            }
                            this.HandleDecompressedData(outputStream);
                            flag2 = true;
                            break;
                        }
                        finally
                        {
                            outputStream.Close();
                            outputStream = null;
                        }
                    }
                    if (stream.Seek((long) num3, SeekOrigin.Current) < num3)
                    {
                        throw new InvalidDataException("Invalid data - reading beyond end of file.");
                    }
                }
            }
            if (!flag2)
            {
                throw new InvalidDataException("Invalid data - No compressed data available.");
            }
        }

        public override string[] Extensions
        {
            get
            {
                return new string[] { ".cgr" };
            }
        }

        public override IFileInfo[] Files
        {
            get
            {
                ArchiveFileInfo[] infoArray = new ArchiveFileInfo[this.directory.Count];
                for (int i = 0; i < this.directory.Count; i++)
                {
                    GenericDirectoryEntry entry = this.directory[i];
                    infoArray[i] = new ArchiveFileInfo((long) i, entry.Name, entry.Length);
                }
                return infoArray;
            }
        }

        public override string TypeName
        {
            get
            {
                return "Audiosurf CGR File";
            }
        }
    }
}

