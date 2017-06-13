namespace Ravioli.ArchiveInterface
{
    using System;
    using System.IO;

    public abstract class GenericCompressedArchive2 : GenericArchive
    {
        private bool compressed;
        private long decompressionOffset;
        private BinaryReader uncompressedReader;
        private Stream uncompressedStream;

        protected GenericCompressedArchive2()
        {
        }

        protected abstract void DecompressArchive(Stream stream, BinaryReader reader, Stream outputStream, long byteCount);
        protected override void ExtractFile(IFileInfo file, Stream stream, Stream outputStream)
        {
            base.CopyData(base.Directory[(int) ((IntPtr) file.ID)], this.uncompressedStream, outputStream, -1L);
        }

        protected override void ExtractFile(IFileInfo file, Stream stream, string outputFileName)
        {
            GenericDirectoryEntry entry = base.Directory[(int) ((IntPtr) file.ID)];
            FileStream outputStream = new FileStream(outputFileName, FileMode.Create, FileAccess.Write);
            try
            {
                base.CopyData(entry, this.uncompressedStream, outputStream, -1L);
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
            base.CopyData(base.Directory[(int) ((IntPtr) file.ID)], this.uncompressedStream, outputStream, byteCount);
        }

        protected override void OnClose()
        {
            base.Directory = null;
            if (this.uncompressedReader != null)
            {
                if (this.compressed)
                {
                    this.uncompressedReader.Close();
                }
                this.uncompressedReader = null;
            }
            if (this.uncompressedStream != null)
            {
                if (this.compressed)
                {
                    this.uncompressedStream.Close();
                }
                this.uncompressedStream = null;
            }
            this.compressed = false;
            this.decompressionOffset = 0L;
        }

        protected override void OnFirstExtraction(Stream stream, BinaryReader reader)
        {
            stream.Position = this.decompressionOffset;
            this.uncompressedStream = new MemoryStream();
            this.uncompressedReader = new BinaryReader(this.uncompressedStream);
            this.compressed = true;
            try
            {
                this.DecompressArchive(stream, reader, this.uncompressedStream, this.UncompressedArchiveLength);
                this.uncompressedStream.Seek(0L, SeekOrigin.Begin);
                if (this.uncompressedStream.Length == 0L)
                {
                    this.uncompressedReader.Close();
                    this.uncompressedReader = null;
                    this.uncompressedStream.Close();
                    this.uncompressedStream = null;
                    this.compressed = false;
                    this.uncompressedStream = stream;
                    this.uncompressedReader = reader;
                }
            }
            catch (Exception)
            {
                if (this.uncompressedReader != null)
                {
                    if (this.compressed)
                    {
                        this.uncompressedReader.Close();
                    }
                    this.uncompressedReader = null;
                }
                if (this.uncompressedStream != null)
                {
                    if (this.compressed)
                    {
                        this.uncompressedStream.Close();
                    }
                    this.uncompressedStream = null;
                }
                this.compressed = false;
                throw;
            }
        }

        protected override void ReadDirectory(Stream stream, BinaryReader reader)
        {
            base.Directory = this.ReadGenericDirectory(stream, reader);
            this.uncompressedStream = null;
            this.uncompressedReader = null;
            this.compressed = false;
            this.decompressionOffset = stream.Position;
        }

        protected abstract long UncompressedArchiveLength { get; }
    }
}

