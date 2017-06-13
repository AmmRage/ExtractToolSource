namespace Ravioli.Scanner
{
    using System;

    [Serializable]
    public class ScanRange
    {
        private long endOffset;
        private long startOffset;

        public ScanRange()
        {
            this.startOffset = 0L;
            this.endOffset = 0L;
        }

        public ScanRange(long startOffset, long endOffset)
        {
            this.startOffset = startOffset;
            this.endOffset = endOffset;
        }

        public long EndOffset
        {
            get
            {
                return this.endOffset;
            }
            set
            {
                this.endOffset = value;
            }
        }

        public long StartOffset
        {
            get
            {
                return this.startOffset;
            }
            set
            {
                this.startOffset = value;
            }
        }
    }
}

