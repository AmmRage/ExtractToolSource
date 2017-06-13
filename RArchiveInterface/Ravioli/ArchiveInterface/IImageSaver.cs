namespace Ravioli.ArchiveInterface
{
    using System;
    using System.Drawing;

    public interface IImageSaver : IClassInfo
    {
        void SaveImage(Image image, string fileName);
    }
}

