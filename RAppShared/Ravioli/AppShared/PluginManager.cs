namespace Ravioli.AppShared
{
    using Ravioli.ArchiveInterface;
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public class PluginManager
    {
        protected object CreateInstance(Type pluginType)
        {
            return AppDomain.CurrentDomain.CreateInstanceAndUnwrap(pluginType.Assembly.FullName, pluginType.FullName);
        }

        private Assembly CurrentDomain_ReflectionOnlyAssemblyResolve(object sender, ResolveEventArgs args)
        {
            return Assembly.ReflectionOnlyLoad(args.Name);
        }

        protected string EnsurePeriod(string extension)
        {
            if (extension.StartsWith("."))
            {
                return extension;
            }
            return ("." + extension);
        }

        protected PluginMapping FindPlugin(string extension, IList<PluginMapping> mappings)
        {
            PluginMapping mapping = null;
            foreach (PluginMapping mapping2 in mappings)
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

        protected List<PluginMapping> LoadPluginInfoFromAssembly(string fileName, Type typeToLoad)
        {
            List<Type> list = this.LoadTypesFromAssembly(fileName, typeToLoad);
            List<PluginMapping> list2 = new List<PluginMapping>();
            foreach (Type type in list)
            {
                try
                {
                    list2.Add(this.ReadPluginInfo(type));
                }
                catch (Exception)
                {
                }
            }
            return list2;
        }

        protected List<Type> LoadTypesFromAssembly(string fileName, Type typeToLoad)
        {
            List<Type> list = new List<Type>();
            bool flag = false;
            try
            {
                if (FileTypeDetector.DetectFileType(fileName) == FileType.Managed)
                {
                    flag = true;
                }
            }
            catch (Exception)
            {
            }
            AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += new ResolveEventHandler(this.CurrentDomain_ReflectionOnlyAssemblyResolve);
            Assembly assembly = null;
            if (flag)
            {
                try
                {
                    assembly = Assembly.ReflectionOnlyLoadFrom(fileName);
                }
                catch (Exception)
                {
                }
            }
            Type[] exportedTypes = null;
            if (assembly != null)
            {
                try
                {
                    exportedTypes = assembly.GetExportedTypes();
                }
                catch (Exception)
                {
                }
            }
            if (exportedTypes != null)
            {
                foreach (Type type2 in exportedTypes)
                {
                    try
                    {
                        if (!type2.IsAbstract && (type2.GetInterface(typeToLoad.ToString()) != null))
                        {
                            list.Add(type2);
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve -= new ResolveEventHandler(this.CurrentDomain_ReflectionOnlyAssemblyResolve);
            return list;
        }

        private PluginMapping ReadPluginInfo(Type type)
        {
            IClassInfo info = (IClassInfo) this.CreateInstance(type);
            if ((string.IsNullOrEmpty(info.TypeName) || (info.Extensions == null)) || (info.Extensions.Length <= 0))
            {
                throw new ApplicationException("Plug-in doesn't have a type name or supported extensions.");
            }
            string[] extensions = new string[info.Extensions.Length];
            for (int i = 0; i < info.Extensions.Length; i++)
            {
                extensions[i] = info.Extensions[i];
            }
            return new PluginMapping(extensions, type, info.TypeName);
        }
    }
}

