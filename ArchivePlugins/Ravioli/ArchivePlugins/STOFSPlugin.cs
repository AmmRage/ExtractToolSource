namespace Ravioli.ArchivePlugins
{
    using Ravioli.ArchiveInterface;
    using Ravioli.PluginHelpers;
    using System;
    using System.Collections.Generic;
    using System.IO;

    public class STOFSPlugin : AbstractorArchive
    {
        protected override AbstractorDefinition[] CreateDefinitions()
        {
            Type pluginType = typeof(HoggFile);
            base.DuplicatesPolicy = AbstractorDuplicatesPolicy.Replace;
            return new List<AbstractorDefinition> { new SubArchiveAbstractorDefinition(@"Star Trek Online\Live\piggs\*.hogg", pluginType, "") }.ToArray();
        }

        protected override bool IsValidFormat(Stream stream, BinaryReader reader)
        {
            return File.Exists(Path.Combine(Path.GetDirectoryName(base.FileName), "Star Trek Online.exe"));
        }

        public override string[] DefaultDirectories
        {
            get
            {
                List<string> list = new List<string>();
                list.AddRange(SystemSearcher.Combine3264ProgramFiles("Cryptic Studios"));
                list.Add(Path.Combine(SystemSearcher.SteamAppsCommonDirectory, "Star Trek Online"));
                list.Add(Path.Combine(SystemSearcher.PublicUserProfileDir, @"Games\Cryptic Studios"));
                return list.ToArray();
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
                return "Star Trek Online";
            }
        }
    }
}

