namespace Ravioli.ArchiveInterface
{
    using System;
    using System.Xml.Serialization;

    [Serializable, XmlType("DirectoryEntry")]
    public class GenericDirectoryEntry
    {
        private long length;
        private string name;
        private long offset;

        public GenericDirectoryEntry()
        {
        }

        public GenericDirectoryEntry(string name, long offset, long length)
        {
            this.name = name;
            this.offset = offset;
            this.length = length;
        }

        public override string ToString()
        {
            return string.Format("Name=\"{0}\" Offset=0x{1:X} Length=0x{2:X}", this.name, this.offset, this.length);
        }

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
    }
}

