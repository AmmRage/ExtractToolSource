namespace Ravioli.ArchivePlugins
{
    using Ravioli.ArchiveInterface;
    using System;
    using System.Xml.Serialization;

    [Serializable]
    public class GenericDirectory
    {
        private GenericDirectoryEntry[] entries;
        private string fileName;
        private long fileSize;

        [XmlArray]
        public GenericDirectoryEntry[] Entries
        {
            get
            {
                return this.entries;
            }
            set
            {
                this.entries = value;
            }
        }

        [XmlElement]
        public string FileName
        {
            get
            {
                return this.fileName;
            }
            set
            {
                this.fileName = value;
            }
        }

        [XmlElement]
        public long FileSize
        {
            get
            {
                return this.fileSize;
            }
            set
            {
                this.fileSize = value;
            }
        }
    }
}

