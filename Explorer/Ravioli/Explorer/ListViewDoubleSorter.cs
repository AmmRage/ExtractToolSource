namespace Ravioli.Explorer
{
    using System;

    internal class ListViewDoubleSorter : ListViewColumnSorter
    {
        protected override int OnCompare(string x, string y)
        {
            double num = (x.Length == 0) ? 0.0 : double.Parse(x);
            double num2 = (y.Length == 0) ? 0.0 : double.Parse(y);
            return num.CompareTo(num2);
        }
    }
}

