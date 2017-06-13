namespace Ravioli.AppShared.Forms
{
    using System;
    using System.ComponentModel;

    public class frmYesNoAlways : frm4ButtonMsgBase
    {
        private IContainer components;

        public frmYesNoAlways()
        {
            this.InitializeComponent();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            base.SuspendLayout();
            base.btnYes.Text = "&Yes";
            base.btnRetry.Text = "&Always";
            base.btnNo.Text = "&No";
            base.btnIgnore.Text = "N&ever";
            base.Name = "frmYesNoAlways";
            this.Text = "frmYesNoAlways";
            base.ResumeLayout(false);
        }
    }
}

