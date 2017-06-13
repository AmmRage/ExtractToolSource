namespace Ravioli.Explorer
{
    using System;
    using System.Windows.Forms;

    internal class SortCriterionMap
    {
        private ColumnHeader column;
        private SortCriterion criterion;
        private ToolStripMenuItem menuItem;

        public SortCriterionMap(SortCriterion criterion)
        {
            this.criterion = criterion;
            this.column = null;
            this.menuItem = null;
        }

        public SortCriterionMap(SortCriterion criterion, ColumnHeader column, ToolStripMenuItem menuItem)
        {
            this.criterion = criterion;
            this.column = column;
            this.menuItem = menuItem;
        }

        public ColumnHeader Column
        {
            get
            {
                return this.column;
            }
            set
            {
                this.column = value;
            }
        }

        public SortCriterion Criterion
        {
            get
            {
                return this.criterion;
            }
            set
            {
                this.criterion = value;
            }
        }

        public ToolStripMenuItem MenuItem
        {
            get
            {
                return this.menuItem;
            }
            set
            {
                this.menuItem = value;
            }
        }
    }
}

