namespace Ravioli.AppShared.Forms
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    public class frm4ButtonMsgBase : Form
    {
        protected Button btnIgnore;
        protected Button btnNo;
        protected Button btnRetry;
        protected Button btnYes;
        private IContainer components;
        private Label lblMessageText;
        private string messageCaption;
        private string messageText;

        public frm4ButtonMsgBase()
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

        private void frm4ButtonMsgBase_Load(object sender, EventArgs e)
        {
            this.Text = this.messageCaption;
            this.lblMessageText.Text = this.messageText;
        }

        private void InitializeComponent()
        {
            this.lblMessageText = new Label();
            this.btnYes = new Button();
            this.btnRetry = new Button();
            this.btnNo = new Button();
            this.btnIgnore = new Button();
            base.SuspendLayout();
            this.lblMessageText.Location = new Point(12, 9);
            this.lblMessageText.Name = "lblMessageText";
            this.lblMessageText.Size = new Size(0x183, 0x92);
            this.lblMessageText.TabIndex = 0;
            this.btnYes.DialogResult = DialogResult.Yes;
            this.btnYes.Location = new Point(0x10, 0xac);
            this.btnYes.Name = "btnYes";
            this.btnYes.Size = new Size(90, 0x17);
            this.btnYes.TabIndex = 1;
            this.btnYes.UseVisualStyleBackColor = true;
            this.btnRetry.DialogResult = DialogResult.Retry;
            this.btnRetry.Location = new Point(0x70, 0xac);
            this.btnRetry.Name = "btnRetry";
            this.btnRetry.Size = new Size(90, 0x17);
            this.btnRetry.TabIndex = 2;
            this.btnRetry.UseVisualStyleBackColor = true;
            this.btnNo.DialogResult = DialogResult.No;
            this.btnNo.Location = new Point(0xd0, 0xac);
            this.btnNo.Name = "btnNo";
            this.btnNo.Size = new Size(90, 0x17);
            this.btnNo.TabIndex = 3;
            this.btnNo.UseVisualStyleBackColor = true;
            this.btnIgnore.DialogResult = DialogResult.Ignore;
            this.btnIgnore.Location = new Point(0x130, 0xac);
            this.btnIgnore.Name = "btnIgnore";
            this.btnIgnore.Size = new Size(90, 0x17);
            this.btnIgnore.TabIndex = 4;
            this.btnIgnore.UseVisualStyleBackColor = true;
            base.AcceptButton = this.btnYes;
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.CancelButton = this.btnNo;
            base.ClientSize = new Size(0x19b, 0xcf);
            base.Controls.Add(this.btnIgnore);
            base.Controls.Add(this.btnNo);
            base.Controls.Add(this.btnRetry);
            base.Controls.Add(this.btnYes);
            base.Controls.Add(this.lblMessageText);
            base.FormBorderStyle = FormBorderStyle.FixedDialog;
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            base.Name = "frm4ButtonMsgBase";
            base.ShowInTaskbar = false;
            base.StartPosition = FormStartPosition.CenterParent;
            this.Text = "frm4ButtonMsgBase";
            base.Load += new EventHandler(this.frm4ButtonMsgBase_Load);
            base.ResumeLayout(false);
        }

        public string MessageCaption
        {
            get
            {
                return this.messageCaption;
            }
            set
            {
                this.messageCaption = value;
            }
        }

        public string MessageText
        {
            get
            {
                return this.messageText;
            }
            set
            {
                this.messageText = value;
            }
        }
    }
}

