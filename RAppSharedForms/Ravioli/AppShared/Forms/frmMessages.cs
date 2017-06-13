namespace Ravioli.AppShared.Forms
{
    using System;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    public class frmMessages : Form
    {
        private Button btnClose;
        private CheckBox chkWordWrap;
        private IContainer components;
        private Label lblMessageType;
        private Label lblMsgCount;
        private TextBox txtMessages;

        public frmMessages()
        {
            this.InitializeComponent();
        }

        private void chkWordWrap_CheckedChanged(object sender, EventArgs e)
        {
            this.txtMessages.WordWrap = this.chkWordWrap.Checked;
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
            this.btnClose = new Button();
            this.lblMessageType = new Label();
            this.lblMsgCount = new Label();
            this.txtMessages = new TextBox();
            this.chkWordWrap = new CheckBox();
            base.SuspendLayout();
            this.btnClose.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            this.btnClose.DialogResult = DialogResult.Cancel;
            this.btnClose.Location = new Point(0x1b9, 0xcc);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new Size(0x4b, 0x17);
            this.btnClose.TabIndex = 4;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.lblMessageType.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
            this.lblMessageType.AutoSize = true;
            this.lblMessageType.Location = new Point(12, 0xd1);
            this.lblMessageType.Name = "lblMessageType";
            this.lblMessageType.Size = new Size(0x25, 13);
            this.lblMessageType.TabIndex = 1;
            this.lblMessageType.Text = "Errors:";
            this.lblMsgCount.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
            this.lblMsgCount.AutoSize = true;
            this.lblMsgCount.Location = new Point(0x49, 0xd1);
            this.lblMsgCount.Name = "lblMsgCount";
            this.lblMsgCount.Size = new Size(13, 13);
            this.lblMsgCount.TabIndex = 2;
            this.lblMsgCount.Text = "0";
            this.txtMessages.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
            this.txtMessages.Location = new Point(12, 12);
            this.txtMessages.Multiline = true;
            this.txtMessages.Name = "txtMessages";
            this.txtMessages.ReadOnly = true;
            this.txtMessages.ScrollBars = ScrollBars.Both;
            this.txtMessages.Size = new Size(0x1f8, 0xba);
            this.txtMessages.TabIndex = 0;
            this.chkWordWrap.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
            this.chkWordWrap.AutoSize = true;
            this.chkWordWrap.Checked = true;
            this.chkWordWrap.CheckState = CheckState.Checked;
            this.chkWordWrap.Location = new Point(0x9e, 0xcd);
            this.chkWordWrap.Name = "chkWordWrap";
            this.chkWordWrap.Size = new Size(0x4e, 0x11);
            this.chkWordWrap.TabIndex = 3;
            this.chkWordWrap.Text = "Word wrap";
            this.chkWordWrap.UseVisualStyleBackColor = true;
            this.chkWordWrap.CheckedChanged += new EventHandler(this.chkWordWrap_CheckedChanged);
            base.AcceptButton = this.btnClose;
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.CancelButton = this.btnClose;
            base.ClientSize = new Size(0x210, 0xef);
            base.Controls.Add(this.chkWordWrap);
            base.Controls.Add(this.txtMessages);
            base.Controls.Add(this.lblMsgCount);
            base.Controls.Add(this.lblMessageType);
            base.Controls.Add(this.btnClose);
            base.MinimizeBox = false;
            base.Name = "frmMessages";
            base.ShowIcon = false;
            base.ShowInTaskbar = false;
            base.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Errors";
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        public void SetMessages(StringCollection messages)
        {
            this.txtMessages.Clear();
            foreach (string str in messages)
            {
                this.txtMessages.AppendText(str);
                this.txtMessages.AppendText(Environment.NewLine);
            }
            this.lblMsgCount.Text = messages.Count.ToString();
        }

        public void SetMessageType(string type)
        {
            this.Text = type;
            this.lblMessageType.Text = type + ":";
        }
    }
}

