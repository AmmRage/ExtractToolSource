namespace Ravioli.Extractor
{
    using System;

    public class JobScanCompletedEventArgs : JobEventArgs
    {
        private int itemsFound;

        public JobScanCompletedEventArgs(ExtractJob job, int itemsFound) : base(job)
        {
            this.itemsFound = itemsFound;
        }

        public int ItemsFound
        {
            get
            {
                return this.itemsFound;
            }
        }
    }
}

