namespace Ravioli.ArchiveInterface
{
    using System;

    public interface IExtractWarnings
    {
        event ExtractWarningEventHandler FileExtractWarning;
    }
}

