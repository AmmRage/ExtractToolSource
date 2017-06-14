namespace Ravioli.ArchivePlugins
{
    using Ravioli.ArchiveInterface;
    using Ravioli.GeneralArchivePlugins;
    using Ravioli.PluginHelpers;
    using System;
    using System.Collections.Generic;
    using System.IO;

    public class EF2FSPlugin : AbstractorArchive
    {
        protected override AbstractorDefinition[] CreateDefinitions()
        {
            Type pluginType = typeof(ZipFile);
            base.DuplicatesPolicy = AbstractorDuplicatesPolicy.Replace;
            List<AbstractorDefinition> list = new List<AbstractorDefinition>();
            string path = Path.Combine(Path.GetDirectoryName(base.FileName), "base");
            string[] files = Directory.GetFiles(path, "pak*.pk3");
            Array.Sort<string>(files);
            foreach (string str3 in files)
            {
                list.Add(new SubArchiveAbstractorDefinition(str3, pluginType));
            }
            string[] array = Directory.GetFiles(path, "locpak*.pk3");
            Array.Sort<string>(array);
            foreach (string str4 in array)
            {
                list.Add(new SubArchiveAbstractorDefinition(str4, pluginType));
            }
            list.Add(new SubArchiveAbstractorDefinition(@"base\hm_*.pk3", pluginType));
            list.Add(new SubArchiveAbstractorDefinition(@"base\dm_*.pk3", pluginType));
            list.Add(new SubArchiveAbstractorDefinition(@"base\ctf_*.pk3", pluginType));
            return list.ToArray();
        }

        protected override bool IsValidFormat(Stream stream, BinaryReader reader)
        {
            string directoryName = Path.GetDirectoryName(base.FileName);
            bool flag = false;
            string path = Path.Combine(directoryName, "base");
            if (Directory.Exists(path))
            {
                flag = Directory.GetFiles(path, "*.pk3").Length > 0;
            }
            return flag;
        }

        public override string[] DefaultDirectories
        {
            get
            {
                return SystemSearcher.Combine3264ProgramFiles(@"Activision\EF2");
            }
        }

        public override string DisplayFileName
        {
            get
            {
                return "Star Trek: Elite Force II";
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
                return "Star Trek: Elite Force II";
            }
        }
    }
}

