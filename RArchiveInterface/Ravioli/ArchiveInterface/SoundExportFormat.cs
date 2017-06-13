namespace Ravioli.ArchiveInterface
{
    using System;

    public class SoundExportFormat
    {
        private string extension;
        public static readonly SoundExportFormat Mp3 = new SoundExportFormat("MPEG Layer 3 Audio", ".mp3");
        private string name;
        public static readonly SoundExportFormat Ogg = new SoundExportFormat("Ogg Vorbis", ".ogg");
        public static readonly SoundExportFormat Wav = new SoundExportFormat("Wave", ".wav");

        internal SoundExportFormat(string name, string extension)
        {
            this.name = name;
            this.extension = extension;
        }

        public static int CompareByName(SoundExportFormat x, SoundExportFormat y)
        {
            if (x == null)
            {
                if (y == null)
                {
                    return 0;
                }
                return -1;
            }
            if (y == null)
            {
                return 1;
            }
            return x.Name.CompareTo(y.Name);
        }

        public override string ToString()
        {
            return string.Format("Name=\"{0}\" Extension=\"{1}\"", this.Name, this.extension);
        }

        public string Extension
        {
            get
            {
                return this.extension;
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }
        }
    }
}

