namespace Ravioli.Explorer
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Forms;

    internal class SortCriterionMapCollection : List<SortCriterionMap>
    {
        public SortCriterion MapColumnToSortCriterion(int column)
        {
            SortCriterion name = SortCriterion.Name;
            bool flag = false;
            foreach (SortCriterionMap map in this)
            {
                if (map.Column.Index == column)
                {
                    name = map.Criterion;
                    flag = true;
                    break;
                }
            }
            if (!flag)
            {
                throw new ArgumentException("No matching sort map found for specified column.");
            }
            return name;
        }

        public SortCriterion MapMenuItemToSortCriterion(ToolStripMenuItem menuItem)
        {
            SortCriterion name = SortCriterion.Name;
            bool flag = false;
            foreach (SortCriterionMap map in this)
            {
                if (map.MenuItem == menuItem)
                {
                    name = map.Criterion;
                    flag = true;
                    break;
                }
            }
            if (!flag)
            {
                throw new ArgumentException("No matching sort map found for specified menu item.");
            }
            return name;
        }

        public SortCriterionMap MapSortCriterionToCriterionMap(SortCriterion criterion)
        {
            SortCriterionMap map = null;
            bool flag = false;
            foreach (SortCriterionMap map2 in this)
            {
                if (map2.Criterion == criterion)
                {
                    map = map2;
                    flag = true;
                    break;
                }
            }
            if (!flag)
            {
                throw new ArgumentException("No matching sort map found for specified criterion.");
            }
            return map;
        }
    }
}

