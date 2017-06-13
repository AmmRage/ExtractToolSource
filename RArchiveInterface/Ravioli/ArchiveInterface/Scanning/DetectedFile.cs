namespace Ravioli.ArchiveInterface.Scanning
{
    using System;

    public class DetectedFile
    {
        private bool broken;
        private Ravioli.ArchiveInterface.Scanning.FileType fileType;
        private long length;
        private long offset;

        public DetectedFile(long offset, long length, Ravioli.ArchiveInterface.Scanning.FileType fileType)
        {
            this.offset = offset;
            this.length = length;
            this.fileType = fileType;
            this.broken = false;
        }

        public DetectedFile(long offset, long length, Ravioli.ArchiveInterface.Scanning.FileType fileType, bool broken)
        {
            this.offset = offset;
            this.length = length;
            this.fileType = fileType;
            this.broken = broken;
        }

        public bool Broken
        {
            get
            {
                return this.broken;
            }
            set
            {
                this.broken = value;
            }
        }

        public Ravioli.ArchiveInterface.Scanning.FileType FileType
        {
            get
            {
                return this.fileType;
            }
            set
            {
                this.fileType = value;
            }
        }

        public long Length
        {
            get
            {
                return this.length;
            }
            set
            {
                this.length = value;
            }
        }

        public long Offset
        {
            get
            {
                return this.offset;
            }
            set
            {
                this.offset = value;
            }
        }
    }
}

