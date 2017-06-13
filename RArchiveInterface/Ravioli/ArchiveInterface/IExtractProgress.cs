namespace Ravioli.ArchiveInterface
{
    using System;

    public interface IExtractProgress
    {
        event PercentProgressEventHandler ExtractProgress;
    }
}

