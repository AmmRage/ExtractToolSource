namespace Ravioli.ArchivePlugins
{
    using Ravioli.ArchiveInterface;
    using Ravioli.PluginHelpers;
    using System;
    using System.Collections.Generic;
    using System.IO;

    public class SAPakFile : GenericArchive
    {
        protected override bool IsValidFormat(Stream stream, BinaryReader reader)
        {
            stream.Seek(0L, SeekOrigin.Begin);
            return (reader.ReadUInt32() == 0xdeadbeef);
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

        protected override GenericDirectoryEntry[] ReadGenericDirectory(Stream stream, BinaryReader reader)
        {
            if (!this.IsValidFormat(stream, reader))
            {
                throw new InvalidDataException("This is not a valid PAK file.");
            }
            stream.Seek(0L, SeekOrigin.Begin);
            reader.ReadUInt32();
            uint num = reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            List<GenericDirectoryEntry> list = new List<GenericDirectoryEntry>();
            int num2 = 1;
            int num3 = 1;
            int num4 = 1;
            SortedList<long, GenericDirectoryEntry[]> list2 = new SortedList<long, GenericDirectoryEntry[]>();
            SortedList<long, GenericDirectoryEntry[]> list3 = new SortedList<long, GenericDirectoryEntry[]>();
            for (int i = 0; i < num; i++)
            {
                string fileName;
                reader.ReadUInt32();
                reader.ReadUInt32();
                uint num6 = reader.ReadUInt32();
                reader.ReadUInt32();
                uint num7 = reader.ReadUInt32();
                reader.ReadUInt32();
                reader.ReadUInt32();
                reader.ReadUInt32();
                reader.ReadUInt32();
                reader.ReadUInt32();
                long position = stream.Position;
                stream.Position = num6;
                switch (reader.ReadUInt32())
                {
                    case 0x5367674f:
                        fileName = FileCounter.GetFileName(num4++, ".ogv");
                        break;

                    case 0x534d5342:
                    {
                        stream.Position = num6;
                        GenericDirectoryEntry[] entryArray = SABsmsFile.ReadBsmsFile(stream, reader, FileCounter.GetFileName("BSMS", num2++));
                        list2.Add((long) num6, entryArray);
                        fileName = null;
                        break;
                    }
                    case 0x3c3c3c3c:
                    {
                        stream.Position = num6;
                        GenericDirectoryEntry[] entryArray2 = SANamesFile.ReadNamesFile(stream, reader, FileCounter.GetFileName("NAMES", num3++));
                        list3.Add((long) num6, entryArray2);
                        fileName = null;
                        break;
                    }
                    default:
                        fileName = FileCounter.GetFileName(num4++);
                        break;
                }
                stream.Position = position;
                if (fileName != null)
                {
                    GenericDirectoryEntry item = new GenericDirectoryEntry(fileName, (long) num6, (long) num7);
                    list.Add(item);
                }
            }
            for (int j = 0; j < list2.Count; j++)
            {
                GenericDirectoryEntry[] collection = list2.Values[j];
                if ((list3.Count - 1) >= j)
                {
                    GenericDirectoryEntry[] entryArray4 = list3.Values[j];
                    for (int k = 0; k < collection.Length; k++)
                    {
                        if (((entryArray4.Length - 1) >= k) && (entryArray4[k] != null))
                        {
                            collection[k].Name = Path.Combine(Path.GetDirectoryName(collection[k].Name), Path.GetFileName(entryArray4[k].Name)) + Path.GetExtension(collection[k].Name);
                        }
                    }
                }
                list.AddRange(collection);
            }
            return list.ToArray();
        }

        public override string[] Extensions
        {
            get
            {
                return new string[] { ".pak" };
            }
        }

        public override string TypeName
        {
            get
            {
                return "Summer Athletics PAK File";
            }
        }
    }
}

