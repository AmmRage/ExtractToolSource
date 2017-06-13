namespace Ravioli.ArchiveInterface
{
    using System;
    using System.IO;

    public interface IOpenFromStream
    {
        bool IsValidFormat(Stream stream, BinaryReader reader);
        void Open(Stream stream, BinaryReader reader);
        void Open(Stream stream, BinaryReader reader, string fileName);
    }
}

