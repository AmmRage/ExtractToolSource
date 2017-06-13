namespace Ravioli.AppShared
{
    using System;
    using System.IO;

    internal class FileTypeDetector
    {
        public static FileType DetectFileType(string fileName)
        {
            FileType unknown;
            FileStream input = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            BinaryReader reader = new BinaryReader(input);
            try
            {
                if (reader.ReadUInt16() == 0x5a4d)
                {
                    unknown = FileType.DosExe;
                    input.Position += 0x3aL;
                    uint num2 = reader.ReadUInt32();
                    input.Position = num2;
                    byte[] buffer = reader.ReadBytes(4);
                    if (buffer.Length == 4)
                    {
                        ushort num3 = BitConverter.ToUInt16(buffer, 0);
                        if (BitConverter.ToUInt32(buffer, 0) == 0x4550)
                        {
                            unknown = FileType.Unmanaged;
                            input.Position += 20L;
                            uint position = (uint) input.Position;
                            switch (reader.ReadUInt16())
                            {
                                case 0x10b:
                                    input.Position = position;
                                    input.Position = position + 0x5c;
                                    if (reader.ReadUInt32() >= 15)
                                    {
                                        input.Position = position + 0xd0;
                                        uint num8 = reader.ReadUInt32();
                                        uint num9 = reader.ReadUInt32();
                                        if ((num8 > 0) && (num9 > 0))
                                        {
                                            unknown = FileType.Managed;
                                        }
                                    }
                                    return unknown;

                                case 0x20b:
                                {
                                    input.Position = position + 0x6c;
                                    if (reader.ReadUInt32() < 15)
                                    {
                                        return unknown;
                                    }
                                    input.Position = position + 0xe0;
                                    uint num11 = reader.ReadUInt32();
                                    uint num12 = reader.ReadUInt32();
                                    if ((num11 > 0) && (num12 > 0))
                                    {
                                        unknown = FileType.Managed;
                                    }
                                    break;
                                }
                            }
                            return unknown;
                        }
                        if (num3 == 0x454e)
                        {
                            unknown = FileType.Win16;
                        }
                    }
                    return unknown;
                }
                if (input.Length <= 0x10000L)
                {
                    return FileType.DosCom;
                }
                unknown = FileType.Unknown;
            }
            finally
            {
                reader.Close();
                input.Close();
            }
            return unknown;
        }
    }
}

