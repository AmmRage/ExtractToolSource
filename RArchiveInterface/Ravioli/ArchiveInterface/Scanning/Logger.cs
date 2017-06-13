namespace Ravioli.ArchiveInterface.Scanning
{
    using System;
    using System.Threading;

    public class Logger
    {
        private int errorCount = 0;
        private int infoCount = 0;
        private int warningCount = 0;

        public event LogEventHandler ErrorLogged;

        public event LogEventHandler InfoLogged;

        public event LogEventHandler WarningLogged;

        public void LogError(string message)
        {
            this.errorCount++;
            if (this.ErrorLogged != null)
            {
                this.ErrorLogged(this, new LogEventArgs(message));
            }
        }

        public void LogInfo(string message)
        {
            this.infoCount++;
            if (this.InfoLogged != null)
            {
                this.InfoLogged(this, new LogEventArgs(message));
            }
        }

        public void LogWarning(string message)
        {
            this.warningCount++;
            if (this.WarningLogged != null)
            {
                this.WarningLogged(this, new LogEventArgs(message));
            }
        }

        public int ErrorCount
        {
            get
            {
                return this.errorCount;
            }
        }

        public int InfoCount
        {
            get
            {
                return this.infoCount;
            }
        }

        public int WarningCount
        {
            get
            {
                return this.warningCount;
            }
        }
    }
}

