namespace Ravioli.ArchiveInterface
{
    using System;

    public class CompressedDirectoryEntry : GenericDirectoryEntry
    {
        private bool compressed;
        private long compressedLength;

        public CompressedDirectoryEntry()
        {
        }

        public CompressedDirectoryEntry(string name, long offset, long length, bool compressed, long compressedLength) : base(name, offset, length)
        {
            this.compressed = compressed;
            this.compressedLength = compressedLength;
        }

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

        public long CompressedLength
        {
            get
            {
                return this.compressedLength;
            }
            set
            {
                this.compressedLength = value;
            }
        }
    }
}

