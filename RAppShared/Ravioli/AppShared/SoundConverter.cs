namespace Ravioli.AppShared
{
    using Ravioli.ArchiveInterface;
    using System;
    using System.IO;

    public class SoundConverter
    {
        private SoundPluginManager soundPluginManager;

        public SoundConverter(SoundPluginManager soundManager)
        {
            if (soundManager == null)
            {
                throw new ArgumentNullException("soundManager");
            }
            this.soundPluginManager = soundManager;
        }

        public void ConvertFormat(IArchive archive, IFileInfo file, PluginMapping playerMapping, SoundExportFormat targetFormat, string outputDir)
        {
            Stream stream = this.CreateSourceCopy(archive, file);
            try
            {
                if (playerMapping != null)
                {
                    using (ISoundPlayer player = this.soundPluginManager.CreatePlayerInstance(playerMapping))
                    {
                        ISoundExport export = player as ISoundExport;
                        if (export == null)
                        {
                            throw new ArgumentException("Sound conversion from the source format is not supported.");
                        }
                        bool flag = false;
                        foreach (SoundExportFormat format in export.SupportedExportFormats)
                        {
                            if (format == targetFormat)
                            {
                                flag = true;
                                string path = Path.ChangeExtension(Path.Combine(outputDir, file.Name), this.soundPluginManager.EnsurePeriod(targetFormat.Extension));
                                string str3 = Path.Combine(Path.GetDirectoryName(path), Path.GetFileName(path));
                                string directoryName = Path.GetDirectoryName(str3);
                                if (!Directory.Exists(directoryName))
                                {
                                    Directory.CreateDirectory(directoryName);
                                }
                                player.LoadFromStream(stream);
                                export.ExportSound(targetFormat, str3);
                                break;
                            }
                        }
                        if (!flag)
                        {
                            throw new ArgumentException("Sound conversion from the format \"" + playerMapping.TypeName + "\" to the format \"" + targetFormat.Name + "\" is not supported.");
                        }
                        return;
                    }
                }
                throw new ArgumentException("No matching sound player found for \"" + file.Name + "\".");
            }
            finally
            {
                this.DestroySourceCopy(stream);
            }
        }

        private Stream CreateSourceCopy(IArchive archive, IFileInfo file)
        {
            Stream stream;
            if (file.Size > 0x500000L)
            {
                stream = new FileStream(Path.GetTempFileName(), FileMode.Create, FileAccess.ReadWrite);
            }
            else
            {
                stream = new MemoryStream();
            }
            lock (archive)
            {
                archive.ExtractFile(file, stream);
            }
            stream.Seek(0L, SeekOrigin.Begin);
            return stream;
        }

        private void DestroySourceCopy(Stream stream)
        {
            if (stream is MemoryStream)
            {
                ((MemoryStream) stream).Close();
            }
            else if (stream is FileStream)
            {
                FileStream stream3 = (FileStream) stream;
                string name = stream3.Name;
                stream3.Close();
                if ((name != null) && File.Exists(name))
                {
                    File.Delete(name);
                }
            }
        }

        public PluginMapping FindSoundPlayer(IArchive archive, IFileInfo file)
        {
            string extension = Path.GetExtension(file.Name);
            return this.soundPluginManager.FindPlayerPlugin(extension);
        }
    }
}

