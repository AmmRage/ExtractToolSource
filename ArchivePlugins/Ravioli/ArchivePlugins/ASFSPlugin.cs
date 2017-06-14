namespace Ravioli.ArchivePlugins
{
    using Ravioli.ArchiveInterface;
    using Ravioli.PluginHelpers;
    using System;
    using System.Collections.Generic;
    using System.IO;

    public class ASFSPlugin : AbstractorArchive
    {
        protected override AbstractorDefinition[] CreateDefinitions()
        {
            Type pluginType = typeof(CgrFile);
            return new List<AbstractorDefinition> { new SubArchiveAbstractorDefinition(@"engine\*.cgr", pluginType, string.Empty, 1, 3), new AbstractorDefinition(@"engine\*.ogg", string.Empty, 1), new AbstractorDefinition(@"engine\*.mp3", string.Empty, 1, 1), new AbstractorDefinition(@"engine\textures\*.*", "textures"), new AbstractorDefinition(@"engine\video\*.*", "video") }.ToArray();
        }

        protected override bool IsValidFormat(Stream stream, BinaryReader reader)
        {
            string directoryName = Path.GetDirectoryName(base.FileName);
            string path = Path.Combine(directoryName, "engine");
            return ((File.Exists(Path.Combine(directoryName, "Audiosurf.exe")) && Directory.Exists(path)) && (Directory.GetFiles(path, "*.cgr").Length > 0));
        }

        public override string[] DefaultDirectories
        {
            get
            {
                return new string[] { Path.Combine(SystemSearcher.SteamAppsCommonDirectory, "audiosurf") };
            }
        }

        public override string DisplayFileName
        {
            get
            {
                return "Audiosurf";
            }
        }

        public override string[] Extensions
        {
            get
            {
                return null;
            }
        }

        public override string TypeName
        {
            get
            {
                return "Audiosurf";
            }
        }
    }
}

