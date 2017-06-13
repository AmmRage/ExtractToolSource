namespace Ravioli.Extractor
{
    using Ravioli.AppShared;
    using Ravioli.ArchiveInterface;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading;

    public class JobQueueCreator
    {
        public event RootDirectoryHandler RootDirectoryNeeded;

        public Queue<ExtractJob> CreateJobQueue(ExtractorParameters parameters, ArchivePluginManager archivePluginManager, ImagePluginManager imagePluginManager, SoundPluginManager soundPluginManager)
        {
            SoundExportFormat format;
            if (parameters == null)
            {
                throw new ArgumentNullException("parameters");
            }
            if (archivePluginManager == null)
            {
                throw new ArgumentNullException("archivePluginManager");
            }
            if (imagePluginManager == null)
            {
                throw new ArgumentNullException("imagePluginManager");
            }
            string[] parsedInputFiles = parameters.GetParsedInputFiles();
            if (parsedInputFiles.Length == 0)
            {
                throw new ArgumentException("An input file is required.");
            }
            if (parameters.OutputDir.Length == 0)
            {
                throw new ArgumentException("An output directory is required.");
            }
            foreach (char ch in Path.GetInvalidPathChars())
            {
                if (parameters.OutputDir.Contains(ch.ToString()))
                {
                    throw new ArgumentException("Invalid character '" + ch + "' in output directory name.");
                }
            }
            if (archivePluginManager.AvailableArchivePlugins.Length == 0)
            {
                throw new InvalidOperationException("No archive plug-ins available.");
            }
            IImageSaver imageSaver = null;
            if (parameters.ImageFormat.Length > 0)
            {
                ImagePluginMapping plugin = imagePluginManager.FindSaverPlugin(parameters.ImageFormat);
                if (plugin != null)
                {
                    try
                    {
                        imageSaver = imagePluginManager.CreateSaverInstance(plugin);
                        goto Label_012E;
                    }
                    catch (Exception exception)
                    {
                        throw new ApplicationException("Unable to create image saver \"" + plugin.TypeName + "\": " + exception.Message, exception);
                    }
                }
                throw new ApplicationException("No image saver available for image format \"" + parameters.ImageFormat + "\".");
            }
        Label_012E:
            format = null;
            if (parameters.SoundFormat.Length > 0)
            {
                format = soundPluginManager.FindExportFormat(parameters.SoundFormat);
                if (format == null)
                {
                    throw new ApplicationException("Conversion to sound format \"" + parameters.SoundFormat + "\" is not supported.");
                }
            }
            ArchivePluginMapping mapping2 = null;
            if (parameters.ArchiveType.Length > 0)
            {
                mapping2 = archivePluginManager.FindPluginByName(parameters.ArchiveType);
                if (mapping2 == null)
                {
                    throw new FileNotFoundException("The archive type with the name \"" + parameters.ArchiveType + "\" was not found.");
                }
            }
            List<string> list = new List<string>();
            foreach (string str in parsedInputFiles)
            {
                try
                {
                    list.AddRange(ResolveInputFile(str));
                }
                catch (Exception exception2)
                {
                    throw new ApplicationException("Could not resolve input file " + str + ": " + exception2.Message, exception2);
                }
            }
            foreach (string str2 in list)
            {
                if (!File.Exists(str2) && !Directory.Exists(str2))
                {
                    throw new FileNotFoundException("The input file \"" + str2 + "\" does not exist.");
                }
            }
            int index = 0;
            Queue<ExtractJob> queue = new Queue<ExtractJob>();
            foreach (string str3 in list)
            {
                ArchivePluginMapping[] mappingArray;
                string inputFile = str3;
                List<ArchivePluginMapping> list2 = new List<ArchivePluginMapping>();
                ArchivePluginMapping pluginMapping = null;
                if (mapping2 == null)
                {
                    mappingArray = archivePluginManager.FindPlugins(Path.GetExtension(str3));
                    if ((mappingArray == null) || (mappingArray.Length == 0))
                    {
                        mappingArray = archivePluginManager.FindGameViewerPlugins(str3);
                        if ((mappingArray == null) || (mappingArray.Length == 0))
                        {
                            throw new FormatException("The file type of \"" + str3 + "\" is unknown.");
                        }
                    }
                }
                else
                {
                    mappingArray = new ArchivePluginMapping[] { mapping2 };
                }
                StringBuilder builder = new StringBuilder();
                foreach (ArchivePluginMapping mapping4 in mappingArray)
                {
                    if (builder.Length > 0)
                    {
                        builder.Append(", ");
                    }
                    builder.Append(mapping4.TypeName);
                    try
                    {
                        string fileOfDirectory;
                        IArchive archive = archivePluginManager.CreateInstance(mapping4);
                        if (mapping4 is GameViewerPluginMapping)
                        {
                            fileOfDirectory = FileSystemHelper.GetFileOfDirectory(str3);
                        }
                        else
                        {
                            fileOfDirectory = str3;
                        }
                        if (archive.IsValidFormat(fileOfDirectory))
                        {
                            list2.Add(mapping4);
                            inputFile = fileOfDirectory;
                        }
                    }
                    catch (Exception exception3)
                    {
                        throw new ApplicationException("The plug-in \"" + mapping4.TypeName + "\" caused an error while checking the file type: " + exception3.Message, exception3);
                    }
                }
                if (list2.Count > 0)
                {
                    if (list2.Count != 1)
                    {
                        StringBuilder builder2 = new StringBuilder();
                        builder2.AppendFormat("Ambiguous match. There are {0} archive types can handle the file \"{1}\". You need to specify the archive type explicitly. The possible types are: ", list2.Count, str3);
                        bool flag = true;
                        foreach (PluginMapping mapping5 in list2)
                        {
                            if (!flag)
                            {
                                builder2.Append(", ");
                            }
                            else
                            {
                                flag = false;
                            }
                            builder2.Append("\"");
                            builder2.Append(mapping5.TypeName);
                            builder2.Append("\"");
                        }
                        throw new ApplicationException(builder2.ToString());
                    }
                    pluginMapping = list2[0];
                }
                else
                {
                    if ((mappingArray.Length == 1) && (mappingArray[0] is GameViewerPluginMapping))
                    {
                        throw new FormatException("\"" + mappingArray[0].TypeName + "\" was not found at the specified location.");
                    }
                    if (!parameters.AllowScanning)
                    {
                        throw new FormatException("The file type of \"" + str3 + "\" is unknown. Allow scanning of unknown files to scan this file and other unknown files for known resources, like images and sounds.");
                    }
                    pluginMapping = null;
                }
                string rootDirectory = string.Empty;
                if ((pluginMapping != null) && pluginMapping.NeedsRootDirectory)
                {
                    string[] parsedRootDirs = parameters.GetParsedRootDirs();
                    if ((parsedRootDirs.GetUpperBound(0) >= index) && !string.IsNullOrEmpty(parsedRootDirs[index]))
                    {
                        rootDirectory = parsedRootDirs[index];
                    }
                    else if (this.RootDirectoryNeeded != null)
                    {
                        RootDirectoryEventArgs e = new RootDirectoryEventArgs(pluginMapping, str3);
                        this.RootDirectoryNeeded(this, e);
                        rootDirectory = e.RootDirectory;
                    }
                    if (string.IsNullOrEmpty(rootDirectory))
                    {
                        throw new ApplicationException("Archive type " + pluginMapping.TypeName + " requires a root directory, but it was not supplied.");
                    }
                }
                queue.Enqueue(new ExtractJob(inputFile, parameters.OutputDir, parameters.InputFileSubDir, pluginMapping, rootDirectory, imageSaver, format));
                index++;
            }
            return queue;
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

        private static List<string> ResolveInputFile(string path)
        {
            List<string> list = new List<string>();
            if (path.Contains("*") || path.Contains("?"))
            {
                string directoryName = Path.GetDirectoryName(path);
                string fileName = Path.GetFileName(path);
                if (directoryName.Length == 0)
                {
                    directoryName = Environment.CurrentDirectory;
                }
                foreach (string str3 in Directory.GetFiles(directoryName, fileName))
                {
                    string item = Path.Combine(directoryName, str3);
                    list.Add(item);
                }
                return list;
            }
            list.Add(path);
            return list;
        }
    }
}

