namespace Ravioli.Scanner
{
    using System;
    using System.Xml.Serialization;

    [Serializable]
    public class ScanResults
    {
        private ScanDirectoryEntry[] entries;
        private string fileName;
        private long fileSize;
        private long lastPosition = -1L;
        private ScanRange range = null;

        [XmlArray]
        public ScanDirectoryEntry[] Entries
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

        [XmlElement]
        public long LastPosition
        {
            get
            {
                if (this.lastPosition == -1L)
                {
                    return this.fileSize;
                }
                return this.lastPosition;
            }
            set
            {
                this.lastPosition = value;
            }
        }

        [XmlElement]
        public ScanRange Range
        {
            get
            {
                return this.range;
            }
            set
            {
                this.range = value;
            }
        }
    }
}

