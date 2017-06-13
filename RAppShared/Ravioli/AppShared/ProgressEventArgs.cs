namespace Ravioli.AppShared
{
    using System;

    public class ProgressEventArgs : EventArgs
    {
        private int done;
        private int total;

        public ProgressEventArgs(int done, int total)
        {
            this.done = done;
            this.total = total;
        }

        public int Done
        {
            get
            {
                return this.done;
            }
        }

        public int Total
        {
            get
            {
                return this.total;
            }
        }
    }
}

