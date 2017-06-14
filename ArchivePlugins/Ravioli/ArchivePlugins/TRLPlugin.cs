namespace Ravioli.ArchivePlugins
{
    using Ravioli.PluginHelpers;
    using System;
    using System.Collections.Generic;
    using System.IO;

    public class TRLPlugin : TRFSPlugin
    {
        public override string[] DefaultDirectories
        {
            get
            {
                List<string> list = new List<string>();
                list.AddRange(SystemSearcher.Combine3264ProgramFiles("Tomb Raider - Legend"));
                list.Add(Path.Combine(SystemSearcher.SteamAppsCommonDirectory, "tomb raider legend"));
                return list.ToArray();
            }
        }

        public override string DisplayFileName
        {
            get
            {
                return "Tomb Raider: Legend";
            }
        }

        public override string TypeName
        {
            get
            {
                return "Tomb Raider: Legend";
            }
        }
    }
}

