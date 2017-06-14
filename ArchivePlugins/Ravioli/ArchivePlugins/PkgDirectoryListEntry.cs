namespace Ravioli.ArchivePlugins
{
    using System;

    internal class PkgDirectoryListEntry
    {
        private uint endIndex;
        private string name;
        private uint startIndex;

        public uint EndIndex
        {
            get
            {
                return this.endIndex;
            }
            set
            {
                this.endIndex = value;
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

        public uint StartIndex
        {
            get
            {
                return this.startIndex;
            }
            set
            {
                this.startIndex = value;
            }
        }
    }
}

