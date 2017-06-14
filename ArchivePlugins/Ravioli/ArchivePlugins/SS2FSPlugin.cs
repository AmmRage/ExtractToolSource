namespace Ravioli.ArchivePlugins
{
    using Ravioli.ArchiveInterface;
    using Ravioli.GeneralArchivePlugins;
    using System;
    using System.Collections.Generic;
    using System.IO;

    public class SS2FSPlugin : AbstractorArchive
    {
        protected override AbstractorDefinition[] CreateDefinitions()
        {
            Type pluginType = typeof(ZipFile);
            string directoryName = Path.GetDirectoryName(base.FileName);
            Path.Combine(directoryName, "Data");
            List<AbstractorDefinition> list = new List<AbstractorDefinition>();
            string path = Path.Combine(directoryName, "res");
            Path.Combine(directoryName, "patch");
            base.DuplicatesPolicy = AbstractorDuplicatesPolicy.Replace;
            if (Directory.GetFiles(directoryName, "*.crf").Length > 0)
            {
                list.Add(new SubArchiveAbstractorDefinition("*.crf", pluginType, "", SearchOption.TopDirectoryOnly, AbstractorListMode.InSubDirectoryWithoutExtension, false, CasingRule.LowerCase));
            }
            else if (Directory.Exists(path) && (Directory.GetFiles(path, "*.crf").Length > 0))
            {
                list.Add(new SubArchiveAbstractorDefinition(@"res\*.crf", pluginType, "", SearchOption.TopDirectoryOnly, AbstractorListMode.InSubDirectoryWithoutExtension, false, CasingRule.LowerCase));
                list.Add(new AbstractorDefinition(@"bitmap\*.*", "bitmap", SearchOption.AllDirectories, true, CasingRule.LowerCase));
                list.Add(new AbstractorDefinition(@"book\*.*", "book", SearchOption.AllDirectories, true, CasingRule.LowerCase));
                list.Add(new AbstractorDefinition(@"editor\*.*", "editor", SearchOption.AllDirectories, true, CasingRule.LowerCase));
                list.Add(new AbstractorDefinition(@"fam\*.*", "fam", SearchOption.AllDirectories, true, CasingRule.LowerCase));
                list.Add(new AbstractorDefinition(@"fonts\*.*", "fonts", SearchOption.AllDirectories, true, CasingRule.LowerCase));
                list.Add(new AbstractorDefinition(@"iface\*.*", "iface", SearchOption.AllDirectories, true, CasingRule.LowerCase));
                list.Add(new AbstractorDefinition(@"intrface\*.*", "intrface", SearchOption.AllDirectories, true, CasingRule.LowerCase));
                list.Add(new AbstractorDefinition(@"mesh\*.*", "mesh", SearchOption.AllDirectories, true, CasingRule.LowerCase));
                list.Add(new AbstractorDefinition(@"motions\*.*", "motions", SearchOption.AllDirectories, true, CasingRule.LowerCase));
                list.Add(new AbstractorDefinition(@"obj\*.*", "obj", SearchOption.AllDirectories, true, CasingRule.LowerCase));
                list.Add(new AbstractorDefinition(@"objicon\*.*", "objicon", SearchOption.AllDirectories, true, CasingRule.LowerCase));
                list.Add(new AbstractorDefinition(@"pal\*.*", "pad", SearchOption.AllDirectories, true, CasingRule.LowerCase));
                list.Add(new AbstractorDefinition(@"snd\*.*", "snd", SearchOption.AllDirectories, true, CasingRule.LowerCase));
                list.Add(new AbstractorDefinition(@"snd2\*.*", "snd2", SearchOption.AllDirectories, true, CasingRule.LowerCase));
                list.Add(new AbstractorDefinition(@"song\*.*", "song", SearchOption.AllDirectories, true, CasingRule.LowerCase));
                list.Add(new AbstractorDefinition(@"strings\*.*", "strings", SearchOption.AllDirectories, true, CasingRule.LowerCase));
            }
            list.Add(new AbstractorDefinition(@"cutscenes\*.avi", "cutscenes", SearchOption.TopDirectoryOnly, false, CasingRule.LowerCase));
            list.Add(new SubArchiveAbstractorDefinition(@"patch\*.crf", pluginType, "", SearchOption.TopDirectoryOnly, AbstractorListMode.InSubDirectoryWithoutExtension, true, CasingRule.LowerCase));
            return list.ToArray();
        }

        protected override bool IsValidFormat(Stream stream, BinaryReader reader)
        {
            string directoryName = Path.GetDirectoryName(base.FileName);
            bool flag = false;
            if (File.Exists(Path.Combine(directoryName, "Shock2.exe")))
            {
                flag = Directory.GetFiles(directoryName, "*.crf").Length > 0;
                if (!flag)
                {
                    flag = Directory.GetFiles(Path.Combine(directoryName, "res"), "*.crf").Length > 0;
                }
            }
            return flag;
        }

        public override string[] DefaultDirectories
        {
            get
            {
                return new string[] { @"C:\Sshock2" };
            }
        }

        public override string DisplayFileName
        {
            get
            {
                return "System Shock 2";
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
                return "System Shock 2";
            }
        }
    }
}

