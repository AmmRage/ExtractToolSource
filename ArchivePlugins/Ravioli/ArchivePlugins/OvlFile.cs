namespace Ravioli.ArchivePlugins
{
    using Ravioli.ArchiveInterface;
    using Ravioli.PluginHelpers;
    using Ravioli.Xbox360Plugins;
    using System;
    using System.IO;
    using System.Text;

    public class OvlFile : GenericCompressedArchive
    {
        private string idString;

        protected override void DecompressArchive(Stream stream, BinaryReader reader, Stream outputStream)
        {
            if (!this.IsValidFormat(stream, reader))
            {
                throw new InvalidDataException("This is not a valid OVL file.");
            }
            reader.ReadUInt16();
            reader.ReadUInt16();
            reader.ReadUInt32();
            reader.ReadUInt32();
            int count = reader.ReadInt32();
            reader.ReadBytes(0x7c);
            byte[] bytes = reader.ReadBytes(count);
            string[] strArray = Encoding.GetEncoding(0x4e4).GetString(bytes).TrimEnd(new char[1]).Split(new char[1]);
            if (strArray.Length < 2)
            {
                throw new InvalidDataException("Invalid resource identifier format.");
            }
            string[] strArray2 = strArray[0].Split(new char[] { ':' });
            if (strArray2.Length < 3)
            {
                throw new InvalidDataException("Invalid resource identifier format.");
            }
            string str2 = strArray2[0];
            string str3 = strArray2[1];
            string str4 = strArray2[2];
            if (str2 != "FGDK")
            {
                throw new InvalidDataException("Invalid resource identifier format.");
            }
            if (str3 != "AudioBank")
            {
                throw new NotSupportedException("Resource type \"" + str3 + "\" is not supported.");
            }
            if (str4 != "bnk")
            {
                throw new NotSupportedException("Audio bank type \"" + str4 + "\" is not supported.");
            }
            string str5 = strArray[1];
            if (str5.Length == 0)
            {
                throw new InvalidDataException("Resource name not found.");
            }
            reader.ReadBytes(0x54);
            reader.ReadUInt32();
            reader.ReadBytes(0x1c);
            Compression.DecompressZlibStream(stream, outputStream, 0xffffffffL);
        }

        protected override bool IsValidFormat(Stream stream, BinaryReader reader)
        {
            stream.Seek(0L, SeekOrigin.Begin);
            return (reader.ReadUInt32() == 0x53455246);
        }

        protected override GenericDirectoryEntry[] ReadGenericDirectory(Stream stream, BinaryReader reader)
        {
            reader.ReadUInt16();
            reader.ReadUInt16();
            reader.ReadBytes(0x1c);
            reader.ReadInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            int num = reader.ReadInt32();
            reader.ReadUInt32();
            reader.ReadInt32();
            reader.ReadBytes(0x2c);
            reader.ReadBytes(0x24);
            byte[] bytes = reader.ReadBytes(num - 0x24);
            string str = Encoding.GetEncoding(0x4e4).GetString(bytes).TrimEnd(new char[1]);
            if (str.Length == 0)
            {
                throw new InvalidDataException("Resource name not found.");
            }
            this.idString = str;
            return BnkReader.ReadBnkDirectory(stream, reader, out this.idString);
        }

        public override string Comment
        {
            get
            {
                return this.idString;
            }
        }

        public override string[] Extensions
        {
            get
            {
                return new string[] { ".ovl" };
            }
        }

        public override string TypeName
        {
            get
            {
                return "Elite Dangerous OVL File";
            }
        }
    }
}

