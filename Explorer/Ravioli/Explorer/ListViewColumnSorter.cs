namespace Ravioli.Explorer
{
    using System;
    using System.Collections;
    using System.Windows.Forms;

    internal abstract class ListViewColumnSorter : IComparer
    {
        private int columnToSort = 0;
        private SortOrder orderOfSort = SortOrder.None;

        public int Compare(object x, object y)
        {
            ListViewItem item = (ListViewItem) x;
            ListViewItem item2 = (ListViewItem) y;
            int num = this.OnCompare(item.SubItems[this.columnToSort].Text, item2.SubItems[this.columnToSort].Text);
            if (this.orderOfSort == SortOrder.Ascending)
            {
                return num;
            }
            if (this.orderOfSort == SortOrder.Descending)
            {
                return -num;
            }
            return 0;
        }

        protected abstract int OnCompare(string x, string y);

        public SortOrder Order
        {
            get
            {
                return this.orderOfSort;
            }
            set
            {
                this.orderOfSort = value;
            }
        }

        public int SortColumn
        {
            get
            {
                return this.columnToSort;
            }
            set
            {
                this.columnToSort = value;
            }
        }
    }
}

