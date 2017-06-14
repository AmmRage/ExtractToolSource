namespace Ravioli.ArchivePlugins
{
    using System;
    using System.IO;

    internal class DltDecompressor
    {
        private const ushort CHUNK_SIZE = 0x1000;
        private const ushort CMP_CHUNK_SIZE = 0x1100;

        public static void Decompress(Stream inputStream, Stream outputStream)
        {
            if (GetU32(inputStream) != 0x50424750)
            {
                throw new ArgumentException("Input file is not a Stargunner compressed file.");
            }
            uint num2 = GetU32(inputStream);
            byte[] buffer = new byte[0x1100];
            byte[] buffer2 = new byte[0x1000];
            while (num2 > 0)
            {
                uint num4;
                ushort count = GetU16(inputStream);
                if (count > 0x1100)
                {
                    throw new IOException("Compressed chunk is too large!");
                }
                inputStream.Read(buffer, 0, count);
                if (num2 < 0x1000)
                {
                    num4 = num2;
                }
                else
                {
                    num4 = 0x1000;
                }
                if (ExplodeChunk(buffer, (long) num4, buffer2) > 0L)
                {
                    throw new IOException("Failed to explode chunk!");
                }
                outputStream.Write(buffer2, 0, (int) num4);
                num2 -= num4;
            }
        }

        private static long ExplodeChunk(byte[] data_in, long expanded_size, byte[] data_out)
        {
            byte[] buffer = new byte[0x100];
            byte[] buffer2 = new byte[0x100];
            long num = 0L;
            long num2 = 0L;
            while (num2 < expanded_size)
            {
                byte num4;
                for (int i = 0; i < 0x100; i++)
                {
                    buffer[i] = (byte) i;
                }
                long num5 = 0L;
                do
                {
                    num += 1L;
                    num4 = data_in[(int) ((IntPtr) num)];
                    if (num4 > 0x7f)
                    {
                        num5 += num4 - 0x7f;
                        num4 = 0;
                    }
                    if (num5 == 0x100L)
                    {
                        break;
                    }
                    for (int j = 0; j <= num4; j++)
                    {
                        num += 1L;
                        byte num7 = data_in[(int) ((IntPtr) num)];
                        buffer[(int) ((IntPtr) num5)] = num7;
                        if (num5 != num7)
                        {
                            num += 1L;
                            buffer2[(int) ((IntPtr) num5)] = data_in[(int) ((IntPtr) num)];
                        }
                        num5 += 1L;
                    }
                }
                while (num5 < 0x100L);
                num += 1L;
                int num8 = data_in[(int) ((IntPtr) num)];
                num += 1L;
                num8 |= data_in[(int) ((IntPtr) num)] << 8;
                int num9 = 0;
                byte[] buffer3 = new byte[0x20];
                while (true)
                {
                    if (num9 > 0)
                    {
                        num4 = buffer3[--num9];
                    }
                    else
                    {
                        if (--num8 == -1)
                        {
                            continue;
                        }
                        num += 1L;
                        num4 = data_in[(int) ((IntPtr) num)];
                    }
                    if (num4 == buffer[num4])
                    {
                        num2 += 1L;
                        data_out[(int) ((IntPtr) num2)] = num4;
                    }
                    else
                    {
                        buffer3[num9++] = buffer2[num4];
                        buffer3[num9++] = buffer[num4];
                    }
                }
            }
            return (num2 - expanded_size);
        }

        private static ushort GetU16(Stream stream)
        {
            byte[] buffer = new byte[2];
            stream.Read(buffer, 0, 2);
            return (ushort) (buffer[0] | (buffer[1] << 8));
        }

        private static uint GetU32(Stream stream)
        {
            byte[] buffer = new byte[4];
            stream.Read(buffer, 0, 4);
            return (uint) (((buffer[0] | (buffer[1] << 8)) | (buffer[2] << 0x10)) | (buffer[3] << 0x18));
        }
    }
}

