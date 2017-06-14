namespace Ravioli.ArchivePlugins
{
    using Ravioli.ArchiveInterface;
    using System;
    using System.IO;

    public class AudFile : GenericArchive
    {
        protected override bool IsValidFormat(Stream stream, BinaryReader reader)
        {
            bool flag = true;
            stream.Seek(0L, SeekOrigin.Begin);
            if (stream.Length < 130L)
            {
                flag = false;
            }
            if (flag && (reader.ReadInt32() != 0x4d42494e))
            {
                flag = false;
            }
            int num2 = 0;
            if (flag)
            {
                num2 = reader.ReadInt32();
                if (num2 != 2)
                {
                    flag = false;
                }
            }
            if (flag)
            {
                for (int i = 0; i < num2; i++)
                {
                    int count = reader.ReadInt32();
                    string str = new string(reader.ReadChars(count));
                    reader.ReadInt32();
                    if (((i == 0) && (str != "class AudioData")) || ((i == 1) && (str != "struct AudioData::Streamed")))
                    {
                        flag = false;
                    }
                }
            }
            if (flag)
            {
                stream.Seek(0x7eL, SeekOrigin.Begin);
                if (reader.ReadInt32() != 0x5367674f)
                {
                    flag = false;
                }
            }
            return flag;
        }

        protected override GenericDirectoryEntry[] ReadGenericDirectory(Stream stream, BinaryReader reader)
        {
            if (!this.IsValidFormat(stream, reader))
            {
                throw new InvalidDataException("This is not a valid AUD file.");
            }
            GenericDirectoryEntry entry = new GenericDirectoryEntry {
                Name = Path.ChangeExtension(Path.GetFileName(base.FileName), ".ogg"),
                Offset = 0x7eL,
                Length = stream.Length - 0x7eL
            };
            return new GenericDirectoryEntry[] { entry };
        }

        public override string[] Extensions
        {
            get
            {
                return new string[] { ".aud" };
            }
        }

        public override string TypeName
        {
            get
            {
                return "Telltale AUD File";
            }
        }
    }
}

