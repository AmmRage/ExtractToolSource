namespace Ravioli.ArchiveInterface
{
    using System;

    public class GenericDirectoryEntryWithHeader : GenericDirectoryEntry
    {
        private byte[] header;

        public GenericDirectoryEntryWithHeader()
        {
            this.header = null;
        }

        public GenericDirectoryEntryWithHeader(string name, long offset, long length, byte[] header) : base(name, offset, length)
        {
            this.header = header;
        }

        public byte[] Header
        {
            get
            {
                return this.header;
            }
            set
            {
                this.header = value;
            }
        }
    }
}

