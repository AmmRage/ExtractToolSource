namespace System.Windows.Forms
{
    using System;
    using System.Drawing;

    public class ToolStripSpringTextBox : ToolStripTextBox
    {
        public override Size GetPreferredSize(Size constrainingSize)
        {
            if (base.IsOnOverflow || (base.Owner.Orientation == Orientation.Vertical))
            {
                return this.DefaultSize;
            }
            int width = base.Owner.DisplayRectangle.Width;
            if (base.Owner.OverflowButton.Visible)
            {
                width = (width - base.Owner.OverflowButton.Width) - base.Owner.OverflowButton.Margin.Horizontal;
            }
            int num2 = 0;
            foreach (ToolStripItem item in base.Owner.Items)
            {
                if (!item.IsOnOverflow)
                {
                    if (item is ToolStripSpringTextBox)
                    {
                        num2++;
                        width -= item.Margin.Horizontal;
                    }
                    else
                    {
                        width = (width - item.Width) - item.Margin.Horizontal;
                    }
                }
            }
            if (num2 > 1)
            {
                width /= num2;
            }
            if (width < this.DefaultSize.Width)
            {
                width = this.DefaultSize.Width;
            }
            Size preferredSize = base.GetPreferredSize(constrainingSize);
            preferredSize.Width = width;
            return preferredSize;
        }
    }
}

