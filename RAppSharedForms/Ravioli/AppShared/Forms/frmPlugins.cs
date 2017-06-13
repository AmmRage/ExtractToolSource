    using Ravioli.AppShared;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Text;
    using System.Windows.Forms;
namespace Ravioli.AppShared.Forms
{

    public class frmPlugins : Form
    {
        private Button btnClose;
        private IContainer components;
        private IEnumerable<PluginMetadataWithCategory> pluginMetadata;
        private TabControl tabControl1;

        public frmPlugins()
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

        private void frmPlugins_Load(object sender, EventArgs e)
        {
            if (this.pluginMetadata != null)
            {
                int num = 1;
                foreach (PluginMetadataWithCategory category in this.pluginMetadata)
                {
                    TabPage page = SetupPluginTabPage(category.Category);
                    ListView view = this.SetupPluginListView();
                    num++;
                    foreach (PluginMetadata metadata in category.Data)
                    {
                        ListViewItem item = new ListViewItem(new string[] { metadata.Name, this.MakeArrayString(metadata.Extensions), metadata.File });
                        view.Items.Add(item);
                    }
                    page.Controls.Add(view);
                    this.tabControl1.TabPages.Add(page);
                }
            }
        }

        private void InitializeComponent()
        {
            this.btnClose = new Button();
            this.tabControl1 = new TabControl();
            base.SuspendLayout();
            this.btnClose.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            this.btnClose.DialogResult = DialogResult.Cancel;
            this.btnClose.Location = new Point(0x18d, 0x11a);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new Size(0x4b, 0x17);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.tabControl1.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
            this.tabControl1.Location = new Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new Size(460, 0x108);
            this.tabControl1.TabIndex = 0;
            base.AcceptButton = this.btnClose;
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.CancelButton = this.btnClose;
            base.ClientSize = new Size(0x1e4, 0x13c);
            base.Controls.Add(this.btnClose);
            base.Controls.Add(this.tabControl1);
            base.FormBorderStyle = FormBorderStyle.FixedDialog;
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            base.Name = "frmPlugins";
            base.ShowInTaskbar = false;
            base.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Plug-in information";
            base.Load += new EventHandler(this.frmPlugins_Load);
            base.ResumeLayout(false);
        }

        private string MakeArrayString(string[] array)
        {
            StringBuilder builder = new StringBuilder();
            if (array != null)
            {
                foreach (string str in array)
                {
                    if (builder.Length > 0)
                    {
                        builder.Append(", ");
                    }
                    builder.Append(str);
                }
            }
            return builder.ToString();
        }

        public void SetPluginMetadata(IEnumerable<PluginMetadataWithCategory> pluginMetadata)
        {
            this.pluginMetadata = pluginMetadata;
        }

        private ListView SetupPluginListView()
        {
            ListView view = new ListView();
            ColumnHeader header = new ColumnHeader {
                Text = "Name",
                Width = 200
            };
            ColumnHeader header2 = new ColumnHeader {
                Text = "Extensions",
                Width = 100
            };
            ColumnHeader header3 = new ColumnHeader {
                Text = "File",
                Width = 120
            };
            view.Columns.AddRange(new ColumnHeader[] { header, header2, header3 });
            view.Dock = DockStyle.Fill;
            view.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            view.Location = new Point(3, 3);
            view.MultiSelect = false;
            view.Size = new Size(0x1be, 0xe8);
            view.TabIndex = 0;
            view.UseCompatibleStateImageBehavior = false;
            view.View = View.Details;
            return view;
        }

        private static TabPage SetupPluginTabPage(string text)
        {
            return new TabPage(text) { Location = new Point(4, 0x16), Padding = new Padding(3), Size = new Size(0x1c4, 0xee), UseVisualStyleBackColor = true };
        }
    }
}

