namespace Ravioli.ArchivePlugins
{
    using Ravioli.ArchiveInterface;
    using Ravioli.PluginHelpers.Reconstruction;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    public class PakFile : GenericArchive
    {
        protected override bool IsValidFormat(Stream stream, BinaryReader reader)
        {
            stream.Seek(0L, SeekOrigin.Begin);
            uint num = reader.ReadUInt32();
            uint num2 = reader.ReadUInt32();
            return (num == num2);
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
                throw new InvalidDataException("This is not a valid PAK/PA2 file.");
            }
            stream.Seek(0L, SeekOrigin.Begin);
            int num2 = (int) ((reader.ReadUInt32() - 4) / 0x15);
            List<GenericDirectoryEntry> list = new List<GenericDirectoryEntry>();
            for (int i = 0; i < num2; i++)
            {
                uint num4 = reader.ReadUInt32();
                uint num5 = reader.ReadUInt32();
                byte[] bytes = reader.ReadBytes(13);
                string path = Encoding.GetEncoding(0x4e4).GetString(bytes).TrimEnd(new char[1]);
                byte[] buffer2 = null;
                if ((path.EndsWith(".MUS") || path.EndsWith(".SND")) && (num5 >= 4))
                {
                    long position = stream.Position;
                    stream.Seek((long) num4, SeekOrigin.Begin);
                    uint num7 = reader.ReadUInt32();
                    stream.Position = position;
                    if (num7 != 0x46464952)
                    {
                        if (path.EndsWith(".MUS"))
                        {
                            buffer2 = WaveFileBuilder.CreateWaveHeader(WaveFormat.Format16000Hz_8Bit_Stereo, (long) num5);
                            path = Path.ChangeExtension(path, ".WAV");
                        }
                        else if (path.EndsWith(".SND") && (!path.StartsWith("T") || (path.Length != 8)))
                        {
                            buffer2 = WaveFileBuilder.CreateWaveHeader(WaveFormat.Format8000Hz_8Bit_Stereo, (long) num5);
                            path = Path.ChangeExtension(path, ".WAV");
                        }
                    }
                    else
                    {
                        path = Path.ChangeExtension(path, ".WAV");
                    }
                }
                if (buffer2 == null)
                {
                    GenericDirectoryEntry item = new GenericDirectoryEntry(path, (long) num4, (long) num5);
                    list.Add(item);
                }
                else
                {
                    GenericDirectoryEntryWithHeader header = new GenericDirectoryEntryWithHeader {
                        Header = buffer2,
                        Name = path,
                        Offset = (long) num4,
                        Length = (long) num5
                    };
                    list.Add(header);
                }
            }
            return list.ToArray();
        }

        public override string[] Extensions
        {
            get
            {
                return new string[] { ".pak", ".pa2" };
            }
        }

        public override string TypeName
        {
            get
            {
                return "Jack Orlando PAK File";
            }
        }
    }
}

