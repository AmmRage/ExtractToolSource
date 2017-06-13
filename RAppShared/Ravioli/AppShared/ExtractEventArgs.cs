namespace Ravioli.AppShared
{
    using System;

    public class ExtractEventArgs : EventArgs
    {
        private string fileName;

        public ExtractEventArgs(string fileName)
        {
            this.fileName = fileName;
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

