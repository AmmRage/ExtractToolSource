namespace Ravioli.ArchiveInterface
{
    using System;

    public interface ISoundPlayer2 : ISoundPlayer, IClassInfo, IDisposable
    {
        long GetLength();
        long GetPosition();
        bool IsPlaying();
        void SetPosition(long position);
    }
}

