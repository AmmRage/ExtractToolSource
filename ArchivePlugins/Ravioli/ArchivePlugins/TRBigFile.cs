namespace Ravioli.ArchivePlugins
{
    using Ravioli.ArchiveInterface;
    using System;
    using System.IO;

    public class TRBigFile : ArchiveBase
    {
        private TRBigFileDirectoryEntry[] directory;

        private void CopyData(TRBigFileDirectoryEntry entry, Stream stream, Stream outputStream, long byteCount)
        {
            string partFileName = GetPartFileName(base.FileName, entry.PartIndex);
            FileStream stream2 = new FileStream(partFileName, FileMode.Open, FileAccess.Read);
            try
            {
                if (entry.Offset > stream2.Length)
                {
                    throw new EndOfStreamException(string.Format("Offset of entry \"{0}\" from {1} is after the end of stream (Offset=0x{2:X8}, StreamLength=0x{3:X8}).", new object[] { entry.Name, Path.GetFileName(partFileName), entry.Offset, stream2.Length }));
                }
                stream2.Seek((long) entry.Offset, SeekOrigin.Begin);
                long num = ((byteCount >= 0L) && (byteCount < entry.Length)) ? byteCount : ((long) entry.Length);
                byte[] buffer = new byte[0x2000];
                while (num > 0L)
                {
                    int count = (num > 0x2000L) ? 0x2000 : ((int) num);
                    int num3 = stream2.Read(buffer, 0, count);
                    outputStream.Write(buffer, 0, num3);
                    num -= num3;
                    if (num3 == 0)
                    {
                        throw new EndOfStreamException("End of stream reached too early while extracting file.");
                    }
                }
            }
            finally
            {
                stream2.Close();
            }
        }

        private static string DetermineFileExtension(TRBigFileDirectoryEntry entry, string firstFileName)
        {
            string partFileName = GetPartFileName(firstFileName, entry.PartIndex);
            string str2 = ".dat";
            FileStream input = new FileStream(partFileName, FileMode.Open, FileAccess.Read);
            BinaryReader reader = new BinaryReader(input);
            try
            {
                input.Position = entry.Offset;
                switch (reader.ReadUInt32())
                {
                    case 0x2173754d:
                        return ".mus";

                    case 0x52415721:
                        return ".raw";

                    case 14:
                        return ".drm";

                    case 0x4d524443:
                        return ".cdrm";

                    case 0x34425346:
                        return ".fsb4";

                    case 0x474e5089:
                        return ".png";
                }
                input.Position = entry.Offset;
                input.Position += 12L;
                uint num2 = reader.ReadUInt32();
                if (((num2 != 1) && (num2 != 2)) && ((num2 != 4) && (num2 != 6)))
                {
                    return str2;
                }
                str2 = ".mul";
            }
            finally
            {
                reader.Close();
                input.Close();
            }
            return str2;
        }

        protected override void ExtractFile(IFileInfo file, Stream stream, Stream outputStream)
        {
            this.CopyData(this.directory[(int) file.ID], stream, outputStream, -1L);
        }

        protected override void ExtractFile(IFileInfo file, Stream stream, string outputFileName)
        {
            TRBigFileDirectoryEntry entry = this.directory[(int) file.ID];
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

        private static string GetPartFileName(string firstFileName, uint index)
        {
            return Path.ChangeExtension(firstFileName, "." + index.ToString("D3"));
        }

        protected override bool IsValidFormat(Stream stream, BinaryReader reader)
        {
            bool flag = false;
            FileStream stream2 = stream as FileStream;
            if (stream2 == null)
            {
                return flag;
            }
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(stream2.Name);
            if (!(fileNameWithoutExtension == "bigfile") && !(fileNameWithoutExtension == "patch"))
            {
                return flag;
            }
            return true;
        }

        protected override void OnClose()
        {
            this.directory = null;
        }

        protected override void ReadDirectory(Stream stream, BinaryReader reader)
        {
            DirectoryFormat legend;
            uint num = 0;
            if (stream.Length <= 0x9600000L)
            {
                legend = DirectoryFormat.Legend;
            }
            else
            {
                legend = DirectoryFormat.Underworld;
            }
            num = reader.ReadUInt32();
            TRBigFileDirectoryEntry[] entryArray = new TRBigFileDirectoryEntry[num];
            for (uint i = 0; i < num; i++)
            {
                TRBigFileDirectoryEntry entry = new TRBigFileDirectoryEntry();
                uint num4 = reader.ReadUInt32();
                entry.Hashcode = num4;
                entryArray[i] = entry;
            }
            for (uint j = 0; j < num; j++)
            {
                TRBigFileDirectoryEntry entry2 = entryArray[j];
                entry2.Length = reader.ReadUInt32();
                uint num6 = reader.ReadUInt32();
                switch (legend)
                {
                    case DirectoryFormat.Legend:
                        entry2.PartIndex = (num6 << 11) / 0x9600000;
                        entry2.Offset = (num6 << 11) % 0x9600000;
                        break;

                    case DirectoryFormat.Underworld:
                        entry2.PartIndex = num6 / 0xffe00;
                        entry2.Offset = (uint) ((num6 % 0xffe00) << 11);
                        break;

                    default:
                        throw new InvalidOperationException("Invalid directory format \"" + legend + "\".");
                }
                entry2.Language = reader.ReadUInt16();
                stream.Position += 6L;
                entry2.Name = string.Format("{0:X8}_{1:X4}{2}", entry2.Hashcode, entry2.Language, DetermineFileExtension(entry2, base.FileName));
                entryArray[j] = entry2;
            }
            this.directory = entryArray;
        }

        public override string[] Extensions
        {
            get
            {
                return new string[] { ".000" };
            }
        }

        public override IFileInfo[] Files
        {
            get
            {
                ArchiveFileInfo[] infoArray = new ArchiveFileInfo[this.directory.Length];
                for (int i = 0; i < this.directory.Length; i++)
                {
                    TRBigFileDirectoryEntry entry = this.directory[i];
                    infoArray[i] = new ArchiveFileInfo((long) i, entry.Name, (long) entry.Length);
                }
                return infoArray;
            }
        }

        public override string TypeName
        {
            get
            {
                return "Tomb Raider Big File";
            }
        }

        private enum DirectoryFormat
        {
            Legend,
            Underworld
        }
    }
}

