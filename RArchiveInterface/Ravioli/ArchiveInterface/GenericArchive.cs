namespace Ravioli.ArchiveInterface
{
    using System;
    using System.IO;

    public abstract class GenericArchive : ArchiveBase
    {
        private GenericDirectoryEntry[] directory;

        protected GenericArchive()
        {
        }

        protected void CopyData(GenericDirectoryEntry entry, Stream stream, Stream outputStream, long byteCount)
        {
            int num = this.OnExtracting(entry, stream, outputStream);
            stream.Seek(entry.Offset, SeekOrigin.Begin);
            long num2 = ((byteCount >= 0L) && (byteCount < entry.Length)) ? (byteCount - num) : entry.Length;
            byte[] buffer = new byte[0x2000];
            while (num2 > 0L)
            {
                int count = (num2 > 0x2000L) ? 0x2000 : ((int) num2);
                int num4 = stream.Read(buffer, 0, count);
                outputStream.Write(buffer, 0, num4);
                num2 -= num4;
                if (num4 == 0)
                {
                    throw new EndOfStreamException("End of stream reached too early while extracting file.");
                }
            }
            this.OnExtracted(entry, stream, outputStream);
        }

        protected override void ExtractFile(IFileInfo file, Stream stream, Stream outputStream)
        {
            this.CopyData(this.directory[(int) ((IntPtr) file.ID)], stream, outputStream, -1L);
        }

        protected override void ExtractFile(IFileInfo file, Stream stream, string outputFileName)
        {
            GenericDirectoryEntry entry = this.directory[(int) ((IntPtr) file.ID)];
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

        protected virtual void OnExtracted(GenericDirectoryEntry entry, Stream stream, Stream outputStream)
        {
        }

        protected virtual int OnExtracting(GenericDirectoryEntry entry, Stream stream, Stream outputStream)
        {
            return 0;
        }

        protected virtual void OnFileInfo(GenericDirectoryEntry entry, ref string name, ref long size)
        {
        }

        protected override void ReadDirectory(Stream stream, BinaryReader reader)
        {
            this.directory = this.ReadGenericDirectory(stream, reader);
        }

        protected abstract GenericDirectoryEntry[] ReadGenericDirectory(Stream stream, BinaryReader reader);

        protected GenericDirectoryEntry[] Directory
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
                ArchiveFileInfo[] infoArray = new ArchiveFileInfo[this.directory.Length];
                for (int i = 0; i < this.directory.Length; i++)
                {
                    GenericDirectoryEntry entry = this.directory[i];
                    string name = entry.Name;
                    long length = entry.Length;
                    this.OnFileInfo(entry, ref name, ref length);
                    infoArray[i] = new ArchiveFileInfo((long) i, name, length);
                }
                return infoArray;
            }
        }
    }
}

