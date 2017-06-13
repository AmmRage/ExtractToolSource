namespace Ravioli.ArchiveInterface
{
    using System;
    using System.IO;

    public interface IArchive : IClassInfo
    {
        void Close();
        void ExtractFile(IFileInfo file, Stream outputStream);
        void ExtractFile(IFileInfo file, string outputFileName);
        void ExtractFile(IFileInfo file, Stream outputStream, long byteCount);
        bool IsValidFormat(string fileName);
        void Open(string fileName);

        string Comment { get; }

        string FileName { get; }

        IFileInfo[] Files { get; }
    }
}

