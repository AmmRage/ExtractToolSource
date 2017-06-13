namespace Ravioli.ArchiveInterface.Scanning
{
    using System;

    public class FileType
    {
        private string extension;
        private Ravioli.ArchiveInterface.Scanning.PerceivedType perceivedType;
        private string typeName;

        public FileType(string typeName, string extension, Ravioli.ArchiveInterface.Scanning.PerceivedType perceivedType)
        {
            this.typeName = typeName;
            this.extension = extension;
            this.perceivedType = perceivedType;
        }

        public string Extension
        {
            get
            {
                return this.extension;
            }
        }

        public Ravioli.ArchiveInterface.Scanning.PerceivedType PerceivedType
        {
            get
            {
                return this.perceivedType;
            }
        }

        public string TypeName
        {
            get
            {
                return this.typeName;
            }
        }
    }
}

