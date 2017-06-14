namespace Ravioli.ArchivePlugins
{
    using Ravioli.ArchiveInterface;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    public class SndFile : GenericArchive
    {
        protected override bool IsValidFormat(Stream stream, BinaryReader reader)
        {
            bool flag = true;
            stream.Seek(0L, SeekOrigin.Begin);
            byte[] buffer = reader.ReadBytes(2);
            flag = (buffer[0] == 0xff) && (buffer[1] == 10);
            if (flag)
            {
                reader.ReadByte();
                if (Encoding.GetEncoding(0x4e4).GetString(reader.ReadBytes(0x15)) != "DELPHIXWAVECOLLECTION")
                {
                    flag = false;
                }
            }
            if (flag)
            {
                reader.ReadByte();
                reader.ReadBytes(6);
                reader.ReadBytes(4);
                byte count = reader.ReadByte();
                if (Encoding.GetEncoding(0x4e4).GetString(reader.ReadBytes(count)) != "TWaveCollectionComponent")
                {
                    flag = false;
                }
            }
            if (flag)
            {
                reader.ReadByte();
                byte num2 = reader.ReadByte();
                if (Encoding.GetEncoding(0x4e4).GetString(reader.ReadBytes(num2)) != "List")
                {
                    flag = false;
                }
            }
            if (flag)
            {
                reader.ReadByte();
            }
            return flag;
        }

        protected override GenericDirectoryEntry[] ReadGenericDirectory(Stream stream, BinaryReader reader)
        {
            byte num2;
            if (!this.IsValidFormat(stream, reader))
            {
                throw new InvalidDataException("This is not a valid SND file.");
            }
            List<GenericDirectoryEntry> list = new List<GenericDirectoryEntry>();
            uint num = 0;
        Label_001D:
            num2 = reader.ReadByte();
            if (num2 != 10)
            {
                byte count = reader.ReadByte();
                reader.ReadBytes(count);
                if (num2 == 6)
                {
                    byte num4 = reader.ReadByte();
                    reader.ReadBytes(num4);
                }
                if ((num2 != 10) && (num2 != 0))
                {
                    goto Label_001D;
                }
            }
            if (num2 != 0)
            {
                uint num5 = reader.ReadUInt32();
                long position = stream.Position;
                GenericDirectoryEntry item = new GenericDirectoryEntry();
                uint num7 = num + 1;
                item.Name = "File" + num7.ToString().PadLeft(4, '0') + ".wav";
                item.Offset = (uint) position;
                item.Length = num5;
                stream.Seek((long) num5, SeekOrigin.Current);
                reader.ReadByte();
                list.Add(item);
                num++;
                if (num2 != 0)
                {
                    goto Label_001D;
                }
            }
            return list.ToArray();
        }

        public override string[] Extensions
        {
            get
            {
                return new string[] { ".snd" };
            }
        }

        public override string TypeName
        {
            get
            {
                return "Snocka Watten SND File";
            }
        }
    }
}

