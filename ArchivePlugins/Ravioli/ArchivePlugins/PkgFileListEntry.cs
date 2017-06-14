namespace Ravioli.ArchivePlugins
{
    using System;

    internal class PkgFileListEntry
    {
        private uint extensionOffset;
        private uint nameOffset;
        private uint offset;
        private uint size;

        public uint ExtensionOffset
        {
            get
            {
                return this.extensionOffset;
            }
            set
            {
                this.extensionOffset = value;
            }
        }

        public uint NameOffset
        {
            get
            {
                return this.nameOffset;
            }
            set
            {
                this.nameOffset = value;
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

        public uint Size
        {
            get
            {
                return this.size;
            }
            set
            {
                this.size = value;
            }
        }
    }
}

