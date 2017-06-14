namespace Ravioli.ArchivePlugins.SaintsRow
{
    using Ravioli.ArchiveInterface;
    using Ravioli.Xbox360Plugins;
    using System;
    using System.Collections.Generic;
    using System.IO;

    public class BnkPcFile : GenericArchive
    {
        protected override bool IsValidFormat(Stream stream, BinaryReader reader)
        {
            uint num = reader.ReadUInt32();
            uint num2 = reader.ReadUInt32();
            return ((num == 0x42535756) && (num2 == 0x20204350));
        }

        protected override GenericDirectoryEntry[] ReadGenericDirectory(Stream stream, BinaryReader reader)
        {
            if (!this.IsValidFormat(stream, reader))
            {
                throw new InvalidDataException("This is not a valid BNK_PC file.");
            }
            reader.ReadUInt32();
            reader.ReadUInt16();
            reader.ReadUInt16();
            reader.ReadUInt32();
            reader.ReadUInt32();
            uint num = reader.ReadUInt32();
            List<GenericDirectoryEntry> list = new List<GenericDirectoryEntry>();
            for (int i = 0; i < num; i++)
            {
                uint num3 = reader.ReadUInt32();
                uint num4 = reader.ReadUInt32();
                reader.ReadUInt32();
                uint num5 = reader.ReadUInt32();
                list.Add(new GenericDirectoryEntry(num3.ToString(), (long) num4, (long) num5));
            }
            foreach (GenericDirectoryEntry entry in list)
            {
                long position = stream.Position;
                stream.Position = entry.Offset;
                if ((entry.Length > 4L) && (reader.ReadUInt32() == 0x56414d44))
                {
                    for (int j = 0x800; j < entry.Length; j += 0x800)
                    {
                        stream.Position = entry.Offset + j;
                        switch (reader.ReadUInt32())
                        {
                            case 0x46464952:
                            case 0x58464952:
                                entry.Offset += j;
                                goto Label_012D;
                        }
                    }
                }
            Label_012D:
                stream.Position = position;
                entry.Name = entry.Name + BnkHelper.DetermineFileExtension(stream, entry, "");
            }
            return list.ToArray();
        }

        public override string[] Extensions
        {
            get
            {
                return new string[] { ".bnk_pc" };
            }
        }

        public override string TypeName
        {
            get
            {
                return "Saints Row 3/4 BNK_PC (VWSB) File";
            }
        }
    }
}

