namespace Ravioli.ArchivePlugins
{
    using Ravioli.PluginHelpers;
    using System;
    using System.Collections.Generic;
    using System.IO;

    public class TRUPlugin : TRFSPlugin
    {
        public override string[] DefaultDirectories
        {
            get
            {
                List<string> list = new List<string>();
                list.AddRange(SystemSearcher.Combine3264ProgramFiles(@"Eidos\Tomb Raider - Underworld"));
                list.Add(Path.Combine(SystemSearcher.SteamAppsCommonDirectory, "tomb raider underworld"));
                return list.ToArray();
            }
        }

        public override string DisplayFileName
        {
            get
            {
                return "Tomb Raider: Underworld";
            }
        }

        public override string TypeName
        {
            get
            {
                return "Tomb Raider: Underworld";
            }
        }
    }
}

