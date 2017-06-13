namespace Ravioli.ArchiveInterface
{
    using System;
    using System.IO;

    public class AbstractorDefinition
    {
        private CasingRule fileNameCasing;
        private bool optional;
        private System.IO.SearchOption searchOption;
        private string sourceFile;
        private string targetDirectory;

        public AbstractorDefinition(string sourceFile)
        {
            this.sourceFile = sourceFile;
            this.targetDirectory = "";
            this.searchOption = System.IO.SearchOption.TopDirectoryOnly;
            this.optional = false;
            this.fileNameCasing = CasingRule.DontChange;
        }

        public AbstractorDefinition(string sourceFile, string targetDirectory)
        {
            this.sourceFile = sourceFile;
            this.targetDirectory = targetDirectory;
            this.searchOption = System.IO.SearchOption.TopDirectoryOnly;
            this.optional = false;
            this.fileNameCasing = CasingRule.DontChange;
        }

        public AbstractorDefinition(string sourceFile, string targetDirectory, System.IO.SearchOption searchOption)
        {
            this.sourceFile = sourceFile;
            this.targetDirectory = targetDirectory;
            this.searchOption = searchOption;
            this.optional = false;
            this.fileNameCasing = CasingRule.DontChange;
        }

        public AbstractorDefinition(string sourceFile, string targetDirectory, System.IO.SearchOption searchOption, bool optional)
        {
            this.sourceFile = sourceFile;
            this.targetDirectory = targetDirectory;
            this.searchOption = searchOption;
            this.optional = optional;
            this.fileNameCasing = CasingRule.DontChange;
        }

        public AbstractorDefinition(string sourceFile, string targetDirectory, System.IO.SearchOption searchOption, bool optional, CasingRule fileNameCasing)
        {
            this.sourceFile = sourceFile;
            this.targetDirectory = targetDirectory;
            this.searchOption = searchOption;
            this.optional = optional;
            this.fileNameCasing = fileNameCasing;
        }

        public CasingRule FileNameCasing
        {
            get
            {
                return this.fileNameCasing;
            }
            set
            {
                this.fileNameCasing = value;
            }
        }

        public bool Optional
        {
            get
            {
                return this.optional;
            }
            set
            {
                this.optional = value;
            }
        }

        public System.IO.SearchOption SearchOption
        {
            get
            {
                return this.searchOption;
            }
            set
            {
                this.searchOption = value;
            }
        }

        public string SourceFile
        {
            get
            {
                return this.sourceFile;
            }
            set
            {
                this.sourceFile = value;
            }
        }

        public string TargetDirectory
        {
            get
            {
                return this.targetDirectory;
            }
            set
            {
                this.targetDirectory = value;
            }
        }
    }
}

