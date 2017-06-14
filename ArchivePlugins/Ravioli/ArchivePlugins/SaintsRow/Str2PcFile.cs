namespace Ravioli.ArchivePlugins.SaintsRow
{
    using Ravioli.PluginHelpers;
    using System;
    using System.IO;

    public class Str2PcFile : VppPcSBFile
    {
        protected override void DecompressArchive(Stream stream, BinaryReader reader, Stream outputStream, long byteCount)
        {
            if (((base.VppDirectory.TotalCompressedLength != uint.MaxValue) && (base.VppDirectory.TotalCompressedLength > 0)) && (base.VppDirectory.Entries.Count > 0))
            {
                Compression.ZlibParameters parameters = null;
                long num = 0L;
                long num2 = 0L;
                foreach (VppPcDirectoryEntry entry in base.VppDirectory.Entries)
                {
                    if (entry.CompressedLength != uint.MaxValue)
                    {
                        byte[] buffer = new byte[entry.CompressedLength];
                        using (MemoryStream stream2 = new MemoryStream(buffer))
                        {
                            stream.Read(buffer, 0, buffer.Length);
                            using (MemoryStream stream3 = new MemoryStream())
                            {
                                if (parameters == null)
                                {
                                    parameters = Compression.ReadZlibParameters(stream2);
                                }
                                num2 = Compression.DecompressZlibStream(stream2, stream3, (long) entry.Length, parameters);
                                if (num < entry.Offset)
                                {
                                    int count = (int) (entry.Offset - num);
                                    byte[] buffer2 = new byte[count];
                                    for (int i = 0; i < count; i++)
                                    {
                                        buffer2[i] = 0;
                                    }
                                    outputStream.Write(buffer2, 0, count);
                                    num += count;
                                }
                                stream3.WriteTo(outputStream);
                                num += num2;
                            }
                        }
                    }
                }
            }
        }

        public override string[] Extensions
        {
            get
            {
                return new string[] { ".str2_pc" };
            }
        }

        public override string TypeName
        {
            get
            {
                return "Saints Row 3/4 STR2_PC File";
            }
        }
    }
}

