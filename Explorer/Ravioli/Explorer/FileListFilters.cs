namespace Ravioli.Explorer
{
    using System;
    using System.Collections.Specialized;

    internal class FileListFilters
    {
        private string directoryFilter = null;
        private StringCollection extensionFilter = null;

        public bool HasActiveFilters()
        {
            if (this.extensionFilter == null)
            {
                return (this.directoryFilter != null);
            }
            return true;
        }

        public string DirectoryFilter
        {
            get
            {
                return this.directoryFilter;
            }
            set
            {
                this.directoryFilter = value;
            }
        }

        public StringCollection ExtensionFilter
        {
            get
            {
                return this.extensionFilter;
            }
            set
            {
                this.extensionFilter = value;
            }
        }
    }
}

