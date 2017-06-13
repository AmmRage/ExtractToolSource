
using Ravioli.AppShared;
using Ravioli.ArchiveInterface;

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Ravioli.Scanner;
using ScanResults = Ravioli.ArchivePlugins.ScanResults;


namespace Ravioli.Extractor
{


    public class ExtractorRunner
    {
        private ArchivePluginManager archivePluginManager;
        private ExtractJob currentScanJob;
        private BaseExtractor extractor;
        private ImagePluginManager imagePluginManager;
        private bool processFileThreadResult;
        private FileScanner scanner;
        private ScanPluginManager scanPluginManager;
        private SoundPluginManager soundPluginManager;
        private bool sync;
        private string threadDeleteFile;
        private IArchive threadFile;
        private Queue<ExtractJob> workerQueue;

        public event EventHandler Finished;

        public event JobErrorEventHandler JobError;

        public event JobInfoEventHandler JobInfo;

        public event JobScanCompletedEventHandler JobScanCompleted;

        public event JobScanProgressEventHandler JobScanProgress;

        public event JobEventHandler JobStarting;

        public ExtractorRunner(Queue<ExtractJob> workerQueue, BaseExtractor extractor, ArchivePluginManager archivePluginManager, ImagePluginManager imagePluginManager, SoundPluginManager soundPluginManager, ScanPluginManager scanPluginManager)
        {
            if (workerQueue == null)
            {
                throw new ArgumentNullException("workerQueue");
            }
            if (extractor == null)
            {
                throw new ArgumentException("extractor");
            }
            if (archivePluginManager == null)
            {
                throw new ArgumentNullException("archivePluginManager");
            }
            if (imagePluginManager == null)
            {
                throw new ArgumentNullException("imagePluginManager");
            }
            if (soundPluginManager == null)
            {
                throw new ArgumentNullException("soundPluginManager");
            }
            if (scanPluginManager == null)
            {
                throw new ArgumentNullException("scanPluginManager");
            }
            this.workerQueue = workerQueue;
            this.extractor = extractor;
            this.archivePluginManager = archivePluginManager;
            this.imagePluginManager = imagePluginManager;
            this.soundPluginManager = soundPluginManager;
            this.scanPluginManager = scanPluginManager;
            this.scanner = new FileScanner(scanPluginManager);
        }

        private void ExecuteNextJob()
        {
            ExtractJob job;
            do
            {
                job = null;
                lock (this.workerQueue)
                {
                    if (this.workerQueue.Count > 0)
                    {
                        job = this.workerQueue.Dequeue();
                    }
                }
                if (job != null)
                {
                    if (this.JobStarting != null)
                    {
                        this.JobStarting(this, new JobEventArgs(job));
                    }
                    if (this.ProcessFile(job))
                    {
                        return;
                    }
                }
            }
            while (job != null);
            this.extractor.Finished -= new EventHandler(this.extractor_Finished);
            if (this.Finished != null)
            {
                this.Finished(this, EventArgs.Empty);
            }
        }

        private void extractor_Finished(object sender, EventArgs e)
        {
            try
            {
                this.threadFile.Close();
                if (this.threadDeleteFile != null)
                {
                    File.Delete(this.threadDeleteFile);
                }
            }
            catch (Exception)
            {
            }
            this.ExecuteNextJob();
        }

        private bool ProcessFile(ExtractJob job)
        {
            if (this.sync)
            {
                this.ProcessFileThread(job);
                return this.processFileThreadResult;
            }
            new Thread(new ParameterizedThreadStart(this.ProcessFileThread)).Start(job);
            return true;
        }

        private void ProcessFileThread(object jobObject)
        {
            ArchivePluginMapping scanArchiveMapping;
            string inputFile;
            bool flag;
            this.processFileThreadResult = false;
            ExtractJob job = (ExtractJob) jobObject;
            if (job.ArchivePlugin == null)
            {
                string str2 = this.ScanUnknownFile(job);
                if (str2 == null)
                {
                    this.processFileThreadResult = false;
                    this.ExecuteNextJob();
                    return;
                }
                scanArchiveMapping = ScanDefinitions.ScanArchiveMapping;
                inputFile = str2;
                flag = true;
            }
            else
            {
                scanArchiveMapping = job.ArchivePlugin;
                inputFile = job.InputFile;
                flag = false;
            }
            this.processFileThreadResult = this.ProcessKnownFile(job, scanArchiveMapping, inputFile, flag);
            if (!this.processFileThreadResult)
            {
                this.ExecuteNextJob();
            }
        }

