namespace Ravioli.ArchiveInterface
{
    using System;

    public class ArchiveFileInfo : IFileInfo, IFileSystemEntry
    {
        private long id;
        private string name;
        private long size;

        public ArchiveFileInfo(long id, string name, long size)
        {
            this.id = id;
            this.name = name;
            this.size = size;
        }

        public override string ToString()
        {
            return string.Format("ID={0} Name=\"{1}\" Size={2}", this.id, this.name, this.size);
        }

        public long ID
        {
            get
            {
                return this.id;
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }
        }

        public long Size
        {
            get
            {
                return this.size;
            }
        }
    }
}

