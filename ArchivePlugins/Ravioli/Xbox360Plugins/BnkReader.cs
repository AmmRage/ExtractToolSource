namespace Ravioli.Xbox360Plugins
{
    using Ravioli.ArchiveInterface;
    using Ravioli.PluginHelpers.Reading;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;

    internal class BnkReader
    {
        public static bool IsValidFormat(Stream stream, BinaryReader reader)
        {
            return (reader.ReadUInt32() == 0x44484b42);
        }

        public static GenericDirectoryEntry[] ReadBnkDirectory(Stream stream, BinaryReader reader, out string idString)
        {
            string str2;
            uint num2;
            long position = stream.Position;
            if (!IsValidFormat(stream, reader))
            {
                string str = "0x" + position.ToString("X");
                throw new InvalidDataException("BNK file @ offset " + str + " is not valid.");
            }
            stream.Seek(position, SeekOrigin.Begin);
            bool littleEndian = true;
            EndianBinaryReader reader2 = new EndianBinaryReader(stream, littleEndian);
            List<GenericDirectoryEntry> list = new List<GenericDirectoryEntry>();
            idString = string.Empty;
            while (RiffReader.ReadChunkStart(stream, littleEndian, out str2, out num2))
            {
                switch (str2)
                {
                    case "DIDX":
                    {
                        long num3 = stream.Position;
                        do
                        {
                            uint num4 = reader2.ReadUInt32();
                            uint num5 = reader2.ReadUInt32();
                            uint num6 = reader2.ReadUInt32();
                            list.Add(new GenericDirectoryEntry(num4.ToString(), (long) num5, (long) num6));
                        }
                        while (stream.Position < (num3 + num2));
                        continue;
                    }
                    case "DATA":
                    {
                        long num7 = stream.Position;
                        foreach (GenericDirectoryEntry entry in list)
                        {
                            entry.Offset += num7;
                            entry.Name = entry.Name + BnkHelper.DetermineFileExtension(stream, entry, "");
                        }
                        stream.Position += num2;
                        continue;
                    }
                    case "BKHD":
                    {
                        if (num2 >= 0x80000)
                        {
                            if (!littleEndian)
                            {
                                throw new InvalidOperationException("Unable to determine byte order in file.");
                            }
                            stream.Position = position;
                            littleEndian = false;
                            reader2 = new EndianBinaryReader(stream, littleEndian);
                        }
                        else
                        {
                            stream.Position += num2;
                        }
                        continue;
                    }
                }
                if (str2 == "HIRC")
                {
                    stream.Position += num2;
                }
                else
                {
                    if (str2 == "STID")
                    {
                        reader2.ReadUInt32();
                        reader2.ReadUInt32();
                        reader2.ReadUInt32();
                        byte count = reader2.ReadByte();
                        byte[] bytes = reader2.ReadBytes(count);
                        idString = Encoding.GetEncoding(0x4e4).GetString(bytes);
                        continue;
                    }
                    stream.Position += num2;
                }
            }
            return list.ToArray();
        }
    }
}

