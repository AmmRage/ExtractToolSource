namespace Ravioli.ArchivePlugins
{
    using System;

    internal class PckDirectoryEntry
    {
        private bool compressed;
        private int fileLength;
        private int fileLengthUncompressed;
        private string fileName;
        private int fileNameLength;
        private int fileOffset;

        public bool Compressed
        {
            get
            {
                return this.compressed;
            }
            set
            {
                this.compressed = value;
            }
        }

        public int FileLength
        {
            get
            {
                return this.fileLength;
            }
            set
            {
                this.fileLength = value;
            }
        }

        public int FileLengthUncompressed
        {
            get
            {
                return this.fileLengthUncompressed;
            }
            set
            {
                this.fileLengthUncompressed = value;
            }
        }

        public string FileName
        {
            get
            {
                return this.fileName;
            }
            set
            {
                this.fileName = value;
            }
        }

        public int FileNameLength
        {
            get
            {
                return this.fileNameLength;
            }
            set
            {
                this.fileNameLength = value;
            }
        }

        public int FileOffset
        {
            get
            {
                return this.fileOffset;
            }
            set
            {
                this.fileOffset = value;
            }
        }
    }
}

