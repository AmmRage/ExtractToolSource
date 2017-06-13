namespace Ravioli.AppShared
{
    using System;

    public class PluginMetadata
    {
        private string[] extensions;
        private string file;
        private string name;

        public static int CompareByName(PluginMetadata x, PluginMetadata y)
        {
            if (x == null)
            {
                if (y == null)
                {
                    return 0;
                }
                return -1;
            }
            if (y == null)
            {
                return 1;
            }
            return x.Name.CompareTo(y.Name);
        }

        public string[] Extensions
        {
            get
            {
                return this.extensions;
            }
            set
            {
                this.extensions = value;
            }
        }

        public string File
        {
            get
            {
                return this.file;
            }
            set
            {
                this.file = value;
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
    }
}

