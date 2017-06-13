namespace Ravioli.AppShared
{
    using Ravioli.ArchiveInterface;
    using System;
    using System.IO;
    using System.Text;
    using System.Threading;

    public class BaseExtractor
    {
        private bool abortThread;
        private IArchive archive;
        private IFileInfo[] files;
        private ImagePluginManager imagePluginManager;
        private string outputDir;
        private bool running;
        private IImageSaver saver;
        private SoundExportFormat soundFormat;
        private SoundPluginManager soundPluginManager;

        public event ProgressEventHandler ArchiveProgress;

        public event PercentProgressEventHandler CurrentFileProgress;

        public event ExtractEventHandler FileExtracted;

        public event ExtractErrorEventHandler FileExtractError;

        public event ExtractEventHandler FileExtracting;

        public event ExtractWarningEventHandler FileExtractWarning;

        public event EventHandler Finished;

        public void Abort()
        {
            this.abortThread = true;
        }

        private void archive_ExtractProgress(object sender, PercentProgressEventArgs e)
        {
            if (this.CurrentFileProgress != null)
            {
                this.CurrentFileProgress(sender, e);
            }
        }

        private void archive_FileExtractWarning(object sender, ExtractWarningEventArgs e)
        {
            if (this.FileExtractWarning != null)
            {
                this.FileExtractWarning(sender, e);
            }
        }

        public void Extract(IArchive archive, string outputDir)
        {
            if (this.running)
            {
                throw new InvalidOperationException("An extraction is already running.");
            }
            this.running = true;
            this.archive = archive;
            this.imagePluginManager = null;
            this.saver = null;
            this.soundPluginManager = null;
            this.soundFormat = null;
            lock (archive)
            {
                this.files = archive.Files;
            }
            this.outputDir = outputDir;
            this.abortThread = false;
            this.ExtractWorker();
        }

        public void Extract(IArchive archive, IFileInfo file, string outputDir)
        {
            if (this.running)
            {
                throw new InvalidOperationException("An extraction is already running.");
            }
            this.running = true;
            this.archive = archive;
            this.imagePluginManager = null;
            this.saver = null;
            this.soundPluginManager = null;
            this.soundFormat = null;
            this.files = new IFileInfo[] { file };
            this.outputDir = outputDir;
            this.abortThread = false;
            this.ExtractWorker();
        }

        public void Extract(IArchive archive, string outputDir, ImagePluginManager imageManager, IImageSaver saver, SoundPluginManager soundManager, SoundExportFormat soundFormat)
        {
            if (this.running)
            {
                throw new InvalidOperationException("An extraction is already running.");
            }
            this.running = true;
            this.archive = archive;
            this.imagePluginManager = imageManager;
            this.saver = saver;
            this.soundPluginManager = soundManager;
            this.soundFormat = soundFormat;
            lock (archive)
            {
                this.files = archive.Files;
            }
            this.outputDir = outputDir;
            this.abortThread = false;
            this.ExtractWorker();
        }

        public void ExtractAsync(IArchive archive, string outputDir)
        {
            this.ExtractAsync(archive, outputDir, null, null, null, null);
        }

        public void ExtractAsync(IArchive archive, IFileInfo[] files, string outputDir)
        {
            this.ExtractAsync(archive, files, outputDir, null, null, null, null);
        }

        public void ExtractAsync(IArchive archive, string outputDir, ImagePluginManager imageManager, IImageSaver saver, SoundPluginManager soundManager, SoundExportFormat soundFormat)
        {
            if (this.running)
            {
                throw new InvalidOperationException("An extraction is already running.");
            }
            this.running = true;
            this.archive = archive;
            this.imagePluginManager = imageManager;
            this.saver = saver;
            this.soundPluginManager = soundManager;
            this.soundFormat = soundFormat;
            lock (archive)
            {
                this.files = archive.Files;
            }
            this.outputDir = outputDir;
            this.abortThread = false;
            new Thread(new ThreadStart(this.ExtractThread)) { Name = "ExtractThread" }.Start();
        }

        public void ExtractAsync(IArchive archive, IFileInfo[] files, string outputDir, ImagePluginManager imageManager, IImageSaver saver, SoundPluginManager soundManager, SoundExportFormat soundFormat)
        {
            if (this.running)
            {
                throw new InvalidOperationException("An extraction is already running.");
            }
            this.running = true;
            this.archive = archive;
            this.imagePluginManager = imageManager;
            this.saver = saver;
            this.soundPluginManager = soundManager;
            this.soundFormat = soundFormat;
            this.files = files;
            this.outputDir = outputDir;
            this.abortThread = false;
            new Thread(new ThreadStart(this.ExtractThread)) { Name = "ExtractThread" }.Start();
        }

        private void ExtractThread()
        {
            this.ExtractWorker();
        }

        private void ExtractWorker()
        {
            int length = this.files.Length;
            int done = 0;
            if (this.ArchiveProgress != null)
            {
                this.ArchiveProgress(this, new ProgressEventArgs(done, length));
            }
            IExtractProgress archive = this.archive as IExtractProgress;
            if (archive != null)
            {
                archive.ExtractProgress += new PercentProgressEventHandler(this.archive_ExtractProgress);
            }
            IExtractWarnings warnings = this.archive as IExtractWarnings;
            if (warnings != null)
            {
                warnings.FileExtractWarning += new ExtractWarningEventHandler(this.archive_FileExtractWarning);
            }
            foreach (IFileInfo info in this.files)
            {
                lock (this.archive)
                {
                    bool flag;
                    bool flag2;
                    bool flag3;
                    if (this.abortThread)
                    {
                        break;
                    }
                    do
                    {
                        flag3 = false;
                        flag = false;
                        flag2 = false;
                        if (this.FileExtracting != null)
                        {
                            this.FileExtracting(this, new ExtractEventArgs(info.Name));
                        }
                        try
                        {
                            bool flag4 = false;
                            bool flag5 = false;
                            if ((!flag5 && (this.imagePluginManager != null)) && (this.saver != null))
                            {
                                ImageConverter converter = new ImageConverter(this.imagePluginManager);
                                ImagePluginMapping loaderMapping = converter.FindImageLoader(this.archive, info);
                                if ((loaderMapping != null) && !this.imagePluginManager.IsSameFormat(loaderMapping, this.saver))
                                {
                                    try
                                    {
                                        converter.ConvertFormat(this.archive, info, loaderMapping, this.saver, this.outputDir);
                                        flag4 = true;
                                    }
                                    catch (Exception exception)
                                    {
                                        flag4 = true;
                                        ApplicationException exception2 = new ApplicationException("Image conversion failed (" + exception.Message + ").");
                                        if (this.FileExtractWarning != null)
                                        {
                                            this.FileExtractWarning(this, new ExtractWarningEventArgs(info.Name, exception2));
                                        }
                                    }
                                }
                            }
                            if ((!flag4 && (this.soundPluginManager != null)) && (this.soundFormat != null))
                            {
                                SoundConverter converter2 = new SoundConverter(this.soundPluginManager);
                                PluginMapping playerMapping = converter2.FindSoundPlayer(this.archive, info);
                                if (playerMapping != null)
                                {
                                    try
                                    {
                                        converter2.ConvertFormat(this.archive, info, playerMapping, this.soundFormat, this.outputDir);
                                        flag5 = true;
                                    }
                                    catch (Exception exception3)
                                    {
                                        flag5 = true;
                                        ApplicationException exception4 = new ApplicationException("Sound conversion failed (" + exception3.Message + ").");
                                        if (this.FileExtractWarning != null)
                                        {
                                            this.FileExtractWarning(this, new ExtractWarningEventArgs(info.Name, exception4));
                                        }
                                    }
                                }
                            }
                            if (!flag4 && !flag5)
                            {
                                string path = Path.Combine(this.outputDir, this.ReplaceInvalidChars(info.Name));
                                string directoryName = Path.GetDirectoryName(path);
                                path = Path.Combine(directoryName, Path.GetFileName(path));
                                if (!Directory.Exists(directoryName))
                                {
                                    Directory.CreateDirectory(directoryName);
                                }
                                this.archive.ExtractFile(info, path);
                            }
                        }
                        catch (Exception exception5)
                        {
                            flag3 = true;
                            if (this.FileExtractError != null)
                            {
                                ExtractErrorEventArgs e = new ExtractErrorEventArgs(info.Name, exception5);
                                this.FileExtractError(this, e);
                                if (e.Action == ErrorAction.Retry)
                                {
                                    flag = true;
                                }
                                else if (e.Action == ErrorAction.Abort)
                                {
                                    flag2 = true;
                                }
                            }
                            else
                            {
                                this.running = false;
                                throw;
                            }
                        }
                    }
                    while (flag);
                    if (flag2)
                    {
                        break;
                    }
                    done++;
                    if ((this.FileExtracted != null) && !flag3)
                    {
                        this.FileExtracted(this, new ExtractEventArgs(info.Name));
                    }
                    if (this.ArchiveProgress != null)
                    {
                        this.ArchiveProgress(this, new ProgressEventArgs(done, length));
                    }
                }
            }
            if (archive != null)
            {
                archive.ExtractProgress -= new PercentProgressEventHandler(this.archive_ExtractProgress);
            }
            if (warnings != null)
            {
                warnings.FileExtractWarning -= new ExtractWarningEventHandler(this.archive_FileExtractWarning);
            }
            this.running = false;
            if (this.Finished != null)
            {
                this.Finished(this, EventArgs.Empty);
            }
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
            builder.Replace('*', '_');
            return builder.ToString();
        }

        public bool IsRunning
        {
            get
            {
                return this.running;
            }
        }
    }
}

