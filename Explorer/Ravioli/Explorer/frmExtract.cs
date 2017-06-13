namespace Ravioli.Explorer
{
    using Ravioli.AppShared;
    using Ravioli.ArchiveInterface;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    internal class frmExtract : Form
    {
        private Button btnBrowse;
        private Button btnCancel;
        private Button btnOK;
        private ComboBox cboImageFormat;
        private ComboBox cboSoundFormat;
        internal CheckBox chkConvertImages;
        internal CheckBox chkConvertSounds;
        internal CheckBox chkCreateSubDirectory;
        private IContainer components;
        private FolderBrowserDialog folderBrowserDialog1;
        private GroupBox gbFiles;
        private GroupBox gbOptions;
        private IList<ImagePluginMapping> imageFormats;
        private Label label1;
        private RadioButton rbAllFiles;
        private RadioButton rbCurrentDir;
        private RadioButton rbDisplayedFiles;
        private RadioButton rbSelectedFiles;
        private IList<SoundExportFormat> soundFormats;
        private TextBox txtOutputDir;

        public frmExtract()
        {
            this.InitializeComponent();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            this.folderBrowserDialog1.Description = "Select output directory.";
            this.folderBrowserDialog1.SelectedPath = this.txtOutputDir.Text;
            if (this.folderBrowserDialog1.ShowDialog(this) == DialogResult.OK)
            {
                this.txtOutputDir.Text = this.folderBrowserDialog1.SelectedPath;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (this.txtOutputDir.Text == string.Empty)
            {
                MessageBox.Show(this, "You need to enter an output directory.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                base.DialogResult = DialogResult.None;
            }
        }

        private void cboImageFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.CheckCommands();
        }

        private void CheckCommands()
        {
            this.btnOK.Enabled = (this.txtOutputDir.Text.Length > 0) && (!this.chkConvertImages.Checked || (this.chkConvertImages.Checked && (this.cboImageFormat.SelectedItem != null)));
            this.cboImageFormat.Enabled = this.chkConvertImages.Checked;
            this.cboSoundFormat.Enabled = this.chkConvertSounds.Checked;
        }

        private void chkConvertImages_CheckedChanged(object sender, EventArgs e)
        {
            this.CheckCommands();
        }

        private void chkConvertSounds_CheckedChanged(object sender, EventArgs e)
        {
            this.CheckCommands();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void frmExtract_Load(object sender, EventArgs e)
        {
            this.CheckCommands();
        }

        private void InitializeComponent()
        {
            this.label1 = new Label();
            this.txtOutputDir = new TextBox();
            this.btnBrowse = new Button();
            this.btnOK = new Button();
            this.btnCancel = new Button();
            this.folderBrowserDialog1 = new FolderBrowserDialog();
            this.rbAllFiles = new RadioButton();
            this.rbSelectedFiles = new RadioButton();
            this.chkCreateSubDirectory = new CheckBox();
            this.chkConvertImages = new CheckBox();
            this.cboImageFormat = new ComboBox();
            this.rbDisplayedFiles = new RadioButton();
            this.gbFiles = new GroupBox();
            this.rbCurrentDir = new RadioButton();
            this.gbOptions = new GroupBox();
            this.chkConvertSounds = new CheckBox();
            this.cboSoundFormat = new ComboBox();
            this.gbFiles.SuspendLayout();
            this.gbOptions.SuspendLayout();
            base.SuspendLayout();
            this.label1.AutoSize = true;
            this.label1.Location = new Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new Size(0x37, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "&Extract to:";
            this.txtOutputDir.Location = new Point(0x4a, 12);
            this.txtOutputDir.Name = "txtOutputDir";
            this.txtOutputDir.Size = new Size(0x131, 20);
            this.txtOutputDir.TabIndex = 1;
            this.txtOutputDir.TextChanged += new EventHandler(this.txtOutputDir_TextChanged);
            this.btnBrowse.Location = new Point(0x181, 10);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new Size(0x4b, 0x17);
            this.btnBrowse.TabIndex = 2;
            this.btnBrowse.Text = "&Browse...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new EventHandler(this.btnBrowse_Click);
            this.btnOK.DialogResult = DialogResult.OK;
            this.btnOK.Location = new Point(0x130, 0x9f);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new Size(0x4b, 0x17);
            this.btnOK.TabIndex = 5;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new EventHandler(this.btnOK_Click);
            this.btnCancel.DialogResult = DialogResult.Cancel;
            this.btnCancel.Location = new Point(0x181, 0x9f);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new Size(0x4b, 0x17);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.rbAllFiles.AutoSize = true;
            this.rbAllFiles.Checked = true;
            this.rbAllFiles.Location = new Point(6, 0x13);
            this.rbAllFiles.Name = "rbAllFiles";
            this.rbAllFiles.Size = new Size(0x6a, 0x11);
            this.rbAllFiles.TabIndex = 0;
            this.rbAllFiles.TabStop = true;
            this.rbAllFiles.Text = "&All files in archive";
            this.rbAllFiles.UseVisualStyleBackColor = true;
            this.rbSelectedFiles.AutoSize = true;
            this.rbSelectedFiles.Location = new Point(6, 0x2a);
            this.rbSelectedFiles.Name = "rbSelectedFiles";
            this.rbSelectedFiles.Size = new Size(0x58, 0x11);
            this.rbSelectedFiles.TabIndex = 1;
            this.rbSelectedFiles.Text = "&Selected files";
            this.rbSelectedFiles.UseVisualStyleBackColor = true;
            this.chkCreateSubDirectory.AutoSize = true;
            this.chkCreateSubDirectory.Location = new Point(6, 0x13);
            this.chkCreateSubDirectory.Name = "chkCreateSubDirectory";
            this.chkCreateSubDirectory.Size = new Size(0xce, 0x11);
            this.chkCreateSubDirectory.TabIndex = 0;
            this.chkCreateSubDirectory.Text = "&Create subdirectory for current archive";
            this.chkCreateSubDirectory.UseVisualStyleBackColor = true;
            this.chkConvertImages.AutoSize = true;
            this.chkConvertImages.Location = new Point(6, 0x2c);
            this.chkConvertImages.Name = "chkConvertImages";
            this.chkConvertImages.Size = new Size(0x72, 0x11);
            this.chkConvertImages.TabIndex = 1;
            this.chkConvertImages.Text = "C&onvert images to:";
            this.chkConvertImages.UseVisualStyleBackColor = true;
            this.chkConvertImages.CheckedChanged += new EventHandler(this.chkConvertImages_CheckedChanged);
            this.cboImageFormat.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cboImageFormat.FormattingEnabled = true;
            this.cboImageFormat.Location = new Point(0x7e, 0x2a);
            this.cboImageFormat.Name = "cboImageFormat";
            this.cboImageFormat.Size = new Size(0xb0, 0x15);
            this.cboImageFormat.TabIndex = 2;
            this.cboImageFormat.SelectedIndexChanged += new EventHandler(this.cboImageFormat_SelectedIndexChanged);
            this.rbDisplayedFiles.AutoSize = true;
            this.rbDisplayedFiles.Location = new Point(6, 0x41);
            this.rbDisplayedFiles.Name = "rbDisplayedFiles";
            this.rbDisplayedFiles.Size = new Size(0x5c, 0x11);
            this.rbDisplayedFiles.TabIndex = 2;
            this.rbDisplayedFiles.TabStop = true;
            this.rbDisplayedFiles.Text = "&Displayed files";
            this.rbDisplayedFiles.UseVisualStyleBackColor = true;
            this.gbFiles.Controls.Add(this.rbCurrentDir);
            this.gbFiles.Controls.Add(this.rbAllFiles);
            this.gbFiles.Controls.Add(this.rbDisplayedFiles);
            this.gbFiles.Controls.Add(this.rbSelectedFiles);
            this.gbFiles.Location = new Point(15, 0x26);
            this.gbFiles.Name = "gbFiles";
            this.gbFiles.Size = new Size(120, 0x73);
            this.gbFiles.TabIndex = 3;
            this.gbFiles.TabStop = false;
            this.gbFiles.Text = "Files";
            this.rbCurrentDir.AutoSize = true;
            this.rbCurrentDir.Location = new Point(6, 0x58);
            this.rbCurrentDir.Name = "rbCurrentDir";
            this.rbCurrentDir.Size = new Size(0x66, 0x11);
            this.rbCurrentDir.TabIndex = 3;
            this.rbCurrentDir.TabStop = true;
            this.rbCurrentDir.Text = "&Current directory";
            this.rbCurrentDir.UseVisualStyleBackColor = true;
            this.gbOptions.Controls.Add(this.chkConvertSounds);
            this.gbOptions.Controls.Add(this.cboSoundFormat);
            this.gbOptions.Controls.Add(this.chkCreateSubDirectory);
            this.gbOptions.Controls.Add(this.chkConvertImages);
            this.gbOptions.Controls.Add(this.cboImageFormat);
            this.gbOptions.Location = new Point(0x8d, 0x26);
            this.gbOptions.Name = "gbOptions";
            this.gbOptions.Size = new Size(0x13f, 0x73);
            this.gbOptions.TabIndex = 4;
            this.gbOptions.TabStop = false;
            this.gbOptions.Text = "Options";
            this.chkConvertSounds.AutoSize = true;
            this.chkConvertSounds.Location = new Point(6, 0x47);
            this.chkConvertSounds.Name = "chkConvertSounds";
            this.chkConvertSounds.Size = new Size(0x73, 0x11);
            this.chkConvertSounds.TabIndex = 3;
            this.chkConvertSounds.Text = "Con&vert sounds to:";
            this.chkConvertSounds.UseVisualStyleBackColor = true;
            this.chkConvertSounds.CheckedChanged += new EventHandler(this.chkConvertSounds_CheckedChanged);
            this.cboSoundFormat.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cboSoundFormat.FormattingEnabled = true;
            this.cboSoundFormat.Location = new Point(0x7e, 0x45);
            this.cboSoundFormat.Name = "cboSoundFormat";
            this.cboSoundFormat.Size = new Size(0xb0, 0x15);
            this.cboSoundFormat.TabIndex = 4;
            base.AcceptButton = this.btnOK;
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.CancelButton = this.btnCancel;
            base.ClientSize = new Size(0x1d8, 0xc2);
            base.Controls.Add(this.gbOptions);
            base.Controls.Add(this.gbFiles);
            base.Controls.Add(this.btnCancel);
            base.Controls.Add(this.btnOK);
            base.Controls.Add(this.btnBrowse);
            base.Controls.Add(this.txtOutputDir);
            base.Controls.Add(this.label1);
            base.FormBorderStyle = FormBorderStyle.FixedDialog;
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            base.Name = "frmExtract";
            base.ShowInTaskbar = false;
            base.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Extract";
            base.Load += new EventHandler(this.frmExtract_Load);
            this.gbFiles.ResumeLayout(false);
            this.gbFiles.PerformLayout();
            this.gbOptions.ResumeLayout(false);
            this.gbOptions.PerformLayout();
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        private void txtOutputDir_TextChanged(object sender, EventArgs e)
        {
            this.CheckCommands();
        }

        public Ravioli.Explorer.ActionScope ActionScope
        {
            get
            {
                if (this.rbAllFiles.Checked)
                {
                    return Ravioli.Explorer.ActionScope.AllFiles;
                }
                if (this.rbSelectedFiles.Checked)
                {
                    return Ravioli.Explorer.ActionScope.SelectedFiles;
                }
                if (this.rbDisplayedFiles.Checked)
                {
                    return Ravioli.Explorer.ActionScope.DisplayedFiles;
                }
                if (!this.rbCurrentDir.Checked)
                {
                    throw new ApplicationException("Cannot determine action scope.");
                }
                return Ravioli.Explorer.ActionScope.CurrentDirectory;
            }
            set
            {
                if (value == Ravioli.Explorer.ActionScope.AllFiles)
                {
                    this.rbAllFiles.Checked = true;
                }
                else if (value == Ravioli.Explorer.ActionScope.SelectedFiles)
                {
                    this.rbSelectedFiles.Checked = true;
                }
                else if (value == Ravioli.Explorer.ActionScope.DisplayedFiles)
                {
                    this.rbDisplayedFiles.Checked = true;
                }
                else
                {
                    if (value != Ravioli.Explorer.ActionScope.CurrentDirectory)
                    {
                        throw new ApplicationException("Unknown action scope \"" + value + "\".");
                    }
                    this.rbCurrentDir.Checked = true;
                }
            }
        }

        public bool AllowCurrentDirectory
        {
            get
            {
                return this.rbCurrentDir.Enabled;
            }
            set
            {
                this.rbCurrentDir.Enabled = value;
                if (!this.rbCurrentDir.Enabled && this.rbCurrentDir.Checked)
                {
                    this.rbAllFiles.Checked = true;
                }
            }
        }

        public bool AllowDisplayedFiles
        {
            get
            {
                return this.rbDisplayedFiles.Enabled;
            }
            set
            {
                this.rbDisplayedFiles.Enabled = value;
                if (!this.rbDisplayedFiles.Enabled && this.rbDisplayedFiles.Checked)
                {
                    this.rbAllFiles.Checked = true;
                }
            }
        }

        public bool AllowSelectedFiles
        {
            get
            {
                return this.rbSelectedFiles.Enabled;
            }
            set
            {
                this.rbSelectedFiles.Enabled = value;
                if (!this.rbSelectedFiles.Enabled && this.rbSelectedFiles.Checked)
                {
                    this.rbAllFiles.Checked = true;
                }
            }
        }

        public bool ConvertImages
        {
            get
            {
                return this.chkConvertImages.Checked;
            }
            set
            {
                this.chkConvertImages.Checked = value;
            }
        }

        public bool ConvertSounds
        {
            get
            {
                return this.chkConvertSounds.Checked;
            }
            set
            {
                this.chkConvertSounds.Checked = value;
            }
        }

        public bool CreateSubDirectory
        {
            get
            {
                return this.chkCreateSubDirectory.Checked;
            }
            set
            {
                this.chkCreateSubDirectory.Checked = value;
            }
        }

        public IList<ImagePluginMapping> ImageFormats
        {
            get
            {
                return this.imageFormats;
            }
            set
            {
                this.cboImageFormat.DataSource = null;
                this.cboImageFormat.DisplayMember = "";
                this.cboImageFormat.ValueMember = "";
                this.cboImageFormat.Items.Clear();
                this.cboImageFormat.DisplayMember = "TypeName";
                this.cboImageFormat.ValueMember = "PluginType";
                this.cboImageFormat.DataSource = value;
                this.imageFormats = value;
            }
        }

        public string OutputDirectory
        {
            get
            {
                return this.txtOutputDir.Text;
            }
            set
            {
                this.txtOutputDir.Text = value;
            }
        }

        public ImagePluginMapping SelectedImageFormat
        {
            get
            {
                return (ImagePluginMapping) this.cboImageFormat.SelectedItem;
            }
            set
            {
                this.cboImageFormat.SelectedItem = value;
            }
        }

        public SoundExportFormat SelectedSoundFormat
        {
            get
            {
                return (SoundExportFormat) this.cboSoundFormat.SelectedItem;
            }
            set
            {
                this.cboSoundFormat.SelectedItem = value;
            }
        }

        public IList<SoundExportFormat> SoundFormats
        {
            get
            {
                return this.soundFormats;
            }
            set
            {
                this.cboSoundFormat.DataSource = null;
                this.cboSoundFormat.DisplayMember = "";
                this.cboSoundFormat.ValueMember = "";
                this.cboSoundFormat.Items.Clear();
                this.cboSoundFormat.DisplayMember = "Name";
                this.cboSoundFormat.ValueMember = "Extension";
                this.cboSoundFormat.DataSource = value;
                this.soundFormats = value;
            }
        }
    }
}

