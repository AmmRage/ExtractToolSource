namespace Ravioli.ArchivePlugins
{
    using Ravioli.ArchiveInterface;
    using Ravioli.GeneralSoundPlugins;
    using Ravioli.PluginHelpers.Reading;
    using Ravioli.PluginHelpers.Reconstruction;
    using System;
    using System.IO;
    using System.Text;

    public class SfxPlayer : ISoundPlayer, IClassInfo, IDisposable, ISoundExport
    {
        private WavPlayer internalPlayer;
        private MemoryStream soundStream;

        private void CreatePlayer()
        {
            if (this.internalPlayer == null)
            {
                this.internalPlayer = new WavPlayer();
            }
        }

        private void DestroyPlayer()
        {
            if (this.internalPlayer != null)
            {
                this.internalPlayer.Dispose();
                this.internalPlayer = null;
            }
            if (this.soundStream != null)
            {
                this.soundStream.Dispose();
                this.soundStream = null;
            }
        }

        public void Dispose()
        {
            this.DestroyPlayer();
        }

        public void ExportSound(SoundExportFormat format, string fileName)
        {
            if (format != SoundExportFormat.Wav)
            {
                throw new ArgumentException("An invalid sound format was specified.", "format");
            }
            if (this.soundStream == null)
            {
                throw new InvalidOperationException("No sound is available for export.");
            }
            FileStream stream = File.Create(fileName);
            try
            {
                this.soundStream.WriteTo(stream);
            }
            finally
            {
                stream.Close();
            }
        }

        public void LoadFromFile(string fileName)
        {
            this.DestroyPlayer();
            this.CreatePlayer();
            FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            try
            {
                this.LoadFromStreamInternal(stream);
            }
            finally
            {
                stream.Close();
            }
        }

        public void LoadFromStream(System.IO.Stream stream)
        {
            this.DestroyPlayer();
            this.CreatePlayer();
            this.LoadFromStreamInternal(stream);
        }

        private void LoadFromStreamInternal(System.IO.Stream stream)
        {
            byte num = (byte) stream.ReadByte();
            if (num != 0xf1)
            {
                throw new InvalidDataException("This is not a valid SFX file.");
            }
            byte count = (byte) stream.ReadByte();
            byte[] bytes = StaticReader.ReadBytes(stream, 30);
            Encoding.GetEncoding(0x4e4).GetString(bytes, 0, count);
            uint sampleRate = StaticReader.ReadUInt32(stream);
            stream.ReadByte();
            ushort num4 = StaticReader.ReadUInt16(stream);
            MemoryStream stream2 = new MemoryStream();
            WaveFormat format = new WaveFormat(sampleRate, 8, 1);
            byte[] buffer = WaveFileBuilder.CreateWaveHeader(format, (long) num4);
            stream2.Write(buffer, 0, buffer.Length);
            byte[] buffer3 = new byte[num4];
            int num5 = stream.Read(buffer3, 0, num4);
            if (num5 < num4)
            {
                throw new EndOfStreamException("End of stream reached too early while reading file.");
            }
            stream2.Write(buffer3, 0, num5);
            stream2.Position = 0L;
            this.soundStream = stream2;
            this.internalPlayer.LoadFromStream(this.soundStream);
        }

        public void Play()
        {
            this.CreatePlayer();
            this.internalPlayer.Play();
        }

        public void Stop()
        {
            this.CreatePlayer();
            this.internalPlayer.Stop();
        }

        public string[] Extensions
        {
            get
            {
                return new string[] { ".sfx" };
            }
        }

        public string FileName
        {
            get
            {
                this.CreatePlayer();
                return this.internalPlayer.FileName;
            }
        }

        public System.IO.Stream Stream
        {
            get
            {
                this.CreatePlayer();
                return this.internalPlayer.Stream;
            }
        }

        public SoundExportFormat[] SupportedExportFormats
        {
            get
            {
                return new SoundExportFormat[] { SoundExportFormat.Wav };
            }
        }

        public string TypeName
        {
            get
            {
                return "Absolute Magic SFX File";
            }
        }
    }
}

