namespace Ravioli.AppShared
{
    using System;

    public class PluginMapping
    {
        private string[] extensions;
        private Type pluginType;
        private string typeName;

        public PluginMapping(string[] extensions, Type pluginType, string typeName)
        {
            this.extensions = extensions;
            this.pluginType = pluginType;
            this.typeName = typeName;
        }

        public static int CompareByTypeName(PluginMapping x, PluginMapping y)
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
            return x.TypeName.CompareTo(y.TypeName);
        }

        public override string ToString()
        {
            return string.Format("TypeName=\"{0}\" PluginType=\"{1}\"", this.typeName, this.pluginType.ToString());
        }

        public string[] Extensions
        {
            get
            {
                return this.extensions;
            }
        }

        public Type PluginType
        {
            get
            {
                return this.pluginType;
            }
        }

        public string TypeName
        {
            get
            {
                return this.typeName;
            }
        }
    }
}

