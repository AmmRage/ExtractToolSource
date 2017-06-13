namespace Ravioli.AppShared
{
    using System;
    using System.Runtime.CompilerServices;

    public class ArchiveBrowseFilter
    {
        public int AllFilesIndex { get; internal set; }

        public string FilterString { get; internal set; }

        public int KnownTypesIndex { get; internal set; }
    }
}

