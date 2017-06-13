namespace Ravioli.ArchiveInterface
{
    using System;
    using System.IO;

    public interface IFormatCheck
    {
        bool IsValidFormat(Stream stream);
        bool IsValidFormat(string fileName);
    }
}

