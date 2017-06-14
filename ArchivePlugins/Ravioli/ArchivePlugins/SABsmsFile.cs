namespace Ravioli.ArchivePlugins
{
    using Ravioli.ArchiveInterface;
    using Ravioli.PluginHelpers;
    using Ravioli.PluginHelpers.Reconstruction;
    using System;
    using System.IO;

    internal class SABsmsFile : GenericArchive
    {
        protected override bool IsValidFormat(Stream stream, BinaryReader reader)
        {
            stream.Seek(0L, SeekOrigin.Begin);
            return (reader.ReadUInt32() == 0x534d5342);
        }

        protected override int OnExtracting(GenericDirectoryEntry entry, Stream stream, Stream outputStream)
        {
            base.OnExtracting(entry, stream, outputStream);
            int length = 0;
            if (entry is GenericDirectoryEntryWithHeader)
            {
                GenericDirectoryEntryWithHeader header = (GenericDirectoryEntryWithHeader) entry;
                if (header != null)
                {
                    outputStream.Write(header.Header, 0, header.Header.Length);
                    length = header.Header.Length;
                }
            }
            return length;
        }

        protected override void OnFileInfo(GenericDirectoryEntry entry, ref string name, ref long size)
        {
            base.OnFileInfo(entry, ref name, ref size);
            if (entry is GenericDirectoryEntryWithHeader)
            {
                GenericDirectoryEntryWithHeader header = (GenericDirectoryEntryWithHeader) entry;
                if (header != null)
                {
                    size += header.Header.Length;
                }
            }
        }

        internal static GenericDirectoryEntry[] ReadBsmsFile(Stream stream, BinaryReader reader, string directoryPrefix)
        {
            long position = stream.Position;
            if (reader.ReadUInt32() != 0x534d5342)
            {
                throw new InvalidDataException("This is not a valid BSMS file.");
            }
            reader.ReadUInt32();
            uint num3 = reader.ReadUInt32();
            uint num4 = reader.ReadUInt32();
            reader.ReadUInt32();
            GenericDirectoryEntry[] entryArray = new GenericDirectoryEntry[num3];
            for (int i = 0; i < num3; i++)
            {
                string fileName;
                uint num6 = 0;
                uint num7 = 0;
                byte[] header = null;
                uint num8 = reader.ReadUInt32();
                reader.ReadUInt32();
                reader.ReadUInt32();
                uint num9 = reader.ReadUInt32();
                reader.ReadUInt32();
                uint num10 = reader.ReadUInt32();
                uint num11 = reader.ReadUInt32();
                uint num12 = reader.ReadUInt32();
                uint num13 = reader.ReadUInt32();
                reader.ReadUInt32();
                reader.ReadUInt32();
                reader.ReadUInt32();
                if (num8 == 0)
                {
                    uint num14 = 20 + (0x30 * num3);
                    num6 = num14 + num10;
                    num7 = num11;
                    switch (num9)
                    {
                        case 0xac44:
                            header = WaveFileBuilder.CreateWaveHeader(WaveFormat.Format44100Hz_16Bit_Stereo, (long) num7);
                            break;

                        case 0x5622:
                            header = WaveFileBuilder.CreateWaveHeader(WaveFormat.Format22050Hz_16Bit_Stereo, (long) num7);
                            break;
                    }
                    fileName = FileCounter.GetFileName(i + 1, ".wav");
                }
                else
                {
                    if (num8 != 1)
                    {
                        throw new NotSupportedException("File type " + num8 + " in BSMS not supported.");
                    }
                    fileName = FileCounter.GetFileName(i + 1, ".ogg");
                    num6 = num12;
                    num7 = num13;
                }
                string name = Path.Combine(directoryPrefix, fileName);
                if (header == null)
                {
                    entryArray[i] = new GenericDirectoryEntry(name, position + num6, (long) num7);
                }
                else
                {
                    entryArray[i] = new GenericDirectoryEntryWithHeader(name, position + num6, (long) num7, header);
                }
            }
            return entryArray;
        }

        protected override GenericDirectoryEntry[] ReadGenericDirectory(Stream stream, BinaryReader reader)
        {
            if (!this.IsValidFormat(stream, reader))
            {
                throw new InvalidDataException("This is not a valid BSMS file.");
            }
            stream.Seek(0L, SeekOrigin.Begin);
            return ReadBsmsFile(stream, reader, "");
        }

        public override string[] Extensions
        {
            get
            {
                return new string[] { ".bsms" };
            }
        }

        public override string TypeName
        {
            get
            {
                return "Summer Athletics BSMS File";
            }
        }
    }
}

