using Ravioli.AppShared;
using Ravioli.ArchiveInterface.Scanning;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Ravioli.Scanner
{


    public class FileScanner
    {
        private bool keepResultsOnCancel = false;
        private bool scanCancelled = false;
        private string scanFileName;
        private ScanPluginManager scanPluginManager;
        private ScanRange scanRange;
        private Thread scanThread;

        public event LogEventHandler ErrorLogged;

        public event LogEventHandler InfoLogged;

        public event ScanCompletedEventHandler ScanCompleted;

        public event ScanItemEventHandler ScanItemFound;

        public event ScanProgressChangedEventHandler ScanProgressChanged;

        public event LogEventHandler WarningLogged;

        public FileScanner(ScanPluginManager scanPluginManager)
        {
            this.scanPluginManager = scanPluginManager;
        }

        private static ScanDirectoryEntry GetEntryFromDetectedFile(DetectedFile file, ref int nextNumber)
        {
            return new ScanDirectoryEntry { Offset = file.Offset, Length = file.Length, Name = GetNextFileName(file.FileType.Extension, ref nextNumber, file.Broken), TypeName = file.FileType.TypeName, PerceivedType = file.FileType.PerceivedType };
        }

        public static string GetFriendlyFileSize(long size)
        {
            if (size >= 0x40000000L)
            {
                double num = Math.Round((double) (((double) size) / 1073741824.0), 2);
                string str = string.Format("{0:G} GB", num);
            }
            if (size >= 0x100000L)
            {
                double num2 = Math.Round((double) (((double) size) / 1048576.0), 2);
                return string.Format("{0:G} MB", num2);
            }
            if (size >= 0x400L)
            {
                double num3 = Math.Round((double) (((double) size) / 1024.0), 2);
                return string.Format("{0:G} KB", num3);
            }
            double num4 = Math.Round((double) size, 2);
            return string.Format("{0:G} bytes", num4);
        }

        private static string GetNextFileName(string extension, ref int nextNumber, bool isBroken)
        {
            if (!extension.StartsWith("."))
            {
                extension = "." + extension;
            }
            string str = isBroken ? "-broken" : "";
            string str2 = string.Format("File{0:D4}{1}{2}", (int) nextNumber, str, extension);
            nextNumber++;
            return str2;
        }

        private void logger_ErrorLogged(object sender, LogEventArgs e)
        {
            if (this.ErrorLogged != null)
            {
                this.ErrorLogged(this, e);
            }
        }

        private void logger_InfoLogged(object sender, LogEventArgs e)
        {
            if (this.InfoLogged != null)
            {
                this.InfoLogged(this, e);
            }
        }

        private void logger_WarningLogged(object sender, LogEventArgs e)
        {
            if (this.WarningLogged != null)
            {
                this.WarningLogged(this, e);
            }
        }

        private static bool ReadWorkBuffer(Stream stream, long currentPosition, long oldPosition, int bigBufferSize, int smallBufferSize, ref byte[] data, ref int dataBytesInBuffer)
        {
            bool flag = true;
            if ((currentPosition == (oldPosition + 1L)) && (dataBytesInBuffer > 0))
            {
                byte[] destinationArray = new byte[bigBufferSize];
                Array.Copy(data, 1, destinationArray, 0, dataBytesInBuffer - 1);
                destinationArray[dataBytesInBuffer - 1] = 0;
                dataBytesInBuffer--;
                data = destinationArray;
                if (dataBytesInBuffer > smallBufferSize)
                {
                    return flag;
                }
                stream.Position = currentPosition + ((long) dataBytesInBuffer);
                byte[] buffer = new byte[smallBufferSize];
                int length = stream.Read(buffer, 0, smallBufferSize);
                if (length > 0)
                {
                    Array.Copy(buffer, 0, data, dataBytesInBuffer, length);
                    dataBytesInBuffer += length;
                    return flag;
                }
                return false;
            }
            stream.Position = currentPosition;
            dataBytesInBuffer = stream.Read(data, 0, bigBufferSize);
            return flag;
        }

        private void ReportProgress(long current, long total, ref int oldPercent, int itemsFound, ref int oldItemsFound, Logger logger)
        {
            int num;
            if (total == 0L)
            {
                num = 0;
            }
            else
            {
                num = (int) Math.Round((double) ((((double) current) / ((double) total)) * 100.0));
            }
            if ((num != oldPercent) || (itemsFound >= (oldItemsFound + 10)))
            {
                oldPercent = num;
                oldItemsFound = itemsFound;
                if (this.ScanProgressChanged != null)
                {
                    this.ScanProgressChanged(this, new ScanProgressChangedEventArgs(num, itemsFound, null));
                }
            }
        }

        public ScanResults Scan(string fileName)
        {
            return this.Scan(fileName, null, true);
        }

        public ScanResults Scan(string fileName, ScanRange range)
        {
            return this.Scan(fileName, range, true);
        }

        private ScanResults Scan(string fileName, ScanRange range, bool reportProgress)
        {
            this.scanCancelled = false;
            Stream input = File.OpenRead(fileName);
            BinaryReader reader = new BinaryReader(input);
            try
            {
                if (range != null)
                {
                    if ((range.StartOffset < 0L) || (range.EndOffset > (input.Length - 1L)))
                    {
                        throw new ArgumentException("Range start offset must be between 0 and the file size - 1.");
                    }
                    if ((range.EndOffset < 0L) || (range.EndOffset > (input.Length - 1L)))
                    {
                        throw new ArgumentException("Range end offset must be between 0 and the file size - 1.");
                    }
                    if (range.EndOffset < range.StartOffset)
                    {
                        throw new ArgumentException("Range end offset must be same or higher than start offset.");
                    }
                }
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                int oldPercent = -1;
                int oldItemsFound = -1;
                Logger logger = new Logger();
                string message = new string('-', 50);
                logger.InfoLogged += new LogEventHandler(this.logger_InfoLogged);
                logger.WarningLogged += new LogEventHandler(this.logger_WarningLogged);
                logger.ErrorLogged += new LogEventHandler(this.logger_ErrorLogged);
                logger.LogInfo(string.Format("Scanning file \"{0}\"...", fileName));
                logger.LogInfo(string.Format("Stream length is 0x{0:X8}.", input.Length));
                long current = 0L;
                long oldPosition = 0L;
                long length = input.Length;
                if (range != null)
                {
                    logger.LogInfo(string.Format("Range: 0x{0:X8}-0x{1:X8}.", range.StartOffset, range.EndOffset));
                    current = range.StartOffset;
                    oldPosition = range.StartOffset;
                }
                logger.LogInfo(message);
                List<ScanDirectoryEntry> list = new List<ScanDirectoryEntry>();
                IFileDetector[] detectorArray = this.scanPluginManager.CreateAllDetectors();
                byte[] data = new byte[0x200];
                int dataBytesInBuffer = 0;
                int nextNumber = 1;
                while (((current < length) && !this.scanCancelled) && ((range == null) || ((range != null) && (current <= range.EndOffset))))
                {
                    if (reportProgress)
                    {
                        this.ReportProgress(current, length, ref oldPercent, list.Count, ref oldItemsFound, logger);
                    }
                    ReadWorkBuffer(input, current, oldPosition, 0x200, 0x100, ref data, ref dataBytesInBuffer);
                    Stream stream2 = new MemoryStream(data, 0, 0x100, false);
                    BinaryReader reader2 = new BinaryReader(stream2);
                    try
                    {
                        DetectedFile file = null;
                        foreach (IFileDetector detector in detectorArray)
                        {
                            stream2.Position = 0L;
                            bool flag = false;
                            try
                            {
                                flag = detector.DetectSignature(stream2, reader2);
                            }
                            catch (Exception exception)
                            {
                                logger.LogError(string.Format("Signature detector in class \"{0}\" failed @ offset 0x{1:X8} with unhandled exception: {2}", detector.GetType().ToString(), current, exception.ToString()));
                            }
                            if (flag)
                            {
                                input.Position = current;
                                int errorCount = logger.ErrorCount;
                                int warningCount = logger.WarningCount;
                                int infoCount = logger.InfoCount;
                                try
                                {
                                    file = detector.DetectFile(input, reader, current, logger);
                                }
                                catch (Exception exception2)
                                {
                                    logger.LogError(string.Format("Detector in class \"{0}\" failed @ offset 0x{1:X8} with unhandled exception: {2}", detector.GetType().ToString(), current, exception2.ToString()));
                                }
                                if (file != null)
                                {
                                    if (file.Offset != current)
                                    {
                                        logger.LogError(string.Format("Detector in class \"{0}\" violates scanning rules @ offset 0x{1:X8}. Offset of detected file must match current position in stream.", detector.GetType().ToString(), current));
                                        file = null;
                                    }
                                    else if ((file.Offset + file.Length) > length)
                                    {
                                        logger.LogError(string.Format("Detector in class \"{0}\" violates scanning rules @ offset 0x{1:X8}. Detected file goes beyond end of stream by {2} bytes.", detector.GetType().ToString(), current, (file.Offset + file.Length) - length));
                                        file = null;
                                    }
                                    else if (file.Length < 0L)
                                    {
                                        logger.LogError(string.Format("Detector in class \"{0}\" violates scanning rules @ offset 0x{1:X8}. Length of detected file is negative ({2} bytes).", detector.GetType().ToString(), current, file.Length));
                                        file = null;
                                    }
                                    else
                                    {
                                        bool flag2 = false;
                                        if (detector.FileTypes != null)
                                        {
                                            foreach (Ravioli.ArchiveInterface.Scanning.FileType type in detector.FileTypes)
                                            {
                                                if (type.TypeName == file.FileType.TypeName)
                                                {
                                                    flag2 = true;
                                                    break;
                                                }
                                            }
                                        }
                                        if (!flag2)
                                        {
                                            logger.LogError(string.Format("Detector in class \"{0}\" violates scanning rules @ offset 0x{1:X8}. Detector returned a file type it does not declare (\"{2}\")", detector.GetType().ToString(), current, file.FileType.TypeName));
                                            file = null;
                                        }
                                    }
                                    if (file != null)
                                    {
                                        logger.LogInfo(string.Format("New file: offset = 0x{0:X8}, length = 0x{1:X8}, typeName = \"{2}\"", file.Offset, file.Length, file.FileType.TypeName));
                                        logger.LogInfo(message);
                                        ScanDirectoryEntry entryFromDetectedFile = GetEntryFromDetectedFile(file, ref nextNumber);
                                        list.Add(entryFromDetectedFile);
                                        if (reportProgress && (this.ScanItemFound != null))
                                        {
                                            this.ScanItemFound(this, new ScanItemEventArgs(entryFromDetectedFile, length, null));
                                        }
                                        break;
                                    }
                                }
                                if ((file == null) && (((logger.ErrorCount > errorCount) || (logger.WarningCount > warningCount)) || (logger.InfoCount > infoCount)))
                                {
                                    logger.LogInfo(message);
                                }
                            }
                        }
                        oldPosition = current;
                        if (file == null)
                        {
                            current += 1L;
                        }
                        else
                        {
                            current += file.Length;
                        }
                        continue;
                    }
                    finally
                    {
                        reader2.Close();
                        stream2.Close();
                    }
                }
                if (reportProgress)
                {
                    this.ReportProgress(current, length, ref oldPercent, list.Count, ref oldItemsFound, logger);
                }
                stopwatch.Stop();
                if (!this.scanCancelled)
                {
                    logger.LogInfo(string.Format("Scanning done, {0} files found, {1} errors, {2} warnings, {3:G4} s.", new object[] { list.Count, logger.ErrorCount, logger.WarningCount, stopwatch.Elapsed.TotalSeconds }));
                }
                else
                {
                    logger.LogError(string.Format("Scanning was cancelled after {0:G4} s.", stopwatch.Elapsed.TotalSeconds));
                }
                if (!this.scanCancelled || this.keepResultsOnCancel)
                {
                    return new ScanResults { FileName = fileName, FileSize = length, Range = range, LastPosition = current, Entries = list.ToArray() };
                }
            }
            finally
            {
                reader.Close();
                input.Close();
            }
            return null;
        }

        public void ScanAsync(string fileName)
        {
            if ((this.scanThread != null) && this.scanThread.IsAlive)
            {
                throw new InvalidOperationException("A scan is already in progress. Only one scan can run at a time.");
            }
            this.scanFileName = fileName;
            this.scanRange = null;
            this.scanThread = new Thread(new ThreadStart(this.ScanThread));
            this.scanThread.Start();
        }

        public void ScanAsync(string fileName, ScanRange range)
        {
            if ((this.scanThread != null) && this.scanThread.IsAlive)
            {
                throw new InvalidOperationException("A scan is already in progress. Only one scan can run at a time.");
            }
            this.scanFileName = fileName;
            this.scanRange = range;
            this.scanThread = new Thread(new ThreadStart(this.ScanThread));
            this.scanThread.Start();
        }

        public void ScanCancelAsync()
        {
            this.scanCancelled = true;
        }

        private void ScanThread()
        {
            ScanResults results = null;
            Exception error = null;
            this.scanCancelled = false;
            try
            {
                results = this.Scan(this.scanFileName, this.scanRange, true);
            }
            catch (Exception exception2)
            {
                error = exception2;
            }
            if (this.ScanCompleted != null)
            {
                this.ScanCompleted(this, new ScanCompletedEventArgs(error, this.scanCancelled && !this.keepResultsOnCancel, results, null));
            }
        }

        public bool KeepResultsOnCancel
        {
            get
            {
                return this.keepResultsOnCancel;
            }
            set
            {
                this.keepResultsOnCancel = value;
            }
        }
    }
}

