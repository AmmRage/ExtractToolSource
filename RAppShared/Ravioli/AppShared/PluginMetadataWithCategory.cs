namespace Ravioli.AppShared
{
    using System;
    using System.Collections.Generic;

    public class PluginMetadataWithCategory
    {
        private string category = string.Empty;
        private List<PluginMetadata> data = new List<PluginMetadata>();

        public string Category
        {
            get
            {
                return this.category;
            }
            set
            {
                this.category = value;
            }
        }

        public List<PluginMetadata> Data
        {
            get
            {
                return this.data;
            }
            set
            {
                this.data = value;
            }
        }
    }
}

