namespace Ravioli.ArchiveInterface
{
    using System;
    using System.IO;

    public abstract class GenericCompressedArchive : GenericArchive
    {
        private BinaryReader uncompressedReader;
        private MemoryStream uncompressedStream;

        protected GenericCompressedArchive()
        {
        }

        protected abstract void DecompressArchive(Stream stream, BinaryReader reader, Stream outputStream);
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
                this.uncompressedReader.Close();
                this.uncompressedReader = null;
            }
            if (this.uncompressedStream != null)
            {
                this.uncompressedStream.Close();
                this.uncompressedStream = null;
            }
        }

        protected override void ReadDirectory(Stream stream, BinaryReader reader)
        {
            this.uncompressedStream = new MemoryStream();
            this.uncompressedReader = new BinaryReader(this.uncompressedStream);
            try
            {
                this.DecompressArchive(stream, reader, this.uncompressedStream);
                this.uncompressedStream.Seek(0L, SeekOrigin.Begin);
                base.Directory = this.ReadGenericDirectory(this.uncompressedStream, this.uncompressedReader);
            }
            catch (Exception)
            {
                if (this.uncompressedReader != null)
                {
                    this.uncompressedReader.Close();
                    this.uncompressedReader = null;
                }
                if (this.uncompressedStream != null)
                {
                    this.uncompressedStream.Close();
                    this.uncompressedStream = null;
                }
                throw;
            }
        }
    }
}

