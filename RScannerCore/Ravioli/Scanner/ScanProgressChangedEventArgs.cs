namespace Ravioli.Scanner
{
    using System;
    using System.ComponentModel;

    public class ScanProgressChangedEventArgs : ProgressChangedEventArgs
    {
        private int itemsFound;

        public ScanProgressChangedEventArgs(int progressPercentage, int itemsFound, object userState) : base(progressPercentage, userState)
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

