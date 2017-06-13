namespace Ravioli.AppShared
{
    using System;
    using System.IO;

    public static class FileSystemHelper
    {
        public static string GetFileOfDirectory(string directory)
        {
            string[] strArray = Directory.GetFiles(directory, "*.*", SearchOption.TopDirectoryOnly);
            if (strArray.Length > 0)
            {
                string str = strArray[0];
                foreach (string str2 in strArray)
                {
                    try
                    {
                        new FileStream(str2, FileMode.Open, FileAccess.Read).Close();
                        return str2;
                    }
                    catch (IOException)
                    {
                    }
                }
                return str;
            }
            return Path.Combine(directory, "Dummy.dat");
        }
    }
}

