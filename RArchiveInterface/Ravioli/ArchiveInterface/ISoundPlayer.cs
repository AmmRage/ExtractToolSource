namespace Ravioli.ArchiveInterface
{
    using System;
    using System.IO;

    public interface ISoundPlayer : IClassInfo, IDisposable
    {
        void LoadFromFile(string fileName);
        void LoadFromStream(System.IO.Stream stream);
        void Play();
        void Stop();

        string FileName { get; }

        System.IO.Stream Stream { get; }
    }
}

