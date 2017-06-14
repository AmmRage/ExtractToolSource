namespace Ravioli.ArchivePlugins
{
    using Ravioli.ArchiveInterface;
    using System;
    using System.IO;
    using System.Text;

    public class BrxFile : GenericArchive
    {
        private AudioFormat DetermineAudioFormat(Stream stream, BinaryReader reader)
        {
            AudioFormat none = AudioFormat.None;
            if (none == AudioFormat.None)
            {
                stream.Seek(0L, SeekOrigin.Begin);
                byte[] bytes = reader.ReadBytes("Creative Voice File".Length);
                if (Encoding.GetEncoding(0x4e4).GetString(bytes) == "Creative Voice File")
                {
                    none = AudioFormat.Sound;
                }
            }
            if (none == AudioFormat.None)
            {
                stream.Seek(0L, SeekOrigin.Begin);
                if (reader.ReadInt32() == 0x464d5443)
                {
                    none = AudioFormat.Music;
                }
            }
            return none;
        }

        protected override bool IsValidFormat(Stream stream, BinaryReader reader)
        {
            return (this.DetermineAudioFormat(stream, reader) != AudioFormat.None);
        }

        protected override GenericDirectoryEntry[] ReadGenericDirectory(Stream stream, BinaryReader reader)
        {
            if (!this.IsValidFormat(stream, reader))
            {
                throw new InvalidDataException("This is not a valid BRX file.");
            }
            AudioFormat format = this.DetermineAudioFormat(stream, reader);
            GenericDirectoryEntry entry = new GenericDirectoryEntry();
            switch (format)
            {
                case AudioFormat.Sound:
                    entry.Name = Path.ChangeExtension(Path.GetFileName(base.FileName), ".VOC");
                    entry.Offset = 0L;
                    entry.Length = stream.Length;
                    break;

                case AudioFormat.Music:
                    entry.Name = Path.ChangeExtension(Path.GetFileName(base.FileName), ".CMF");
                    entry.Offset = 0L;
                    entry.Length = stream.Length;
                    break;

                default:
                    throw new InvalidDataException("Invalid audio format.");
            }
            return new GenericDirectoryEntry[] { entry };
        }

        public override string[] Extensions
        {
            get
            {
                return new string[] { ".brx" };
            }
        }

        public override string TypeName
        {
            get
            {
                return "Brix BRX File";
            }
        }

        private enum AudioFormat
        {
            None,
            Sound,
            Music
        }
    }
}

