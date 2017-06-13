namespace Ravioli.Extractor
{
    using Ravioli.AppShared;
    using System;

    public class RootDirectoryEventArgs : EventArgs
    {
        private string inputFile;
        private ArchivePluginMapping pluginMapping;
        private string rootDirectory;

        public RootDirectoryEventArgs(ArchivePluginMapping pluginMapping, string inputFile)
        {
            this.pluginMapping = pluginMapping;
            this.inputFile = inputFile;
            this.rootDirectory = string.Empty;
        }

        public string InputFile
        {
            get
            {
                return this.inputFile;
            }
        }

        public ArchivePluginMapping PluginMapping
        {
            get
            {
                return this.pluginMapping;
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

