namespace Ravioli.ArchivePlugins
{
    using System;
    using System.IO;

    public class RgdFile : ScanArchiveBase
    {
        protected override Type GetSerializedType()
        {
            return typeof(GenericDirectory);
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
                return new string[] { "rgd" };
            }
        }

        public override string TypeName
        {
            get
            {
                return "Ravioli Generic Directory File";
            }
        }
    }
}

