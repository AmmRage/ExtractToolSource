namespace Ravioli.ArchiveInterface
{
    using System;

    public interface IGameViewer : IFileSystemAbstractor, IArchive, IClassInfo
    {
        string[] DefaultDirectories { get; }
    }
}

