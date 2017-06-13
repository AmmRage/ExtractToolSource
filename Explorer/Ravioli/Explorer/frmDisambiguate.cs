namespace Ravioli.Explorer
{
    using Ravioli.AppShared;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.IO;
    using System.Windows.Forms;

    public class frmDisambiguate : Form
    {
        private Button btnCancel;
        private Button btnOK;
        private IContainer components;
        private string fileName;
        private Label lblFileName;
        private Label lblPrompt;
        private ListBox lstFormat;
        private PluginMapping[] mappings;
        private PluginMapping selectedMapping;

        public frmDisambiguate()
        {
            this.InitializeComponent();
            this.mappings = null;
            this.fileName = null;
            this.selectedMapping = null;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (this.lstFormat.SelectedItem != null)
            {
                this.selectedMapping = (PluginMapping) this.lstFormat.SelectedItem;
            }
            else
            {
                MessageBox.Show(this, "You need to select a format.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                base.DialogResult = DialogResult.None;
            }
        }

        private void CheckCommands()
        {
            this.btnOK.Enabled = this.lstFormat.SelectedItem != null;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void frmDisambiguate_Load(object sender, EventArgs e)
        {
            if ((this.mappings != null) && (this.fileName != null))
            {
                this.lstFormat.DisplayMember = "TypeName";
                this.lstFormat.ValueMember = "PluginType";
                this.lstFormat.Items.AddRange(this.mappings);
                this.lblPrompt.Text = "Please specify the type of the following file:";
                this.lblFileName.Text = Path.GetFileName(this.fileName);
            }
            this.CheckCommands();
        }

        public PluginMapping GetData()
        {
            return this.selectedMapping;
        }

        private void InitializeComponent()
        {
            this.lstFormat = new ListBox();
            this.lblPrompt = new Label();
            this.btnOK = new Button();
            this.btnCancel = new Button();
            this.lblFileName = new Label();
            base.SuspendLayout();
            this.lstFormat.FormattingEnabled = true;
            this.lstFormat.Location = new Point(12, 0x41);
            this.lstFormat.Name = "lstFormat";
            this.lstFormat.Size = new Size(360, 0x93);
            this.lstFormat.TabIndex = 0;
            this.lstFormat.SelectedIndexChanged += new EventHandler(this.lstFormat_SelectedIndexChanged);
            this.lstFormat.DoubleClick += new EventHandler(this.lstFormat_DoubleClick);
            this.lblPrompt.Location = new Point(12, 9);
            this.lblPrompt.Name = "lblPrompt";
            this.lblPrompt.Size = new Size(360, 30);
            this.lblPrompt.TabIndex = 1;
            this.lblPrompt.Text = "<<prompt>>";
            this.btnOK.DialogResult = DialogResult.OK;
            this.btnOK.Location = new Point(0xd8, 0xda);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new Size(0x4b, 0x17);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new EventHandler(this.btnOK_Click);
            this.btnCancel.DialogResult = DialogResult.Cancel;
            this.btnCancel.Location = new Point(0x129, 0xda);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new Size(0x4b, 0x17);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.lblFileName.Location = new Point(12, 0x27);
            this.lblFileName.Name = "lblFileName";
            this.lblFileName.Size = new Size(360, 0x17);
            this.lblFileName.TabIndex = 4;
            this.lblFileName.Text = "<<fileName>>";
            base.AcceptButton = this.btnOK;
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.CancelButton = this.btnCancel;
            base.ClientSize = new Size(0x180, 0xfd);
            base.Controls.Add(this.lblFileName);
            base.Controls.Add(this.btnCancel);
            base.Controls.Add(this.btnOK);
            base.Controls.Add(this.lblPrompt);
            base.Controls.Add(this.lstFormat);
            base.FormBorderStyle = FormBorderStyle.FixedDialog;
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            base.Name = "frmDisambiguate";
            base.ShowInTaskbar = false;
            base.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Resolve Ambiguity";
            base.Load += new EventHandler(this.frmDisambiguate_Load);
            base.ResumeLayout(false);
        }

        private void lstFormat_DoubleClick(object sender, EventArgs e)
        {
            if (this.btnOK.Enabled)
            {
                base.DialogResult = DialogResult.OK;
                this.btnOK_Click(this, EventArgs.Empty);
            }
        }

        private void lstFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.CheckCommands();
        }

        public void SetData(string fileName, PluginMapping[] mappings)
        {
            this.fileName = fileName;
            this.mappings = mappings;
        }
    }
}

