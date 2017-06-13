namespace Ravioli.ArchiveInterface
{
    using System;
    using System.IO;

    public abstract class CompressedItemsArchive : ArchiveBase
    {
        private CompressedDirectoryEntry[] directory;

        protected CompressedItemsArchive()
        {
        }

        private void CopyData(CompressedDirectoryEntry entry, Stream stream, Stream outputStream, long byteCount)
        {
            stream.Position = entry.Offset;
            long num = ((byteCount >= 0L) && (byteCount < entry.Length)) ? byteCount : entry.Length;
            if (entry.Compressed)
            {
                if (this.DecompressFile(stream, outputStream, num) < num)
                {
                    throw new EndOfStreamException("End of stream reached too early while extracting file.");
                }
            }
            else
            {
                byte[] buffer = new byte[0x2000];
                while (num > 0L)
                {
                    int count = (num > 0x2000L) ? 0x2000 : ((int) num);
                    int num4 = stream.Read(buffer, 0, count);
                    outputStream.Write(buffer, 0, num4);
                    num -= num4;
                    if (num4 == 0)
                    {
                        throw new EndOfStreamException("End of stream reached too early while extracting file.");
                    }
                }
            }
        }

        protected abstract long DecompressFile(Stream stream, Stream outputStream, long byteCount);
        protected override void ExtractFile(IFileInfo file, Stream stream, Stream outputStream)
        {
            this.CopyData(this.directory[(int) ((IntPtr) file.ID)], stream, outputStream, -1L);
        }

        protected override void ExtractFile(IFileInfo file, Stream stream, string outputFileName)
        {
            CompressedDirectoryEntry entry = this.directory[(int) ((IntPtr) file.ID)];
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
            this.CopyData(this.directory[(int) ((IntPtr) file.ID)], stream, outputStream, byteCount);
        }

        protected override void OnClose()
        {
            this.directory = null;
        }

        protected abstract CompressedDirectoryEntry[] ReadCompressedDirectory(Stream stream, BinaryReader reader);
        protected override void ReadDirectory(Stream stream, BinaryReader reader)
        {
            this.directory = this.ReadCompressedDirectory(stream, reader);
        }

        protected CompressedDirectoryEntry[] Directory
        {
            get
            {
                return this.directory;
            }
            set
            {
                this.directory = value;
            }
        }

        public override IFileInfo[] Files
        {
            get
            {
                CompressedFileInfo[] infoArray = new CompressedFileInfo[this.directory.Length];
                for (int i = 0; i < this.directory.Length; i++)
                {
                    CompressedDirectoryEntry entry = this.directory[i];
                    string name = entry.Name;
                    long length = entry.Length;
                    infoArray[i] = new CompressedFileInfo((long) i, name, length, entry.Compressed, entry.CompressedLength);
                }
                return infoArray;
            }
        }
    }
}

