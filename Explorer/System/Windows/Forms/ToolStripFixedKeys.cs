namespace System.Windows.Forms
{
    using System;

    internal class ToolStripFixedKeys : ToolStrip
    {
        protected override bool ProcessCmdKey(ref Message m, Keys keyData)
        {
            if ((keyData & (Keys.Control | Keys.ControlKey)) == (Keys.Control | Keys.ControlKey))
            {
                return false;
            }
            return base.ProcessCmdKey(ref m, keyData);
        }
    }
}

