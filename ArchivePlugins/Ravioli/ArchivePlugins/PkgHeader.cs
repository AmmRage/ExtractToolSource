namespace Ravioli.ArchivePlugins
{
    using System;

    internal class PkgHeader
    {
        private uint dataStartOffset;
        private uint directoryCharCount;
        private uint directoryListOffset;
        private uint extensionListOffset;
        private uint fileCount;
        private uint fileNameListOffset;
        private uint signature;
        private uint version;

        public uint DataStartOffset
        {
            get
            {
                return this.dataStartOffset;
            }
            set
            {
                this.dataStartOffset = value;
            }
        }

        public uint DirectoryCharCount
        {
            get
            {
                return this.directoryCharCount;
            }
            set
            {
                this.directoryCharCount = value;
            }
        }

        public uint DirectoryListOffset
        {
            get
            {
                return this.directoryListOffset;
            }
            set
            {
                this.directoryListOffset = value;
            }
        }

        public uint ExtensionListOffset
        {
            get
            {
                return this.extensionListOffset;
            }
            set
            {
                this.extensionListOffset = value;
            }
        }

        public uint FileCount
        {
            get
            {
                return this.fileCount;
            }
            set
            {
                this.fileCount = value;
            }
        }

        public uint FileNameListOffset
        {
            get
            {
                return this.fileNameListOffset;
            }
            set
            {
                this.fileNameListOffset = value;
            }
        }

        public uint Signature
        {
            get
            {
                return this.signature;
            }
            set
            {
                this.signature = value;
            }
        }

        public uint Version
        {
            get
            {
                return this.version;
            }
            set
            {
                this.version = value;
            }
        }
    }
}

