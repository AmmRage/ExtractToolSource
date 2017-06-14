namespace Ravioli.ArchivePlugins.SaintsRow
{
    using Ravioli.ArchiveInterface;
    using Ravioli.PluginHelpers;
    using System;
    using System.Collections.Generic;
    using System.IO;

    public class SR4InitFSPlugin : AbstractorArchive
    {
        protected override AbstractorDefinition[] CreateDefinitions()
        {
            List<Type> list3;
            List<AbstractorDefinition> list = new List<AbstractorDefinition>();
            List<Type> pluginTypes = new List<Type> {
                typeof(VppPcFile),
                typeof(VppPcSBFile)
            };
            SubArchiveAbstractorDefinition definition = new SubArchiveAbstractorDefinition(@"packfiles\pc\cache\*.vpp_pc", pluginTypes, "", SearchOption.TopDirectoryOnly, AbstractorListMode.InSubDirectoryWithoutExtension);
            list3 = new List<Type> {
                typeof(BnkPcFile),
                typeof(BnkPcFile2),
                new SubArchiveAbstractorDefinition("*_media.bnk_pc", list3, "", SearchOption.TopDirectoryOnly, AbstractorListMode.InSubDirectoryWithoutExtension),
                definition,
                new AbstractorDefinition(@"videos\*.bik", "videos")
            };
            return list.ToArray();
        }

        protected override bool IsValidFormat(Stream stream, BinaryReader reader)
        {
            return File.Exists(Path.Combine(Path.GetDirectoryName(base.FileName), "SaintsRowIV_InaugurationStation.exe"));
        }

        public override string[] DefaultDirectories
        {
            get
            {
                return new List<string> { Path.Combine(SystemSearcher.SteamAppsCommonDirectory, "Saints Row IV Inauguration Station") }.ToArray();
            }
        }

        public override string DisplayFileName
        {
            get
            {
                return this.TypeName;
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
                return "Saints Row IV: Inauguration Station";
            }
        }
    }
}

