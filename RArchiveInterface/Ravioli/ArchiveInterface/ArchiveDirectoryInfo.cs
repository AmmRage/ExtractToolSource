namespace Ravioli.ArchiveInterface
{
    using System;

    public class ArchiveDirectoryInfo : IDirectoryInfo, IFileSystemEntry
    {
        private string name;

        public ArchiveDirectoryInfo(string name)
        {
            this.name = name;
        }

        public string Name
        {
            get
            {
                return this.name;
            }
        }
    }
}

