namespace Ravioli.Extractor
{
    using System;

    public class CommandLineParser
    {
        public static ExtractorParameters Parse(string[] args)
        {
            ExtractorParameters parameters = new ExtractorParameters();
            if ((args.Length > 0) && !args[0].StartsWith("/"))
            {
                parameters.InputFiles = args[0];
            }
            if ((args.Length > 1) && !args[1].StartsWith("/"))
            {
                parameters.OutputDir = args[1];
            }
            foreach (string str in args)
            {
                if (string.Compare(str, "/subdir", true) == 0)
                {
                    parameters.InputFileSubDir = true;
                }
                else if (string.Compare(str, "/extract", true) == 0)
                {
                    parameters.AutoExtract = true;
                }
                else if (str.StartsWith("/imageformat:", StringComparison.CurrentCultureIgnoreCase) && (str.Length > 13))
                {
                    parameters.ImageFormat = str.Substring(13);
                }
                else if (str.StartsWith("/soundformat:", StringComparison.CurrentCultureIgnoreCase) && (str.Length > 13))
                {
                    parameters.SoundFormat = str.Substring(13);
                }
                else if (str.StartsWith("/archivetype:", StringComparison.CurrentCultureIgnoreCase) && (str.Length > 13))
                {
                    parameters.ArchiveType = str.Substring(13);
                }
                else if (string.Compare(str, "/allowscanning", true) == 0)
                {
                    parameters.AllowScanning = true;
                }
                else if (str.StartsWith("/rootdir:", StringComparison.CurrentCultureIgnoreCase) && (str.Length > 9))
                {
                    parameters.RootDirs = str.Substring(9);
                }
                else if ((string.Compare(str, "/?", true) == 0) || (string.Compare(str, "/help", true) == 0))
                {
                    parameters.Help = true;
                }
                else if (str.StartsWith("/"))
                {
                    throw new ArgumentException("Invalid option \"" + str + "\".");
                }
            }
            return parameters;
        }
    }
}

