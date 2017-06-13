namespace Ravioli.Explorer
{
    using Ravioli.AppShared;
    using System;

    public class GameListEntry
    {
        private int existsDirectoryIndex;
        private bool existsOnSystem;
        private string name;
        private GameViewerPluginMapping plugin;

        public GameListEntry()
        {
            this.existsOnSystem = false;
            this.existsDirectoryIndex = -1;
        }

        public GameListEntry(string name, GameViewerPluginMapping plugin)
        {
            this.name = name;
            this.plugin = plugin;
        }

        public int ExistsDirectoryIndex
        {
            get
            {
                return this.existsDirectoryIndex;
            }
            set
            {
                this.existsDirectoryIndex = value;
            }
        }

        public bool ExistsOnSystem
        {
            get
            {
                return this.existsOnSystem;
            }
            set
            {
                this.existsOnSystem = value;
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        public GameViewerPluginMapping Plugin
        {
            get
            {
                return this.plugin;
            }
            set
            {
                this.plugin = value;
            }
        }
    }
}

