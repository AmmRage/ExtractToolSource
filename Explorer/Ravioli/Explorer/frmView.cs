namespace Ravioli.Explorer
{
    using Ravioli.AppShared.Forms;
    using Ravioli.ArchiveInterface;
    using Ravioli.Explorer.Properties;
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Windows.Forms;

    internal class frmView : Form
    {
        private bool allowZoom;
        private Button btnClose;
        private ToolStripButton btnPlay;
        private ToolStripButton btnSave;
        private ToolStripButton btnStop;
        private ToolStripButton btnZoomIn;
        private ToolStripButton btnZoomOriginalSize;
        private ToolStripButton btnZoomOut;
        private ToolStripButton btnZoomToFit;
        private IContainer components;
        private IFileInfo file;
        private IImageLoader imageLoader;
        private Image imageOriginal;
        private ImageZoomer imageZoomer;
        private Label lblDetails;
        private Label lblZoomInfo;
        private ISoundPlayer soundPlayer;
        private PictureBox viewPictureBox;
        private RichTextBox viewRichTextBox;
        private ToolStripSeparator viewSeparator;
        private ToolStrip viewToolStrip;
        private WindowSettings windowSettings;

        public event SaveViewEventHandler SaveView;

        public frmView()
        {
            this.InitializeComponent();
            this.lblDetails.Text = string.Empty;
            this.imageZoomer = null;
            this.allowZoom = false;
            this.viewSeparator.Visible = false;
            this.btnZoomIn.Visible = false;
            this.btnZoomIn.Enabled = false;
            this.btnZoomOut.Visible = false;
            this.btnZoomOut.Enabled = false;
            this.btnZoomOriginalSize.Visible = false;
            this.btnZoomOriginalSize.Enabled = false;
            this.btnZoomToFit.Visible = false;
            this.btnZoomToFit.Enabled = false;
            this.btnPlay.Visible = false;
            this.btnPlay.Enabled = false;
            this.btnStop.Visible = false;
            this.btnStop.Enabled = false;
        }

        public void AppendText(string text)
        {
            this.viewRichTextBox.BringToFront();
            this.viewRichTextBox.Visible = true;
            this.viewPictureBox.Visible = false;
            this.viewRichTextBox.AppendText(text);
            this.lblDetails.Text = string.Empty;
            this.btnSave.Enabled = false;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if (this.viewPictureBox.Image != null)
            {
                this.viewPictureBox.Image.Dispose();
                this.viewPictureBox.Image = null;
            }
            base.Close();
        }

        private void btnClose_KeyDown(object sender, KeyEventArgs e)
        {
            this.HandleZoomKeyDownEvent(e);
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            if (this.soundPlayer != null)
            {
                this.soundPlayer.Play();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (this.SaveView != null)
            {
                this.SaveView(this, new SaveViewEventArgs(this.file, this.imageOriginal, this.imageLoader, this.soundPlayer, this.soundPlayer as ISoundExport));
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (this.soundPlayer != null)
            {
                this.soundPlayer.Stop();
            }
        }

        private void btnZoomIn_Click(object sender, EventArgs e)
        {
            if ((this.imageZoomer != null) && this.allowZoom)
            {
                this.imageZoomer.ZoomImageIn();
            }
        }

        private void btnZoomOriginalSize_Click(object sender, EventArgs e)
        {
            if ((this.imageZoomer != null) && this.allowZoom)
            {
                this.imageZoomer.ZoomImageOriginalSize();
            }
        }

        private void btnZoomOut_Click(object sender, EventArgs e)
        {
            if ((this.imageZoomer != null) && this.allowZoom)
            {
                this.imageZoomer.ZoomImageOut();
            }
        }

        private void btnZoomToFit_Click(object sender, EventArgs e)
        {
            if ((this.imageZoomer != null) && this.allowZoom)
            {
                this.imageZoomer.ZoomImageToFit();
            }
        }

        public void Clear()
        {
            this.viewRichTextBox.Clear();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void frmView_Load(object sender, EventArgs e)
        {
            if (this.windowSettings != null)
            {
                WindowSettings.SetWindowSettings(this, this.windowSettings);
            }
        }

        private void frmView_Resize(object sender, EventArgs e)
        {
            if ((this.imageZoomer != null) && (this.imageZoomer.Percent < 0))
            {
                this.imageZoomer.FitImage();
            }
        }

        private void frmView_Shown(object sender, EventArgs e)
        {
            try
            {
                if (this.soundPlayer != null)
                {
                    this.soundPlayer.Play();
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(this, exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                base.Close();
            }
        }

        private void HandleZoomKeyDownEvent(KeyEventArgs e)
        {
            if ((this.imageZoomer != null) && this.allowZoom)
            {
                if (e.KeyCode == Keys.Add)
                {
                    this.imageZoomer.ZoomImageIn();
                    e.SuppressKeyPress = true;
                }
                else if (e.KeyCode == Keys.Subtract)
                {
                    this.imageZoomer.ZoomImageOut();
                    e.SuppressKeyPress = true;
                }
                else if (e.KeyCode == Keys.Multiply)
                {
                    this.imageZoomer.ZoomImageToFit();
                    e.SuppressKeyPress = true;
                }
                else if (e.KeyCode == Keys.Divide)
                {
                    this.imageZoomer.ZoomImageOriginalSize();
                    e.SuppressKeyPress = true;
                }
            }
        }

        private void InitializeComponent()
        {
            ComponentResourceManager manager = new ComponentResourceManager(typeof(frmView));
            this.viewRichTextBox = new RichTextBox();
            this.btnClose = new Button();
            this.lblDetails = new Label();
            this.lblZoomInfo = new Label();
            this.viewToolStrip = new ToolStrip();
            this.btnSave = new ToolStripButton();
            this.viewSeparator = new ToolStripSeparator();
            this.btnZoomIn = new ToolStripButton();
            this.btnZoomOut = new ToolStripButton();
            this.btnZoomOriginalSize = new ToolStripButton();
            this.btnZoomToFit = new ToolStripButton();
            this.btnPlay = new ToolStripButton();
            this.btnStop = new ToolStripButton();
            this.viewPictureBox = new PictureBox();
            this.viewToolStrip.SuspendLayout();
            ((ISupportInitialize) this.viewPictureBox).BeginInit();
            base.SuspendLayout();
            this.viewRichTextBox.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
            this.viewRichTextBox.BackColor = SystemColors.Window;
            this.viewRichTextBox.Font = new Font("Lucida Console", 9.75f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.viewRichTextBox.Location = new Point(5, 0x1c);
            this.viewRichTextBox.Margin = new Padding(0);
            this.viewRichTextBox.Name = "viewRichTextBox";
            this.viewRichTextBox.ReadOnly = true;
            this.viewRichTextBox.Size = new Size(0x278, 0x17f);
            this.viewRichTextBox.TabIndex = 0;
            this.viewRichTextBox.Text = "";
            this.viewRichTextBox.LinkClicked += new LinkClickedEventHandler(this.rchText_LinkClicked);
            this.btnClose.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            this.btnClose.DialogResult = DialogResult.Cancel;
            this.btnClose.Location = new Point(0x22d, 0x19f);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new Size(0x4b, 0x17);
            this.btnClose.TabIndex = 0;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new EventHandler(this.btnClose_Click);
            this.btnClose.KeyDown += new KeyEventHandler(this.btnClose_KeyDown);
            this.lblDetails.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
            this.lblDetails.AutoSize = true;
            this.lblDetails.Location = new Point(12, 420);
            this.lblDetails.Name = "lblDetails";
            this.lblDetails.Size = new Size(0x3f, 13);
            this.lblDetails.TabIndex = 2;
            this.lblDetails.Text = "<<Details>>";
            this.lblZoomInfo.AutoSize = true;
            this.lblZoomInfo.BackColor = Color.Black;
            this.lblZoomInfo.ForeColor = Color.White;
            this.lblZoomInfo.Location = new Point(5, 0x1c);
            this.lblZoomInfo.Name = "lblZoomInfo";
            this.lblZoomInfo.Size = new Size(0x21, 13);
            this.lblZoomInfo.TabIndex = 5;
            this.lblZoomInfo.Text = "100%";
            this.lblZoomInfo.Visible = false;
            this.viewToolStrip.Items.AddRange(new ToolStripItem[] { this.btnSave, this.viewSeparator, this.btnZoomIn, this.btnZoomOut, this.btnZoomOriginalSize, this.btnZoomToFit, this.btnPlay, this.btnStop });
            this.viewToolStrip.Location = new Point(0, 0);
            this.viewToolStrip.Name = "viewToolStrip";
            this.viewToolStrip.RenderMode = ToolStripRenderMode.System;
            this.viewToolStrip.Size = new Size(0x284, 0x19);
            this.viewToolStrip.TabIndex = 6;
            this.viewToolStrip.Text = "viewToolStrip";
            this.btnSave.Image = Resources.SaveHS;
            this.btnSave.ImageTransparentColor = Color.Magenta;
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new Size(60, 0x16);
            this.btnSave.Text = "&Save...";
            this.btnSave.Click += new EventHandler(this.btnSave_Click);
            this.viewSeparator.Name = "viewSeparator";
            this.viewSeparator.Size = new Size(6, 0x19);
            this.btnZoomIn.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.btnZoomIn.Image = Resources.ZoomIn;
            this.btnZoomIn.ImageTransparentColor = Color.Magenta;
            this.btnZoomIn.Name = "btnZoomIn";
            this.btnZoomIn.Size = new Size(0x17, 0x16);
            this.btnZoomIn.Text = "Zoom &in";
            this.btnZoomIn.ToolTipText = "Zoom in (Numpad +)";
            this.btnZoomIn.Click += new EventHandler(this.btnZoomIn_Click);
            this.btnZoomOut.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.btnZoomOut.Image = Resources.ZoomOut;
            this.btnZoomOut.ImageTransparentColor = Color.Magenta;
            this.btnZoomOut.Name = "btnZoomOut";
            this.btnZoomOut.Size = new Size(0x17, 0x16);
            this.btnZoomOut.Text = "Zoom &out";
            this.btnZoomOut.ToolTipText = "Zoom out (Numpad -)";
            this.btnZoomOut.Click += new EventHandler(this.btnZoomOut_Click);
            this.btnZoomOriginalSize.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.btnZoomOriginalSize.Image = Resources.ZoomOriginalSize;
            this.btnZoomOriginalSize.ImageTransparentColor = Color.Magenta;
            this.btnZoomOriginalSize.Name = "btnZoomOriginalSize";
            this.btnZoomOriginalSize.Size = new Size(0x17, 0x16);
            this.btnZoomOriginalSize.Text = "Original &size";
            this.btnZoomOriginalSize.ToolTipText = "Original size (Numpad /)";
            this.btnZoomOriginalSize.Click += new EventHandler(this.btnZoomOriginalSize_Click);
            this.btnZoomToFit.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.btnZoomToFit.Image = Resources.ZoomFit;
            this.btnZoomToFit.ImageTransparentColor = Color.Magenta;
            this.btnZoomToFit.Name = "btnZoomToFit";
            this.btnZoomToFit.Size = new Size(0x17, 0x16);
            this.btnZoomToFit.Text = "&Fit image";
            this.btnZoomToFit.ToolTipText = "Fit image (Numpad *)";
            this.btnZoomToFit.Click += new EventHandler(this.btnZoomToFit_Click);
            this.btnPlay.Image = Resources.PlayHS;
            this.btnPlay.ImageTransparentColor = Color.Magenta;
            this.btnPlay.Name = "btnPlay";
            this.btnPlay.Size = new Size(0x31, 0x16);
            this.btnPlay.Text = "P&lay";
            this.btnPlay.Click += new EventHandler(this.btnPlay_Click);
            this.btnStop.Image = Resources.StopHS;
            this.btnStop.ImageTransparentColor = Color.Magenta;
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new Size(0x33, 0x16);
            this.btnStop.Text = "S&top";
            this.btnStop.Click += new EventHandler(this.btnStop_Click);
            this.viewPictureBox.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
            this.viewPictureBox.Location = new Point(5, 0x1c);
            this.viewPictureBox.Name = "viewPictureBox";
            this.viewPictureBox.Size = new Size(0x278, 0x17f);
            this.viewPictureBox.TabIndex = 2;
            this.viewPictureBox.TabStop = false;
            base.AcceptButton = this.btnClose;
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.CancelButton = this.btnClose;
            base.ClientSize = new Size(0x284, 0x1be);
            base.Controls.Add(this.viewToolStrip);
            base.Controls.Add(this.lblZoomInfo);
            base.Controls.Add(this.viewPictureBox);
            base.Controls.Add(this.btnClose);
            base.Controls.Add(this.viewRichTextBox);
            base.Controls.Add(this.lblDetails);
            base.Icon = (Icon) manager.GetObject("$this.Icon");
            base.MinimizeBox = false;
            base.Name = "frmView";
            base.ShowInTaskbar = false;
            base.StartPosition = FormStartPosition.CenterParent;
            this.Text = "View";
            base.Load += new EventHandler(this.frmView_Load);
            base.Shown += new EventHandler(this.frmView_Shown);
            base.Resize += new EventHandler(this.frmView_Resize);
            this.viewToolStrip.ResumeLayout(false);
            this.viewToolStrip.PerformLayout();
            ((ISupportInitialize) this.viewPictureBox).EndInit();
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        private void rchText_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            Process.Start(e.LinkText);
        }

        public void ResetCursorPosition()
        {
            this.viewRichTextBox.SelectionStart = 0;
        }

        public void SetTextDetails(string details)
        {
            this.lblDetails.Text = details;
        }

        public void SetWindowSettings(WindowSettings settings)
        {
            if (base.Created)
            {
                WindowSettings.SetWindowSettings(this, settings);
            }
            else
            {
                this.windowSettings = settings;
            }
        }

        public void ShowImage(IFileInfo file, Image image, string detailInfo, IImageLoader imageLoader)
        {
            this.file = file;
            this.imageOriginal = image;
            this.imageLoader = imageLoader;
            this.ShowImageInternal(image, detailInfo);
            this.btnSave.Enabled = true;
            this.allowZoom = true;
            this.viewSeparator.Visible = true;
            this.btnZoomIn.Visible = true;
            this.btnZoomIn.Enabled = true;
            this.btnZoomOut.Visible = true;
            this.btnZoomOut.Enabled = true;
            this.btnZoomOriginalSize.Visible = true;
            this.btnZoomOriginalSize.Enabled = true;
            this.btnZoomToFit.Visible = true;
            this.btnZoomToFit.Enabled = true;
        }

        private void ShowImageInternal(Image image, string detailInfo)
        {
            this.viewPictureBox.BringToFront();
            this.viewPictureBox.Visible = true;
            this.viewRichTextBox.Visible = false;
            if (this.viewPictureBox.Image != null)
            {
                this.viewPictureBox.Image.Dispose();
                this.viewPictureBox.Image = null;
            }
            this.imageZoomer = new ImageZoomer(image, this.viewPictureBox, this.lblZoomInfo, -1);
            this.lblDetails.Text = detailInfo;
        }

        public void ShowSound(IFileInfo file, ISoundPlayer soundPlayer, string detailInfo, Image placeHolderImage)
        {
            this.file = file;
            this.soundPlayer = soundPlayer;
            this.ShowImageInternal(placeHolderImage, detailInfo);
            this.btnSave.Enabled = soundPlayer is ISoundExport;
            this.allowZoom = false;
            this.viewSeparator.Visible = true;
            this.btnPlay.Visible = true;
            this.btnPlay.Enabled = true;
            this.btnStop.Visible = true;
            this.btnStop.Enabled = true;
        }

        internal class SaveViewEventArgs : EventArgs
        {
            private IFileInfo file;
            private System.Drawing.Image image;
            private IImageLoader imageLoader;
            private ISoundExport soundExport;
            private ISoundPlayer soundPlayer;

            public SaveViewEventArgs(IFileInfo file, System.Drawing.Image image, IImageLoader imageLoader, ISoundPlayer soundPlayer, ISoundExport soundExport)
            {
                this.file = file;
                this.image = image;
                this.imageLoader = imageLoader;
                this.soundPlayer = soundPlayer;
                this.soundExport = soundExport;
            }

            public IFileInfo File
            {
                get
                {
                    return this.file;
                }
            }

            public System.Drawing.Image Image
            {
                get
                {
                    return this.image;
                }
            }

            public IImageLoader ImageLoader
            {
                get
                {
                    return this.imageLoader;
                }
            }

            public ISoundExport SoundExport
            {
                get
                {
                    return this.soundExport;
                }
            }

            public ISoundPlayer SoundPlayer
            {
                get
                {
                    return this.soundPlayer;
                }
            }
        }

        internal delegate void SaveViewEventHandler(object sender, frmView.SaveViewEventArgs e);
    }
}

