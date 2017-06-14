namespace Ravioli.ArchivePlugins
{
    using Ravioli.ArchiveInterface;
    using Ravioli.PluginHelpers.Imaging;
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;

    public class GfxLoader : IImageLoader, IClassInfo, IPaletteConsumer
    {
        private byte[] paletteBytes;

        public Image LoadImage(Stream stream)
        {
            Image image;
            BinaryReader reader = new BinaryReader(stream);
            try
            {
                image = this.LoadImage(stream, reader);
            }
            finally
            {
                reader.Close();
            }
            return image;
        }

        public Image LoadImage(string fileName)
        {
            Image image;
            FileStream input = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            BinaryReader reader = new BinaryReader(input);
            try
            {
                image = this.LoadImage(input, reader);
            }
            finally
            {
                reader.Close();
                input.Close();
            }
            return image;
        }

        private Image LoadImage(Stream stream, BinaryReader reader)
        {
            ushort num;
            ushort num2;
            byte num3 = reader.ReadByte();
            if (num3 != 0xe5)
            {
                byte num4 = reader.ReadByte();
                num = BitConverter.ToUInt16(new byte[] { num3, num4 }, 0);
                num2 = reader.ReadUInt16();
            }
            else
            {
                num = reader.ReadUInt16();
                num2 = reader.ReadUInt16();
            }
            if (this.paletteBytes == null)
            {
                throw new InvalidOperationException("This image type requires a color palette set.");
            }
            ColorPalette palette = ImageHelper.ConvertRGBColorPalette(this.paletteBytes, 0x100);
            return this.Read256ColorIndexedBitmap(stream, num, num2, palette);
        }

        private Bitmap Read256ColorIndexedBitmap(Stream stream, ushort width, ushort height, ColorPalette palette)
        {
            Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format8bppIndexed) {
                Palette = palette
            };
            IndexedBitmapAccess access = new IndexedBitmapAccess(bitmap);
            access.Lock();
            try
            {
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        int num3 = stream.ReadByte();
                        if (num3 < 0)
                        {
                            throw new EndOfStreamException("End of stream reached too early while reading pixel data.");
                        }
                        access.SetPixel(j, i, (byte) num3);
                    }
                }
            }
            finally
            {
                access.Unlock(true);
            }
            return bitmap;
        }

        public string[] Extensions
        {
            get
            {
                return new string[] { ".gfx" };
            }
        }

        public byte[] Palette
        {
            get
            {
                return this.paletteBytes;
            }
            set
            {
                this.paletteBytes = value;
            }
        }

        public string TypeName
        {
            get
            {
                return "Absolute Magic GFX File";
            }
        }
    }
}

