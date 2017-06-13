namespace Ravioli.Scanner
{
    using System;
    using System.ComponentModel;

    public class ScanCompletedEventArgs : AsyncCompletedEventArgs
    {
        private ScanResults results;

        public ScanCompletedEventArgs(Exception error, bool cancelled, ScanResults results, object userState) : base(error, cancelled, userState)
        {
            this.results = results;
        }

        public ScanResults Results
        {
            get
            {
                base.RaiseExceptionIfNecessary();
                return this.results;
            }
        }
    }
}

