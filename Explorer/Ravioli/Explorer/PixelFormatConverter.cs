namespace Ravioli.Explorer
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;

    internal class PixelFormatConverter
    {
        public static PixelFormat DetermineIndexedFormat(int colors)
        {
            if (colors > 0x10)
            {
                return PixelFormat.Format8bppIndexed;
            }
            if (colors > 2)
            {
                return PixelFormat.Format4bppIndexed;
            }
            return PixelFormat.Format1bppIndexed;
        }

        public static Bitmap FrameImage(Image image, int width, int height, Color backColor, Color foreColor)
        {
            Bitmap bitmap = new Bitmap(width, height, image.PixelFormat);
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.Clear(backColor);
                using (Pen pen = new Pen(foreColor))
                {
                    graphics.DrawRectangle(pen, 0, 0, width - 1, height - 1);
                }
                if ((width >= image.Width) && (height >= image.Height))
                {
                    graphics.DrawImage(image, (int) ((width - image.Width) / 2), (int) ((height - image.Height) / 2));
                    return bitmap;
                }
                graphics.DrawImage(image, 0, 0);
            }
            return bitmap;
        }

        public static Bitmap GenerateThumbnail(Icon icon, int width, int height)
        {
            return GenerateThumbnail(icon.ToBitmap(), width, height);
        }

        public static Bitmap GenerateThumbnail(Image image, int width, int height)
        {
            Bitmap bitmap;
            if ((image.Width <= width) && (image.Height <= height))
            {
                bitmap = ResizeImage(image, image.Width, image.Height);
            }
            else
            {
                int num2;
                int num3;
                double num = ((double) image.Width) / ((double) image.Height);
                if (num > 1.0)
                {
                    num2 = width;
                    num3 = (int) Math.Ceiling((double) (((double) height) / num));
                }
                else if (num < 1.0)
                {
                    num2 = (int) Math.Ceiling((double) (width * num));
                    num3 = height;
                }
                else
                {
                    num2 = width;
                    num3 = height;
                }
                bitmap = ResizeImage(image, num2, num3);
            }
            Bitmap bitmap2 = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            Graphics graphics = Graphics.FromImage(bitmap2);
            try
            {
                graphics.DrawImage(bitmap, (width - bitmap.Width) / 2, (height - bitmap.Height) / 2, bitmap.Width, bitmap.Height);
            }
            finally
            {
                graphics.Dispose();
                bitmap.Dispose();
            }
            return bitmap2;
        }

        public static ColorPalette GetColorPalette(int colors)
        {
            PixelFormat format = DetermineIndexedFormat(colors);
            Bitmap bitmap = new Bitmap(1, 1, format);
            ColorPalette palette = bitmap.Palette;
            bitmap.Dispose();
            return palette;
        }

        public static string GetPixelFormatText(PixelFormat format)
        {
            int pixelFormatSize = Image.GetPixelFormatSize(format);
            if (pixelFormatSize <= 8)
            {
                return Math.Pow(2.0, (double) pixelFormatSize).ToString();
            }
            return (pixelFormatSize + "b");
        }

        public static Bitmap ResizeImage(Image original, int width, int height)
        {
            return ResizeImage(original, width, height, InterpolationMode.Low);
        }

        public static Bitmap ResizeImage(Image original, int width, int height, InterpolationMode mode)
        {
            PixelFormat pixelFormat;
            if ((original.PixelFormat & PixelFormat.Indexed) > PixelFormat.Undefined)
            {
                pixelFormat = PixelFormat.Format24bppRgb;
            }
            else
            {
                pixelFormat = original.PixelFormat;
            }
            Bitmap image = new Bitmap(width, height, pixelFormat);
            Graphics graphics = Graphics.FromImage(image);
            graphics.InterpolationMode = mode;
            graphics.DrawImage(original, new Rectangle(0, 0, width, height));
            graphics.Dispose();
            return image;
        }
    }
}

