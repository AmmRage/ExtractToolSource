namespace Ravioli.Scanner
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;

    public class FileOperations
    {
        private const int FILE_ATTRIBUTE_DIRECTORY = 0x10;
        private const int FILE_ATTRIBUTE_NORMAL = 0x80;

        public static void Combine(string inputDirectory, ICollection<ScanDirectoryEntry> entries, string outputFileName)
        {
            Stream outputStream = File.Create(outputFileName);
            try
            {
                foreach (ScanDirectoryEntry entry in entries)
                {
                    string path = Path.Combine(inputDirectory, entry.Name);
                    if (!File.Exists(path))
                    {
                        throw new FileNotFoundException("The file \"" + entry.Name + "\" is missing. ");
                    }
                    Stream stream = File.OpenRead(path);
                    try
                    {
                        if (stream.Length != entry.Length)
                        {
                            throw new InvalidDataException("The file \"" + entry.Name + "\" does not have the same size as originally scanned.");
                        }
                        CopyData(stream, entry, outputStream);
                    }
                    finally
                    {
                        stream.Close();
                    }
                }
                outputStream.Close();
            }
            catch (Exception)
            {
                outputStream.Close();
                File.Delete(outputFileName);
                throw;
            }
        }

        private static void CopyData(Stream stream, ScanDirectoryEntry entry, Stream outputStream)
        {
            long length = entry.Length;
            byte[] buffer = new byte[0x2000];
            while (length > 0L)
            {
                int count = (length > 0x2000L) ? 0x2000 : ((int) length);
                int num3 = stream.Read(buffer, 0, count);
                length -= num3;
                outputStream.Write(buffer, 0, num3);
                if (num3 == 0)
                {
                    break;
                }
            }
            if (length > 0L)
            {
                throw new EndOfStreamException(string.Concat(new object[] { "Unexpected end of stream. ", length, " bytes are missing for the file \"", entry.Name, "\"." }));
            }
        }

        private static void Extract(Stream stream, ICollection<ScanDirectoryEntry> entries, string outputDirectory)
        {
            foreach (ScanDirectoryEntry entry in entries)
            {
                stream.Position = entry.Offset;
                long length = entry.Length;
                Stream outputStream = new FileStream(Path.Combine(outputDirectory, entry.Name), FileMode.Create, FileAccess.Write);
                try
                {
                    CopyData(stream, entry, outputStream);
                }
                finally
                {
                    outputStream.Close();
                }
            }
        }

        public static void Extract(string fileName, ICollection<ScanDirectoryEntry> entries, string outputDirectory)
        {
            FileStream stream = File.OpenRead(fileName);
            try
            {
                Extract(stream, entries, outputDirectory);
            }
            finally
            {
                stream.Close();
            }
        }

        public static void ExtractAll(ScanResults results, string outputDirectory)
        {
            FileStream stream = File.OpenRead(results.FileName);
            try
            {
                Extract(stream, results.Entries, outputDirectory);
            }
            finally
            {
                stream.Close();
            }
        }

        private static string GetAbsolutePath(string directoryName, string fileName)
        {
            return Path.GetFullPath(Path.Combine(directoryName, fileName));
        }

        public static ScanResults LoadScanResults(string fileName)
        {
            ScanResults results;
            XmlSerializer serializer = new XmlSerializer(typeof(ScanResults));
            XmlReader xmlReader = new XmlTextReader(fileName);
            try
            {
                results = (ScanResults) serializer.Deserialize(xmlReader);
            }
            finally
            {
                xmlReader.Close();
            }
            if (!Path.IsPathRooted(results.FileName))
            {
                results.FileName = GetAbsolutePath(Path.GetDirectoryName(fileName), results.FileName);
            }
            return results;
        }

        [DllImport("shlwapi.dll", SetLastError=true)]
        private static extern bool PathRelativePathTo(StringBuilder pszPath, string pszFrom, int dwAttrFrom, string pszTo, int dwAttrTo);
        public static void SaveScanResults(ScanResults results, string fileName)
        {
            string str = null;
            string str2;
            if ((Path.IsPathRooted(results.FileName) && (Path.GetDirectoryName(Path.GetFullPath(fileName)) == Path.GetDirectoryName(results.FileName))) && TryGetRelativePath(fileName, results.FileName, false, false, out str2))
            {
                str = results.FileName;
                results.FileName = str2;
            }
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ScanResults));
                XmlWriter xmlWriter = new XmlTextWriter(fileName, Encoding.UTF8);
                try
                {
                    serializer.Serialize(xmlWriter, results);
                }
                finally
                {
                    xmlWriter.Close();
                }
            }
            finally
            {
                if (str != null)
                {
                    results.FileName = str;
                }
            }
        }

        private static bool TryGetRelativePath(string fromPath, string toPath, bool isFromDirectory, bool isToDirectory, out string result)
        {
            int dwAttrFrom = isFromDirectory ? 0x10 : 0x80;
            int dwAttrTo = isToDirectory ? 0x10 : 0x80;
            StringBuilder pszPath = new StringBuilder(260);
            if (!PathRelativePathTo(pszPath, fromPath, dwAttrFrom, toPath, dwAttrTo))
            {
                result = null;
                return false;
            }
            result = pszPath.ToString();
            return true;
        }
    }
}

