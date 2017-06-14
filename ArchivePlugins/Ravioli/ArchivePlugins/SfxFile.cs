namespace Ravioli.ArchivePlugins
{
    using Ravioli.ArchiveInterface;
    using Ravioli.PluginHelpers;
    using System;
    using System.Collections.Generic;
    using System.IO;

    public class SfxFile : GenericArchive
    {
        protected override bool IsValidFormat(Stream stream, BinaryReader reader)
        {
            return true;
        }

        protected override GenericDirectoryEntry[] ReadGenericDirectory(Stream stream, BinaryReader reader)
        {
            stream.Seek(0L, SeekOrigin.Begin);
            List<GenericDirectoryEntry> list = new List<GenericDirectoryEntry>();
            FileCounter counter = new FileCounter();
            while (stream.Position < stream.Length)
            {
                GenericDirectoryEntry item = new GenericDirectoryEntry();
                uint num = reader.ReadUInt32();
                byte num2 = reader.ReadByte();
                item.Offset = stream.Position;
                string extension = (num2 == 2) ? ".cmf" : ".dat";
                item.Name = counter.GetNextFileName(extension);
                item.Length = num;
                list.Add(item);
                stream.Seek((long) num, SeekOrigin.Current);
            }
            return list.ToArray();
        }

        public override string[] Extensions
        {
            get
            {
                return new string[] { ".sfx" };
            }
        }

        public override string TypeName
        {
            get
            {
                return "Arnie Goes 4 Gold SFX File";
            }
        }
    }
}

