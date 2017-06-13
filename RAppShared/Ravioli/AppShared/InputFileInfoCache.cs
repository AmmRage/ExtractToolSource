namespace Ravioli.AppShared
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class InputFileInfoCache : IEnumerable<InputFileInfo>, IEnumerable
    {
        private List<InputFileInfo> list = new List<InputFileInfo>();

        public void Add(InputFileInfo item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            if (!this.Exists(item.FilePath))
            {
                this.list.Add(item);
            }
            else
            {
                this.Find(item.FilePath).MergeWith(item);
            }
        }

        public void Add(object item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            if (!(item is InputFileInfo))
            {
                throw new ArgumentException("Item must be of type InputFileInfo.");
            }
            this.Add(item);
        }

        public bool Exists(string path)
        {
            FileInfoChecker checker = new FileInfoChecker(path);
            return this.list.Exists(new Predicate<InputFileInfo>(checker.DoesFilePathMatch));
        }

        public InputFileInfo Find(string path)
        {
            FileInfoChecker checker = new FileInfoChecker(path);
            return this.list.Find(new Predicate<InputFileInfo>(checker.DoesFilePathMatch));
        }

        public IEnumerator<InputFileInfo> GetEnumerator()
        {
            return this.list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.list.GetEnumerator();
        }
    }
}

