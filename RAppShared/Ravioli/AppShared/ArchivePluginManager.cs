namespace Ravioli.AppShared
{
    using Ravioli.ArchiveInterface;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Text;

    public class ArchivePluginManager
    {
        private ArchivePluginMapping[] availableArchivePlugins;
        private GameViewerPluginMapping[] availableGameViewerPlugins;
        private string pluginDataDirectory;
        private string pluginDirectory;

        public ArchivePluginManager()
        {
            string location = Assembly.GetExecutingAssembly().Location;
            this.pluginDirectory = Path.Combine(Path.GetDirectoryName(location), "Plugins");
            this.pluginDataDirectory = Path.Combine(this.pluginDirectory, "PluginData");
        }

        public ArchiveBrowseFilter CreateBrowseFilter()
        {
            List<string> list = new List<string>();
            StringBuilder builder = new StringBuilder();
            StringBuilder builder2 = new StringBuilder();
            ArchivePluginMapping[] availableArchivePlugins = this.availableArchivePlugins;
            for (int i = 0; i < availableArchivePlugins.Length; i++)
            {
                PluginMapping mapping = availableArchivePlugins[i];
                StringBuilder builder3 = new StringBuilder();
                for (int j = 0; j < mapping.Extensions.Length; j++)
                {
                    string extension = mapping.Extensions[j];
                    if (j > 0)
                    {
                        builder3.Append(";");
                    }
                    builder3.Append("*");
                    builder3.Append(this.EnsurePeriod(extension));
                }
                builder.Append(string.Concat(new object[] { mapping.TypeName, " (", builder3, ")|", builder3, "|" }));
                list.Add(FormatFilterEntry(mapping.TypeName, builder3.ToString()));
                if (builder2.Length > 0)
                {
                    builder2.Append(";");
                }
                builder2.Append(builder3);
            }
            builder.Append("All Files (*.*)|*.*");
            list.Add(FormatFilterEntry("All Files", "*.*"));
            bool flag = false;
            if (builder2.Length > 0)
            {
                builder.Insert(0, "All supported types (*....)|" + builder2.ToString() + "|");
                list.Insert(0, FormatFilterEntry2("All supported types (*....)", builder2.ToString()));
                flag = true;
            }
            ArchiveBrowseFilter filter = new ArchiveBrowseFilter();
            StringBuilder builder4 = new StringBuilder();
            foreach (string str2 in list)
            {
                if (builder4.Length > 0)
                {
                    builder4.Append("|");
                }
                builder4.Append(str2);
            }
            filter.FilterString = builder4.ToString();
            filter.KnownTypesIndex = flag ? 1 : list.Count;
            filter.AllFilesIndex = list.Count;
            builder.ToString();
            return filter;
        }

        public IArchive CreateInstance(ArchivePluginMapping plugin)
        {
            return this.CreateInstance(plugin.PluginType);
        }

        public IArchive CreateInstance(GameViewerPluginMapping plugin)
        {
            return this.CreateInstance(plugin.PluginType);
        }

        private IArchive CreateInstance(Type pluginType)
        {
            return (IArchive) AppDomain.CurrentDomain.CreateInstanceAndUnwrap(pluginType.Assembly.FullName, pluginType.FullName);
        }

        private Assembly CurrentDomain_ReflectionOnlyAssemblyResolve(object sender, ResolveEventArgs args)
        {
            return Assembly.ReflectionOnlyLoad(args.Name);
        }

        public string EnsurePeriod(string extension)
        {
            if (extension.StartsWith("."))
            {
                return extension;
            }
            return ("." + extension);
        }

        public void EnumeratePlugins()
        {
            Type type = typeof(IArchive);
            Type type2 = typeof(IGameViewer);
            List<ArchivePluginMapping> list = new List<ArchivePluginMapping>();
            List<GameViewerPluginMapping> list2 = new List<GameViewerPluginMapping>();
            if (Directory.Exists(this.pluginDirectory))
            {
                string[] files = Directory.GetFiles(this.pluginDirectory, "*.dll");
                AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += new ResolveEventHandler(this.CurrentDomain_ReflectionOnlyAssemblyResolve);
                foreach (string str in files)
                {
                    Assembly assembly;
                    Type[] exportedTypes;
                    try
                    {
                        if (FileTypeDetector.DetectFileType(str) != FileType.Managed)
                        {
                            continue;
                        }
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                    try
                    {
                        assembly = Assembly.ReflectionOnlyLoadFrom(str);
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                    try
                    {
                        exportedTypes = assembly.GetExportedTypes();
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                    foreach (Type type4 in exportedTypes)
                    {
                        try
                        {
                            if (!type4.IsAbstract)
                            {
                                if (type4.GetInterface(type2.Name) != null)
                                {
                                    list2.Add(this.ReadGameViewerPluginInfo(type4));
                                }
                                else if (type4.GetInterface(type.Name) != null)
                                {
                                    list.Add(this.ReadArchivePluginInfo(type4));
                                }
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
                AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve -= new ResolveEventHandler(this.CurrentDomain_ReflectionOnlyAssemblyResolve);
                list.Sort(new Comparison<ArchivePluginMapping>(PluginMapping.CompareByTypeName));
                list2.Sort(new Comparison<GameViewerPluginMapping>(PluginMapping.CompareByTypeName));
            }
            this.availableArchivePlugins = list.ToArray();
            this.availableGameViewerPlugins = list2.ToArray();
        }

        public GameViewerPluginMapping[] FindGameViewerPlugins(string directory)
        {
            List<GameViewerPluginMapping> list = new List<GameViewerPluginMapping>();
            foreach (GameViewerPluginMapping mapping in this.availableGameViewerPlugins)
            {
                foreach (string str in mapping.DefaultDirectories)
                {
                    string str2 = Path.DirectorySeparatorChar.ToString();
                    string strA = str + (str.EndsWith(str2) ? string.Empty : str2);
                    string strB = directory + (directory.EndsWith(str2) ? string.Empty : str2);
                    if ((string.Compare(strA, strB, true) == 0) && Directory.Exists(str))
                    {
                        list.Add(mapping);
                        break;
                    }
                }
            }
            return list.ToArray();
        }

        public ArchivePluginMapping FindPluginByName(string name)
        {
            ArchivePluginMapping mapping = null;
            foreach (ArchivePluginMapping mapping2 in this.availableArchivePlugins)
            {
                if (string.Compare(mapping2.TypeName, name, true) == 0)
                {
                    mapping = mapping2;
                    break;
                }
            }
            if (mapping == null)
            {
                foreach (GameViewerPluginMapping mapping3 in this.availableGameViewerPlugins)
                {
                    if (string.Compare(mapping3.TypeName, name, true) == 0)
                    {
                        return mapping3;
                    }
                }
            }
            return mapping;
        }

        public ArchivePluginMapping[] FindPlugins(string extension)
        {
            List<ArchivePluginMapping> list = new List<ArchivePluginMapping>();
            foreach (ArchivePluginMapping mapping in this.availableArchivePlugins)
            {
                if (mapping.ExtensionIndependent)
                {
                    list.Add(mapping);
                }
                else
                {
                    foreach (string str in mapping.Extensions)
                    {
                        if (string.Compare(this.EnsurePeriod(str), this.EnsurePeriod(extension), true) == 0)
                        {
                            list.Add(mapping);
                            break;
                        }
                    }
                }
            }
            ArchivePluginMapping[] array = new ArchivePluginMapping[list.Count];
            list.CopyTo(array);
            return array;
        }

        private static string FormatFilterEntry(string typeName, string filterPattern)
        {
            return string.Format("{0} ({1})|{2}", typeName, filterPattern, filterPattern);
        }

        private static string FormatFilterEntry2(string filterDescription, string filterPattern)
        {
            return string.Format("{0}|{1}", filterDescription, filterPattern);
        }

        private ArchivePluginMapping ReadArchivePluginInfo(Type type)
        {
            IClassInfo info = this.CreateInstance(type);
            if ((string.IsNullOrEmpty(info.TypeName) || (info.Extensions == null)) || (info.Extensions.Length <= 0))
            {
                throw new ApplicationException("Plug-in doesn't have a type name or supported extensions.");
            }
            string[] extensions = new string[info.Extensions.Length];
            for (int i = 0; i < info.Extensions.Length; i++)
            {
                extensions[i] = info.Extensions[i];
            }
            bool needsRootDirectory = info is IRootDirectory;
            IExtensionIndependent independent = info as IExtensionIndependent;
            return new ArchivePluginMapping(extensions, type, info.TypeName, needsRootDirectory, (independent != null) && independent.ExtensionIndependent);
        }

        private GameViewerPluginMapping ReadGameViewerPluginInfo(Type type)
        {
            IGameViewer viewer = (IGameViewer) this.CreateInstance(type);
            if (string.IsNullOrEmpty(viewer.TypeName))
            {
                throw new ApplicationException("Plug-in doesn't have a type name.");
            }
            string[] extensions = null;
            if ((viewer.Extensions != null) && (viewer.Extensions.Length > 0))
            {
                extensions = new string[viewer.Extensions.Length];
                for (int i = 0; i < viewer.Extensions.Length; i++)
                {
                    extensions[i] = viewer.Extensions[i];
                }
            }
            return new GameViewerPluginMapping(extensions, type, viewer.TypeName, viewer.DefaultDirectories);
        }

        public ArchivePluginMapping[] AvailableArchiveAndGameViewerPlugins
        {
            get
            {
                ArchivePluginMapping[] destinationArray = new ArchivePluginMapping[this.availableArchivePlugins.Length + this.availableGameViewerPlugins.Length];
                Array.Copy(this.availableArchivePlugins, destinationArray, this.availableArchivePlugins.Length);
                Array.Copy(this.availableGameViewerPlugins, 0, destinationArray, this.availableArchivePlugins.Length, this.availableGameViewerPlugins.Length);
                return destinationArray;
            }
        }

        public ArchivePluginMapping[] AvailableArchivePlugins
        {
            get
            {
                return this.availableArchivePlugins;
            }
        }

        public GameViewerPluginMapping[] AvailableGameViewerPlugins
        {
            get
            {
                return this.availableGameViewerPlugins;
            }
        }

        public string PluginDataDirectory
        {
            get
            {
                return this.pluginDataDirectory;
            }
        }

        public string PluginDirectory
        {
            get
            {
                return this.pluginDirectory;
            }
        }
    }
}

