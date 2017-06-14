namespace Ravioli.ArchivePlugins.SaintsRow
{
    using System;
    using System.Runtime.CompilerServices;

    internal class VppPcHeader
    {
        public uint Flags { get; set; }

        public uint ItemCount { get; set; }

        public uint ItemDataLength { get; set; }

        public uint ItemNamesLength { get; set; }

        public uint TotalFileLength { get; set; }

        public uint Unknown { get; set; }

        public uint Version { get; set; }
    }
}

