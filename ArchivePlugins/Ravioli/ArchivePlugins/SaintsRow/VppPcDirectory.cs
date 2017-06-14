namespace Ravioli.ArchivePlugins.SaintsRow
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    internal class VppPcDirectory
    {
        private List<VppPcDirectoryEntry> entries = new List<VppPcDirectoryEntry>();
        private List<string> names = new List<string>();

        public long DataStartOffset { get; set; }

        public List<VppPcDirectoryEntry> Entries
        {
            get
            {
                return this.entries;
            }
        }

        public uint TotalCompressedLength { get; set; }

        public uint TotalUncompressedLength { get; set; }
    }
}

