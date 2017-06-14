namespace Ravioli.ArchivePlugins
{
    using System;

    internal class TRBigFileDirectoryEntry
    {
        private uint hashcode = 0;
        private uint language = 0;
        private uint length = 0;
        private string name = string.Empty;
        private uint offset = 0;
        private uint partIndex = 0;

        public uint Hashcode
        {
            get
            {
                return this.hashcode;
            }
            set
            {
                this.hashcode = value;
            }
        }

        public uint Language
        {
            get
            {
                return this.language;
            }
            set
            {
                this.language = value;
            }
        }

        public uint Length
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

        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        public uint Offset
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

        public uint PartIndex
        {
            get
            {
                return this.partIndex;
            }
            set
            {
                this.partIndex = value;
            }
        }
    }
}

