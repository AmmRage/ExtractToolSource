namespace Ravioli.Extractor
{
    using Ravioli.AppShared;
    using Ravioli.ArchiveInterface;
    using System;

    public class ExtractJob
    {
        private ArchivePluginMapping archivePlugin;
        private string baseOutputDir;
        private IImageSaver imageSaver;
        private string inputFile;
        private bool inputFileSubDir;
        private string rootDirectory;
        private SoundExportFormat soundFormat;

        public ExtractJob(string inputFile, string baseOutputDir, bool inputFileSubDir, ArchivePluginMapping archivePlugin, string rootDirectory, IImageSaver imageSaver, SoundExportFormat soundFormat)
        {
            this.inputFile = inputFile;
            this.baseOutputDir = baseOutputDir;
            this.inputFileSubDir = inputFileSubDir;
            this.archivePlugin = archivePlugin;
            this.rootDirectory = rootDirectory;
            this.imageSaver = imageSaver;
            this.soundFormat = soundFormat;
        }

        public ArchivePluginMapping ArchivePlugin
        {
            get
            {
                return this.archivePlugin;
            }
        }

        public string BaseOutputDir
        {
            get
            {
                return this.baseOutputDir;
            }
        }

        public IImageSaver ImageSaver
        {
            get
            {
                return this.imageSaver;
            }
        }

        public string InputFile
        {
            get
            {
                return this.inputFile;
            }
        }

        public bool InputFileSubDir
        {
            get
            {
                return this.inputFileSubDir;
            }
        }

        public string RootDirectory
        {
            get
            {
                return this.rootDirectory;
            }
        }

        public SoundExportFormat SoundFormat
        {
            get
            {
                return this.soundFormat;
            }
        }
    }
}

