namespace Ravioli.ArchiveInterface
{
    using System;

    public class ExtractWarningEventArgs : EventArgs
    {
        private System.Exception exception;
        private string fileName;

        public ExtractWarningEventArgs(string fileName, System.Exception exception)
        {
            this.fileName = fileName;
            this.exception = exception;
        }

        public System.Exception Exception
        {
            get
            {
                return this.exception;
            }
        }

        public string FileName
        {
            get
            {
                return this.fileName;
            }
        }
    }
}

