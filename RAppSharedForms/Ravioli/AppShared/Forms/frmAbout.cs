    using Ravioli.AppShared.Forms.Properties;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Reflection;
    using System.Windows.Forms;
namespace Ravioli.AppShared.Forms
{

    public class frmAbout : Form
    {
        private Button btnClose;
        private Button btnPlugins;
        private IContainer components;
        private Label lblAuthor;
        private Label lblName;
        private Label lblVersion;
        private PictureBox LogoPictureBox;
        private IEnumerable<PluginMetadataWithCategory> pluginMetadata;

        public frmAbout()
        {
            this.InitializeComponent();
        }

        private void btnPlugins_Click(object sender, EventArgs e)
        {
            frmPlugins plugins = new frmPlugins();
            plugins.SetPluginMetadata(this.pluginMetadata);
            plugins.ShowDialog();
            plugins.Dispose();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void FillAboutBox()
        {
            Assembly entryAssembly = Assembly.GetEntryAssembly();
            string title = "A Ravioli Game Tools application";
            AssemblyTitleAttribute[] customAttributes = (AssemblyTitleAttribute[]) entryAssembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
            if (customAttributes.Length > 0)
            {
                title = customAttributes[0].Title;
            }
            string str2 = "Version " + entryAssembly.GetName().Version.ToString(2);
            string copyright = "Copyright information missing";
            AssemblyCopyrightAttribute[] attributeArray2 = (AssemblyCopyrightAttribute[]) entryAssembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
            if (attributeArray2.Length > 0)
            {
                copyright = attributeArray2[0].Copyright;
            }
            this.lblName.Text = title;
            this.lblVersion.Text = str2;
            this.lblAuthor.Text = copyright;
            this.btnPlugins.Enabled = this.pluginMetadata != null;
        }

        private void frmAbout_Load(object sender, EventArgs e)
        {
            this.FillAboutBox();
        }

        private void InitializeComponent()
        {
            this.lblName = new Label();
            this.lblVersion = new Label();
            this.lblAuthor = new Label();
            this.btnPlugins = new Button();
            this.btnClose = new Button();
            this.LogoPictureBox = new PictureBox();
            ((ISupportInitialize) this.LogoPictureBox).BeginInit();
            base.SuspendLayout();
            this.lblName.Location = new Point(0x57, 0x13);
            this.lblName.Name = "lblName";
            this.lblName.Size = new Size(0xcd, 0x1b);
            this.lblName.TabIndex = 2;
            this.lblName.Text = "<<Name>>";
            this.lblVersion.Location = new Point(0x57, 0x2e);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new Size(0xcd, 0x1b);
            this.lblVersion.TabIndex = 3;
            this.lblVersion.Text = "<<Version>>";
            this.lblAuthor.Location = new Point(0x57, 0x49);
            this.lblAuthor.Name = "lblAuthor";
            this.lblAuthor.Size = new Size(0xcd, 0x1b);
            this.lblAuthor.TabIndex = 4;
            this.lblAuthor.Text = "<<Author>>";
            this.btnPlugins.Location = new Point(0x86, 0x6f);
            this.btnPlugins.Name = "btnPlugins";
            this.btnPlugins.Size = new Size(0x76, 0x17);
            this.btnPlugins.TabIndex = 1;
            this.btnPlugins.Text = "&Plug-in information...";
            this.btnPlugins.UseVisualStyleBackColor = true;
            this.btnPlugins.Click += new EventHandler(this.btnPlugins_Click);
            this.btnClose.DialogResult = DialogResult.Cancel;
            this.btnClose.Location = new Point(0x35, 0x6f);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new Size(0x4b, 0x17);
            this.btnClose.TabIndex = 0;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.LogoPictureBox.Image = Resources.Ravioli48x48_32bit;
            this.LogoPictureBox.Location = new Point(12, 0x13);
            this.LogoPictureBox.Name = "LogoPictureBox";
            this.LogoPictureBox.Size = new Size(60, 60);
            this.LogoPictureBox.SizeMode = PictureBoxSizeMode.CenterImage;
            this.LogoPictureBox.TabIndex = 5;
            this.LogoPictureBox.TabStop = false;
            base.AcceptButton = this.btnClose;
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.CancelButton = this.btnClose;
            base.ClientSize = new Size(0x130, 0x97);
            base.Controls.Add(this.LogoPictureBox);
            base.Controls.Add(this.btnClose);
            base.Controls.Add(this.btnPlugins);
            base.Controls.Add(this.lblName);
            base.Controls.Add(this.lblAuthor);
            base.Controls.Add(this.lblVersion);
            base.FormBorderStyle = FormBorderStyle.FixedDialog;
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            base.Name = "frmAbout";
            base.ShowInTaskbar = false;
            base.StartPosition = FormStartPosition.CenterParent;
            this.Text = "About";
            base.Load += new EventHandler(this.frmAbout_Load);
            ((ISupportInitialize) this.LogoPictureBox).EndInit();
            base.ResumeLayout(false);
        }

        public void SetPluginMetadata(IEnumerable<PluginMetadataWithCategory> pluginMetadata)
        {
            this.pluginMetadata = pluginMetadata;
            this.btnPlugins.Enabled = this.pluginMetadata != null;
        }
    }
}

