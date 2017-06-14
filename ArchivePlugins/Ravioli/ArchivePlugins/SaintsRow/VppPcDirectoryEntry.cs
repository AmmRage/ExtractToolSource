namespace Ravioli.ArchivePlugins.SaintsRow
{
    using System;
    using System.Runtime.CompilerServices;

    internal class VppPcDirectoryEntry
    {
        public uint CompressedLength { get; set; }

        public uint Length { get; set; }

        public string Name { get; set; }

        public uint Offset { get; set; }

        public uint Unknown1 { get; set; }

        public uint Unknown2 { get; set; }

        public uint Unknown3 { get; set; }
    }
}

