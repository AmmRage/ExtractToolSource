namespace Ravioli.ArchiveInterface.Scanning
{
    using Ravioli.ArchiveInterface;
    using System;
    using System.IO;

    public interface IFileDetector : IClassInfo
    {
        DetectedFile DetectFile(Stream stream, BinaryReader reader, long currentPosition, Logger logger);
        bool DetectSignature(Stream stream, BinaryReader reader);

        FileType[] FileTypes { get; }

        string Name { get; }
    }
}

