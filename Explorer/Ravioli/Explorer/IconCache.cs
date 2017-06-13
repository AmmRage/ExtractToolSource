namespace Ravioli.Explorer
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;

    internal class IconCache : IDisposable
    {
        private Dictionary<IconSize, Dictionary<string, Bitmap>> icons = new Dictionary<IconSize, Dictionary<string, Bitmap>>();

        public void Dispose()
        {
            foreach (Dictionary<string, Bitmap> dictionary in this.icons.Values)
            {
                if (dictionary != null)
                {
                    foreach (Bitmap bitmap in dictionary.Values)
                    {
                        if (bitmap != null)
                        {
                            bitmap.Dispose();
                        }
                    }
                    dictionary.Clear();
                }
            }
            this.icons.Clear();
        }

        public Bitmap GetIcon(string extension, IconSize size)
        {
            Dictionary<string, Bitmap> dictionary;
            if (!this.icons.ContainsKey(size))
            {
                dictionary = new Dictionary<string, Bitmap>();
                this.icons.Add(size, dictionary);
            }
            else
            {
                dictionary = this.icons[size];
            }
            if (dictionary.ContainsKey(extension))
            {
                return (Bitmap) dictionary[extension].Clone();
            }
            Icon icon = IconHandler.IconFromExtension(extension, size);
            if (icon != null)
            {
                Bitmap bitmap = icon.ToBitmap();
                dictionary.Add(extension, (Bitmap) bitmap.Clone());
                icon.Dispose();
                return bitmap;
            }
            return null;
        }
    }
}

