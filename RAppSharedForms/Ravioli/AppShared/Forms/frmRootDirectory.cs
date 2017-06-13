namespace Ravioli.AppShared.Forms
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.IO;
    using System.Windows.Forms;

    public class frmRootDirectory : Form
    {
        private Button btnBrowse;
        private Button btnCancel;
        private Button btnOK;
        private IContainer components;
        private FolderBrowserDialog folderBrowserDialog1;
        private string inputFile;
        private Label label1;
        private Label lblPrompt;
        private TextBox txtRootDir;

        public frmRootDirectory()
        {
            this.InitializeComponent();
            this.inputFile = string.Empty;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            this.folderBrowserDialog1.Description = "Select root directory.";
            this.folderBrowserDialog1.SelectedPath = this.txtRootDir.Text;
            if (this.folderBrowserDialog1.ShowDialog(this) == DialogResult.OK)
            {
                this.txtRootDir.Text = this.folderBrowserDialog1.SelectedPath;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (this.txtRootDir.Text == string.Empty)
            {
                MessageBox.Show(this, "You need to enter a root directory.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                base.DialogResult = DialogResult.None;
            }
            else if (!Directory.Exists(this.txtRootDir.Text))
            {
                MessageBox.Show(this, "The root directory does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                base.DialogResult = DialogResult.None;
            }
        }

        private void CheckCommands()
        {
            this.btnOK.Enabled = this.txtRootDir.Text.Length > 0;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void frmRootDirectory_Load(object sender, EventArgs e)
        {
            this.ShowPrompt();
            this.CheckCommands();
        }

        private void InitializeComponent()
        {
            this.label1 = new Label();
            this.txtRootDir = new TextBox();
            this.btnBrowse = new Button();
            this.btnOK = new Button();
            this.btnCancel = new Button();
            this.folderBrowserDialog1 = new FolderBrowserDialog();
            this.lblPrompt = new Label();
            base.SuspendLayout();
            this.label1.AutoSize = true;
            this.label1.Location = new Point(12, 0x3e);
            this.label1.Name = "label1";
            this.label1.Size = new Size(0x4e, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "&Root Directory:";
            this.txtRootDir.Location = new Point(0x60, 0x3b);
            this.txtRootDir.Name = "txtRootDir";
            this.txtRootDir.Size = new Size(0xfc, 20);
            this.txtRootDir.TabIndex = 1;
            this.txtRootDir.TextChanged += new EventHandler(this.txtRootDir_TextChanged);
            this.btnBrowse.Location = new Point(0x162, 0x39);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new Size(0x4b, 0x17);
            this.btnBrowse.TabIndex = 2;
            this.btnBrowse.Text = "&Browse...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new EventHandler(this.btnBrowse_Click);
            this.btnOK.DialogResult = DialogResult.OK;
            this.btnOK.Location = new Point(0x8d, 0x60);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new Size(0x4b, 0x17);
            this.btnOK.TabIndex = 6;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new EventHandler(this.btnOK_Click);
            this.btnCancel.DialogResult = DialogResult.Cancel;
            this.btnCancel.Location = new Point(0xde, 0x60);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new Size(0x4b, 0x17);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.lblPrompt.Location = new Point(12, 9);
            this.lblPrompt.Name = "lblPrompt";
            this.lblPrompt.Size = new Size(0x1a1, 0x24);
            this.lblPrompt.TabIndex = 8;
            this.lblPrompt.Text = "<<prompt>>";
            base.AcceptButton = this.btnOK;
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.CancelButton = this.btnCancel;
            base.ClientSize = new Size(0x1b7, 0x83);
            base.Controls.Add(this.lblPrompt);
            base.Controls.Add(this.btnCancel);
            base.Controls.Add(this.btnOK);
            base.Controls.Add(this.btnBrowse);
            base.Controls.Add(this.txtRootDir);
            base.Controls.Add(this.label1);
            base.FormBorderStyle = FormBorderStyle.FixedDialog;
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            base.Name = "frmRootDirectory";
            base.ShowInTaskbar = false;
            base.StartPosition = FormStartPosition.CenterParent;
            this.Text = "More info needed";
            base.Load += new EventHandler(this.frmRootDirectory_Load);
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        private void ShowPrompt()
        {
            if (!string.IsNullOrEmpty(this.inputFile))
            {
                this.lblPrompt.Text = "More info is needed for \"" + this.inputFile + "\".";
            }
            else
            {
                this.lblPrompt.Text = "More info is needed.";
            }
        }

        private void txtRootDir_TextChanged(object sender, EventArgs e)
        {
            this.CheckCommands();
        }

        public string InputFile
        {
            get
            {
                return this.inputFile;
            }
            set
            {
                this.inputFile = value;
                this.ShowPrompt();
            }
        }

        public string RootDirectory
        {
            get
            {
                return this.txtRootDir.Text;
            }
            set
            {
                this.txtRootDir.Text = value;
            }
        }
    }
}

