namespace Ravioli.ArchivePlugins
{
    using Ravioli.ArchiveInterface;
    using System;
    using System.IO;

    public class ScanArchive : ScanArchiveBase, IFileSystemAbstractor, IArchive, IClassInfo
    {
        public string DisplayFileName
        {
            get
            {
                if (base.ScanResults != null)
                {
                    return Path.GetFileName(base.ScanResults.FileName);
                }
                return "Unknown";
            }
        }

        public override string[] Extensions
        {
            get
            {
                return new string[] { "rsr" };
            }
        }

        public override string TypeName
        {
            get
            {
                return "Unknown file type";
            }
        }
    }
}

