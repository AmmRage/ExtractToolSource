namespace Ravioli.Xbox360Plugins
{
    using Ravioli.ArchiveInterface;
    using Ravioli.PluginHelpers.Reading;
    using System;
    using System.IO;
    using System.Text;

    internal class BnkHelper
    {
        public static string DetermineFileExtension(Stream stream, GenericDirectoryEntry entry, string defaultForUnknownTypes)
        {
            string str = defaultForUnknownTypes;
            if (entry.Length >= 0x16L)
            {
                byte[] buffer;
                int num2;
                long position = stream.Position;
                stream.Seek(entry.Offset, SeekOrigin.Begin);
                RiffInformation information = RiffReader.ReadRiffInformation(stream, out buffer, out num2);
                if ((information != null) && (information.RiffFormType == "WAVE"))
                {
                    if ((information.FormatTag == 0x165) || (information.FormatTag == 0x166))
                    {
                        str = ".xma";
                    }
                    else if (information.FormatTag == 0xffff)
                    {
                        str = ".wwise_v";
                    }
                    else if ((information.FormatTag == 0x69) || (information.FormatTag == 2))
                    {
                        str = ".wwise_a";
                    }
                    else if (information.FormatTag == 0xfffe)
                    {
                        str = ".wwise_p";
                    }
                    else
                    {
                        str = ".wav";
                    }
                }
                else if (((buffer != null) && (num2 >= "BKHD".Length)) && (Encoding.ASCII.GetString(buffer, 0, "BKHD".Length) == "BKHD"))
                {
                    str = ".bnk";
                }
                stream.Position = position;
            }
            return str;
        }
    }
}

