    using Ravioli.ArchiveInterface.Scanning;
    using System;
    using System.Xml.Serialization;
namespace Ravioli.Scanner
{

    [Serializable, XmlType("DirectoryEntry")]
    public class ScanDirectoryEntry
    {
        private long length;
        private string name;
        private long offset;
        private Ravioli.ArchiveInterface.Scanning.PerceivedType perceivedType;
        private string typeName;

        [XmlAttribute]
        public long Length
        {
            get
            {
                return this.length;
            }
            set
            {
                this.length = value;
            }
        }

        [XmlAttribute]
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        [XmlAttribute]
        public long Offset
        {
            get
            {
                return this.offset;
            }
            set
            {
                this.offset = value;
            }
        }

        [XmlAttribute]
        public Ravioli.ArchiveInterface.Scanning.PerceivedType PerceivedType
        {
            get
            {
                return this.perceivedType;
            }
            set
            {
                this.perceivedType = value;
            }
        }

        [XmlAttribute]
        public string TypeName
        {
            get
            {
                return this.typeName;
            }
            set
            {
                this.typeName = value;
            }
        }
    }
}

