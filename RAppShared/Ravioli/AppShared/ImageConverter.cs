namespace Ravioli.AppShared
{
    using Ravioli.ArchiveInterface;
    using System;
    using System.Drawing;
    using System.IO;

    public class ImageConverter
    {
        private ImagePluginManager imagePluginManager;

        public ImageConverter(ImagePluginManager imageManager)
        {
            if (imageManager == null)
            {
                throw new ArgumentNullException("imageManager");
            }
            this.imagePluginManager = imageManager;
        }

        public void ConvertFormat(IArchive archive, IFileInfo file, ImagePluginMapping loaderMapping, IImageSaver outputSaver, string outputDir)
        {
            Stream stream = this.CreateSourceCopy(archive, file);
            try
            {
                if (loaderMapping != null)
                {
                    IImageLoader loader = this.imagePluginManager.CreateLoaderInstance(loaderMapping);
                    IPaletteConsumer consumer = loader as IPaletteConsumer;
                    IPaletteProvider provider = archive as IPaletteProvider;
                    if ((consumer != null) && (provider != null))
                    {
                        consumer.Palette = provider.Palette;
                    }
                    Image image = loader.LoadImage(stream);
                    string path = Path.ChangeExtension(Path.Combine(outputDir, file.Name), this.imagePluginManager.EnsurePeriod(outputSaver.Extensions[0]));
                    string str3 = Path.Combine(Path.GetDirectoryName(path), Path.GetFileName(path));
                    string directoryName = Path.GetDirectoryName(str3);
                    if (!Directory.Exists(directoryName))
                    {
                        Directory.CreateDirectory(directoryName);
                    }
                    lock (outputSaver)
                    {
                        outputSaver.SaveImage(image, str3);
                        return;
                    }
                }
                throw new ArgumentException("No matching image loader found for \"" + file.Name + "\".");
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

        public ImagePluginMapping FindImageLoader(IArchive archive, IFileInfo file)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                int num = 0x100;
                archive.ExtractFile(file, stream, (long) num);
                stream.Position = 0L;
                return this.imagePluginManager.DetectImageFormat(file, stream);
            }
        }
    }
}

