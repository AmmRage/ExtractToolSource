namespace Ravioli.AppShared
{
    using System;

    public class FileInfoChecker
    {
        private string filePath;

        public FileInfoChecker(string filePath)
        {
            this.filePath = filePath;
        }

        public bool DoesFilePathMatch(InputFileInfo info)
        {
            return (info.FilePath == this.filePath);
        }

        public string FilePath
        {
            get
            {
                return this.filePath;
            }
        }
    }
}

