namespace Ravioli.Scanner
{
    using System;
    using System.Xml.Serialization;

    public class DirectoryEntryWithUnknowns : ScanDirectoryEntry
    {
        private bool isUnknown = false;

        [XmlAttribute]
        public bool IsUnknown
        {
            get
            {
                return this.isUnknown;
            }
            set
            {
                this.isUnknown = value;
            }
        }
    }
}

