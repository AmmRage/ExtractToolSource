namespace Ravioli.AppShared
{
    using Ravioli.ArchiveInterface;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Text;

    public class ImagePluginManager
    {
        private ImagePluginMapping[] availableLoaderPlugins;
        private ImagePluginMapping[] availableSaverPlugins;
        private string pluginDirectory;

        public ImagePluginManager()
        {
            string location = Assembly.GetExecutingAssembly().Location;
            this.pluginDirectory = Path.Combine(Path.GetDirectoryName(location), "Plugins");
        }

        public IImageLoader CreateLoaderInstance(ImagePluginMapping plugin)
        {
            return this.CreateLoaderInstance(plugin.PluginType);
        }

        private IImageLoader CreateLoaderInstance(Type pluginType)
        {
            return (IImageLoader) AppDomain.CurrentDomain.CreateInstanceAndUnwrap(pluginType.Assembly.FullName, pluginType.ToString());
        }

        private string CreateSaveBrowseFilter(IList<ImagePluginMapping> mappings)
        {
            StringBuilder builder = new StringBuilder();
            foreach (PluginMapping mapping in mappings)
            {
                StringBuilder builder2 = new StringBuilder();
                for (int i = 0; i < mapping.Extensions.Length; i++)
                {
                    string extension = mapping.Extensions[i];
                    if (i > 0)
                    {
                        builder2.Append(";");
                    }
                    builder2.Append("*");
                    builder2.Append(this.EnsurePeriod(extension));
                }
                if (builder.Length > 0)
                {
                    builder.Append("|");
                }
                builder.Append(string.Concat(new object[] { mapping.TypeName, " (", builder2, ")|", builder2 }));
            }
            return builder.ToString();
        }

        public string CreateSaverFilter()
        {
            return this.CreateSaveBrowseFilter(this.availableSaverPlugins);
        }

        public IImageSaver CreateSaverInstance(ImagePluginMapping plugin)
        {
            return this.CreateSaverInstance(plugin.PluginType);
        }

        private IImageSaver CreateSaverInstance(Type pluginType)
        {
            return (IImageSaver) AppDomain.CurrentDomain.CreateInstanceAndUnwrap(pluginType.Assembly.FullName, pluginType.FullName);
        }

        private Assembly CurrentDomain_ReflectionOnlyAssemblyResolve(object sender, ResolveEventArgs args)
        {
            return Assembly.ReflectionOnlyLoad(args.Name);
        }

        public ImagePluginMapping DetectImageFormat(IFileInfo file, Stream stream)
        {
            string extension = Path.GetExtension(file.Name);
            ImagePluginMapping item = this.FindLoaderPlugin(extension);
            ImagePluginMapping mapping2 = null;
            bool flag = false;
            if (stream != null)
            {
                List<ImagePluginMapping> list = new List<ImagePluginMapping>();
                if (item != null)
                {
                    list.Add(item);
                }
                list.AddRange(this.AvailableLoaderPlugins);
                long position = stream.Position;
                foreach (ImagePluginMapping mapping3 in list)
                {
                    IFormatCheck check = this.CreateLoaderInstance(mapping3) as IFormatCheck;
                    if (check != null)
                    {
                        if (mapping3 == item)
                        {
                            flag = true;
                        }
                        bool flag2 = check.IsValidFormat(stream);
                        stream.Position = position;
                        if (flag2)
                        {
                            mapping2 = mapping3;
                            break;
                        }
                    }
                }
            }
            ImagePluginMapping mapping4 = null;
            if (mapping2 != null)
            {
                return mapping2;
            }
            if (((item == null) || (stream != null)) && (((item == null) || (stream == null)) || flag))
            {
                return mapping4;
            }
            return item;
        }

        public ImagePluginMapping DetectImageFormat(IFileInfo file, string filePath)
        {
            ImagePluginMapping mapping;
            FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            try
            {
                mapping = this.DetectImageFormat(file, stream);
            }
            finally
            {
                stream.Close();
            }
            return mapping;
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
            List<ImagePluginMapping> list = new List<ImagePluginMapping>();
            List<ImagePluginMapping> list2 = new List<ImagePluginMapping>();
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
                    foreach (Type type2 in exportedTypes)
                    {
                        try
                        {
                            if (!type2.IsAbstract)
                            {
                                if (type2.GetInterface(typeof(IImageLoader).ToString()) != null)
                                {
                                    list.Add(this.ReadPluginInfo(type2));
                                }
                                if (type2.GetInterface(typeof(IImageSaver).ToString()) != null)
                                {
                                    list2.Add(this.ReadPluginInfo(type2));
                                }
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
                AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve -= new ResolveEventHandler(this.CurrentDomain_ReflectionOnlyAssemblyResolve);
                list.Sort(new Comparison<ImagePluginMapping>(PluginMapping.CompareByTypeName));
                list2.Sort(new Comparison<ImagePluginMapping>(PluginMapping.CompareByTypeName));
            }
            this.availableLoaderPlugins = list.ToArray();
            this.availableSaverPlugins = list2.ToArray();
        }

        public ImagePluginMapping FindLoaderPlugin(string extension)
        {
            return this.FindPlugin(extension, this.availableLoaderPlugins);
        }

        private ImagePluginMapping FindPlugin(string extension, IList<ImagePluginMapping> mappings)
        {
            ImagePluginMapping mapping = null;
            foreach (ImagePluginMapping mapping2 in mappings)
            {
                foreach (string str in mapping2.Extensions)
                {
                    if (string.Compare(this.EnsurePeriod(str), this.EnsurePeriod(extension), true) == 0)
                    {
                        mapping = mapping2;
                        break;
                    }
                }
                if (mapping != null)
                {
                    return mapping;
                }
            }
            return mapping;
        }

        public ImagePluginMapping FindSaverPlugin(string extension)
        {
            return this.FindPlugin(extension, this.availableSaverPlugins);
        }

        public bool IsSameFormat(ImagePluginMapping loaderMapping, IImageSaver saver)
        {
            bool flag = false;
            if (((loaderMapping != null) && (saver != null)) && (loaderMapping.PluginType.AssemblyQualifiedName == saver.GetType().AssemblyQualifiedName))
            {
                flag = true;
            }
            return flag;
        }

        public bool IsSameFormat(IImageLoader loader, IImageSaver saver)
        {
            bool flag = false;
            if (((loader != null) && (saver != null)) && (loader.GetType().AssemblyQualifiedName == saver.GetType().AssemblyQualifiedName))
            {
                flag = true;
            }
            return flag;
        }

        private ImagePluginMapping ReadPluginInfo(Type type)
        {
            IClassInfo info = this.CreateLoaderInstance(type);
            if ((string.IsNullOrEmpty(info.TypeName) || (info.Extensions == null)) || (info.Extensions.Length <= 0))
            {
                throw new ApplicationException("Plug-in doesn't have a type name or supported extensions.");
            }
            string[] extensions = new string[info.Extensions.Length];
            for (int i = 0; i < info.Extensions.Length; i++)
            {
                extensions[i] = info.Extensions[i];
            }
            return new ImagePluginMapping(extensions, type, info.TypeName);
        }

        public ImagePluginMapping[] AvailableLoaderPlugins
        {
            get
            {
                return this.availableLoaderPlugins;
            }
        }

        public ImagePluginMapping[] AvailableSaverPlugins
        {
            get
            {
                return this.availableSaverPlugins;
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

