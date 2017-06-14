namespace Ravioli.ArchivePlugins.SaintsRow
{
    using Ravioli.PluginHelpers.Reading;
    using System;
    using System.IO;

    internal class VppPcReader
    {
        public static bool CheckVppSignature(Stream stream, BinaryReader reader)
        {
            return (reader.ReadUInt32() == 0x51890ace);
        }

        private static void FixCompressedOffsets(VppPcDirectory directory)
        {
            uint num = 0;
            uint num2 = 0;
            foreach (VppPcDirectoryEntry entry in directory.Entries)
            {
                if ((num != 0) && (num2 != 0))
                {
                    if (entry.Offset != num2)
                    {
                        throw new InvalidDataException("Items are in wrong order, cannot calculate offsets.");
                    }
                    num2 = (entry.Offset + entry.Length) + GetDiffToNextSector(entry.Length);
                    entry.Offset = num;
                }
                else
                {
                    num2 = (entry.Offset + entry.Length) + GetDiffToNextSector(entry.Length);
                }
                if (entry.CompressedLength == uint.MaxValue)
                {
                    break;
                }
                num = (entry.Offset + entry.CompressedLength) + GetDiffToNextSector(entry.CompressedLength);
            }
        }

        private static uint GetDiffToNextSector(uint position)
        {
            if ((position % 0x800) == 0)
            {
                return 0;
            }
            return (0x800 - (position % 0x800));
        }

        private static bool MapVersionToSR3(uint version)
        {
            if (version == 6)
            {
                return true;
            }
            if (version != 10)
            {
                throw new NotSupportedException("This version of the VPP_PC file format is not supported.");
            }
            return false;
        }

        private static void MoveToNextSector(Stream stream)
        {
            long num = ((stream.Position % 0x800L) != 0L) ? (0x800L - (stream.Position % 0x800L)) : 0L;
            stream.Position += num;
        }

        public static VppPcDirectory ReadVppDirectory(Stream stream, BinaryReader reader, VppPcHeader header, bool fixOffsets)
        {
            bool flag = MapVersionToSR3(header.Version);
            VppPcDirectory directory = new VppPcDirectory {
                TotalUncompressedLength = reader.ReadUInt32(),
                TotalCompressedLength = reader.ReadUInt32()
            };
            long position = stream.Position;
            if (flag)
            {
                MoveToNextSector(stream);
            }
            for (int i = 0; i < header.ItemCount; i++)
            {
                VppPcDirectoryEntry item = new VppPcDirectoryEntry {
                    Unknown1 = reader.ReadUInt32(),
                    Unknown2 = reader.ReadUInt32(),
                    Offset = reader.ReadUInt32(),
                    Length = reader.ReadUInt32(),
                    CompressedLength = reader.ReadUInt32(),
                    Unknown3 = reader.ReadUInt32()
                };
                directory.Entries.Add(item);
            }
            if (flag)
            {
                MoveToNextSector(stream);
            }
            long num2 = stream.Position;
            for (int j = 0; j < header.ItemCount; j++)
            {
                string str;
                do
                {
                    str = StaticReader.ReadZeroTerminatedString(stream);
                }
                while (!flag && (str.Length == 0));
                directory.Entries[j].Name = str;
            }
            if (flag)
            {
                MoveToNextSector(stream);
            }
            else
            {
                stream.Position = num2 + header.ItemNamesLength;
            }
            directory.DataStartOffset = stream.Position;
            if (flag && fixOffsets)
            {
                FixCompressedOffsets(directory);
            }
            return directory;
        }

        public static VppPcHeader ReadVppHeader(Stream stream, BinaryReader reader, uint version)
        {
            VppPcHeader header = new VppPcHeader();
            bool flag = MapVersionToSR3(version);
            header.Version = version;
            if (flag)
            {
                stream.Position = 0x14cL;
                header.Flags = reader.ReadUInt32();
                header.Unknown = reader.ReadUInt32();
                header.ItemCount = reader.ReadUInt32();
                header.TotalFileLength = reader.ReadUInt32();
            }
            else
            {
                header.Unknown = reader.ReadUInt32();
                header.TotalFileLength = reader.ReadUInt32();
                header.Flags = reader.ReadUInt32();
                header.ItemCount = reader.ReadUInt32();
            }
            header.ItemDataLength = reader.ReadUInt32();
            header.ItemNamesLength = reader.ReadUInt32();
            return header;
        }

        public static uint ReadVppVersion(Stream stream, BinaryReader reader)
        {
            return reader.ReadUInt32();
        }
    }
}

