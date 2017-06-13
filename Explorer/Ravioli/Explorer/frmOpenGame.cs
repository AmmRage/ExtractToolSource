namespace Ravioli.Explorer
{
    using Ravioli.AppShared;
    using Ravioli.ArchiveInterface;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.IO;
    using System.Windows.Forms;

    public class frmOpenGame : Form
    {
        private Button btnBrowseDir;
        private Button btnCancel;
        private Button btnOK;
        private ComboBox cboInstallDir;
        private IContainer components;
        private FolderBrowserDialog folderBrowserDialog1;
        private string installDir;
        private Label lblGame;
        private Label lblInstallDir;
        private ListBox lstGame;
        private ArchivePluginManager pluginManager;
        private GameViewerPluginMapping selectedGame;

        public frmOpenGame()
        {
            this.InitializeComponent();
        }

        private void btnBrowseDir_Click(object sender, EventArgs e)
        {
            if (this.lstGame.SelectedItem != null)
            {
                this.folderBrowserDialog1.Description = "Select " + this.lstGame.Text + " installation directory.";
            }
            else
            {
                this.folderBrowserDialog1.Description = "Select installation directory.";
            }
            if (this.folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                this.cboInstallDir.Text = this.folderBrowserDialog1.SelectedPath;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (this.lstGame.SelectedValue == null)
            {
                this.ShowWarning("You need to select a game.");
                base.DialogResult = DialogResult.None;
            }
            else if (this.cboInstallDir.Text.Length == 0)
            {
                this.ShowWarning("You need to specify an installation directory.");
                base.DialogResult = DialogResult.None;
            }
            else if (!Directory.Exists(this.cboInstallDir.Text))
            {
                this.ShowWarning("The installation directory does not exist.");
                base.DialogResult = DialogResult.None;
            }
            else
            {
                GameListEntry selectedValue = (GameListEntry) this.lstGame.SelectedValue;
                this.selectedGame = selectedValue.Plugin;
                this.installDir = this.cboInstallDir.Text;
            }
        }

        private void cboInstallDir_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.CheckCommands();
        }

        private void cboInstallDir_TextChanged(object sender, EventArgs e)
        {
            this.CheckCommands();
        }

        private void CheckCommands()
        {
            this.btnOK.Enabled = ((this.lstGame.SelectedItem != null) && (this.cboInstallDir.Text.Length > 0)) && Directory.Exists(this.cboInstallDir.Text);
        }

        private static void CheckInstallation(ArchivePluginManager pluginManager, GameListEntry entry)
        {
            string[] defaultDirectories = entry.Plugin.DefaultDirectories;
            IArchive archive = null;
            for (int i = 0; i < defaultDirectories.Length; i++)
            {
                string path = defaultDirectories[i];
                if (Directory.Exists(path))
                {
                    if (archive == null)
                    {
                        archive = pluginManager.CreateInstance(entry.Plugin);
                    }
                    if (archive.IsValidFormat(FileSystemHelper.GetFileOfDirectory(path)))
                    {
                        entry.ExistsOnSystem = true;
                        entry.ExistsDirectoryIndex = i;
                        return;
                    }
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void frmOpenGame_Load(object sender, EventArgs e)
        {
            this.lstGame.SelectedItem = this.selectedGame;
            this.cboInstallDir.Text = this.installDir;
            foreach (GameListEntry entry in this.lstGame.Items)
            {
                Cursor.Current = Cursors.WaitCursor;
                try
                {
                    CheckInstallation(this.pluginManager, entry);
                }
                catch (IOException exception)
                {
                    Cursor.Current = Cursors.Default;
                    MessageBox.Show(this, "A file access error occurred while checking the existence of game \"" + entry.Name + "\": " + exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
                catch (Exception exception2)
                {
                    Cursor.Current = Cursors.Default;
                    MessageBox.Show(this, "An error occurred while checking the existence of game \"" + entry.Name + "\": " + exception2.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
            }
            Cursor.Current = Cursors.Default;
            this.CheckCommands();
        }

        private void InitializeComponent()
        {
            this.lblGame = new Label();
            this.lstGame = new ListBox();
            this.lblInstallDir = new Label();
            this.btnBrowseDir = new Button();
            this.btnOK = new Button();
            this.btnCancel = new Button();
            this.folderBrowserDialog1 = new FolderBrowserDialog();
            this.cboInstallDir = new ComboBox();
            base.SuspendLayout();
            this.lblGame.AutoSize = true;
            this.lblGame.Location = new Point(9, 9);
            this.lblGame.Name = "lblGame";
            this.lblGame.Size = new Size(0x45, 13);
            this.lblGame.TabIndex = 0;
            this.lblGame.Text = "&Select game:";
            this.lstGame.DrawMode = DrawMode.OwnerDrawFixed;
            this.lstGame.FormattingEnabled = true;
            this.lstGame.Location = new Point(12, 0x19);
            this.lstGame.Name = "lstGame";
            this.lstGame.Size = new Size(0x196, 0xee);
            this.lstGame.TabIndex = 1;
            this.lstGame.MouseDoubleClick += new MouseEventHandler(this.lstGame_MouseDoubleClick);
            this.lstGame.DrawItem += new DrawItemEventHandler(this.lstGame_DrawItem);
            this.lstGame.SelectedIndexChanged += new EventHandler(this.lstGame_SelectedIndexChanged);
            this.lblInstallDir.AutoSize = true;
            this.lblInstallDir.Location = new Point(9, 0x113);
            this.lblInstallDir.Name = "lblInstallDir";
            this.lblInstallDir.Size = new Size(0x67, 13);
            this.lblInstallDir.TabIndex = 2;
            this.lblInstallDir.Text = "&Installation directory:";
            this.btnBrowseDir.Location = new Point(0x157, 0x121);
            this.btnBrowseDir.Name = "btnBrowseDir";
            this.btnBrowseDir.Size = new Size(0x4b, 0x17);
            this.btnBrowseDir.TabIndex = 4;
            this.btnBrowseDir.Text = "&Browse...";
            this.btnBrowseDir.UseVisualStyleBackColor = true;
            this.btnBrowseDir.Click += new EventHandler(this.btnBrowseDir_Click);
            this.btnOK.DialogResult = DialogResult.OK;
            this.btnOK.Location = new Point(0x106, 0x153);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new Size(0x4b, 0x17);
            this.btnOK.TabIndex = 5;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new EventHandler(this.btnOK_Click);
            this.btnCancel.DialogResult = DialogResult.Cancel;
            this.btnCancel.Location = new Point(0x157, 0x153);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new Size(0x4b, 0x17);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.folderBrowserDialog1.ShowNewFolderButton = false;
            this.cboInstallDir.FormattingEnabled = true;
            this.cboInstallDir.Location = new Point(12, 0x123);
            this.cboInstallDir.Name = "cboInstallDir";
            this.cboInstallDir.Size = new Size(0x145, 0x15);
            this.cboInstallDir.TabIndex = 3;
            this.cboInstallDir.SelectedIndexChanged += new EventHandler(this.cboInstallDir_SelectedIndexChanged);
            this.cboInstallDir.TextChanged += new EventHandler(this.cboInstallDir_TextChanged);
            base.AcceptButton = this.btnOK;
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.CancelButton = this.btnCancel;
            base.ClientSize = new Size(430, 0x176);
            base.Controls.Add(this.cboInstallDir);
            base.Controls.Add(this.btnCancel);
            base.Controls.Add(this.btnOK);
            base.Controls.Add(this.btnBrowseDir);
            base.Controls.Add(this.lblInstallDir);
            base.Controls.Add(this.lstGame);
            base.Controls.Add(this.lblGame);
            base.FormBorderStyle = FormBorderStyle.FixedDialog;
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            base.Name = "frmOpenGame";
            base.ShowInTaskbar = false;
            base.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Open Game";
            base.Load += new EventHandler(this.frmOpenGame_Load);
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        private void lstGame_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index >= 0)
            {
                Graphics graphics = e.Graphics;
                GameListEntry entry = (GameListEntry) this.lstGame.Items[e.Index];
                if (entry.ExistsOnSystem && ((e.State & DrawItemState.Selected) != DrawItemState.Selected))
                {
                    graphics.FillRectangle(new SolidBrush(Color.LightGreen), e.Bounds);
                    graphics.DrawString(entry.Name, e.Font, new SolidBrush(Color.Black), e.Bounds);
                }
                else
                {
                    e.DrawBackground();
                    graphics.DrawString(entry.Name, e.Font, new SolidBrush(e.ForeColor), e.Bounds);
                }
                e.DrawFocusRectangle();
            }
        }

        private void lstGame_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.btnOK.Enabled)
            {
                base.DialogResult = DialogResult.OK;
                this.btnOK_Click(this, EventArgs.Empty);
            }
        }

        private void lstGame_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.lstGame.SelectedValue != null)
            {
                GameListEntry selectedValue = (GameListEntry) this.lstGame.SelectedValue;
                this.cboInstallDir.Items.Clear();
                if (selectedValue.Plugin.DefaultDirectories != null)
                {
                    this.cboInstallDir.Items.AddRange(selectedValue.Plugin.DefaultDirectories);
                }
                if (selectedValue.ExistsOnSystem)
                {
                    this.cboInstallDir.Text = (string) this.cboInstallDir.Items[selectedValue.ExistsDirectoryIndex];
                }
                else
                {
                    this.cboInstallDir.Text = (this.cboInstallDir.Items.Count > 0) ? ((string) this.cboInstallDir.Items[0]) : string.Empty;
                }
            }
            this.CheckCommands();
        }

        public void SetGames(IList<GameViewerPluginMapping> games)
        {
            this.lstGame.DataSource = null;
            List<GameListEntry> list = new List<GameListEntry>();
            foreach (GameViewerPluginMapping mapping in games)
            {
                list.Add(new GameListEntry(mapping.TypeName, mapping));
            }
            this.lstGame.DisplayMember = "Name";
            this.lstGame.ValueMember = "Game";
            this.lstGame.DataSource = list;
        }

        private void ShowWarning(string message)
        {
            MessageBox.Show(this, message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        public string InstallDir
        {
            get
            {
                return this.installDir;
            }
        }

        public ArchivePluginManager PluginManager
        {
            get
            {
                return this.pluginManager;
            }
            set
            {
                this.pluginManager = value;
            }
        }

        public GameViewerPluginMapping SelectedGame
        {
            get
            {
                return this.selectedGame;
            }
        }
    }
}

