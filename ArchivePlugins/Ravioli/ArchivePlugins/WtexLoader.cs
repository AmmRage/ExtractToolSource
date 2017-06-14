namespace Ravioli.ArchivePlugins
{
    using Ravioli.ArchiveInterface;
    using Ravioli.DdsImagePlugin;
    using System;
    using System.Drawing;
    using System.IO;

    public class WtexLoader : IImageLoader, IClassInfo
    {
        public Image LoadImage(Stream stream)
        {
            Image image;
            BinaryReader reader = new BinaryReader(stream);
            try
            {
                uint num = reader.ReadUInt32();
                reader.ReadUInt32();
                reader.ReadUInt32();
                reader.ReadUInt32();
                uint num2 = reader.ReadUInt32();
                stream.Position += num - 20;
                if (num2 < 0x30000000)
                {
                    DdsLoader loader = new DdsLoader();
                    return loader.LoadImage(stream);
                }
                image = new WtexSecondFormat().LoadImage(stream);
            }
            finally
            {
                reader.Close();
            }
            return image;
        }

        public Image LoadImage(string fileName)
        {
            Image image;
            FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            try
            {
                image = this.LoadImage(stream);
            }
            finally
            {
                stream.Close();
            }
            return image;
        }

        public string[] Extensions
        {
            get
            {
                return new string[] { ".wtex" };
            }
        }

        public string TypeName
        {
            get
            {
                return "Star Trek Online WTEX Texture";
            }
        }
    }
}

