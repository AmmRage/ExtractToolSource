namespace Ravioli.ArchiveInterface
{
    using System;

    internal class AbstractorDirectoryEntry
    {
        private string displayName = string.Empty;
        private bool isSubArchive = false;
        private IFileInfo nestedSubArchiveFileInfo;
        private Type nestedSubArchivePluginType;
        private long size = 0L;
        private string sourceFSPath = string.Empty;
        private IFileInfo subArchiveFileInfo = null;
        private Type subArchivePluginType = null;

        public AbstractorDirectoryEntry()
        {
            this.NestedSubArchiveFileInfo = null;
            this.NestedSubArchivePluginType = null;
        }

        public string DisplayName
        {
            get
            {
                return this.displayName;
            }
            set
            {
                this.displayName = value;
            }
        }

        public bool IsSubArchive
        {
            get
            {
                return this.isSubArchive;
            }
            set
            {
                this.isSubArchive = value;
            }
        }

        public IFileInfo NestedSubArchiveFileInfo
        {
            get
            {
                return this.nestedSubArchiveFileInfo;
            }
            set
            {
                this.nestedSubArchiveFileInfo = value;
            }
        }

        public Type NestedSubArchivePluginType
        {
            get
            {
                return this.nestedSubArchivePluginType;
            }
            set
            {
                this.nestedSubArchivePluginType = value;
            }
        }

        public long Size
        {
            get
            {
                return this.size;
            }
            set
            {
                this.size = value;
            }
        }

        public string SourceFSPath
        {
            get
            {
                return this.sourceFSPath;
            }
            set
            {
                this.sourceFSPath = value;
            }
        }

        public IFileInfo SubArchiveFileInfo
        {
            get
            {
                return this.subArchiveFileInfo;
            }
            set
            {
                this.subArchiveFileInfo = value;
            }
        }

        public Type SubArchivePluginType
        {
            get
            {
                return this.subArchivePluginType;
            }
            set
            {
                this.subArchivePluginType = value;
            }
        }
    }
}

