namespace Ravioli.ArchivePlugins
{
    using System;
    using System.IO;

    public class RsrFile : ScanArchiveBase
    {
        protected override Type GetSerializedType()
        {
            return typeof(ScanResults);
        }

        public override string Comment
        {
            get
            {
                if (base.ScanResults != null)
                {
                    return ("Original file: " + Path.GetFileName(base.ScanResults.FileName));
                }
                return string.Empty;
            }
        }

        public override string[] Extensions
        {
            get
            {
                return new string[] { ".rsr" };
            }
        }

        public override string TypeName
        {
            get
            {
                return "Ravioli Scan Results File";
            }
        }
    }
}

