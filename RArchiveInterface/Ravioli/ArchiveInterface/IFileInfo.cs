namespace Ravioli.ArchiveInterface
{
    using System;

    public interface IFileInfo : IFileSystemEntry
    {
        long ID { get; }

        long Size { get; }
    }
}

