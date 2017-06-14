namespace Ravioli.ArchivePlugins
{
    using Ravioli.ArchiveInterface;
    using System;

    internal class GfxDirectoryEntry : GenericDirectoryEntry
    {
        private int paletteIndex = -1;

        public int PaletteIndex
        {
            get
            {
                return this.paletteIndex;
            }
            set
            {
                this.paletteIndex = value;
            }
        }
    }
}

