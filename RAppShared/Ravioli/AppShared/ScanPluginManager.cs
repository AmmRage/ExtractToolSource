namespace Ravioli.AppShared
{
    using Ravioli.ArchiveInterface.Scanning;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;

    public class ScanPluginManager : PluginManager
    {
        private PluginMapping[] availableDetectors;
        private string pluginDirectory;

        public ScanPluginManager()
        {
            string location = Assembly.GetExecutingAssembly().Location;
            this.pluginDirectory = Path.Combine(Path.GetDirectoryName(location), "Plugins");
        }

        public IFileDetector[] CreateAllDetectors()
        {
            List<IFileDetector> list = new List<IFileDetector>();
            if (this.availableDetectors != null)
            {
                foreach (PluginMapping mapping in this.availableDetectors)
                {
                    list.Add(this.CreateDetectorInstance(mapping));
                }
            }
            return list.ToArray();
        }

        public IFileDetector CreateDetectorInstance(PluginMapping plugin)
        {
            return (IFileDetector) base.CreateInstance(plugin.PluginType);
        }

        public new string EnsurePeriod(string extension)
        {
            return base.EnsurePeriod(extension);
        }

        public void EnumeratePlugins()
        {
            List<PluginMapping> list = new List<PluginMapping>();
            if (Directory.Exists(this.pluginDirectory))
            {
                foreach (string str in Directory.GetFiles(this.pluginDirectory, "*.dll"))
                {
                    list.AddRange(base.LoadPluginInfoFromAssembly(str, typeof(IFileDetector)));
                }
                list.Sort(new Comparison<PluginMapping>(PluginMapping.CompareByTypeName));
            }
            this.availableDetectors = list.ToArray();
        }

        public PluginMapping FindDetectorPlugin(string extension)
        {
            return base.FindPlugin(extension, this.availableDetectors);
        }

        public PluginMetadataWithCategory[] GetDetectorMetadata()
        {
            IFileDetector[] detectorArray = this.CreateAllDetectors();
            PluginMetadataWithCategory category = new PluginMetadataWithCategory {
                Category = "Image Detectors"
            };
            PluginMetadataWithCategory category2 = new PluginMetadataWithCategory {
                Category = "Audio Detectors"
            };
            PluginMetadataWithCategory category3 = new PluginMetadataWithCategory {
                Category = "Video Detectors"
            };
            PluginMetadataWithCategory category4 = new PluginMetadataWithCategory {
                Category = "Container Detectors"
            };
            PluginMetadataWithCategory item = new PluginMetadataWithCategory {
                Category = "Other Detectors"
            };
            foreach (IFileDetector detector in detectorArray)
            {
                for (int i = 0; i < detector.FileTypes.Length; i++)
                {
                    PluginMetadata metadata = new PluginMetadata();
                    Ravioli.ArchiveInterface.Scanning.FileType type = detector.FileTypes[i];
                    metadata.Name = type.TypeName;
                    metadata.Extensions = new string[] { type.Extension };
                    metadata.File = Path.GetFileName(detector.GetType().Assembly.Location);
                    switch (type.PerceivedType)
                    {
                        case PerceivedType.Image:
                            category.Data.Add(metadata);
                            break;

                        case PerceivedType.Audio:
                            category2.Data.Add(metadata);
                            break;

                        case PerceivedType.Video:
                            category3.Data.Add(metadata);
                            break;

                        case PerceivedType.Container:
                            category4.Data.Add(metadata);
                            break;

                        default:
                            item.Data.Add(metadata);
                            break;
                    }
                }
            }
            List<PluginMetadataWithCategory> list = new List<PluginMetadataWithCategory>(new PluginMetadataWithCategory[] { category, category2, category3, category4 });
            if (item.Data.Count > 0)
            {
                list.Add(item);
            }
            foreach (PluginMetadataWithCategory category6 in list)
            {
                category6.Data.Sort(new Comparison<PluginMetadata>(PluginMetadata.CompareByName));
            }
            return list.ToArray();
        }

        public PluginMapping[] AvailableDetectors
        {
            get
            {
                return this.availableDetectors;
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

