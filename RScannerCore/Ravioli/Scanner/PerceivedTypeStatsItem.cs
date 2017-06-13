    using Ravioli.ArchiveInterface.Scanning;
    using System;
namespace Ravioli.Scanner
{

    public class PerceivedTypeStatsItem
    {
        private long bytes;
        private int count;
        private Ravioli.ArchiveInterface.Scanning.PerceivedType perceivedType;

        public PerceivedTypeStatsItem(Ravioli.ArchiveInterface.Scanning.PerceivedType perceivedType, int count, long bytes)
        {
            this.perceivedType = perceivedType;
            this.count = count;
            this.bytes = bytes;
        }

        public long Bytes
        {
            get
            {
                return this.bytes;
            }
            set
            {
                this.bytes = value;
            }
        }

        public int Count
        {
            get
            {
                return this.count;
            }
            set
            {
                this.count = value;
            }
        }

        public Ravioli.ArchiveInterface.Scanning.PerceivedType PerceivedType
        {
            get
            {
                return this.perceivedType;
            }
        }
    }
}

