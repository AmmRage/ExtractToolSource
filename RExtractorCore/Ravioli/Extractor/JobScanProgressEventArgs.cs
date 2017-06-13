namespace Ravioli.Extractor
{
    using System;

    public class JobScanProgressEventArgs : JobEventArgs
    {
        private int progressPercentage;

        public JobScanProgressEventArgs(ExtractJob job, int progressPercentage) : base(job)
        {
            this.progressPercentage = progressPercentage;
        }

        public int ProgressPercentage
        {
            get
            {
                return this.progressPercentage;
            }
        }
    }
}

