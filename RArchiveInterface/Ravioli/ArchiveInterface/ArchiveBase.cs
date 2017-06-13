namespace Ravioli.ArchiveInterface
{
    using System;
    using System.IO;

    public abstract class ArchiveBase : IArchive, IClassInfo, IOpenFromStream
    {
        private bool extractCalled;
        private string fileName;
        private bool openedFromStream;
        private BinaryReader reader;
        private Stream stream;

        protected ArchiveBase()
        {
        }

        public void Close()
        {
            try
            {
                this.OnClose();
            }
            catch (Exception)
            {
            }
            if ((this.reader != null) && !this.openedFromStream)
            {
                this.reader.Close();
            }
            this.reader = null;
            if ((this.stream != null) && !this.openedFromStream)
            {
                this.stream.Close();
            }
            this.stream = null;
            this.fileName = null;
            this.openedFromStream = false;
            this.extractCalled = false;
        }

        public void ExtractFile(IFileInfo file, Stream outputStream)
        {
            if (file == null)
            {
                throw new ArgumentNullException("file");
            }
            if (outputStream == null)
            {
                throw new ArgumentNullException("outputStream");
            }
            if (this.stream == null)
            {
                throw new InvalidOperationException("Archive not open.");
            }
            if (!this.extractCalled)
            {
                this.OnFirstExtraction(this.stream, this.reader);
                this.extractCalled = true;
            }
            this.ExtractFile(file, this.stream, outputStream);
        }

        public void ExtractFile(IFileInfo file, string outputFileName)
        {
            if (file == null)
            {
                throw new ArgumentNullException("file");
            }
            if (string.IsNullOrEmpty(outputFileName))
            {
                throw new ArgumentException("No output file name specified.", "outputFileName");
            }
            if (this.stream == null)
            {
                throw new InvalidOperationException("Archive not open.");
            }
            if (!this.extractCalled)
            {
                this.OnFirstExtraction(this.stream, this.reader);
                this.extractCalled = true;
            }
            this.ExtractFile(file, this.stream, outputFileName);
        }

        public void ExtractFile(IFileInfo file, Stream outputStream, long byteCount)
        {
            if (file == null)
            {
                throw new ArgumentNullException("file");
            }
            if (outputStream == null)
            {
                throw new ArgumentNullException("outputStream");
            }
            if (this.stream == null)
            {
                throw new InvalidOperationException("Archive not open.");
            }
            if (byteCount < 0L)
            {
                throw new ArgumentException("byteCount must not be negative.", "byteCount");
            }
            if (!this.extractCalled)
            {
                this.OnFirstExtraction(this.stream, this.reader);
                this.extractCalled = true;
            }
            this.ExtractFile(file, this.stream, outputStream, byteCount);
        }

        protected abstract void ExtractFile(IFileInfo file, Stream stream, Stream outputStream);
        protected abstract void ExtractFile(IFileInfo file, Stream stream, string outputFileName);
        protected abstract void ExtractFile(IFileInfo file, Stream stream, Stream outputStream, long byteCount);
        public bool IsValidFormat(string fileName)
        {
            return this.OpenInternal(fileName, true);
        }

        protected abstract bool IsValidFormat(Stream stream, BinaryReader reader);
        protected abstract void OnClose();
        protected virtual void OnFirstExtraction(Stream stream, BinaryReader reader)
        {
        }

        public void Open(string fileName)
        {
            this.OpenInternal(fileName, false);
        }

        private bool OpenInternal(string fileName, bool checkFormatOnly)
        {
            bool flag;
            if (fileName == null)
            {
                throw new ArgumentNullException("fileName");
            }
            if (fileName.Length == 0)
            {
                throw new ArgumentException("No file name specified.", "fileName");
            }
            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException("File \"" + fileName + "\" does not exist.");
            }
            if (this.stream != null)
            {
                throw new InvalidOperationException("Archive is already open.");
            }
            this.stream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            this.reader = new BinaryReader(this.stream);
            this.fileName = fileName;
            this.openedFromStream = false;
            this.extractCalled = false;
            try
            {
                if (checkFormatOnly)
                {
                    flag = this.IsValidFormat(this.stream, this.reader);
                    this.Close();
                    return flag;
                }
                this.ReadDirectory(this.stream, this.reader);
                flag = true;
            }
            catch (Exception)
            {
                this.Close();
                throw;
            }
            return flag;
        }

        private bool OpenInternal(Stream stream, BinaryReader reader, bool checkFormatOnly, string fileName)
        {
            bool flag;
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }
            if (this.stream != null)
            {
                throw new InvalidOperationException("Archive is already open.");
            }
            this.stream = stream;
            this.reader = reader;
            this.fileName = fileName;
            this.openedFromStream = true;
            this.extractCalled = false;
            try
            {
                if (checkFormatOnly)
                {
                    flag = this.IsValidFormat(this.stream, this.reader);
                    this.Close();
                    return flag;
                }
                this.ReadDirectory(this.stream, this.reader);
                flag = true;
            }
            catch (Exception)
            {
                this.Close();
                throw;
            }
            return flag;
        }

        bool IOpenFromStream.IsValidFormat(Stream stream, BinaryReader reader)
        {
            return this.OpenInternal(stream, reader, true, "");
        }

        void IOpenFromStream.Open(Stream stream, BinaryReader reader)
        {
            this.OpenInternal(stream, reader, false, "");
        }

        void IOpenFromStream.Open(Stream stream, BinaryReader reader, string fileName)
        {
            this.OpenInternal(stream, reader, false, fileName);
        }

        protected abstract void ReadDirectory(Stream stream, BinaryReader reader);

        public virtual string Comment
        {
            get
            {
                return string.Empty;
            }
        }

        public abstract string[] Extensions { get; }

        public string FileName
        {
            get
            {
                return this.fileName;
            }
        }

        public abstract IFileInfo[] Files { get; }

        public abstract string TypeName { get; }
    }
}

