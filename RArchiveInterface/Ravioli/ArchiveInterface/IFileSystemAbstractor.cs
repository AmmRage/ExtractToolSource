namespace Ravioli.ArchiveInterface
{
    using System;

    public interface IFileSystemAbstractor : IArchive, IClassInfo
    {
        string DisplayFileName { get; }
    }
}

