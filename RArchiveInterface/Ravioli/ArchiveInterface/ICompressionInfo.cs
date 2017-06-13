namespace Ravioli.ArchiveInterface
{
    using System;

    public interface ICompressionInfo
    {
        bool Compressed { get; }

        long CompressedSize { get; }
    }
}

