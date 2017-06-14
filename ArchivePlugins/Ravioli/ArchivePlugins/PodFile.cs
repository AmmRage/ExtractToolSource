namespace Ravioli.ArchivePlugins
{
    using Ravioli.ArchiveInterface;
    using Ravioli.PluginHelpers.Reading;
    using System;
    using System.IO;
    using System.Text;

    public class PodFile : GenericArchive
    {
        private string comment = string.Empty;

        protected override bool IsValidFormat(Stream stream, BinaryReader reader)
        {
            stream.Seek(0L, SeekOrigin.Begin);
            return (reader.ReadUInt32() == 0x33444f50);
        }

        protected override GenericDirectoryEntry[] ReadGenericDirectory(Stream stream, BinaryReader reader)
        {
            if (!this.IsValidFormat(stream, reader))
            {
                throw new InvalidDataException("This is not a valid POD file.");
            }
            stream.Seek(0L, SeekOrigin.Begin);
            reader.ReadUInt32();
            reader.ReadUInt32();
            byte[] bytes = reader.ReadBytes(80);
            uint num = reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadBytes(160);
            uint num2 = reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            this.comment = Encoding.GetEncoding(0x4e4).GetString(bytes).TrimEnd(new char[1]);
            GenericDirectoryEntry[] entryArray = new GenericDirectoryEntry[num];
            stream.Seek((long) num2, SeekOrigin.Begin);
            for (int i = 0; i < num; i++)
            {
                reader.ReadUInt32();
                uint num4 = reader.ReadUInt32();
                uint num5 = reader.ReadUInt32();
                reader.ReadUInt32();
                reader.ReadUInt32();
                entryArray[i] = new GenericDirectoryEntry("", (long) num5, (long) num4);
            }
            for (int j = 0; j < num; j++)
            {
                string str = StaticReader.ReadZeroTerminatedString(stream);
                entryArray[j].Name = str;
            }
            return entryArray;
        }

        public override string Comment
        {
            get
            {
                return this.comment;
            }
        }

        public override string[] Extensions
        {
            get
            {
                return new string[] { ".pod" };
            }
        }

        public override string TypeName
        {
            get
            {
                return "BloodRayne POD File";
            }
        }
    }
}

