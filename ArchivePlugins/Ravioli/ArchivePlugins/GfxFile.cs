namespace Ravioli.ArchivePlugins
{
    using Ravioli.ArchiveInterface;
    using Ravioli.PluginHelpers;
    using Ravioli.PluginHelpers.Imaging;
    using Ravioli.PluginHelpers.Reading;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;

    public class GfxFile : ArchiveBase
    {
        private ColorPalette basePalette;
        private int basePaletteStartIndex;
        private GfxDirectoryEntry[] directory;

        private void CopyData(GfxDirectoryEntry entry, Stream stream, Stream outputStream, long byteCount)
        {
            stream.Seek(entry.Offset, SeekOrigin.Begin);
            uint dataLength = StaticReader.ReadUInt32(stream);
            ushort width = StaticReader.ReadUInt16(stream);
            ushort height = StaticReader.ReadUInt16(stream);
            switch (((byte) stream.ReadByte()))
            {
                case 4:
                case 5:
                {
                    Bitmap bitmap = this.ReadAndConvertBitmap(stream, dataLength, width, height);
                    if (entry.PaletteIndex >= 0)
                    {
                        GfxDirectoryEntry entry2 = this.directory[entry.PaletteIndex];
                        stream.Seek(entry2.Offset, SeekOrigin.Begin);
                        bitmap.Palette = this.ReadAndConvertPalette(stream);
                    }
                    if (byteCount < 0L)
                    {
                        bitmap.Save(outputStream, ImageFormat.Bmp);
                    }
                    else
                    {
                        MemoryStream stream2 = new MemoryStream();
                        bitmap.Save(stream2, ImageFormat.Bmp);
                        byte[] buffer = stream2.ToArray();
                        outputStream.Write(buffer, 0, (int) byteCount);
                    }
                    bitmap.Dispose();
                    return;
                }
            }
            stream.Seek(entry.Offset, SeekOrigin.Begin);
            long num5 = ((byteCount >= 0L) && (byteCount < entry.Length)) ? byteCount : entry.Length;
            byte[] buffer2 = new byte[0x2000];
            while (num5 > 0L)
            {
                int count = (num5 > 0x2000L) ? 0x2000 : ((int) num5);
                int num7 = stream.Read(buffer2, 0, count);
                outputStream.Write(buffer2, 0, num7);
                num5 -= num7;
            }
        }

        protected override void ExtractFile(IFileInfo file, Stream stream, Stream outputStream)
        {
            this.CopyData(this.directory[(int) file.ID], stream, outputStream, -1L);
        }

        protected override void ExtractFile(IFileInfo file, Stream stream, string outputFileName)
        {
            GfxDirectoryEntry entry = this.directory[(int) file.ID];
            FileStream outputStream = new FileStream(outputFileName, FileMode.Create, FileAccess.Write);
            try
            {
                this.CopyData(entry, stream, outputStream, -1L);
                outputStream.Close();
            }
            catch (Exception)
            {
                outputStream.Close();
                if (File.Exists(outputFileName))
                {
                    File.Delete(outputFileName);
                }
                throw;
            }
        }

        protected override void ExtractFile(IFileInfo file, Stream stream, Stream outputStream, long byteCount)
        {
            this.CopyData(this.directory[(int) file.ID], stream, outputStream, byteCount);
        }

        protected override bool IsValidFormat(Stream stream, BinaryReader reader)
        {
            return true;
        }

        private void MergeBaseColors(ColorPalette palette)
        {
            palette.Entries[0xc5] = Color.FromArgb(0xec, 0xf4, 0xfc);
            palette.Entries[0xc6] = Color.FromArgb(0xf4, 0xfc, 0xfc);
            palette.Entries[0xc7] = Color.FromArgb(0xfc, 0xf4, 0xec);
            palette.Entries[0xc9] = Color.FromArgb(0xb8, 60, 0x24);
            palette.Entries[0xca] = Color.FromArgb(200, 200, 200);
            palette.Entries[0xcb] = Color.FromArgb(0xe4, 0x5c, 0x40);
            palette.Entries[0xcc] = Color.FromArgb(0xfc, 0xe4, 0x84);
            palette.Entries[0xcd] = Color.FromArgb(0xec, 0x84, 0x68);
            palette.Entries[0xce] = Color.FromArgb(0x4c, 100, 0x7c);
            palette.Entries[0xcf] = Color.FromArgb(240, 0xcc, 0xb0);
            palette.Entries[0xd0] = Color.FromArgb(0xf4, 0xa8, 0x94);
            palette.Entries[0xd1] = Color.FromArgb(0x5c, 0x5c, 0x5c);
            palette.Entries[210] = Color.FromArgb(0x68, 140, 0xa8);
            palette.Entries[0xd3] = Color.FromArgb(140, 140, 140);
            palette.Entries[0xd4] = Color.FromArgb(0x90, 0xc0, 0xd4);
            palette.Entries[0xd5] = Color.FromArgb(0xfc, 0xd0, 0x34);
            palette.Entries[0xd6] = Color.FromArgb(0xfc, 160, 0x1c);
            palette.Entries[0xd7] = Color.FromArgb(0x30, 0x60, 0x20);
            palette.Entries[0xd8] = Color.FromArgb(0x20, 0x48, 0x10);
            palette.Entries[0xd9] = Color.FromArgb(80, 140, 0x38);
            palette.Entries[0xda] = Color.FromArgb(120, 0xbc, 0x58);
            palette.Entries[0xdb] = Color.FromArgb(0xac, 0xec, 140);
            palette.Entries[220] = Color.FromArgb(0x58, 80, 0x30);
            palette.Entries[0xdd] = Color.FromArgb(0x40, 0x40, 0x40);
            palette.Entries[0xde] = Color.FromArgb(220, 0xc4, 0x70);
            palette.Entries[0xdf] = Color.FromArgb(0xac, 0x9c, 0x58);
            palette.Entries[0xe0] = Color.FromArgb(120, 0x68, 0x40);
            palette.Entries[0xe1] = Color.FromArgb(140, 0x18, 0x18);
            palette.Entries[0xe2] = Color.FromArgb(0xe4, 140, 8);
            palette.Entries[0xe3] = Color.FromArgb(0x84, 0x30, 140);
            palette.Entries[0xe4] = Color.FromArgb(0x9c, 0x58, 0x84);
            palette.Entries[0xe5] = Color.FromArgb(0xec, 0x58, 140);
            palette.Entries[230] = Color.FromArgb(0x9c, 220, 0x48);
            palette.Entries[0xe7] = Color.FromArgb(0xc4, 0x84, 0xa8);
            palette.Entries[0xe8] = Color.FromArgb(0, 0, 0);
            palette.Entries[0xe9] = Color.FromArgb(0, 0, 0);
            palette.Entries[0xea] = Color.FromArgb(0, 0, 0);
            palette.Entries[0xeb] = Color.FromArgb(0, 0, 0);
            palette.Entries[0xec] = Color.FromArgb(0, 0, 0);
            palette.Entries[0xed] = Color.FromArgb(0, 0, 0);
            palette.Entries[0xee] = Color.FromArgb(0, 0, 0);
            palette.Entries[0xef] = Color.FromArgb(0, 0, 0);
            palette.Entries[240] = Color.FromArgb(0, 0, 0);
            palette.Entries[0xf1] = Color.FromArgb(0, 0, 0);
            palette.Entries[0xf2] = Color.FromArgb(0, 0, 0);
            palette.Entries[0xf3] = Color.FromArgb(0, 0, 0);
            palette.Entries[0xf4] = Color.FromArgb(0, 0, 0);
            palette.Entries[0xf5] = Color.FromArgb(0, 0, 0);
            palette.Entries[0xf6] = Color.FromArgb(0, 0, 0);
            palette.Entries[0xf7] = Color.FromArgb(0, 0, 0);
            palette.Entries[0xf8] = Color.FromArgb(0, 0, 0);
            palette.Entries[0xf9] = Color.FromArgb(0x5c, 0xfc, 0xfc);
            palette.Entries[250] = Color.FromArgb(0x68, 120, 0xc4);
            palette.Entries[0xfb] = Color.FromArgb(0, 0x5c, 0xa8);
            palette.Entries[0xfc] = Color.FromArgb(0xc4, 0xc4, 0xc4);
            palette.Entries[0xfd] = Color.FromArgb(0xb8, 60, 0x24);
            palette.Entries[0xfe] = Color.FromArgb(0xc4, 0xc4, 0xc4);
            palette.Entries[0xff] = Color.FromArgb(0xfc, 0xfc, 0xfc);
        }

        protected override void OnClose()
        {
            this.directory = null;
        }

        private Bitmap ReadAndConvertBitmap(Stream stream, uint dataLength, ushort width, ushort height)
        {
            byte[] buffer = new byte[dataLength];
            stream.Read(buffer, 0, (int) dataLength);
            Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
            IndexedBitmapAccess access = new IndexedBitmapAccess(bitmap);
            access.Lock();
            try
            {
                uint num = 0;
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        access.SetPixel(j, i, buffer[num++]);
                    }
                }
            }
            finally
            {
                access.Unlock(true);
            }
            return bitmap;
        }

        private ColorPalette ReadAndConvertPalette(Stream stream)
        {
            long position = stream.Position;
            uint length = StaticReader.ReadUInt32(stream);
            ushort paletteOffset = StaticReader.ReadUInt16(stream);
            ushort unknown = StaticReader.ReadUInt16(stream);
            uint fileType = StaticReader.ReadUInt32(stream);
            return this.ReadAndConvertPalette(stream, position, length, paletteOffset, unknown, fileType);
        }

        private ColorPalette ReadAndConvertPalette(Stream stream, long startPosition, uint length, ushort paletteOffset, ushort unknown, uint fileType)
        {
            int num = paletteOffset + 1;
            ColorPalette colorPalette = ImageHelper.GetColorPalette(0x100);
            for (int i = 0; i < colorPalette.Entries.Length; i++)
            {
                colorPalette.Entries[i] = Color.Black;
            }
            byte[] buffer = new byte[3];
            while (((stream.Position - startPosition) < length) && (num < 0x100))
            {
                if (stream.Read(buffer, 0, 3) < 3)
                {
                    break;
                }
                Color color = Color.FromArgb(buffer[0], buffer[1], buffer[2]);
                colorPalette.Entries[num++] = color;
            }
            this.MergeBaseColors(colorPalette);
            return colorPalette;
        }

        protected override void ReadDirectory(Stream stream, BinaryReader reader)
        {
            stream.Seek(0L, SeekOrigin.Begin);
            int index = -1;
            int num2 = -1;
            int num3 = -1;
            bool flag = false;
            List<GfxDirectoryEntry> list = new List<GfxDirectoryEntry>();
            FileCounter counter = new FileCounter();
            while (stream.Position < stream.Length)
            {
                GfxDirectoryEntry item = new GfxDirectoryEntry {
                    Offset = stream.Position,
                    Length = (long) reader.ReadUInt32()
                };
                ushort width = StaticReader.ReadUInt16(stream);
                ushort height = StaticReader.ReadUInt16(stream);
                byte fileType = (byte) stream.ReadByte();
                string extension = ((fileType == 4) || (fileType == 5)) ? ".bmp" : ".dat";
                item.Name = counter.GetNextFileName(extension);
                switch (fileType)
                {
                    case 4:
                    case 5:
                    {
                        Bitmap bitmap = this.ReadAndConvertBitmap(stream, (uint) item.Length, width, height);
                        MemoryStream stream2 = new MemoryStream();
                        bitmap.Save(stream2, ImageFormat.Bmp);
                        item.Length = stream2.Length;
                        stream2.Close();
                        bitmap.Dispose();
                        break;
                    }
                    default:
                        if (fileType == 6)
                        {
                            stream.Seek(item.Length, SeekOrigin.Current);
                        }
                        else
                        {
                            stream.Seek(item.Length, SeekOrigin.Current);
                        }
                        break;
                }
                list.Add(item);
                switch (fileType)
                {
                    case 4:
                        if (num2 >= 0)
                        {
                            item.PaletteIndex = num2;
                            num2 = -1;
                        }
                        else
                        {
                            index = list.IndexOf(item);
                        }
                        break;

                    case 6:
                        if (!flag)
                        {
                            int num7 = list.IndexOf(item);
                            for (int i = 0; i < num7; i++)
                            {
                                if (list[i].PaletteIndex < 0)
                                {
                                    list[i].PaletteIndex = num7;
                                }
                            }
                            num3 = num7;
                            this.basePalette = this.ReadAndConvertPalette(stream, item.Offset, (uint) item.Length, width, height, fileType);
                            this.basePaletteStartIndex = 0xc9;
                            flag = true;
                        }
                        else
                        {
                            if (index >= 0)
                            {
                                int num9 = list.IndexOf(item);
                                list[index].PaletteIndex = num9;
                                index = -1;
                            }
                            else
                            {
                                num2 = list.IndexOf(item);
                            }
                            num3 = list.IndexOf(item);
                        }
                        break;
                }
                if ((fileType == 5) && (num3 >= 0))
                {
                    item.PaletteIndex = num3;
                }
            }
            this.directory = list.ToArray();
        }

        public override string[] Extensions
        {
            get
            {
                return new string[] { ".gfx" };
            }
        }

        public override IFileInfo[] Files
        {
            get
            {
                ArchiveFileInfo[] infoArray = new ArchiveFileInfo[this.directory.Length];
                for (int i = 0; i < this.directory.Length; i++)
                {
                    GfxDirectoryEntry entry = this.directory[i];
                    infoArray[i] = new ArchiveFileInfo((long) i, entry.Name, entry.Length);
                }
                return infoArray;
            }
        }

        public override string TypeName
        {
            get
            {
                return "Arnie Goes 4 Gold GFX File";
            }
        }
    }
}

