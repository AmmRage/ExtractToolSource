namespace Ravioli.Explorer
{
    using Ravioli.AppShared.Forms;
    using System;
    using System.Windows.Forms;

    [Serializable]
    public class ExplorerConfig
    {
        private BrowserConfig browser = new BrowserConfig();
        private ExtractConfig extract = new ExtractConfig();
        private GeneralConfig general = new GeneralConfig();
        private UnknownFilesConfig unknownFiles = new UnknownFilesConfig();
        private ViewerConfig viewer = new ViewerConfig();

        public BrowserConfig Browser
        {
            get
            {
                return this.browser;
            }
            set
            {
                this.browser = value;
            }
        }

        public ExtractConfig Extract
        {
            get
            {
                return this.extract;
            }
            set
            {
                this.extract = value;
            }
        }

        public GeneralConfig General
        {
            get
            {
                return this.general;
            }
            set
            {
                this.general = value;
            }
        }

        public UnknownFilesConfig UnknownFiles
        {
            get
            {
                return this.unknownFiles;
            }
            set
            {
                this.unknownFiles = value;
            }
        }

        public ViewerConfig Viewer
        {
            get
            {
                return this.viewer;
            }
            set
            {
                this.viewer = value;
            }
        }

        [Serializable]
        public class BrowserConfig
        {
            private int directoryTreeWidth = 0;
            private Ravioli.Explorer.ExplorerConfig.DirectoryView directoryView = Ravioli.Explorer.ExplorerConfig.DirectoryView.Hierarchical;
            private Ravioli.Explorer.ExplorerConfig.FileListView fileListView = Ravioli.Explorer.ExplorerConfig.FileListView.Details;
            private ExplorerConfig.OutputConfig output = new ExplorerConfig.OutputConfig();
            private ExplorerConfig.PreviewConfig preview = new ExplorerConfig.PreviewConfig();
            private Ravioli.Explorer.SortCriterion sortCriterion = Ravioli.Explorer.SortCriterion.Name;
            private System.Windows.Forms.SortOrder sortOrder = System.Windows.Forms.SortOrder.Ascending;
            private Ravioli.AppShared.Forms.WindowSettings windowSettings = new Ravioli.AppShared.Forms.WindowSettings();

            public int DirectoryTreeWidth
            {
                get
                {
                    return this.directoryTreeWidth;
                }
                set
                {
                    this.directoryTreeWidth = value;
                }
            }

            public Ravioli.Explorer.ExplorerConfig.DirectoryView DirectoryView
            {
                get
                {
                    return this.directoryView;
                }
                set
                {
                    this.directoryView = value;
                }
            }

            public Ravioli.Explorer.ExplorerConfig.FileListView FileListView
            {
                get
                {
                    return this.fileListView;
                }
                set
                {
                    this.fileListView = value;
                }
            }

            public ExplorerConfig.OutputConfig Output
            {
                get
                {
                    return this.output;
                }
                set
                {
                    this.output = value;
                }
            }

            public ExplorerConfig.PreviewConfig Preview
            {
                get
                {
                    return this.preview;
                }
                set
                {
                    this.preview = value;
                }
            }

            public Ravioli.Explorer.SortCriterion SortCriterion
            {
                get
                {
                    return this.sortCriterion;
                }
                set
                {
                    this.sortCriterion = value;
                }
            }

            public System.Windows.Forms.SortOrder SortOrder
            {
                get
                {
                    return this.sortOrder;
                }
                set
                {
                    this.sortOrder = value;
                }
            }

            public Ravioli.AppShared.Forms.WindowSettings WindowSettings
            {
                get
                {
                    return this.windowSettings;
                }
                set
                {
                    this.windowSettings = value;
                }
            }
        }

        [Serializable]
        public enum DirectoryView
        {
            Hierarchical,
            Flat
        }

        [Serializable]
        public class ExtractConfig
        {
            private bool convertImages = false;
            private bool convertSounds = false;
            private bool createSubDirectory = false;
            private string lastExtractDir = string.Empty;
            private string lastSaveDir = string.Empty;
            private string targetImageFormat = string.Empty;
            private string targetSoundFormat = string.Empty;

            public bool ConvertImages
            {
                get
                {
                    return this.convertImages;
                }
                set
                {
                    this.convertImages = value;
                }
            }

            public bool ConvertSounds
            {
                get
                {
                    return this.convertSounds;
                }
                set
                {
                    this.convertSounds = value;
                }
            }

            public bool CreateSubDirectory
            {
                get
                {
                    return this.createSubDirectory;
                }
                set
                {
                    this.createSubDirectory = value;
                }
            }

            public string LastExtractDir
            {
                get
                {
                    return this.lastExtractDir;
                }
                set
                {
                    this.lastExtractDir = value;
                }
            }

            public string LastSaveDir
            {
                get
                {
                    return this.lastSaveDir;
                }
                set
                {
                    this.lastSaveDir = value;
                }
            }

            public string TargetImageFormat
            {
                get
                {
                    return this.targetImageFormat;
                }
                set
                {
                    this.targetImageFormat = value;
                }
            }

            public string TargetSoundFormat
            {
                get
                {
                    return this.targetSoundFormat;
                }
                set
                {
                    this.targetSoundFormat = value;
                }
            }
        }

        [Serializable]
        public enum FileListView
        {
            Details,
            Thumbnails
        }

        [Serializable]
        public class GeneralConfig
        {
            private string lastOpenDir = string.Empty;
            private Ravioli.Explorer.ExplorerConfig.StartupBehaviour startupBehaviour = Ravioli.Explorer.ExplorerConfig.StartupBehaviour.DoNothing;

            public string LastOpenDir
            {
                get
                {
                    return this.lastOpenDir;
                }
                set
                {
                    this.lastOpenDir = value;
                }
            }

            public Ravioli.Explorer.ExplorerConfig.StartupBehaviour StartupBehaviour
            {
                get
                {
                    return this.startupBehaviour;
                }
                set
                {
                    this.startupBehaviour = value;
                }
            }
        }

        [Serializable]
        public class OutputConfig
        {
            private int height = 0;

            public int Height
            {
                get
                {
                    return this.height;
                }
                set
                {
                    this.height = value;
                }
            }
        }

        [Serializable]
        public class PreviewConfig
        {
            private bool active = true;
            private bool autoPlaySounds = true;
            private bool lockImageZoom = false;
            private bool previewSounds = true;
            private int width = 0;

            public bool Active
            {
                get
                {
                    return this.active;
                }
                set
                {
                    this.active = value;
                }
            }

            public bool AutoPlaySounds
            {
                get
                {
                    return this.autoPlaySounds;
                }
                set
                {
                    this.autoPlaySounds = value;
                }
            }

            public bool LockImageZoom
            {
                get
                {
                    return this.lockImageZoom;
                }
                set
                {
                    this.lockImageZoom = value;
                }
            }

            public bool PreviewSounds
            {
                get
                {
                    return this.previewSounds;
                }
                set
                {
                    this.previewSounds = value;
                }
            }

            public int Width
            {
                get
                {
                    return this.width;
                }
                set
                {
                    this.width = value;
                }
            }
        }

        [Serializable]
        public enum StartupBehaviour
        {
            DoNothing = 0,
            Open = 1,
            OpenFile = 1,
            OpenGame = 2
        }

        public class UnknownFilesConfig
        {
            private bool allowScanning = true;
            private bool askBeforeScanning = true;

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

            public bool AskBeforeScanning
            {
                get
                {
                    return this.askBeforeScanning;
                }
                set
                {
                    this.askBeforeScanning = value;
                }
            }
        }

        [Serializable]
        public class ViewerConfig
        {
            private Ravioli.AppShared.Forms.WindowSettings windowSettings = new Ravioli.AppShared.Forms.WindowSettings();

            public Ravioli.AppShared.Forms.WindowSettings WindowSettings
            {
                get
                {
                    return this.windowSettings;
                }
                set
                {
                    this.windowSettings = value;
                }
            }
        }
    }
}

