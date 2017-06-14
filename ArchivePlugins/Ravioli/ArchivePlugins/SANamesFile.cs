namespace Ravioli.ArchivePlugins
{
    using Ravioli.ArchiveInterface;
    using Ravioli.PluginHelpers.Reading;
    using System;
    using System.IO;

    internal class SANamesFile : GenericArchive
    {
        protected override bool IsValidFormat(Stream stream, BinaryReader reader)
        {
            stream.Seek(0L, SeekOrigin.Begin);
            return (reader.ReadUInt32() == 0x3c3c3c3c);
        }

        protected override GenericDirectoryEntry[] ReadGenericDirectory(Stream stream, BinaryReader reader)
        {
            if (!this.IsValidFormat(stream, reader))
            {
                throw new InvalidDataException("This is not a valid NAMES file.");
            }
            stream.Seek(0L, SeekOrigin.Begin);
            return ReadNamesFile(stream, reader, "");
        }

        private static string[] ReadNames(Stream stream, BinaryReader reader, uint count)
        {
            long position = stream.Position;
            string[] strArray = new string[count];
            for (int i = 0; i < count; i++)
            {
                uint index = reader.ReadUInt32();
                uint num4 = reader.ReadUInt32();
                long num5 = stream.Position;
                stream.Position = position + num4;
                strArray[index] = StaticReader.ReadZeroTerminatedUnicodeString(stream);
                stream.Position = num5;
            }
            return strArray;
        }

        internal static GenericDirectoryEntry[] ReadNamesFile(Stream stream, BinaryReader reader, string directoryPrefix)
        {
            uint dataCount;
            long position = stream.Position;
            if (reader.ReadUInt32() != 0x3c3c3c3c)
            {
                throw new InvalidDataException("This is not a valid NAMES file.");
            }
            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            uint num2 = reader.ReadUInt32();
            uint num3 = reader.ReadUInt32();
            if ((num2 == 1) && (num3 == 1))
            {
                dataCount = ReadType1Header(stream, reader).DataCount;
            }
            else
            {
                reader.ReadUInt32();
                dataCount = reader.ReadUInt32();
            }
            uint num5 = reader.ReadUInt32();
            long num6 = stream.Position;
            GenericDirectoryEntry[] entryArray = new GenericDirectoryEntry[dataCount];
            string[] strArray = ReadNames(stream, reader, dataCount);
            stream.Position = num6 + num5;
            for (int i = 0; i < dataCount; i++)
            {
                long offset = stream.Position;
                long length = 0L;
                byte[] buffer = reader.ReadBytes(60);
                uint num10 = reader.ReadUInt32();
                uint num11 = reader.ReadUInt32();
                long num12 = -1L;
                if ((num10 == 1) && (num11 == 1))
                {
                    Type1Header header2 = ReadType1Header(stream, reader);
                    if (header2.DataCount != uint.MaxValue)
                    {
                        byte[] buffer2 = reader.ReadBytes((int) (header2.DataCount * 0x70));
                        length = ((((buffer.Length + 8) + 4) + header2.NamesLength) + 20L) + buffer2.Length;
                    }
                }
                else
                {
                    byte[] buffer3 = reader.ReadBytes(20);
                    num12 = reader.ReadUInt32();
                    byte[] buffer4 = reader.ReadBytes(0x48);
                    length = (((buffer.Length + 8) + buffer3.Length) + 4) + buffer4.Length;
                }
                if ((num12 >= 0L) && (num12 < dataCount))
                {
                    string name = Path.Combine(directoryPrefix, strArray[i]);
                    entryArray[(int) ((IntPtr) num12)] = new GenericDirectoryEntry(name, offset, length);
                }
            }
            return entryArray;
        }

        private static Type1Header ReadType1Header(Stream stream, BinaryReader reader)
        {
            Type1Header header = new Type1Header {
                NamesLength = reader.ReadUInt32()
            };
            long position = stream.Position;
            header.Names = ReadNames(stream, reader, 1);
            stream.Position = position + header.NamesLength;
            header.Unknown1 = reader.ReadUInt32();
            header.Unknown2 = reader.ReadUInt32();
            header.Unknown3 = reader.ReadUInt32();
            header.Unknown4 = reader.ReadUInt32();
            header.DataCount = reader.ReadUInt32();
            return header;
        }

        public override string[] Extensions
        {
            get
            {
                return new string[] { ".names" };
            }
        }

        public override string TypeName
        {
            get
            {
                return "Summer Athletics NAMES File";
            }
        }

        private class Type1Header
        {
            private uint dataCount;
            private string[] names;
            private uint namesLength;
            private uint unknown1;
            private uint unknown2;
            private uint unknown3;
            private uint unknown4;

            public uint DataCount
            {
                get
                {
                    return this.dataCount;
                }
                set
                {
                    this.dataCount = value;
                }
            }

            public string[] Names
            {
                get
                {
                    return this.names;
                }
                set
                {
                    this.names = value;
                }
            }

            public uint NamesLength
            {
                get
                {
                    return this.namesLength;
                }
                set
                {
                    this.namesLength = value;
                }
            }

            public uint Unknown1
            {
                get
                {
                    return this.unknown1;
                }
                set
                {
                    this.unknown1 = value;
                }
            }

            public uint Unknown2
            {
                get
                {
                    return this.unknown2;
                }
                set
                {
                    this.unknown2 = value;
                }
            }

            public uint Unknown3
            {
                get
                {
                    return this.unknown3;
                }
                set
                {
                    this.unknown3 = value;
                }
            }

            public uint Unknown4
            {
                get
                {
                    return this.unknown4;
                }
                set
                {
                    this.unknown4 = value;
                }
            }
        }
    }
}

