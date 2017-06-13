namespace Ravioli.Explorer
{
    using Ravioli.ArchiveInterface;
    using System;

    internal class OpenFileWaitData
    {
        private IArchive archive = null;
        private string file = null;

        public IArchive Archive
        {
            get
            {
                return this.archive;
            }
            set
            {
                this.archive = value;
            }
        }

        public string File
        {
            get
            {
                return this.file;
            }
            set
            {
                this.file = value;
            }
        }
    }
}

