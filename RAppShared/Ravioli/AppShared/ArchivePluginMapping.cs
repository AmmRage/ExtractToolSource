namespace Ravioli.AppShared
{
    using System;

    public class ArchivePluginMapping : PluginMapping
    {
        private bool extensionIndependent;
        private bool needsRootDirectory;

        public ArchivePluginMapping(string[] extensions, Type pluginType, string typeName, bool needsRootDirectory, bool extensionIndependent) : base(extensions, pluginType, typeName)
        {
            this.needsRootDirectory = needsRootDirectory;
            this.extensionIndependent = extensionIndependent;
        }

        public bool ExtensionIndependent
        {
            get
            {
                return this.extensionIndependent;
            }
        }

        public bool NeedsRootDirectory
        {
            get
            {
                return this.needsRootDirectory;
            }
        }
    }
}

