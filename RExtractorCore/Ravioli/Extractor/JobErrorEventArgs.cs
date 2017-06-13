namespace Ravioli.Extractor
{
    using System;

    public class JobErrorEventArgs : JobEventArgs
    {
        private string message;
        private bool retry;

        public JobErrorEventArgs(ExtractJob job, string message) : base(job)
        {
            this.message = message;
            this.retry = false;
        }

        public string Message
        {
            get
            {
                return this.message;
            }
        }

        public bool Retry
        {
            get
            {
                return this.retry;
            }
            set
            {
                this.retry = value;
            }
        }
    }
}

