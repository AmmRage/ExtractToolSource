namespace Ravioli.Extractor
{
    using Ravioli.ArchiveInterface;
    using System;

    public class JobInfoEventArgs : JobEventArgs
    {
        private IExtractProgress extractProgress;
        private IFileSystemAbstractor fileSystemAbstractor;

        public JobInfoEventArgs(ExtractJob job, IExtractProgress progress, IFileSystemAbstractor abstractor) : base(job)
        {
            this.extractProgress = progress;
            this.fileSystemAbstractor = abstractor;
        }

        public IExtractProgress ExtractProgress
        {
            get
            {
                return this.extractProgress;
            }
        }

        public IFileSystemAbstractor FileSystemAbstractor
        {
            get
            {
                return this.fileSystemAbstractor;
            }
        }

        public bool IsFileSystemAbstractor
        {
            get
            {
                return (this.fileSystemAbstractor != null);
            }
        }

        public bool SupportsCurrentFileProgress
        {
            get
            {
                return (this.extractProgress != null);
            }
        }
    }
}

