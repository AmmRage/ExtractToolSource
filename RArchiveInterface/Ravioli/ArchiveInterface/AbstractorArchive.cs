namespace Ravioli.ArchiveInterface
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Text;

    public abstract class AbstractorArchive : ArchiveBase, IGameViewer, IFileSystemAbstractor, IArchive, IClassInfo
    {
        private List<AbstractorDirectoryEntry> directory;
        private AbstractorDuplicatesPolicy duplicatesPolicy;
        private Dictionary<string, int> lookupTable;
        private const int maxPreviousSubArchives = 2;
        private IArchive previousNestedSubArchive;
        private BinaryReader previousNestedSubArchiveReader;
        private Stream previousNestedSubArchiveStream;
        private List<IArchive> previousSubArchives;

        protected AbstractorArchive()
        {
        }

        private AddResult AddDirectoryEntry(AbstractorDirectoryEntry entry)
        {
            if (this.duplicatesPolicy == AbstractorDuplicatesPolicy.Keep)
            {
                this.directory.Add(entry);
                if (this.lookupTable.ContainsKey(entry.DisplayName))
                {
                    return AddResult.AddedDuplicate;
                }
                this.lookupTable.Add(entry.DisplayName, this.directory.Count - 1);
                return AddResult.Added;
            }
            if (this.duplicatesPolicy == AbstractorDuplicatesPolicy.Replace)
            {
                bool flag = false;
                if (this.lookupTable.ContainsKey(entry.DisplayName))
                {
                    int num = this.lookupTable[entry.DisplayName];
                    AbstractorDirectoryEntry entry2 = this.directory[num];
                    if (entry2.DisplayName != entry.DisplayName)
                    {
                        throw new InvalidDataException("Entry from lookup table differs from Display name.");
                    }
                    this.directory[num] = entry;
                    this.lookupTable[entry.DisplayName] = num;
                    flag = true;
                }
                if (!flag)
                {
                    this.directory.Add(entry);
                    this.lookupTable.Add(entry.DisplayName, this.directory.Count - 1);
                    return AddResult.Added;
                }
                return AddResult.Replaced;
            }
            if (this.duplicatesPolicy == AbstractorDuplicatesPolicy.Rename)
            {
                if (this.lookupTable.ContainsKey(entry.DisplayName))
                {
                    string str;
                    int num2 = 1;
                    Path.GetFileNameWithoutExtension(entry.DisplayName);
                    string extension = Path.GetExtension(entry.DisplayName);
                    do
                    {
                        num2++;
                        str = string.Format("{0} ({1}){2}", entry.DisplayName, num2, extension);
                    }
                    while (this.lookupTable.ContainsKey(str));
                    entry.DisplayName = str;
                    this.directory.Add(entry);
                    this.lookupTable.Add(entry.DisplayName, this.directory.Count - 1);
                    return AddResult.AddedRenamed;
                }
                this.directory.Add(entry);
                this.lookupTable.Add(entry.DisplayName, this.directory.Count - 1);
                return AddResult.Added;
            }
            this.directory.Add(entry);
            if (this.lookupTable.ContainsKey(entry.DisplayName))
            {
                return AddResult.AddedDuplicate;
            }
            this.lookupTable.Add(entry.DisplayName, this.directory.Count - 1);
            return AddResult.Added;
        }

        private void CloseNestedSubArchive()
        {
            if (this.previousNestedSubArchive != null)
            {
                this.previousNestedSubArchive.Close();
                this.previousNestedSubArchive = null;
            }
            if (this.previousNestedSubArchiveReader != null)
            {
                this.previousNestedSubArchiveReader.Close();
                this.previousNestedSubArchiveReader = null;
            }
            if (this.previousNestedSubArchiveStream != null)
            {
                this.previousNestedSubArchiveStream.Close();
                this.previousNestedSubArchiveStream = null;
            }
        }

        private void CopyData(AbstractorDirectoryEntry entry, Stream stream, Stream outputStream, long byteCount)
        {
            if (!entry.IsSubArchive)
            {
                FileStream stream2 = new FileStream(entry.SourceFSPath, FileMode.Open, FileAccess.Read);
                try
                {
                    long num = ((byteCount >= 0L) && (byteCount < entry.Size)) ? byteCount : entry.Size;
                    byte[] buffer = new byte[0x2000];
                    while (num > 0L)
                    {
                        int count = (num > 0x2000L) ? 0x2000 : ((int) num);
                        int num3 = stream2.Read(buffer, 0, count);
                        outputStream.Write(buffer, 0, num3);
                        num -= num3;
                    }
                    return;
                }
                finally
                {
                    stream2.Close();
                }
            }
            if (this.previousSubArchives == null)
            {
                this.previousSubArchives = new List<IArchive>();
            }
            IArchive archive = null;
            foreach (IArchive archive2 in this.previousSubArchives)
            {
                if (archive2.FileName == entry.SourceFSPath)
                {
                    archive = archive2;
                    break;
                }
            }
            if (archive == null)
            {
                archive = (IArchive) Activator.CreateInstance(entry.SubArchivePluginType);
                this.OnOpeningForExtraction(archive);
                archive.Open(entry.SourceFSPath);
                while (this.previousSubArchives.Count >= 2)
                {
                    this.previousSubArchives[0].Close();
                    this.previousSubArchives.RemoveAt(0);
                }
                this.previousSubArchives.Add(archive);
                this.CloseNestedSubArchive();
            }
            if (entry.NestedSubArchiveFileInfo == null)
            {
                if (byteCount < 0L)
                {
                    archive.ExtractFile(entry.SubArchiveFileInfo, outputStream);
                }
                else
                {
                    archive.ExtractFile(entry.SubArchiveFileInfo, outputStream, byteCount);
                }
            }
            else
            {
                IArchive previousNestedSubArchive = null;
                string fileName = entry.SourceFSPath + "#" + entry.SubArchiveFileInfo.Name;
                if ((this.previousNestedSubArchive != null) && (this.previousNestedSubArchive.FileName == fileName))
                {
                    previousNestedSubArchive = this.previousNestedSubArchive;
                }
                else
                {
                    MemoryStream stream3 = new MemoryStream();
                    BinaryReader reader = null;
                    try
                    {
                        archive.ExtractFile(entry.SubArchiveFileInfo, stream3);
                        previousNestedSubArchive = (IArchive) Activator.CreateInstance(entry.NestedSubArchivePluginType);
                        reader = new BinaryReader(stream3);
                        IOpenFromStream stream4 = (IOpenFromStream) previousNestedSubArchive;
                        stream3.Position = 0L;
                        stream4.Open(stream3, reader, fileName);
                    }
                    catch (Exception)
                    {
                        if (previousNestedSubArchive != null)
                        {
                            previousNestedSubArchive.Close();
                            previousNestedSubArchive = null;
                        }
                        if (reader != null)
                        {
                            reader.Close();
                            reader = null;
                        }
                        if (stream3 != null)
                        {
                            stream3.Close();
                            stream3 = null;
                        }
                        throw;
                    }
                    this.CloseNestedSubArchive();
                    this.previousNestedSubArchive = previousNestedSubArchive;
                    this.previousNestedSubArchiveStream = stream3;
                    this.previousNestedSubArchiveReader = reader;
                }
                if (byteCount < 0L)
                {
                    previousNestedSubArchive.ExtractFile(entry.NestedSubArchiveFileInfo, outputStream);
                }
                else
                {
                    previousNestedSubArchive.ExtractFile(entry.NestedSubArchiveFileInfo, outputStream, byteCount);
                }
            }
        }

        protected abstract AbstractorDefinition[] CreateDefinitions();
        private static bool DoesFilterMatch(string fileName, string filterPattern)
        {
            int length = filterPattern.LastIndexOf('.');
            string str = "";
            string str2 = "";
            if (length >= 0)
            {
                str = filterPattern.Substring(0, length);
                str2 = filterPattern.Substring(length);
            }
            else
            {
                str = filterPattern;
                str2 = "";
            }
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            string extension = Path.GetExtension(fileName);
            return (str2.Equals(extension, StringComparison.OrdinalIgnoreCase) && ((((str.Equals("*", StringComparison.OrdinalIgnoreCase) || ((str.StartsWith("*", StringComparison.OrdinalIgnoreCase) && (str.Length > 1)) && fileNameWithoutExtension.EndsWith(str.Substring(1), StringComparison.OrdinalIgnoreCase))) || ((str.EndsWith("*", StringComparison.OrdinalIgnoreCase) && (str.Length > 1)) && fileNameWithoutExtension.StartsWith(str.Substring(0, str.Length - 1), StringComparison.OrdinalIgnoreCase))) || ((str.Contains("*") && !str.StartsWith("*", StringComparison.OrdinalIgnoreCase)) && ((!str.EndsWith("*", StringComparison.OrdinalIgnoreCase) && fileNameWithoutExtension.StartsWith(str.Substring(0, str.IndexOf("*")), StringComparison.OrdinalIgnoreCase)) && fileNameWithoutExtension.EndsWith(str.Substring(str.IndexOf("*") + 1), StringComparison.OrdinalIgnoreCase)))) || ((!str.Contains("*") && !str.Contains("?")) && str.Equals(fileNameWithoutExtension))));
        }

        protected override void ExtractFile(IFileInfo file, Stream stream, Stream outputStream)
        {
            this.CopyData(this.directory[(int) file.ID], stream, outputStream, -1L);
        }

        protected override void ExtractFile(IFileInfo file, Stream stream, string outputFileName)
        {
            AbstractorDirectoryEntry entry = this.directory[(int) file.ID];
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

        private void LogAddStatistics(AddStatistics statistics, string description, long elapsedMilliseconds)
        {
            StringBuilder builder = new StringBuilder();
            if (string.IsNullOrEmpty(description))
            {
                description = "Items added";
            }
            builder.AppendFormat("{0}", description);
            builder.AppendFormat(", {0} ms", elapsedMilliseconds);
            builder.AppendFormat(", {0} added", statistics.Added);
            if (statistics.Replaced != 0)
            {
                builder.AppendFormat(", {0} replaced", statistics.Replaced);
            }
            if (statistics.AddedDuplicate != 0)
            {
                builder.AppendFormat(", {0} added as duplicate", statistics.AddedDuplicate);
            }
            if (statistics.AddedRenamed != 0)
            {
                builder.AppendFormat(", {0} added renamed", statistics.AddedRenamed);
            }
            if (statistics.Unknown != 0)
            {
                builder.AppendFormat(", {0} unknown", statistics.Unknown);
            }
            if (statistics.Removed != 0)
            {
                builder.AppendFormat(", {0} removed", statistics.Removed);
            }
            builder.ToString();
        }

        private void ModifyNewEntry(AbstractorDirectoryEntry entry, CasingRule fileNameCasing, bool unifyDirSeparator)
        {
            if (fileNameCasing == CasingRule.UpperCase)
            {
                entry.DisplayName = entry.DisplayName.ToUpper();
            }
            else if (fileNameCasing == CasingRule.LowerCase)
            {
                entry.DisplayName = entry.DisplayName.ToLower();
            }
            if (unifyDirSeparator)
            {
                entry.DisplayName = entry.DisplayName.Replace('/', Path.DirectorySeparatorChar);
                entry.DisplayName = entry.DisplayName.Replace('\\', Path.DirectorySeparatorChar);
            }
        }

        protected override void OnClose()
        {
            this.directory = null;
            this.CloseNestedSubArchive();
            if (this.previousSubArchives != null)
            {
                foreach (IArchive archive in this.previousSubArchives)
                {
                    archive.Close();
                }
                this.previousSubArchives.Clear();
                this.previousSubArchives = null;
            }
        }

        protected virtual void OnOpeningForDirectory(IArchive archive)
        {
        }

        protected virtual void OnOpeningForExtraction(IArchive archive)
        {
        }

        protected override void ReadDirectory(Stream stream, BinaryReader reader)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            this.directory = new List<AbstractorDirectoryEntry>();
            this.lookupTable = new Dictionary<string, int>();
            string directoryName = Path.GetDirectoryName(base.FileName);
            this.duplicatesPolicy = AbstractorDuplicatesPolicy.Keep;
            AbstractorDefinition[] definitionArray = this.CreateDefinitions();
            if (definitionArray == null)
            {
                throw new InvalidOperationException("No abstractor definitions were assigned.");
            }
            foreach (AbstractorDefinition definition in definitionArray)
            {
                string str4;
                string path = Path.GetDirectoryName(definition.SourceFile);
                string fileName = Path.GetFileName(definition.SourceFile);
                if (!Path.IsPathRooted(path))
                {
                    str4 = Path.Combine(directoryName, path);
                }
                else
                {
                    str4 = path;
                }
                string[] strArray = new string[0];
                if (Directory.Exists(str4))
                {
                    strArray = Directory.GetFiles(str4, fileName, definition.SearchOption);
                }
                if ((strArray.Length == 0) && !definition.Optional)
                {
                    throw new FileNotFoundException("No files found that match \"" + fileName + "\" in directory \"" + str4 + "\".");
                }
                AddStatistics statistics = null;
                foreach (string str5 in strArray)
                {
                    string str6 = Path.GetDirectoryName(str5.Substring(str4.Length + 1));
                    string str7 = Path.Combine(definition.TargetDirectory, str6);
                    string str8 = Path.Combine(str7, Path.GetFileName(str5));
                    if (definition is SubArchiveAbstractorDefinition)
                    {
                        string targetDirectory;
                        SubArchiveAbstractorDefinition definition2 = definition as SubArchiveAbstractorDefinition;
                        if (definition2.ListMode == AbstractorListMode.InTargetDirectory)
                        {
                            targetDirectory = definition2.TargetDirectory;
                        }
                        else if (definition2.ListMode == AbstractorListMode.InSubDirectory)
                        {
                            targetDirectory = Path.Combine(definition2.TargetDirectory, Path.GetFileName(str5));
                        }
                        else if (definition2.ListMode == AbstractorListMode.InSubDirectoryWithoutExtension)
                        {
                            targetDirectory = Path.Combine(definition2.TargetDirectory, Path.GetFileNameWithoutExtension(str5));
                        }
                        else
                        {
                            if (definition2.ListMode != AbstractorListMode.RetainSourceStructure)
                            {
                                throw new NotSupportedException("List mode \"" + definition2.ListMode.ToString() + "\" is not supported.");
                            }
                            targetDirectory = str7;
                        }
                        this.ReadSubArchive(str5, definition2.PluginTypes, targetDirectory, definition2.FileNameCasing, definition2.NestedSubArchives);
                    }
                    else
                    {
                        AbstractorDirectoryEntry entry = new AbstractorDirectoryEntry();
                        FileInfo info = new FileInfo(str5);
                        entry.SourceFSPath = str5;
                        entry.Size = info.Length;
                        entry.DisplayName = str8;
                        this.ModifyNewEntry(entry, definition.FileNameCasing, false);
                        if (definition is AbstractorRemoveDefinition)
                        {
                            bool flag = this.RemoveDirectoryEntry(entry);
                            if (statistics == null)
                            {
                                statistics = new AddStatistics();
                            }
                            if (flag)
                            {
                                statistics.UpdateRemoval();
                            }
                        }
                        else if (definition != null)
                        {
                            AddResult result = this.AddDirectoryEntry(entry);
                            if (statistics == null)
                            {
                                statistics = new AddStatistics();
                            }
                            statistics.Update(result);
                        }
                    }
                }
                if (statistics != null)
                {
                    this.LogAddStatistics(statistics, "Files from FS added", stopwatch.ElapsedMilliseconds);
                }
            }
            stopwatch.Stop();
        }

        private void ReadNestedSubArchive(string fileName, Type pluginType, IArchive archive, IFileInfo fileInfo, IList<Type> nestedPluginTypes, string targetDirectory, CasingRule fileNameCasing, AddStatistics statistics)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            AddStatistics statistics2 = new AddStatistics();
            MemoryStream outputStream = new MemoryStream();
            BinaryReader reader = null;
            try
            {
                archive.ExtractFile(fileInfo, outputStream);
                reader = new BinaryReader(outputStream);
                outputStream.Position = 0L;
                bool flag = false;
                foreach (Type type in nestedPluginTypes)
                {
                    if (!typeof(IArchive).IsAssignableFrom(type))
                    {
                        throw new ArgumentException("Type \"" + type.FullName + "\" does not implement the IArchive interface.");
                    }
                    if (!typeof(IOpenFromStream).IsAssignableFrom(type))
                    {
                        throw new ArgumentException("Type \"" + type.FullName + "\" does not implement the IOpenFromStream interface.");
                    }
                    IArchive archive2 = (IArchive) Activator.CreateInstance(type);
                    IOpenFromStream stream2 = (IOpenFromStream) archive2;
                    outputStream.Position = 0L;
                    if (stream2.IsValidFormat(outputStream, reader))
                    {
                        int count = nestedPluginTypes.Count;
                        flag = true;
                        outputStream.Position = 0L;
                        stream2.Open(outputStream, reader);
                        try
                        {
                            foreach (IFileInfo info in archive2.Files)
                            {
                                AbstractorDirectoryEntry entry = new AbstractorDirectoryEntry {
                                    SourceFSPath = fileName,
                                    DisplayName = Path.Combine(targetDirectory, info.Name),
                                    Size = info.Size,
                                    IsSubArchive = true,
                                    SubArchiveFileInfo = fileInfo,
                                    SubArchivePluginType = pluginType,
                                    NestedSubArchiveFileInfo = info,
                                    NestedSubArchivePluginType = type
                                };
                                this.ModifyNewEntry(entry, fileNameCasing, true);
                                AddResult result = this.AddDirectoryEntry(entry);
                                statistics.Update(result);
                                statistics2.Update(result);
                            }
                            break;
                        }
                        finally
                        {
                            archive2.Close();
                        }
                    }
                }
                if (!flag)
                {
                    throw new InvalidDataException("The nested archive \"" + fileInfo.Name + "\" is not in a valid format.");
                }
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                    reader = null;
                }
                if (outputStream != null)
                {
                    outputStream.Close();
                    outputStream = null;
                }
            }
            stopwatch.Stop();
            this.LogAddStatistics(statistics2, "Nested sub-archive read", stopwatch.ElapsedMilliseconds);
        }

        private void ReadSubArchive(string fileName, IList<Type> pluginTypes, string targetDirectory, CasingRule fileNameCasing, IList<SubArchiveAbstractorDefinition> nestedSubArchives)
        {
            IArchive archive = null;
            Type c = null;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            IEnumerator<Type> enumerator = pluginTypes.GetEnumerator();
            while (enumerator.MoveNext())
            {
                c = enumerator.Current;
                if (!typeof(IArchive).IsAssignableFrom(c))
                {
                    throw new ArgumentException("Type \"" + c.FullName + "\" does not implement the IArchive interface.");
                }
                archive = (IArchive) Activator.CreateInstance(c);
                if (!archive.IsValidFormat(fileName))
                {
                    archive = null;
                    c = null;
                }
                else
                {
                    if (pluginTypes.Count <= 1)
                    {
                    }
                    break;
                }
            }
            if ((archive == null) || (c == null))
            {
                throw new InvalidDataException("The archive \"" + fileName + "\" is not in a valid format.");
            }
            this.OnOpeningForDirectory(archive);
            AddStatistics statistics = new AddStatistics();
            archive.Open(fileName);
            try
            {
                foreach (IFileInfo info in archive.Files)
                {
                    bool flag = false;
                    foreach (SubArchiveAbstractorDefinition definition in nestedSubArchives)
                    {
                        if (DoesFilterMatch(info.Name, definition.SourceFile))
                        {
                            string str;
                            if (definition.ListMode == AbstractorListMode.InTargetDirectory)
                            {
                                str = Path.Combine(targetDirectory, definition.TargetDirectory);
                            }
                            else if (definition.ListMode == AbstractorListMode.InSubDirectory)
                            {
                                str = Path.Combine(targetDirectory, definition.TargetDirectory, Path.GetFileName(info.Name));
                            }
                            else if (definition.ListMode == AbstractorListMode.InSubDirectoryWithoutExtension)
                            {
                                str = Path.Combine(targetDirectory, definition.TargetDirectory, Path.GetFileNameWithoutExtension(info.Name));
                            }
                            else
                            {
                                if (definition.ListMode != AbstractorListMode.RetainSourceStructure)
                                {
                                    throw new NotSupportedException("List mode \"" + definition.ListMode.ToString() + "\" is not supported.");
                                }
                                str = targetDirectory;
                            }
                            this.ReadNestedSubArchive(fileName, c, archive, info, definition.PluginTypes, str, fileNameCasing, statistics);
                            flag = true;
                            break;
                        }
                    }
                    if (!flag)
                    {
                        AbstractorDirectoryEntry entry = new AbstractorDirectoryEntry {
                            SourceFSPath = fileName,
                            DisplayName = Path.Combine(targetDirectory, info.Name),
                            Size = info.Size,
                            IsSubArchive = true,
                            SubArchiveFileInfo = info,
                            SubArchivePluginType = c
                        };
                        this.ModifyNewEntry(entry, fileNameCasing, true);
                        AddResult result = this.AddDirectoryEntry(entry);
                        statistics.Update(result);
                    }
                }
            }
            finally
            {
                archive.Close();
            }
            stopwatch.Stop();
            this.LogAddStatistics(statistics, "Sub-archive read", stopwatch.ElapsedMilliseconds);
        }

        private bool RemoveDirectoryEntry(AbstractorDirectoryEntry entry)
        {
            if (this.lookupTable.ContainsKey(entry.DisplayName))
            {
                int num = this.lookupTable[entry.DisplayName];
                AbstractorDirectoryEntry local1 = this.directory[num];
                this.lookupTable.Remove(entry.DisplayName);
                this.directory[num] = null;
                return true;
            }
            return false;
        }

        public abstract string[] DefaultDirectories { get; }

        public abstract string DisplayFileName { get; }

        protected AbstractorDuplicatesPolicy DuplicatesPolicy
        {
            get
            {
                return this.duplicatesPolicy;
            }
            set
            {
                this.duplicatesPolicy = value;
            }
        }

        public override IFileInfo[] Files
        {
            get
            {
                ArchiveFileInfo[] infoArray = new ArchiveFileInfo[this.lookupTable.Count];
                int num = 0;
                for (int i = 0; i < this.directory.Count; i++)
                {
                    AbstractorDirectoryEntry entry = this.directory[i];
                    if (entry != null)
                    {
                        ArchiveFileInfo info = new ArchiveFileInfo((long) i, entry.DisplayName, entry.Size);
                        infoArray[num++] = info;
                    }
                }
                return infoArray;
            }
        }

        private enum AddResult
        {
            Added,
            Replaced,
            AddedDuplicate,
            AddedRenamed
        }

        private class AddStatistics
        {
            private int added = 0;
            private int addedDuplicate = 0;
            private int addedRenamed = 0;
            private int removed = 0;
            private int replaced = 0;
            private int unknown = 0;

            public void Update(AbstractorArchive.AddResult result)
            {
                if (result == AbstractorArchive.AddResult.Added)
                {
                    this.added++;
                }
                else if (result == AbstractorArchive.AddResult.Replaced)
                {
                    this.replaced++;
                }
                else if (result == AbstractorArchive.AddResult.AddedDuplicate)
                {
                    this.addedDuplicate++;
                }
                else if (result == AbstractorArchive.AddResult.AddedRenamed)
                {
                    this.addedRenamed++;
                }
                else
                {
                    this.unknown++;
                }
            }

            public void UpdateRemoval()
            {
                this.removed++;
            }

            public int Added
            {
                get
                {
                    return this.added;
                }
            }

            public int AddedDuplicate
            {
                get
                {
                    return this.addedDuplicate;
                }
            }

            public int AddedRenamed
            {
                get
                {
                    return this.addedRenamed;
                }
            }

            public int Removed
            {
                get
                {
                    return this.removed;
                }
            }

            public int Replaced
            {
                get
                {
                    return this.replaced;
                }
            }

            public int Unknown
            {
                get
                {
                    return this.unknown;
                }
            }
        }
    }
}

