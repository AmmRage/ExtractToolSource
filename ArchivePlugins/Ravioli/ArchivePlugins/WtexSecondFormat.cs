namespace Ravioli.ArchivePlugins
{
    using System;
    using System.Drawing;
    using System.IO;

    internal class WtexSecondFormat
    {
        public Image LoadImage(Stream stream)
        {
            throw new NotSupportedException("File is not a supported WTEX format.");
        }
    }
}

