namespace Ravioli.Scanner
{
    using System;

    public class ScanItemEventArgs : EventArgs
    {
        private ScanDirectoryEntry entry;
        private long fileSize;
        private object userState;

        public ScanItemEventArgs(ScanDirectoryEntry entry, long fileSize, object userState)
        {
            this.entry = entry;
            this.fileSize = fileSize;
            this.userState = userState;
        }

        public long FileSize
        {
            get
            {
                return this.fileSize;
            }
        }

        public ScanDirectoryEntry Item
        {
            get
            {
                return this.entry;
            }
        }

        public object UserState
        {
            get
            {
                return this.userState;
            }
        }
    }
}

