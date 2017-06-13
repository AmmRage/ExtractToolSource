namespace Ravioli.AppShared.Forms
{
    using System;
    using System.ComponentModel;

    public class frmYesAllNoAll : frm4ButtonMsgBase
    {
        private IContainer components;

        public frmYesAllNoAll()
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
            base.btnRetry.Text = "Yes to &all";
            base.btnNo.Text = "&No";
            base.btnIgnore.Text = "N&o to all";
            base.Name = "frmYesAllNoAll";
            this.Text = "frmYesAllNoAll";
            base.ResumeLayout(false);
        }

        public bool NoToAllAvailable
        {
            get
            {
                return base.btnIgnore.Enabled;
            }
            set
            {
                base.btnIgnore.Enabled = value;
                base.btnIgnore.Visible = value;
            }
        }
    }
}

