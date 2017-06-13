namespace Ravioli.ArchivePlugins
{
    using Ravioli.ArchiveInterface;
    using System;
    using System.IO;
    using System.Xml;
    using System.Xml.Serialization;

    public abstract class ScanArchiveBase : ArchiveBase
    {
        private Stream innerStream;
        private GenericDirectory scanResults;
        private XmlReader xmlReader;

        protected ScanArchiveBase()
        {
        }

        private void CopyData(GenericDirectoryEntry entry, Stream stream, Stream outputStream, long byteCount)
        {
            this.innerStream.Seek(entry.Offset, SeekOrigin.Begin);
            long num = ((byteCount >= 0L) && (byteCount < entry.Length)) ? byteCount : entry.Length;
            byte[] buffer = new byte[0x2000];
            while (num > 0L)
            {
                int count = (num > 0x2000L) ? 0x2000 : ((int) num);
                int num3 = this.innerStream.Read(buffer, 0, count);
                outputStream.Write(buffer, 0, num3);
                num -= (long) ((ulong) num3);
                if (num3 == 0)
                {
                    throw new EndOfStreamException(string.Concat(new object[] { "Unexpected end of stream. ", num, " bytes are missing for the file \"", entry.Name, "\"." }));
                }
            }
        }

        protected override void ExtractFile(IFileInfo file, Stream stream, Stream outputStream)
        {
            this.CopyData(this.scanResults.Entries[(int) file.ID], stream, outputStream, -1L);
        }

        protected override void ExtractFile(IFileInfo file, Stream stream, string outputFileName)
        {
            GenericDirectoryEntry entry = this.scanResults.Entries[(int) file.ID];
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
            this.CopyData(this.scanResults.Entries[(int) file.ID], stream, outputStream, byteCount);
        }

        private static string GetAbsolutePath(string directoryName, string fileName)
        {
            return Path.GetFullPath(Path.Combine(directoryName, fileName));
        }

        protected virtual Type GetSerializedType()
        {
            return typeof(Ravioli.ArchivePlugins.ScanResults);
        }

        protected override bool IsValidFormat(Stream stream, BinaryReader reader)
        {
            bool flag = false;
            stream.Seek(0L, SeekOrigin.Begin);
            XmlSerializer serializer = new XmlSerializer(this.GetSerializedType());
            this.xmlReader = new XmlTextReader(stream);
            try
            {
                flag = serializer.CanDeserialize(this.xmlReader);
            }
            catch (XmlException)
            {
                this.xmlReader.Close();
                this.xmlReader = null;
            }
            catch (Exception)
            {
                this.xmlReader.Close();
                this.xmlReader = null;
                throw;
            }
            return flag;
        }

        protected override void OnClose()
        {
            if (this.xmlReader != null)
            {
                this.xmlReader.Close();
                this.xmlReader = null;
            }
            if (this.innerStream != null)
            {
                this.innerStream.Close();
                this.innerStream = null;
            }
            this.scanResults = null;
        }

        protected override void ReadDirectory(Stream stream, BinaryReader reader)
        {
            if (!this.IsValidFormat(stream, reader))
            {
                throw new InvalidDataException("This is not a valid RSR/RGD file.");
            }
            XmlSerializer serializer = new XmlSerializer(this.GetSerializedType());
            if (this.xmlReader == null)
            {
                this.xmlReader = new XmlTextReader(stream);
            }
            try
            {
                this.scanResults = (GenericDirectory) serializer.Deserialize(this.xmlReader);
                if (!Path.IsPathRooted(this.scanResults.FileName))
                {
                    this.scanResults.FileName = GetAbsolutePath(Path.GetDirectoryName(base.FileName), this.scanResults.FileName);
                }
                if (!File.Exists(this.scanResults.FileName))
                {
                    throw new FileNotFoundException("The referenced file \"" + this.scanResults.FileName + "\" does not exist.");
                }
                this.innerStream = new FileStream(this.scanResults.FileName, FileMode.Open, FileAccess.Read);
                if (this.innerStream.Length != this.scanResults.FileSize)
                {
                    throw new InvalidDataException("Size of file \"" + this.scanResults.FileName + "\" differs from originally referenced file.");
                }
            }
            catch (Exception)
            {
                if (this.xmlReader != null)
                {
                    this.xmlReader.Close();
                    this.xmlReader = null;
                }
                if (this.innerStream != null)
                {
                    this.innerStream.Close();
                    this.innerStream = null;
                }
                this.scanResults = null;
                throw;
            }
        }

        public override IFileInfo[] Files
        {
            get
            {
                ArchiveFileInfo[] infoArray = new ArchiveFileInfo[this.scanResults.Entries.Length];
                for (int i = 0; i < this.scanResults.Entries.Length; i++)
                {
                    GenericDirectoryEntry entry = this.scanResults.Entries[i];
                    infoArray[i] = new ArchiveFileInfo((long) i, entry.Name, entry.Length);
                }
                return infoArray;
            }
        }

        protected GenericDirectory ScanResults
        {
            get
            {
                return this.scanResults;
            }
        }
    }
}

