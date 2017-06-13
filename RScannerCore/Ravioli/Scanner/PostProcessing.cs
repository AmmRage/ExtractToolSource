    using Ravioli.ArchiveInterface.Scanning;
    using System;
    using System.Collections.Generic;
namespace Ravioli.Scanner
{

    public class PostProcessing
    {
        public const string UnknownExtension = ".dat";
        public const int UnknownForeColor = 0xff0000;
        public const string UnknownTypeName = "Unknown";

        public static DirectoryEntryWithUnknowns[] AddUnknownEntries(ScanDirectoryEntry[] entries, long fileSize)
        {
            ScanDirectoryEntry entry = null;
            List<DirectoryEntryWithUnknowns> list = new List<DirectoryEntryWithUnknowns>();
            int nextNumber = 1;
            int num2 = 1;
            foreach (ScanDirectoryEntry entry2 in entries)
            {
                if (entry == null)
                {
                    long offset = entry2.Offset;
                    if (offset != 0L)
                    {
                        DirectoryEntryWithUnknowns unknowns = new DirectoryEntryWithUnknowns {
                            Offset = 0L,
                            Length = offset,
                            Name = GetNextUnknownName(ref num2),
                            IsUnknown = true,
                            TypeName = "Unknown",
                            PerceivedType = PerceivedType.Unknown
                        };
                        list.Add(unknowns);
                    }
                }
                else
                {
                    long num4 = entry2.Offset - (entry.Offset + entry.Length);
                    if (num4 != 0L)
                    {
                        DirectoryEntryWithUnknowns unknowns2 = new DirectoryEntryWithUnknowns {
                            Offset = entry.Offset + entry.Length,
                            Length = num4,
                            Name = GetNextUnknownName(ref num2),
                            IsUnknown = true,
                            TypeName = "Unknown",
                            PerceivedType = PerceivedType.Unknown
                        };
                        list.Add(unknowns2);
                    }
                }
                DirectoryEntryWithUnknowns item = new DirectoryEntryWithUnknowns {
                    Offset = entry2.Offset,
                    Length = entry2.Length,
                    Name = entry2.Name,
                    TypeName = entry2.TypeName,
                    PerceivedType = entry2.PerceivedType
                };
                list.Add(item);
                entry = entry2;
            }
            if (entry != null)
            {
                long num5 = fileSize - (entry.Offset + entry.Length);
                if (num5 != 0L)
                {
                    DirectoryEntryWithUnknowns unknowns4 = new DirectoryEntryWithUnknowns {
                        Offset = entry.Offset + entry.Length,
                        Length = num5,
                        Name = GetNextUnknownName(ref num2),
                        IsUnknown = true,
                        TypeName = "Unknown",
                        PerceivedType = PerceivedType.Unknown
                    };
                    list.Add(unknowns4);
                }
            }
            if (list.Count == 0)
            {
                DirectoryEntryWithUnknowns unknowns5 = new DirectoryEntryWithUnknowns {
                    Offset = 0L,
                    Length = fileSize,
                    Name = GetNextUnknownName(ref nextNumber),
                    IsUnknown = true,
                    TypeName = "Unknown",
                    PerceivedType = PerceivedType.Unknown
                };
                list.Add(unknowns5);
            }
            return list.ToArray();
        }

        public static DirectoryEntryWithUnknowns[] CopyEntries(ScanDirectoryEntry[] entries)
        {
            DirectoryEntryWithUnknowns[] unknownsArray = new DirectoryEntryWithUnknowns[entries.Length];
            for (int i = 0; i < entries.Length; i++)
            {
                ScanDirectoryEntry entry = entries[i];
                unknownsArray[i] = new DirectoryEntryWithUnknowns { Offset = entry.Offset, Length = entry.Length, Name = entry.Name, TypeName = entry.TypeName, PerceivedType = entry.PerceivedType, IsUnknown = false };
            }
            return unknownsArray;
        }

        private static string GetNextUnknownName(ref int nextNumber)
        {
            string str = string.Format("Unkn{0:D4}{1}", (int) nextNumber, ".dat");
            nextNumber++;
            return str;
        }
    }
}

