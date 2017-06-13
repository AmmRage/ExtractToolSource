namespace Ravioli.Extractor
{
    using System;

    public class JobEventArgs : EventArgs
    {
        private ExtractJob job;

        public JobEventArgs(ExtractJob job)
        {
            this.job = job;
        }

        public ExtractJob Job
        {
            get
            {
                return this.job;
            }
        }
    }
}

