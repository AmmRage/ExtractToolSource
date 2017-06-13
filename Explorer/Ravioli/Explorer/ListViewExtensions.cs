namespace Ravioli.Explorer
{
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class ListViewExtensions
    {
        private const int HDF_SORTDOWN = 0x200;
        private const int HDF_SORTUP = 0x400;
        private const int HDI_FORMAT = 4;
        private const int HDM_GETITEM = 0x120b;
        private const int HDM_SETITEM = 0x120c;
        private const int LVM_FIRST = 0x1000;
        private const int LVM_GETHEADER = 0x101f;
        private const int LVM_SETICONSPACING = 0x1035;

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", EntryPoint="SendMessage")]
        private static extern IntPtr SendMessageLVCOLUMN(IntPtr hWnd, int Msg, IntPtr wParam, ref LVCOLUMN lPLVCOLUMN);
        public static void SetIconSpacing(ListView ListViewControl, int cx, int cy)
        {
            int num;
            if ((cx >= 0) && (cy >= 0))
            {
                num = cx | (cy << 0x10);
            }
            else
            {
                if ((cx != -1) || (cy != -1))
                {
                    throw new ArgumentException("cx and cy must be both positive, zero or -1.");
                }
                num = -1;
            }
            SendMessage(ListViewControl.Handle, 0x1035, IntPtr.Zero, new IntPtr(num));
        }

        public static void SetSortIcon(ListView ListViewControl, int ColumnIndex, SortOrder Order)
        {
            IntPtr hWnd = SendMessage(ListViewControl.Handle, 0x101f, IntPtr.Zero, IntPtr.Zero);
            for (int i = 0; i <= (ListViewControl.Columns.Count - 1); i++)
            {
                IntPtr wParam = new IntPtr(i);
                LVCOLUMN lPLVCOLUMN = new LVCOLUMN {
                    mask = 4
                };
                SendMessageLVCOLUMN(hWnd, 0x120b, wParam, ref lPLVCOLUMN);
                if ((Order != SortOrder.None) && (i == ColumnIndex))
                {
                    switch (Order)
                    {
                        case SortOrder.Ascending:
                            lPLVCOLUMN.fmt &= -513;
                            lPLVCOLUMN.fmt |= 0x400;
                            break;

                        case SortOrder.Descending:
                            lPLVCOLUMN.fmt &= -1025;
                            lPLVCOLUMN.fmt |= 0x200;
                            break;
                    }
                }
                else
                {
                    lPLVCOLUMN.fmt &= -1537;
                }
                SendMessageLVCOLUMN(hWnd, 0x120c, wParam, ref lPLVCOLUMN);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct LVCOLUMN
        {
            public int mask;
            public int cx;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pszText;
            public IntPtr hbm;
            public int cchTextMax;
            public int fmt;
            public int iSubItem;
            public int iImage;
            public int iOrder;
        }
    }
}

