namespace Ravioli.ArchivePlugins.SaintsRow
{
    using Ravioli.ArchiveInterface;
    using Ravioli.PluginHelpers;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.InteropServices;

    public class VppPcSBFile : GenericCompressedArchive2
    {
        private VppPcDirectory vppDirectory;

        private bool CheckFormatAndGetHeader(Stream stream, BinaryReader reader, out VppPcHeader header)
        {
            header = null;
            bool flag = false;
            stream.Seek(0L, SeekOrigin.Begin);
            if (!VppPcReader.CheckVppSignature(stream, reader))
            {
                return flag;
            }
            uint version = VppPcReader.ReadVppVersion(stream, reader);
            if ((version != 6) && (version != 10))
            {
                return flag;
            }
            header = VppPcReader.ReadVppHeader(stream, reader, version);
            if ((header.Flags != 0x4803) && ((version != 10) || (header.Flags != 2)))
            {
                return flag;
            }
            return true;
        }

        protected override void DecompressArchive(Stream stream, BinaryReader reader, Stream outputStream, long byteCount)
        {
            if ((this.vppDirectory.TotalCompressedLength != uint.MaxValue) && (this.vppDirectory.TotalCompressedLength > 0))
            {
                Compression.DecompressZlibStream(stream, outputStream, byteCount);
            }
        }

        protected override bool IsValidFormat(Stream stream, BinaryReader reader)
        {
            VppPcHeader header;
            return this.CheckFormatAndGetHeader(stream, reader, out header);
        }

        protected override GenericDirectoryEntry[] ReadGenericDirectory(Stream stream, BinaryReader reader)
        {
            VppPcHeader header;
            if (!this.CheckFormatAndGetHeader(stream, reader, out header))
            {
                throw new InvalidDataException("This is not a valid VPP_PC file.");
            }
            VppPcDirectory directory = VppPcReader.ReadVppDirectory(stream, reader, header, false);
            List<CompressedDirectoryEntry> list = new List<CompressedDirectoryEntry>();
            foreach (VppPcDirectoryEntry entry in directory.Entries)
            {
                CompressedDirectoryEntry entry2;
                entry2 = new CompressedDirectoryEntry {
                    Name = entry.Name,
                    Offset = (long) entry.Offset,
                    Length = (long) entry.Length,
                    CompressedLength = (long) entry.CompressedLength,
                    Compressed = entry2.CompressedLength != 0xffffffffL
                };
                if (!entry2.Compressed)
                {
                    entry2.CompressedLength = entry2.Length;
                }
                list.Add(entry2);
            }
            this.vppDirectory = directory;
            return list.ToArray();
        }

        public override string[] Extensions
        {
            get
            {
                return new string[] { ".vpp_pc" };
            }
        }

        public override string TypeName
        {
            get
            {
                return "Saints Row 3/4 VPP_PC (SB) File";
            }
        }

        protected override long UncompressedArchiveLength
        {
            get
            {
                return (long) this.vppDirectory.TotalUncompressedLength;
            }
        }

        internal VppPcDirectory VppDirectory
        {
            get
            {
                return this.vppDirectory;
            }
        }
    }
}

