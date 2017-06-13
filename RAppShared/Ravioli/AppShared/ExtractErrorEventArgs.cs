namespace Ravioli.AppShared
{
    using System;

    public class ExtractErrorEventArgs : EventArgs
    {
        private ErrorAction action;
        private System.Exception exception;
        private string fileName;

        public ExtractErrorEventArgs(string fileName, System.Exception exception)
        {
            this.fileName = fileName;
            this.exception = exception;
            this.action = ErrorAction.Abort;
        }

        public ErrorAction Action
        {
            get
            {
                return this.action;
            }
            set
            {
                this.action = value;
            }
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

