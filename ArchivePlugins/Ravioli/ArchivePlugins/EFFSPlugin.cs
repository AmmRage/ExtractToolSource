namespace Ravioli.ArchivePlugins
{
    using Ravioli.ArchiveInterface;
    using Ravioli.GeneralArchivePlugins;
    using Ravioli.PluginHelpers;
    using System;
    using System.Collections.Generic;
    using System.IO;

    public class EFFSPlugin : AbstractorArchive
    {
        protected override AbstractorDefinition[] CreateDefinitions()
        {
            Type pluginType = typeof(ZipFile);
            base.DuplicatesPolicy = AbstractorDuplicatesPolicy.Replace;
            List<AbstractorDefinition> list = new List<AbstractorDefinition>();
            string[] files = Directory.GetFiles(Path.Combine(Path.GetDirectoryName(base.FileName), "BaseEF"), "pak*.pk3");
            Array.Sort<string>(files);
            foreach (string str3 in files)
            {
                list.Add(new SubArchiveAbstractorDefinition(str3, pluginType));
            }
            return list.ToArray();
        }

        protected override bool IsValidFormat(Stream stream, BinaryReader reader)
        {
            string directoryName = Path.GetDirectoryName(base.FileName);
            bool flag = false;
            string path = Path.Combine(directoryName, "BaseEF");
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
                return SystemSearcher.Combine3264ProgramFiles(@"Raven\Star Trek Voyager Elite Force");
            }
        }

        public override string DisplayFileName
        {
            get
            {
                return "Star Trek: Voyager - Elite Force";
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
                return "Star Trek: Voyager - Elite Force";
            }
        }
    }
}

