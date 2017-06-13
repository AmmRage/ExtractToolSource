namespace Ravioli.AppShared
{
    using System;

    public class InputFileInfo
    {
        private string filePath;
        private bool filePathSet;
        private string rootDirectory;

        public InputFileInfo()
        {
            this.filePath = string.Empty;
            this.filePathSet = false;
            this.rootDirectory = string.Empty;
        }

        public InputFileInfo(string filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException("filePath");
            }
            if ((filePath != null) && (filePath.Length == 0))
            {
                throw new ArgumentException("FilePath must not be an empty string.");
            }
            this.filePath = filePath;
            this.filePathSet = false;
            this.rootDirectory = string.Empty;
        }

        public void MergeWith(InputFileInfo info)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            this.rootDirectory = info.RootDirectory;
        }

        public string FilePath
        {
            get
            {
                return this.filePath;
            }
            set
            {
                if (this.filePathSet)
                {
                    throw new InvalidOperationException("FilePath can be set only once.");
                }
                this.filePath = value;
                this.filePathSet = true;
            }
        }

        public string RootDirectory
        {
            get
            {
                return this.rootDirectory;
            }
            set
            {
                this.rootDirectory = value;
            }
        }
    }
}

