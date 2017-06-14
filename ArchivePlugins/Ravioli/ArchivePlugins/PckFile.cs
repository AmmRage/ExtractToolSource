namespace Ravioli.ArchivePlugins
{
    using Ravioli.ArchiveInterface;
    using System;
    using System.IO;
    using System.IO.Compression;
    using System.Text;

    public class PckFile : IArchive, IClassInfo
    {
        private string comment;
        private PckDirectoryEntry[] directory;
        private string fileName;
        private BinaryReader reader;
        private FileStream stream;

        public void Close()
        {
            if (this.reader != null)
            {
                this.reader.Close();
                this.reader = null;
            }
            if (this.stream != null)
            {
                this.stream.Close();
                this.stream = null;
            }
            this.fileName = null;
            this.comment = null;
            this.directory = null;
        }

        private void CopyData(PckDirectoryEntry entry, Stream outputStream, bool dump, long byteCount)
        {
            int num;
            int num2;
            this.stream.Seek((long) entry.FileOffset, SeekOrigin.Begin);
            if (!entry.Compressed || dump)
            {
                num2 = ((byteCount >= 0L) && (byteCount < entry.FileLength)) ? ((int) byteCount) : entry.FileLength;
                num = ExtractUncompressedFile(this.stream, outputStream, num2);
            }
            else
            {
                num2 = ((byteCount >= 0L) && (byteCount < entry.FileLengthUncompressed)) ? ((int) byteCount) : entry.FileLengthUncompressed;
                num = ExtractCompressedFile(this.stream, outputStream, num2);
            }
            if (num != num2)
            {
                throw new InvalidDataException("Extracted file size does not match original size.");
            }
        }

        private static int ExtractCompressedFile(Stream inputStream, Stream outputStream, int length)
        {
            int num = 0;
            int num2 = length;
            DeflateStream stream = new DeflateStream(inputStream, CompressionMode.Decompress, true);
            try
            {
                while (num2 > 0)
                {
                    int count = (num2 > 0x4000) ? 0x4000 : num2;
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

        public void ExtractFile(IFileInfo file, Stream outputStream)
        {
            if (file == null)
            {
                throw new ArgumentNullException("file");
            }
            if (this.stream == null)
            {
                throw new InvalidOperationException("Archive not open.");
            }
            PckDirectoryEntry entry = this.directory[(int) file.ID];
            this.CopyData(entry, outputStream, false, -1L);
        }

        public void ExtractFile(IFileInfo file, string outputFileName)
        {
            if (file == null)
            {
                throw new ArgumentNullException("file");
            }
            if (this.stream == null)
            {
                throw new InvalidOperationException("Archive not open.");
            }
            PckDirectoryEntry entry = this.directory[(int) file.ID];
            bool dump = false;
            if (dump && entry.Compressed)
            {
                outputFileName = outputFileName + ".compressed";
            }
            FileStream outputStream = new FileStream(outputFileName, FileMode.Create, FileAccess.Write);
            try
            {
                this.CopyData(entry, outputStream, dump, -1L);
            }
            catch
            {
                outputStream.Close();
                if (File.Exists(outputFileName))
                {
                    File.Delete(outputFileName);
                }
                throw;
            }
            outputStream.Close();
        }

        public void ExtractFile(IFileInfo file, Stream outputStream, long byteCount)
        {
            if (file == null)
            {
                throw new ArgumentNullException("file");
            }
            if (this.stream == null)
            {
                throw new InvalidOperationException("Archive not open.");
            }
            if (byteCount < 0L)
            {
                throw new ArgumentException("byteCount must not be negative.", "byteCount");
            }
            PckDirectoryEntry entry = this.directory[(int) file.ID];
            this.CopyData(entry, outputStream, false, byteCount);
        }

        private static int ExtractUncompressedFile(Stream inputStream, Stream outputStream, int length)
        {
            int count = length;
            int num2 = 0;
            while (count > 0)
            {
                byte[] buffer;
                int num3;
                if (count > 0x4000)
                {
                    buffer = new byte[0x4000];
                    num3 = inputStream.Read(buffer, 0, 0x4000);
                }
                else
                {
                    buffer = new byte[count];
                    num3 = inputStream.Read(buffer, 0, count);
                }
                if (num3 <= 0)
                {
                    return num2;
                }
                outputStream.Write(buffer, 0, num3);
                count -= num3;
                num2 += num3;
            }
            return num2;
        }

        private bool IsValidFormat()
        {
            this.stream.Seek(0L, SeekOrigin.Begin);
            return (this.reader.ReadInt32() == 0x464b4350);
        }

        public bool IsValidFormat(string fileName)
        {
            return this.OpenInternal(fileName, true);
        }

        public void Open(string fileName)
        {
            this.OpenInternal(fileName, false);
        }

        private bool OpenInternal(string fileName, bool checkFormatOnly)
        {
            if (fileName == null)
            {
                throw new ArgumentNullException("fileName");
            }
            if (fileName.Length == 0)
            {
                throw new ArgumentException("No file name specified.", "fileName");
            }
            if (this.stream != null)
            {
                throw new InvalidOperationException("Archive is already open.");
            }
            bool flag = true;
            this.stream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            this.reader = new BinaryReader(this.stream);
            this.fileName = fileName;
            try
            {
                if (checkFormatOnly)
                {
                    flag = this.IsValidFormat();
                    this.Close();
                    return flag;
                }
                this.ReadDirectory();
            }
            catch (Exception)
            {
                this.Close();
                throw;
            }
            return flag;
        }

        private void ReadDirectory()
        {
            if (!this.IsValidFormat())
            {
                throw new InvalidDataException("This is not a valid PCK file.");
            }
            this.stream.Seek(0L, SeekOrigin.Begin);
            this.reader.ReadInt32();
            byte[] bytes = this.reader.ReadBytes(0x80);
            string str = Encoding.GetEncoding(0x4e4).GetString(bytes).Trim(new char[1]);
            int num = this.reader.ReadInt32();
            PckDirectoryEntry[] entryArray = new PckDirectoryEntry[num];
            for (int i = 0; i < num; i++)
            {
                PckDirectoryEntry entry = new PckDirectoryEntry {
                    FileLength = this.reader.ReadInt32(),
                    FileLengthUncompressed = this.reader.ReadInt32(),
                    FileOffset = this.reader.ReadInt32(),
                    FileNameLength = this.reader.ReadInt32(),
                    Compressed = this.reader.ReadInt32() != 0
                };
                byte[] buffer2 = this.reader.ReadBytes(entry.FileNameLength);
                entry.FileName = Encoding.GetEncoding(0x4e4).GetString(buffer2);
                entryArray[i] = entry;
            }
            this.comment = str;
            this.directory = entryArray;
        }

        public string Comment
        {
            get
            {
                return this.comment;
            }
        }

        public string[] Extensions
        {
            get
            {
                return new string[] { ".pck" };
            }
        }

        public string FileName
        {
            get
            {
                return this.fileName;
            }
        }

        public IFileInfo[] Files
        {
            get
            {
                CompressedFileInfo[] infoArray = new CompressedFileInfo[this.directory.Length];
                for (int i = 0; i < this.directory.Length; i++)
                {
                    PckDirectoryEntry entry = this.directory[i];
                    infoArray[i] = new CompressedFileInfo((long) i, entry.FileName, (long) entry.FileLengthUncompressed, entry.Compressed, (long) entry.FileLength);
                }
                return infoArray;
            }
        }

        public string TypeName
        {
            get
            {
                return "In The Groove PCK File";
            }
        }
    }
}

