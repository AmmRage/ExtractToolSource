namespace Ravioli.ArchiveInterface.Scanning
{
    using System;

    public class LogEventArgs : EventArgs
    {
        private string message;

        public LogEventArgs(string message)
        {
            this.message = message;
        }

        public string Message
        {
            get
            {
                return this.message;
            }
        }
    }
}