        private bool ProcessKnownFile(ExtractJob job, ArchivePluginMapping useArchivePlugin, string useInputFile, bool deleteUseInputFile)
        {
            IArchive archive = null;
            bool retry;
            string baseOutputDir;
            do
            {
                retry = false;
                try
                {
                    archive = this.archivePluginManager.CreateInstance(useArchivePlugin);
                }
                catch (Exception exception)
                {
                    if (this.JobError != null)
                    {
                        string message = string.Format("Failed to create an instance of plug-in \"{0}\": {1}", useArchivePlugin.TypeName, exception.ToString());
                        JobErrorEventArgs e = new JobErrorEventArgs(job, message);
                        this.JobError(this, e);
                        retry = e.Retry;
                    }
                    if (!retry)
                    {
                        return false;
                    }
                }
            }
            while (retry);
            do
            {
                retry = false;
                string str2 = "(Unknown)";
                try
                {
                    IRootDirectory directory = archive as IRootDirectory;
                    if (directory != null)
                    {
                        str2 = "RootDirectory";
                        directory.RootDirectory = job.RootDirectory;
                    }
                    IDataWriter writer = archive as IDataWriter;
                    if (writer != null)
                    {
                        str2 = "DataDirectory";
                        string pluginDataDirectory = this.archivePluginManager.PluginDataDirectory;
                        if (!Directory.Exists(pluginDataDirectory))
                        {
                            Directory.CreateDirectory(pluginDataDirectory);
                        }
                        writer.DataDirectory = pluginDataDirectory;
                    }
                }
                catch (Exception exception2)
                {
                    if (this.JobError != null)
                    {
                        string str4 = "Unable to set additional interface property \"" + str2 + "\". " + exception2.Message;
                        JobErrorEventArgs args2 = new JobErrorEventArgs(job, str4);
                        this.JobError(this, args2);
                        retry = args2.Retry;
                    }
                    if (!retry)
                    {
                        archive.Close();
                        return false;
                    }
                }
            }
            while (retry);
            do
            {
                retry = false;
                try
                {
                    archive.Open(useInputFile);
                }
                catch (Exception exception3)
                {
                    if (this.JobError != null)
                    {
                        string str5 = string.Format("Unable to open the input file \"{0}\". {1}", useInputFile, exception3.Message);
                        JobErrorEventArgs args3 = new JobErrorEventArgs(job, str5);
                        this.JobError(this, args3);
                        retry = args3.Retry;
                    }
                    if (!retry)
                    {
                        return false;
                    }
                }
            }
            while (retry);
            if (this.JobInfo != null)
            {
                IExtractProgress progress = archive as IExtractProgress;
                IFileSystemAbstractor abstractor = archive as IFileSystemAbstractor;
                JobInfoEventArgs args4 = new JobInfoEventArgs(job, progress, abstractor);
                this.JobInfo(this, args4);
            }
            if (job.InputFileSubDir)
            {
                IFileSystemAbstractor abstractor2 = archive as IFileSystemAbstractor;
                string str7 = (abstractor2 != null) ? this.ReplaceInvalidChars(abstractor2.DisplayFileName) : Path.GetFileNameWithoutExtension(useInputFile);
                baseOutputDir = Path.Combine(job.BaseOutputDir, str7);
            }
            else
            {
                baseOutputDir = job.BaseOutputDir;
            }
            this.threadFile = archive;
            this.threadDeleteFile = deleteUseInputFile ? useInputFile : null;
            if ((job.ImageSaver != null) || (job.SoundFormat != null))
            {
                if (this.sync)
                {
                    this.extractor.Extract(archive, baseOutputDir, this.imagePluginManager, job.ImageSaver, this.soundPluginManager, job.SoundFormat);
                }
                else
                {
                    this.extractor.ExtractAsync(archive, baseOutputDir, this.imagePluginManager, job.ImageSaver, this.soundPluginManager, job.SoundFormat);
                }
            }
            else if (this.sync)
            {
                this.extractor.Extract(archive, baseOutputDir);
            }
            else
            {
                this.extractor.ExtractAsync(archive, baseOutputDir);
            }
            return true;
        }

        private string ReplaceInvalidChars(string fileName)
        {
            StringBuilder builder = new StringBuilder(fileName);
            foreach (char ch in Path.GetInvalidPathChars())
            {
                builder.Replace(ch, '_');
            }
            builder.Replace(':', '_');
            builder.Replace('?', '_');
            return builder.ToString();
        }

        public void Run()
        {
            this.sync = true;
            this.RunInternal();
        }

        public void RunAsync()
        {
            this.sync = false;
            this.RunInternal();
        }

        public void RunCancelAsync()
        {
            lock (this.workerQueue)
            {
                this.workerQueue.Clear();
            }
            this.scanner.ScanCancelAsync();
            this.extractor.Abort();
        }

        private void RunInternal()
        {
            this.extractor.Finished += new EventHandler(this.extractor_Finished);
            this.ExecuteNextJob();
        }

        private void scanner_ScanProgressChanged(object sender, ScanProgressChangedEventArgs e)
        {
            if (this.JobScanProgress != null)
            {
                this.JobScanProgress(this, new JobScanProgressEventArgs(this.currentScanJob, e.ProgressPercentage));
            }
        }

        private string ScanUnknownFile(ExtractJob job)
        {
            string tempResultsFileName;
            bool retry;
            do
            {
                tempResultsFileName = null;
                retry = false;
                try
                {
                    Scanner.ScanResults results;
                    this.currentScanJob = job;
                    this.scanner.ScanProgressChanged += new ScanProgressChangedEventHandler(this.scanner_ScanProgressChanged);
                    try
                    {
                        results = this.scanner.Scan(job.InputFile);
                    }
                    finally
                    {
                        this.scanner.ScanProgressChanged -= new ScanProgressChangedEventHandler(this.scanner_ScanProgressChanged);
                    }
                    if (results != null)
                    {
                        if (this.JobScanCompleted != null)
                        {
                            this.JobScanCompleted(this, new JobScanCompletedEventArgs(job, results.Entries.Length));
                        }
                        this.currentScanJob = null;
                        tempResultsFileName = ScanDefinitions.GetTempResultsFileName(job.InputFile);
                        FileOperations.SaveScanResults(results, tempResultsFileName);
                    }
                }
                catch (Exception exception)
                {
                    if (this.JobError != null)
                    {
                        string message = string.Format("Failed to scan file \"{0}\": {1}", job.InputFile, exception.Message);
                        JobErrorEventArgs e = new JobErrorEventArgs(job, message);
                        this.JobError(this, e);
                        retry = e.Retry;
                    }
                    if (!retry)
                    {
                        return null;
                    }
                }
            }
            while (retry);
            return tempResultsFileName;
        }
    }
}

