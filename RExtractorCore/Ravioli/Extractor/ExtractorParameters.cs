namespace Ravioli.Extractor
{
    using Ravioli.AppShared;
    using System;

    public class ExtractorParameters
    {
        private bool allowScanning = false;
        private string archiveType = string.Empty;
        private bool autoExtract = false;
        private bool help = false;
        private string imageFormat = string.Empty;
        private string inputFiles = string.Empty;
        private bool inputFileSubDir = false;
        private string outputDir = string.Empty;
        private string rootDirs = string.Empty;
        private string soundFormat = string.Empty;

        public InputFileInfoCache GetFileInfoCache()
        {
            InputFileInfoCache cache = new InputFileInfoCache();
            string[] parsedInputFiles = this.GetParsedInputFiles();
            string[] parsedRootDirs = this.GetParsedRootDirs();
            if ((parsedInputFiles.Length > 0) && (parsedRootDirs.Length > 0))
            {
                string[] strArray3 = parsedInputFiles;
                for (int i = 0; i < strArray3.Length; i++)
                {
                    string text1 = strArray3[i];
                    for (int j = 0; j < parsedInputFiles.Length; j++)
                    {
                        InputFileInfo item = new InputFileInfo(parsedInputFiles[j]) {
                            RootDirectory = parsedRootDirs[j]
                        };
                        cache.Add(item);
                    }
                }
            }
            return cache;
        }

        public string[] GetParsedInputFiles()
        {
            if (this.inputFiles.Length <= 0)
            {
                return new string[0];
            }
            return this.inputFiles.Split(new char[] { ';' });
        }

        public string[] GetParsedRootDirs()
        {
            if (this.rootDirs.Length <= 0)
            {
                return new string[0];
            }
            return this.rootDirs.Split(new char[] { ';' });
        }

        public bool AllowScanning
        {
            get
            {
                return this.allowScanning;
            }
            set
            {
                this.allowScanning = value;
            }
        }

        public string ArchiveType
        {
            get
            {
                return this.archiveType;
            }
            set
            {
                this.archiveType = value;
            }
        }

        public bool AutoExtract
        {
            get
            {
                return this.autoExtract;
            }
            set
            {
                this.autoExtract = value;
            }
        }

        public bool Help
        {
            get
            {
                return this.help;
            }
            set
            {
                this.help = value;
            }
        }

        public string ImageFormat
        {
            get
            {
                return this.imageFormat;
            }
            set
            {
                this.imageFormat = value;
            }
        }

        public string InputFiles
        {
            get
            {
                return this.inputFiles;
            }
            set
            {
                this.inputFiles = value;
            }
        }

        public bool InputFileSubDir
        {
            get
            {
                return this.inputFileSubDir;
            }
            set
            {
                this.inputFileSubDir = value;
            }
        }

        public string OutputDir
        {
            get
            {
                return this.outputDir;
            }
            set
            {
                this.outputDir = value;
            }
        }

        public string RootDirs
        {
            get
            {
                return this.rootDirs;
            }
            set
            {
                this.rootDirs = value;
            }
        }

        public string SoundFormat
        {
            get
            {
                return this.soundFormat;
            }
            set
            {
                this.soundFormat = value;
            }
        }
    }
}

