namespace Ravioli.Explorer
{
    using System;
    using System.Collections;

    internal class ListViewCaseInsensitiveSorter : ListViewColumnSorter
    {
        private CaseInsensitiveComparer objectCompare = new CaseInsensitiveComparer();

        protected override int OnCompare(string x, string y)
        {
            return this.objectCompare.Compare(x, y);
        }
    }
}

