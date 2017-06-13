namespace Ravioli.Explorer
{
    using Ravioli.AppShared;
    using Ravioli.ArchiveInterface;
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Runtime.InteropServices;

    internal class ThumbnailGenerator : IDisposable
    {
        private IconCache iconCache;
        private ImagePluginManager imagePluginManager;

        public ThumbnailGenerator(ImagePluginManager imagePluginManager)
        {
            this.imagePluginManager = imagePluginManager;
            this.iconCache = new IconCache();
        }

        public void Dispose()
        {
            this.iconCache.Dispose();
        }

        public Bitmap GetEmptyThumbnail(int width, int height)
        {
            return new Bitmap(width, height, PixelFormat.Format32bppArgb);
        }

        public Bitmap GetErrorThumbnail(int width, int height)
        {
            Bitmap image = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            using (Graphics graphics = Graphics.FromImage(image))
            {
                using (Pen pen = new Pen(Color.Red))
                {
                    pen.Width = 1f;
                    graphics.DrawLine(pen, 0, 0, width - 1, height - 1);
                    graphics.DrawLine(pen, width - 1, 0, 0, height - 1);
                }
            }
            return image;
        }

        public Bitmap GetThumbnail(IArchive archive, IFileInfo fileInfo, int width, int height, out ThumbnailOrigin origin)
        {
            Bitmap errorThumbnail;
            try
            {
                errorThumbnail = this.GetThumbnailByFileContent(archive, fileInfo, width, height);
                origin = ThumbnailOrigin.Content;
            }
            catch (Exception)
            {
                errorThumbnail = this.GetErrorThumbnail(width, height);
                origin = ThumbnailOrigin.Error;
            }
            if (errorThumbnail == null)
            {
                try
                {
                    errorThumbnail = this.GetThumbnailByFileType(fileInfo, width, height);
                    origin = ThumbnailOrigin.Type;
                }
                catch (Exception)
                {
                    errorThumbnail = this.GetErrorThumbnail(width, height);
                    origin = ThumbnailOrigin.Error;
                }
            }
            return errorThumbnail;
        }

        public Bitmap GetThumbnailByFileContent(IArchive archive, IFileInfo fileInfo, int width, int height)
        {
            Bitmap bitmap = null;
            ImagePluginMapping mapping;
            using (MemoryStream stream = new MemoryStream())
            {
                int num = 0x100;
                archive.ExtractFile(fileInfo, stream, (long) num);
                stream.Position = 0L;
                mapping = this.imagePluginManager.DetectImageFormat(fileInfo, stream);
            }
            if (mapping != null)
            {
                IImageLoader loader = this.imagePluginManager.CreateLoaderInstance(mapping);
                IPaletteConsumer consumer = loader as IPaletteConsumer;
                IPaletteProvider provider = archive as IPaletteProvider;
                if ((consumer != null) && (provider != null))
                {
                    consumer.Palette = provider.Palette;
                }
                using (MemoryStream stream2 = new MemoryStream())
                {
                    archive.ExtractFile(fileInfo, stream2);
                    stream2.Position = 0L;
                    using (Image image = loader.LoadImage(stream2))
                    {
                        bitmap = PixelFormatConverter.GenerateThumbnail(image, width, height);
                    }
                }
            }
            return bitmap;
        }

        public Bitmap GetThumbnailByFileType(IFileInfo fileInfo, int width, int height)
        {
            string extension = Path.GetExtension(fileInfo.Name);
            Bitmap icon = this.iconCache.GetIcon(extension, IconSize.Large);
            if (icon != null)
            {
                try
                {
                    return PixelFormatConverter.GenerateThumbnail(icon, width, height);
                }
                finally
                {
                    icon.Dispose();
                }
            }
            return this.GetEmptyThumbnail(width, height);
        }

        public enum ThumbnailOrigin
        {
            Content,
            Type,
            Error
        }
    }
}

