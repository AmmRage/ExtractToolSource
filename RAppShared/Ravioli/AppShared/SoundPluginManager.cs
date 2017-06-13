namespace Ravioli.AppShared
{
    using Ravioli.ArchiveInterface;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Text;

    public class SoundPluginManager : PluginManager
    {
        private SoundExportFormat[] availableExportFormats;
        private PluginMapping[] availablePlayers;
        private string pluginDirectory;

        public SoundPluginManager()
        {
            string location = Assembly.GetExecutingAssembly().Location;
            this.pluginDirectory = Path.Combine(Path.GetDirectoryName(location), "Plugins");
        }

        public string CreateExportFilter(SoundExportFormat[] formats)
        {
            StringBuilder builder = new StringBuilder();
            foreach (SoundExportFormat format in formats)
            {
                if (builder.Length > 0)
                {
                    builder.Append("|");
                }
                StringBuilder builder2 = new StringBuilder();
                builder2.Append("*");
                if (!format.Extension.StartsWith("."))
                {
                    builder2.Append(".");
                }
                builder2.Append(format.Extension);
                builder.AppendFormat("{0}|{1}", format.Name, builder2.ToString());
            }
            return builder.ToString();
        }

        public ISoundPlayer CreatePlayerInstance(PluginMapping plugin)
        {
            return (ISoundPlayer) base.CreateInstance(plugin.PluginType);
        }

        public new string EnsurePeriod(string extension)
        {
            return base.EnsurePeriod(extension);
        }

        public void EnumeratePlugins()
        {
            List<PluginMapping> list = new List<PluginMapping>();
            List<SoundExportFormat> list2 = new List<SoundExportFormat>();
            if (Directory.Exists(this.pluginDirectory))
            {
                foreach (string str in Directory.GetFiles(this.pluginDirectory, "*.dll"))
                {
                    list.AddRange(base.LoadPluginInfoFromAssembly(str, typeof(ISoundPlayer)));
                    foreach (Type type in base.LoadTypesFromAssembly(str, typeof(ISoundExport)))
                    {
                        try
                        {
                            foreach (SoundExportFormat format in this.ReadSoundExportFormats(type))
                            {
                                if (!list2.Contains(format))
                                {
                                    list2.Add(format);
                                }
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
                list.Sort(new Comparison<PluginMapping>(PluginMapping.CompareByTypeName));
                list2.Sort(new Comparison<SoundExportFormat>(SoundExportFormat.CompareByName));
            }
            this.availablePlayers = list.ToArray();
            this.availableExportFormats = list2.ToArray();
        }

        public SoundExportFormat FindExportFormat(string extension)
        {
            foreach (SoundExportFormat format2 in this.availableExportFormats)
            {
                string str = format2.Extension;
                if (string.Compare(this.EnsurePeriod(str), this.EnsurePeriod(extension), true) == 0)
                {
                    return format2;
                }
            }
            return null;
        }

        public PluginMapping FindPlayerPlugin(string extension)
        {
            return base.FindPlugin(extension, this.availablePlayers);
        }

        private SoundExportFormat[] ReadSoundExportFormats(Type type)
        {
            ISoundExport export = (ISoundExport) base.CreateInstance(type);
            if ((export.SupportedExportFormats == null) || (export.SupportedExportFormats.Length <= 0))
            {
                throw new ApplicationException("Plug-in doesn't have any supported export formats.");
            }
            return export.SupportedExportFormats;
        }

        public SoundExportFormat[] AvailableExportFormats
        {
            get
            {
                return this.availableExportFormats;
            }
        }

        public PluginMapping[] AvailablePlayers
        {
            get
            {
                return this.availablePlayers;
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

