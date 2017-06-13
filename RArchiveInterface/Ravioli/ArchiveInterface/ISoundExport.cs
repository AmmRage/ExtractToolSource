namespace Ravioli.ArchiveInterface
{
    using System;

    public interface ISoundExport
    {
        void ExportSound(SoundExportFormat format, string fileName);

        SoundExportFormat[] SupportedExportFormats { get; }
    }
}

