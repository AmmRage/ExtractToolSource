namespace Ravioli.ArchiveInterface
{
    using System;
    using System.Drawing;
    using System.IO;

    public interface IImageLoader : IClassInfo
    {
        Image LoadImage(Stream stream);
        Image LoadImage(string fileName);
    }
}

