namespace Ravioli.ArchiveInterface
{
    using System;

    public class CompressedFileInfo : ArchiveFileInfo, ICompressionInfo
    {
        private bool compressed;
        private long compressedSize;

        public CompressedFileInfo(long id, string name, long size, bool compressed, long compressedSize) : base(id, name, size)
        {
            this.compressed = compressed;
            this.compressedSize = compressedSize;
        }

        public bool Compressed
        {
            get
            {
                return this.compressed;
            }
        }

        public long CompressedSize
        {
            get
            {
                return this.compressedSize;
            }
        }
    }
}

