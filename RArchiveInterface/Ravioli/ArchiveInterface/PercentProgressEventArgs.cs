namespace Ravioli.ArchiveInterface
{
    using System;

    public class PercentProgressEventArgs : EventArgs
    {
        private int percent;

        public PercentProgressEventArgs(int percent)
        {
            this.percent = percent;
        }

        public int Percent
        {
            get
            {
                return this.percent;
            }
        }
    }
}

