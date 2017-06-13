namespace Ravioli.AppShared
{
    using System;

    public class GameViewerPluginMapping : ArchivePluginMapping
    {
        private string[] defaultDirectories;

        public GameViewerPluginMapping(string[] extensions, Type pluginType, string typeName, string[] defaultDirectories) : base(extensions, pluginType, typeName, false, false)
        {
            this.defaultDirectories = defaultDirectories;
        }

        public string[] DefaultDirectories
        {
            get
            {
                return this.defaultDirectories;
            }
        }
    }
}

