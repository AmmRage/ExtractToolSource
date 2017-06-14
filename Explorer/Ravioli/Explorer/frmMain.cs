namespace Ravioli.Explorer
{
    using Ravioli.AppShared;
    using Ravioli.AppShared.Forms;
    using Ravioli.ArchiveInterface;
    using Ravioli.Explorer.Properties;
    using Ravioli.Scanner;
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading;
    using System.Windows.Forms;
    using System.Xml.Serialization;

    internal class frmMain : Form
    {
        private bool allowPreviewZoom;
        private ArchivePluginManager archivePluginManager;
        private StatusStrip archiveStatus;
        private SplitContainer browseOutputSplitter;
        private SplitContainer browsePreviewSplitter;
        private SplitContainer browserSplitter;
        private ToolStripButton btnAbout;
        private ToolStripButton btnExtract;
        private ToolStripDropDownButton btnFilter;
        private ToolStripButton btnGoTo;
        private ToolStripButton btnOpen;
        private ToolStripButton btnOpenGame;
        private ToolStripDropDownButton btnOptions;
        private ToolStripButton btnPlayPreview;
        private ToolStripButton btnSavePreview;
        private ToolStripButton btnStop;
        private ToolStripButton btnStopPreview;
        private ToolStripSplitButton btnView;
        private ToolStripMenuItem btnViewAsText;
        private ToolStripButton btnZoomOriginalSize;
        private ToolStripButton btnZoomPreviewIn;
        private ToolStripButton btnZoomPreviewOut;
        private ToolStripButton btnZoomToFit;
        private ListViewItem[] cachedListViewItems;
        private bool closePending;
        private ColumnHeader colCompressed;
        private ColumnHeader colCompressedSize;
        private ColumnHeader colName;
        private ColumnHeader colPath;
        private ColumnHeader colSize;
        private ColumnHeader colTypeName;
        private IContainer components;
        private ExplorerConfig config;
        private ContextMenuStrip contextMenu;
        private IArchive currentFile;
        private const string defaultFilterString = "&Filter";
        private string defaultImageSaveFilter;
        private int defaultImageSaveFilterIndex;
        private const int defaultUnlockedImageZoom = -1;
        private ImageList directoryImageList;
        private const int displayThumbHeight = 0x60;
        private const int displayThumbWidth = 0x60;
        private IFileInfo[] dragOutFileInfo;
        private BaseExtractor extractor;
        private InputFileInfoCache fileInfoCache;
        private FileListFilters fileListFilters;
        private SortCriterionMapCollection fileListSortCriterionMaps;
        private ImageList fileTypeImageList;
        private ImageList filterImageList;
        private ImagePluginManager imagePluginManager;
        private ToolStripStatusLabel lblArchiveType;
        private ToolStripStatusLabel lblCurrentAction;
        private ToolStripStatusLabel lblFileCount;
        private ToolStripLabel lblPath;
        private ToolStripStatusLabel lblPreviewStatus;
        private ToolStripStatusLabel lblTotalSize;
        private Label lblZoomInfo;
        private ListViewColumnSorter listViewColumnSorter;
        private ListViewDoubleSorter listViewLongSorter;
        private ListViewCaseInsensitiveSorter listViewTextSorter;
        private ListView lvwInfo;
        private ToolStrip mainToolStrip;
        private ToolStripMenuItem mnuAllowScanning;
        private ToolStripMenuItem mnuAskBeforeScanning;
        private ToolStripMenuItem mnuAutoPlaySounds;
        private ToolStripMenuItem mnuDirectoryDisplayFlat;
        private ToolStripMenuItem mnuDirectoryDisplayHierarchical;
        private ToolStripMenuItem mnuDirectoryDisplayMode;
        private ToolStripMenuItem mnuExtract;
        private ToolStripMenuItem mnuFileListDisplayDetails;
        private ToolStripMenuItem mnuFileListDisplayMode;
        private ToolStripMenuItem mnuFileListDisplayThumbnails;
        private ToolStripMenuItem mnuLockImageZoom;
        private ToolStripMenuItem mnuOpenFile;
        private ToolStripMenuItem mnuPlayPreview;
        private ToolStripMenuItem mnuSortItemsBy;
        private ToolStripMenuItem mnuSortItemsByCompressed;
        private ToolStripMenuItem mnuSortItemsByCSize;
        private ToolStripMenuItem mnuSortItemsByName;
        private ToolStripMenuItem mnuSortItemsByPath;
        private ToolStripMenuItem mnuSortItemsBySize;
        private ToolStripMenuItem mnuSortItemsByType;
        private ToolStripMenuItem mnuStartupBehaviour;
        private ToolStripMenuItem mnuStartupBehaviourDoNothing;
        private ToolStripMenuItem mnuStartupBehaviourOpenFile;
        private ToolStripMenuItem mnuStartupBehaviourOpenGame;
        private ToolStripMenuItem mnuStopPreview;
        private ToolStripMenuItem mnuTogglePreview;
        private ToolStripMenuItem mnuToggleSoundPreview;
        private ToolStripMenuItem mnuUnknownFiles;
        private ToolStripMenuItem mnuView;
        private ToolStripMenuItem mnuViewAsText;
        private OpenFileDialog openFileDialog;
        private ToolStripFixedKeys pathToolStrip;
        private IFileInfo previewFile;
        private IImageLoader previewImageLoader;
        private Image previewImageOriginal;
        private int previewImageZoomDefault;
        private ImageZoomer previewImageZoomer;
        private PictureBox previewPictureBox;
        private RichTextBox previewRichTextBox;
        private ToolStripSeparator previewSeparator;
        private ISoundPlayer previewSoundPlayer;
        private StatusStrip previewStatus;
        private Stream previewStream;
        private ToolStrip previewToolStrip;
        private ToolStripMenuItem previewToolStripMenuItem;
        private SaveFileDialog saveFileDialog;
        private ScanPluginManager scanPluginManager;
        private SoundPluginManager soundPluginManager;
        private string startDocument;
        private StringCollection temporaryDirs;
        private const int thumbHeight = 80;
        private const int thumbSpacingHorizontal = 150;
        private const int thumbSpacingVertical = 150;
        private const int thumbWidth = 80;
        private ToolStripContainer toolStripContainer1;
        private ToolStripProgressBar toolStripProgressBar1;
        private string treeViewRootKey;
        private const int tryArchiveLockWaitTime = 0x4e20;
        private TreeView tvwDirectories;
        private ToolStripSpringTextBox txtPath;
        private TextBox txtProgress;
        private ISoundPlayer viewSoundPlayer;
        private StringCollection warningMessages;

        public frmMain()
        {
            this.InitializeComponent();
            this.currentFile = null;
            this.startDocument = string.Empty;
            this.temporaryDirs = new StringCollection();
            this.closePending = false;
            this.config = new ExplorerConfig();
            this.fileInfoCache = new InputFileInfoCache();
            this.fileListFilters = null;
            this.warningMessages = null;
            this.previewImageZoomer = null;
            this.allowPreviewZoom = false;
            this.previewImageZoomDefault = -1;
            this.fileListSortCriterionMaps = null;
            this.treeViewRootKey = string.Empty;
            this.cachedListViewItems = null;
        }

        private void AppendPreviewText(string text)
        {
            this.previewRichTextBox.BringToFront();
            this.previewRichTextBox.AppendText(text);
        }

        private void AppendText(string text)
        {
            if (this.txtProgress.InvokeRequired)
            {
                AppendTextCallback method = new AppendTextCallback(this.AppendText);
                base.BeginInvoke(method, new object[] { text });
            }
            else
            {
                this.txtProgress.AppendText(text);
            }
        }

        private IFileInfo[] ApplyFileListFilters()
        {
            List<IFileInfo> list = new List<IFileInfo>();
            foreach (IFileInfo info in this.currentFile.Files)
            {
                if ((this.fileListFilters != null) && this.fileListFilters.HasActiveFilters())
                {
                    if (this.fileListFilters.DirectoryFilter != null)
                    {
                        string directoryName = Path.GetDirectoryName(info.Name);
                        if (!directoryName.StartsWith(this.treeViewRootKey))
                        {
                            directoryName = this.treeViewRootKey + directoryName;
                        }
                        if (directoryName != this.fileListFilters.DirectoryFilter)
                        {
                            continue;
                        }
                    }
                    if (this.fileListFilters.ExtensionFilter != null)
                    {
                        bool flag = false;
                        foreach (string str2 in this.fileListFilters.ExtensionFilter)
                        {
                            if (Path.GetExtension(info.Name) == str2)
                            {
                                flag = true;
                                break;
                            }
                        }
                        if (!flag)
                        {
                            continue;
                        }
                    }
                }
                list.Add(info);
            }
            return list.ToArray();
        }

        private void ApplyFileListSortSettings(SortCriterion sortBy, SortOrder sortOrder)
        {
            if (sortOrder != SortOrder.None)
            {
                SortCriterionMap map = this.fileListSortCriterionMaps.MapSortCriterionToCriterionMap(sortBy);
                if ((map.Column.Index == this.colSize.Index) || (map.Column.Index == this.colCompressedSize.Index))
                {
                    this.listViewLongSorter.SortColumn = map.Column.Index;
                    this.listViewLongSorter.Order = sortOrder;
                    this.listViewColumnSorter = this.listViewLongSorter;
                }
                else
                {
                    this.listViewTextSorter.SortColumn = map.Column.Index;
                    this.listViewTextSorter.Order = sortOrder;
                    this.listViewColumnSorter = this.listViewTextSorter;
                }
                this.lvwInfo.ListViewItemSorter = this.listViewColumnSorter;
                ListViewExtensions.SetSortIcon(this.lvwInfo, this.listViewColumnSorter.SortColumn, this.listViewColumnSorter.Order);
                this.RefreshFileList(true);
                foreach (SortCriterionMap map2 in this.fileListSortCriterionMaps)
                {
                    if (map2.MenuItem != null)
                    {
                        map2.MenuItem.Checked = false;
                    }
                }
                map.MenuItem.Checked = true;
            }
        }

        private DialogResult AskAbortRetryIgnore(string message, string caption)
        {
            if (base.InvokeRequired)
            {
                return (DialogResult) base.Invoke(new AskAbortRetryIgnoreCallback(this.AskAbortRetryIgnore), new object[] { message, caption });
            }
            return MessageBox.Show(this, message, caption, MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Exclamation);
        }

        private bool AskForRootDirectory(ArchivePluginMapping pluginMapping, string inputFile, out string rootDirectory)
        {
            rootDirectory = string.Empty;
            if (pluginMapping.NeedsRootDirectory)
            {
                if (this.fileInfoCache.Exists(inputFile))
                {
                    InputFileInfo info = this.fileInfoCache.Find(inputFile);
                    rootDirectory = info.RootDirectory;
                }
                frmRootDirectory directory = new frmRootDirectory {
                    RootDirectory = rootDirectory,
                    InputFile = inputFile
                };
                if (directory.ShowDialog(this) != DialogResult.OK)
                {
                    return false;
                }
                rootDirectory = directory.RootDirectory;
                InputFileInfo item = null;
                if (this.fileInfoCache.Exists(inputFile))
                {
                    item = this.fileInfoCache.Find(inputFile);
                }
                else
                {
                    item = new InputFileInfo(inputFile);
                    this.fileInfoCache.Add(item);
                }
                item.RootDirectory = rootDirectory;
            }
            return true;
        }

        private DialogResult AskYesNo(string message, string caption)
        {
            return MessageBox.Show(this, message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        }

        private DialogResult AskYesNoWarning(string message, string caption)
        {
            return MessageBox.Show(this, message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
        }

        private void btnAbout_Click(object sender, EventArgs e)
        {
            List<PluginMetadataWithCategory> pluginMetadata = this.GetPluginMetadata();
            using (frmAbout about = new frmAbout())
            {
                about.SetPluginMetadata(pluginMetadata);
                about.ShowDialog();
            }
        }

        private void btnExtract_Click(object sender, EventArgs e)
        {
            this.ExtractFiles();
        }

        private void btnFilter_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            ToolStripMenuItem clickedItem = e.ClickedItem as ToolStripMenuItem;
            if (clickedItem != null)
            {
                if (!Monitor.TryEnter(this.currentFile, 0x4e20))
                {
                    this.ShowError("Cannot lock the archive.");
                }
                else
                {
                    try
                    {
                        if (this.IsPreviewActive())
                        {
                            this.ClearPreview();
                        }
                        foreach (ToolStripMenuItem item2 in this.btnFilter.DropDownItems)
                        {
                            item2.Checked = false;
                        }
                        clickedItem.Checked = true;
                        if (this.fileListFilters == null)
                        {
                            this.fileListFilters = new FileListFilters();
                        }
                        if (clickedItem.Tag != null)
                        {
                            StringCollection tag = (StringCollection) clickedItem.Tag;
                            this.fileListFilters.ExtensionFilter = tag;
                            this.RefreshFileList();
                            string extensionDisplay = MakeArrayString(tag);
                            this.btnFilter.Text = "&Filter: " + GetShortExtensionDisplay(extensionDisplay);
                            this.btnFilter.Image = this.filterImageList.Images[1];
                        }
                        else
                        {
                            this.fileListFilters.ExtensionFilter = null;
                            this.RefreshFileList();
                            this.btnFilter.Text = "&Filter";
                            this.btnFilter.Image = this.filterImageList.Images[0];
                        }
                    }
                    finally
                    {
                        Monitor.Exit(this.currentFile);
                    }
                    this.CheckCommands();
                }
            }
        }

        private void btnGoTo_Click(object sender, EventArgs e)
        {
            this.NavigateUserSpecifiedPath();
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            this.openFileDialog.FileName = "";
            if ((this.config.General.LastOpenDir.Length > 0) && Directory.Exists(this.config.General.LastOpenDir))
            {
                this.openFileDialog.InitialDirectory = this.config.General.LastOpenDir;
            }
            if (this.openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                this.config.General.LastOpenDir = Path.GetDirectoryName(this.openFileDialog.FileName);
                this.OpenFile(this.openFileDialog.FileName);
            }
        }

        private void btnOpenGame_Click(object sender, EventArgs e)
        {
            using (frmOpenGame game = new frmOpenGame())
            {
                game.SetGames(this.archivePluginManager.AvailableGameViewerPlugins);
                game.PluginManager = this.archivePluginManager;
                if (game.ShowDialog() == DialogResult.OK)
                {
                    this.OpenGame(game.InstallDir, game.SelectedGame);
                }
            }
        }

        private void btnPlayPreview_Click(object sender, EventArgs e)
        {
            if (this.previewSoundPlayer != null)
            {
                this.previewSoundPlayer.Play();
            }
        }

        private void btnSavePreview_Click(object sender, EventArgs e)
        {
            if (this.previewImageLoader != null)
            {
                this.SaveImage(this.previewFile, this.GetCurrentPreviewImage(), this.previewImageLoader);
            }
            else if (this.previewSoundPlayer != null)
            {
                this.ExportSound(this.previewFile, this.GetCurrentPreviewSoundExport(), this.previewSoundPlayer);
            }
            else
            {
                this.ShowWarning("Unknown media type in preview.");
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (this.extractor.IsRunning)
            {
                this.extractor.Abort();
            }
            if (this.previewSoundPlayer != null)
            {
                this.ClearPreview();
            }
        }

        private void btnStopPreview_Click(object sender, EventArgs e)
        {
            if (this.previewSoundPlayer != null)
            {
                this.previewSoundPlayer.Stop();
            }
        }

        private void btnView_ButtonClick(object sender, EventArgs e)
        {
            this.ViewSelectedFiles();
        }

        private void btnViewAsText_Click(object sender, EventArgs e)
        {
            this.ViewSelectedFiles(true);
        }

        private void btnZoomOriginalSize_Click(object sender, EventArgs e)
        {
            if ((this.previewImageZoomer != null) && this.allowPreviewZoom)
            {
                this.previewImageZoomer.ZoomImageOriginalSize();
                if (this.IsPreviewImageZoomLocked())
                {
                    this.previewImageZoomDefault = this.previewImageZoomer.Percent;
                }
            }
        }

        private void btnZoomPreviewIn_Click(object sender, EventArgs e)
        {
            if ((this.previewImageZoomer != null) && this.allowPreviewZoom)
            {
                this.previewImageZoomer.ZoomImageIn();
                if (this.IsPreviewImageZoomLocked())
                {
                    this.previewImageZoomDefault = this.previewImageZoomer.Percent;
                }
            }
        }

        private void btnZoomPreviewOut_Click(object sender, EventArgs e)
        {
            if ((this.previewImageZoomer != null) && this.allowPreviewZoom)
            {
                this.previewImageZoomer.ZoomImageOut();
                if (this.IsPreviewImageZoomLocked())
                {
                    this.previewImageZoomDefault = this.previewImageZoomer.Percent;
                }
            }
        }

        private void btnZoomToFit_Click(object sender, EventArgs e)
        {
            if ((this.previewImageZoomer != null) && this.allowPreviewZoom)
            {
                this.previewImageZoomer.ZoomImageToFit();
                if (this.IsPreviewImageZoomLocked())
                {
                    this.previewImageZoomDefault = this.previewImageZoomer.Percent;
                }
            }
        }

        private void ChangeFileListSort(SortCriterion sortBy)
        {
            SortOrder descending;
            if ((this.config.Browser.SortCriterion == sortBy) && (this.config.Browser.FileListView == ExplorerConfig.FileListView.Details))
            {
                if (this.config.Browser.SortOrder == SortOrder.Ascending)
                {
                    descending = SortOrder.Descending;
                }
                else
                {
                    descending = SortOrder.Ascending;
                }
            }
            else
            {
                descending = SortOrder.Ascending;
            }
            this.ApplyFileListSortSettings(sortBy, descending);
            this.config.Browser.SortCriterion = sortBy;
            this.config.Browser.SortOrder = descending;
        }

        private void CheckCommands()
        {
            if (this.lvwInfo.InvokeRequired)
            {
                this.lvwInfo.BeginInvoke(new NotifyCallback(this.CheckCommands));
            }
            else
            {
                int count = this.lvwInfo.Items.Count;
                int num = this.lvwInfo.SelectedIndices.Count;
                bool isRunning = this.extractor.IsRunning;
                this.mnuExtract.Enabled = ((this.currentFile != null) && (this.currentFile.Files.Length > 0)) && !isRunning;
                this.btnExtract.Enabled = this.mnuExtract.Enabled;
                this.btnOpen.Enabled = !isRunning;
                this.btnOpenGame.Enabled = !isRunning;
                this.mnuOpenFile.Enabled = num > 0;
                this.mnuView.Enabled = num > 0;
                this.mnuViewAsText.Enabled = num > 0;
                this.btnView.Enabled = num > 0;
                this.btnViewAsText.Enabled = num > 0;
                this.btnStop.Enabled = isRunning;
                this.txtPath.Enabled = this.currentFile != null;
                this.btnGoTo.Enabled = this.currentFile != null;
            }
        }

        private void CleanTempDirs(bool canTryAgainLater)
        {
            StringCollection strings = new StringCollection();
            foreach (string str in this.temporaryDirs)
            {
                try
                {
                    if (Directory.Exists(str))
                    {
                        Directory.Delete(str, true);
                    }
                }
                catch (Exception)
                {
                    if (canTryAgainLater)
                    {
                        strings.Add(str);
                    }
                }
            }
            this.temporaryDirs = strings;
        }

        private void ClearPreview()
        {
            this.btnSavePreview.Enabled = false;
            this.previewSeparator.Visible = false;
            this.btnPlayPreview.Enabled = false;
            this.btnPlayPreview.Visible = false;
            this.btnStopPreview.Enabled = false;
            this.btnStopPreview.Visible = false;
            this.mnuPlayPreview.Enabled = false;
            this.mnuPlayPreview.Visible = false;
            this.mnuStopPreview.Enabled = false;
            this.mnuStopPreview.Visible = false;
            this.btnZoomPreviewIn.Enabled = false;
            this.btnZoomPreviewIn.Visible = false;
            this.btnZoomPreviewOut.Enabled = false;
            this.btnZoomPreviewOut.Visible = false;
            this.btnZoomOriginalSize.Enabled = false;
            this.btnZoomOriginalSize.Visible = false;
            this.btnZoomToFit.Enabled = false;
            this.btnZoomToFit.Visible = false;
            this.lblZoomInfo.Visible = false;
            this.allowPreviewZoom = false;
            this.previewRichTextBox.Clear();
            if (this.previewImageZoomer != null)
            {
                this.previewImageZoomer = null;
            }
            if (this.previewPictureBox.Image != null)
            {
                this.previewPictureBox.Image.Dispose();
                this.previewPictureBox.Image = null;
            }
            if (this.previewSoundPlayer != null)
            {
                this.previewSoundPlayer.Stop();
                this.previewSoundPlayer.Dispose();
                this.previewSoundPlayer = null;
            }
            if (this.previewImageLoader != null)
            {
                this.previewImageLoader = null;
            }
            if (this.previewImageOriginal != null)
            {
                this.previewImageOriginal.Dispose();
                this.previewImageOriginal = null;
            }
            if (this.previewStream != null)
            {
                this.previewStream.Close();
                this.previewStream = null;
            }
            this.previewFile = null;
            this.lblPreviewStatus.Text = string.Empty;
        }

        private void CloseInternal()
        {
            if (base.InvokeRequired)
            {
                base.BeginInvoke(new NotifyCallback(this.CloseInternal));
            }
            else
            {
                base.Close();
            }
        }

        private static int CompareStringCollection(StringCollection x, StringCollection y)
        {
            return MakeArrayString(x).CompareTo(MakeArrayString(y));
        }

        private void CreateDirectoryFilter()
        {
            List<string> list = new List<string>();
            foreach (IFileInfo info in this.currentFile.Files)
            {
                string directoryName = Path.GetDirectoryName(info.Name);
                if (!list.Contains(directoryName))
                {
                    list.Add(directoryName);
                }
            }
            list.Sort();
            this.tvwDirectories.Nodes.Clear();
            this.tvwDirectories.ImageList = this.directoryImageList;
            this.tvwDirectories.SelectedImageIndex = 2;
            string treeViewRootKey = this.treeViewRootKey;
            string displayFileName = this.GetDisplayFileName();
            TreeNode node = this.tvwDirectories.Nodes.Add(treeViewRootKey, displayFileName);
            node.ImageIndex = 0;
            node.SelectedImageIndex = 0;
            node.Tag = treeViewRootKey;
            foreach (string str4 in list)
            {
                string[] strArray = str4.Split(new char[] { Path.DirectorySeparatorChar });
                if ((strArray == null) || ((strArray != null) && (strArray[0] == str4)))
                {
                    strArray = str4.Split(new char[] { Path.AltDirectorySeparatorChar });
                }
                TreeNode node2 = node;
                for (int i = 0; i < strArray.Length; i++)
                {
                    string text = strArray[i];
                    StringBuilder builder = new StringBuilder();
                    for (int j = 0; j <= i; j++)
                    {
                        if ((j > 0) || (this.treeViewRootKey.Length > 0))
                        {
                            builder.Append((j > 0) ? Convert.ToString(Path.DirectorySeparatorChar) : this.treeViewRootKey);
                        }
                        builder.Append(strArray[j]);
                    }
                    string key = builder.ToString();
                    TreeNode[] nodeArray = null;
                    if (node2 != null)
                    {
                        nodeArray = node2.Nodes.Find(key, false);
                    }
                    if ((nodeArray == null) || ((nodeArray != null) && (nodeArray.Length == 0)))
                    {
                        nodeArray = this.tvwDirectories.Nodes.Find(key, false);
                    }
                    if (nodeArray.Length > 0)
                    {
                        node2 = nodeArray[0];
                    }
                    else
                    {
                        TreeNode node3;
                        if (node2 != null)
                        {
                            node3 = node2.Nodes.Add(key, text);
                        }
                        else
                        {
                            node3 = this.tvwDirectories.Nodes.Add(key, text);
                        }
                        node3.Tag = key;
                        node3.ImageIndex = 1;
                        node3.SelectedImageIndex = 2;
                        node2 = node3;
                    }
                }
            }
            node.Expand();
        }

        private void CreateExtensionFilter()
        {
            List<StringCollection> list = new List<StringCollection>();
            foreach (IFileInfo info in this.currentFile.Files)
            {
                bool flag = false;
                bool flag2 = false;
                string extension = Path.GetExtension(info.Name);
                foreach (StringCollection strings in list)
                {
                    if (strings.Contains(extension))
                    {
                        flag = true;
                        break;
                    }
                    StringEnumerator enumerator2 = strings.GetEnumerator();
                        while (enumerator2.MoveNext())
                        {
                            if (string.Compare(enumerator2.Current, extension, true) == 0)
                            {
                                strings.Add(extension);
                                flag2 = true;
                                goto Label_00A2;
                            }
                        }
                Label_00A2:
                    if (flag2 || flag)
                    {
                        break;
                    }
                }
                if (!flag2 && !flag)
                {
                    StringCollection strings2 = new StringCollection();
                    strings2.Add(extension);
                    list.Add(strings2);
                }
            }
            list.Sort(new Comparison<StringCollection>(frmMain.CompareStringCollection));
            ToolStripMenuItem item = new ToolStripMenuItem("No filter") {
                Checked = true,
                Tag = null
            };
            this.btnFilter.DropDownItems.Add(item);
            this.btnFilter.Text = "&Filter";
            this.btnFilter.Image = this.filterImageList.Images[0];
            foreach (StringCollection strings3 in list)
            {
                ToolStripMenuItem item2 = new ToolStripMenuItem(MakeArrayString(strings3)) {
                    Tag = strings3
                };
                this.btnFilter.DropDownItems.Add(item2);
            }
        }

        private void CreateFilters()
        {
            if (this.currentFile != null)
            {
                this.CreateExtensionFilter();
                this.CreateDirectoryFilter();
            }
        }

        private ImageList CreateSmallIconImageList()
        {
            return new ImageList { ColorDepth = ColorDepth.Depth32Bit };
        }

        private ImageList CreateThumbnailImageList()
        {
            return new ImageList { ColorDepth = ColorDepth.Depth32Bit, ImageSize = new Size(0x60, 0x60) };
        }

        private void DestroyFilters()
        {
            this.btnFilter.DropDownItems.Clear();
            this.btnFilter.Text = "&Filter";
            this.btnFilter.Image = this.filterImageList.Images[0];
            this.tvwDirectories.Nodes.Clear();
            this.txtPath.Text = string.Empty;
            this.fileListFilters = null;
        }

        private PluginMapping DisambiguateFileFormat(string fileName, PluginMapping[] mappings)
        {
            PluginMapping data = null;
            using (frmDisambiguate disambiguate = new frmDisambiguate())
            {
                disambiguate.SetData(fileName, mappings);
                if (disambiguate.ShowDialog(this) == DialogResult.OK)
                {
                    data = disambiguate.GetData();
                }
            }
            return data;
        }

        private void DisplayFiles(IList<IFileSystemEntry> files)
        {
            Cursor.Current = Cursors.WaitCursor;
            this.lvwInfo.Items.Clear();
            this.lvwInfo.VirtualListSize = 0;
            this.cachedListViewItems = null;
            this.lvwInfo.ListViewItemSorter = null;
            if (this.lvwInfo.SmallImageList != null)
            {
                this.lvwInfo.SmallImageList.Dispose();
                this.lvwInfo.SmallImageList = null;
            }
            if (this.lvwInfo.LargeImageList != null)
            {
                this.lvwInfo.LargeImageList.Dispose();
                this.lvwInfo.LargeImageList = null;
            }
            if (this.config.Browser.FileListView == ExplorerConfig.FileListView.Details)
            {
                this.lvwInfo.View = View.Details;
            }
            else if (this.config.Browser.FileListView == ExplorerConfig.FileListView.Thumbnails)
            {
                this.lvwInfo.View = View.LargeIcon;
            }
            StringDictionary dictionary = new StringDictionary();
            new IconCache();
            long size = 0L;
            ListViewItem[] itemArray = new ListViewItem[files.Count];
            int num2 = 0;
            bool flag = true;
            foreach (IFileSystemEntry entry in files)
            {
                string extensionWithPeriod = this.GetExtensionWithPeriod(entry.Name);
                string typeName = null;
                long num3 = -1L;
                ICompressionInfo info = null;
                if (entry is IFileInfo)
                {
                    IFileInfo info2 = (IFileInfo) entry;
                    if (dictionary.ContainsKey(extensionWithPeriod))
                    {
                        typeName = dictionary[extensionWithPeriod];
                    }
                    else
                    {
                        typeName = IconHandler.GetTypeName(extensionWithPeriod);
                        dictionary[extensionWithPeriod] = typeName;
                    }
                    num3 = info2.Size;
                    if (num3 >= 0L)
                    {
                        size += info2.Size;
                    }
                    else
                    {
                        flag = false;
                    }
                    if (info2 is ICompressionInfo)
                    {
                        info = info2 as ICompressionInfo;
                    }
                }
                ListViewItem item = new ListViewItem(new string[] { Path.GetFileName(entry.Name), (typeName != null) ? typeName : "", (num3 >= 0L) ? string.Format("{0:n0}", num3) : "?", (info == null) ? null : string.Format("{0:n0}", info.CompressedSize), (info == null) ? null : (info.Compressed ? "Yes" : "No"), (this.config.Browser.DirectoryView == ExplorerConfig.DirectoryView.Flat) ? Path.GetDirectoryName(entry.Name) : "" }) {
                    Tag = entry
                };
                itemArray[num2++] = item;
            }
            this.cachedListViewItems = itemArray;
            this.lvwInfo.SmallImageList = this.CreateSmallIconImageList();
            this.lvwInfo.LargeImageList = this.CreateThumbnailImageList();
            this.lvwInfo.ListViewItemSorter = this.listViewColumnSorter;
            Array.Sort(this.cachedListViewItems, this.lvwInfo.ListViewItemSorter);
            this.lvwInfo.VirtualListSize = this.cachedListViewItems.Length;
            if (this.lvwInfo.VirtualListSize > 0)
            {
                this.lvwInfo.EnsureVisible(0);
            }
            int count = files.Count;
            if (count == 1)
            {
                this.lblFileCount.Text = "1 entry";
            }
            else
            {
                this.lblFileCount.Text = count.ToString() + " entries";
            }
            this.lblTotalSize.Text = this.GetBinarySizeText(size) + (flag ? "" : " +?");
            this.lblFileCount.Visible = true;
            this.lblTotalSize.Visible = true;
            Cursor.Current = Cursors.Default;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void dlg_SaveView(object sender, frmView.SaveViewEventArgs e)
        {
            if (e.ImageLoader != null)
            {
                this.SaveImage(e.File, e.Image, e.ImageLoader);
            }
            else if (e.SoundPlayer != null)
            {
                this.ExportSound(e.File, e.SoundExport, e.SoundPlayer);
            }
            else
            {
                this.ShowWarning("Unknown media type in view.");
            }
        }

        private void DragDropCallbackThread(object parameter)
        {
            if (base.InvokeRequired)
            {
                base.Invoke(new OpenFileCallback(this.OpenFile), new object[] { (string) parameter });
            }
            else
            {
                this.OpenFile((string) parameter);
            }
        }

        private void ExportSound(IFileInfo file, ISoundExport export, ISoundPlayer player)
        {
            if (export != null)
            {
                SoundExportFormat[] supportedExportFormats = export.SupportedExportFormats;
                if ((supportedExportFormats == null) || (supportedExportFormats.Length <= 0))
                {
                    this.ShowWarning("Exporting is not supported for this file type.");
                }
                else
                {
                    string name = file.Name;
                    this.saveFileDialog.FileName = Path.GetFileNameWithoutExtension(name);
                    if (this.config.Extract.LastSaveDir.Length > 0)
                    {
                        this.saveFileDialog.InitialDirectory = this.config.Extract.LastSaveDir;
                    }
                    this.saveFileDialog.Filter = this.soundPluginManager.CreateExportFilter(supportedExportFormats);
                    this.saveFileDialog.FilterIndex = 1;
                    if (this.saveFileDialog.ShowDialog(this) == DialogResult.OK)
                    {
                        this.config.Extract.LastSaveDir = Path.GetDirectoryName(this.saveFileDialog.FileName);
                        int index = this.saveFileDialog.FilterIndex - 1;
                        SoundExportFormat format = supportedExportFormats[index];
                        string fileName = this.saveFileDialog.FileName;
                        Cursor.Current = Cursors.WaitCursor;
                        try
                        {
                            bool flag = false;
                            long position = 0L;
                            ISoundPlayer2 player2 = null;
                            if (player is ISoundPlayer2)
                            {
                                player2 = (ISoundPlayer2) player;
                                flag = player2.IsPlaying();
                                if (flag)
                                {
                                    position = player2.GetPosition();
                                }
                            }
                            player.Stop();
                            export.ExportSound(format, fileName);
                            if ((player2 != null) && flag)
                            {
                                player2.Play();
                                player2.SetPosition(position);
                            }
                            Cursor.Current = Cursors.Default;
                        }
                        catch (Exception exception)
                        {
                            Cursor.Current = Cursors.Default;
                            this.ShowError(exception.Message);
                        }
                    }
                }
            }
            else
            {
                this.ShowWarning("Exporting is not supported for this file type.");
            }
        }

        private void ExtractAllFiles(IArchive archive, string outputDirectory, IImageSaver imageSaver, SoundExportFormat soundFormat)
        {
            if ((archive != null) && !string.IsNullOrEmpty(outputDirectory))
            {
                if (!File.Exists(archive.FileName))
                {
                    this.ShowWarning("Input file \"" + archive + "\" does not exist.");
                }
                else
                {
                    this.txtProgress.Clear();
                    this.lblCurrentAction.Text = "Extracting...";
                    try
                    {
                        if ((imageSaver != null) || (soundFormat != null))
                        {
                            this.extractor.ExtractAsync(archive, outputDirectory, this.imagePluginManager, imageSaver, this.soundPluginManager, soundFormat);
                        }
                        else
                        {
                            this.extractor.ExtractAsync(archive, outputDirectory);
                        }
                    }
                    catch (Exception exception)
                    {
                        this.ShowError(exception.Message);
                    }
                }
            }
        }

        private void ExtractFiles()
        {
            if (this.currentFile == null)
            {
                this.ShowWarning("No archive open.");
            }
            else
            {
                frmExtract extract = new frmExtract {
                    OutputDirectory = this.config.Extract.LastExtractDir,
                    CreateSubDirectory = this.config.Extract.CreateSubDirectory,
                    ImageFormats = this.imagePluginManager.AvailableSaverPlugins,
                    SoundFormats = this.soundPluginManager.AvailableExportFormats
                };
                if (this.imagePluginManager.FindSaverPlugin(this.config.Extract.TargetImageFormat) != null)
                {
                    extract.SelectedImageFormat = this.imagePluginManager.FindSaverPlugin(this.config.Extract.TargetImageFormat);
                    extract.ConvertImages = this.config.Extract.ConvertImages;
                }
                SoundExportFormat format = this.soundPluginManager.FindExportFormat(this.config.Extract.TargetSoundFormat);
                if (format != null)
                {
                    extract.SelectedSoundFormat = format;
                    extract.ConvertSounds = this.config.Extract.ConvertSounds;
                }
                extract.AllowSelectedFiles = this.lvwInfo.SelectedIndices.Count > 0;
                extract.AllowDisplayedFiles = (this.fileListFilters != null) && this.fileListFilters.HasActiveFilters();
                extract.AllowCurrentDirectory = (this.fileListFilters != null) && (this.fileListFilters.DirectoryFilter != null);
                if (this.lvwInfo.SelectedIndices.Count == 0)
                {
                    if ((this.fileListFilters == null) || ((this.fileListFilters != null) && !this.fileListFilters.HasActiveFilters()))
                    {
                        extract.ActionScope = ActionScope.AllFiles;
                    }
                    else if ((this.fileListFilters.DirectoryFilter != null) && (this.fileListFilters.ExtensionFilter == null))
                    {
                        if (this.fileListFilters.DirectoryFilter == this.treeViewRootKey)
                        {
                            extract.ActionScope = ActionScope.AllFiles;
                        }
                        else
                        {
                            extract.ActionScope = ActionScope.CurrentDirectory;
                        }
                    }
                    else if (this.fileListFilters.ExtensionFilter != null)
                    {
                        extract.ActionScope = ActionScope.DisplayedFiles;
                    }
                    else
                    {
                        extract.ActionScope = ActionScope.DisplayedFiles;
                    }
                }
                else
                {
                    extract.ActionScope = ActionScope.SelectedFiles;
                }
                if (extract.ShowDialog(this) == DialogResult.OK)
                {
                    this.config.Extract.LastExtractDir = extract.OutputDirectory;
                    this.config.Extract.CreateSubDirectory = extract.CreateSubDirectory;
                    this.config.Extract.ConvertImages = extract.ConvertImages;
                    this.config.Extract.TargetImageFormat = (extract.SelectedImageFormat == null) ? string.Empty : extract.SelectedImageFormat.Extensions[0];
                    this.config.Extract.ConvertSounds = extract.ConvertSounds;
                    this.config.Extract.TargetSoundFormat = (extract.SelectedSoundFormat == null) ? string.Empty : extract.SelectedSoundFormat.Extension;
                    string outputDirectory = extract.CreateSubDirectory ? Path.Combine(extract.OutputDirectory, this.ReplaceInvalidChars(this.GetDisplayFileNameNoExt())) : extract.OutputDirectory;
                    IImageSaver imageSaver = null;
                    if (extract.ConvertImages && (extract.SelectedImageFormat != null))
                    {
                        ImagePluginMapping selectedImageFormat = extract.SelectedImageFormat;
                        try
                        {
                            imageSaver = this.imagePluginManager.CreateSaverInstance(selectedImageFormat);
                        }
                        catch (Exception exception)
                        {
                            this.ShowError("Unable to create image saver \"" + selectedImageFormat.TypeName + "\"" + Environment.NewLine + Environment.NewLine + exception.Message);
                            return;
                        }
                    }
                    SoundExportFormat soundFormat = null;
                    if (extract.ConvertSounds && (extract.SelectedSoundFormat != null))
                    {
                        soundFormat = extract.SelectedSoundFormat;
                    }
                    this.warningMessages = new StringCollection();
                    if (extract.ActionScope == ActionScope.AllFiles)
                    {
                        this.ExtractAllFiles(this.currentFile, outputDirectory, imageSaver, soundFormat);
                    }
                    else if (extract.ActionScope == ActionScope.SelectedFiles)
                    {
                        IFileInfo[] files = new IFileInfo[this.lvwInfo.SelectedIndices.Count];
                        int num = 0;
                        foreach (int num2 in this.lvwInfo.SelectedIndices)
                        {
                            files[num++] = (IFileInfo) this.cachedListViewItems[num2].Tag;
                        }
                        this.ExtractSpecificFiles(outputDirectory, files, imageSaver, soundFormat);
                    }
                    else if (extract.ActionScope == ActionScope.DisplayedFiles)
                    {
                        IFileInfo[] infoArray2 = new IFileInfo[this.lvwInfo.Items.Count];
                        int num3 = 0;
                        foreach (ListViewItem item in this.cachedListViewItems)
                        {
                            infoArray2[num3++] = (IFileInfo) item.Tag;
                        }
                        this.ExtractSpecificFiles(outputDirectory, infoArray2, imageSaver, soundFormat);
                    }
                    else if (extract.ActionScope == ActionScope.CurrentDirectory)
                    {
                        string directoryFilter = this.fileListFilters.DirectoryFilter;
                        if (directoryFilter != null)
                        {
                            List<IFileInfo> list = new List<IFileInfo>();
                            foreach (IFileInfo info in this.currentFile.Files)
                            {
                                string directoryName = Path.GetDirectoryName(info.Name);
                                if (!directoryName.StartsWith(this.treeViewRootKey))
                                {
                                    directoryName = this.treeViewRootKey + directoryName;
                                }
                                if (directoryName.StartsWith(directoryFilter))
                                {
                                    list.Add(info);
                                }
                            }
                            IFileInfo[] array = new IFileInfo[list.Count];
                            list.CopyTo(array);
                            this.ExtractSpecificFiles(outputDirectory, array, imageSaver, soundFormat);
                        }
                    }
                }
                extract.Dispose();
            }
        }

        private void extractor_ArchiveProgress(object sender, ProgressEventArgs e)
        {
            this.ShowProgress(e.Done, e.Total);
        }

        private void extractor_FileExtracted(object sender, ExtractEventArgs e)
        {
            this.AppendText("OK" + Environment.NewLine);
        }

        private void extractor_FileExtractError(object sender, ExtractErrorEventArgs e)
        {
            ErrorAction abort;
            this.AppendText("Error: " + e.Exception.Message + Environment.NewLine);
            string message = "Unable to extract \"" + e.FileName + "\"." + Environment.NewLine + Environment.NewLine + e.Exception.Message;
            DialogResult result = this.AskAbortRetryIgnore(message, "Error");
            switch (result)
            {
                case DialogResult.Abort:
                    abort = ErrorAction.Abort;
                    break;

                case DialogResult.Retry:
                    abort = ErrorAction.Retry;
                    break;

                case DialogResult.Ignore:
                    abort = ErrorAction.Ignore;
                    break;

                default:
                    throw new ApplicationException("Unknown dialog result \"" + result.ToString() + "\".");
            }
            e.Action = abort;
        }

        private void extractor_FileExtracting(object sender, ExtractEventArgs e)
        {
            this.CheckCommands();
            this.AppendText("Extracting " + e.FileName + "...");
        }

        private void extractor_FileExtractWarning(object sender, ExtractWarningEventArgs e)
        {
            this.AppendText("Warning: " + e.Exception.Message + Environment.NewLine);
            if (this.warningMessages != null)
            {
                this.warningMessages.Add(e.FileName + ": " + e.Exception.Message);
            }
        }

        private void extractor_Finished(object sender, EventArgs e)
        {
            this.ShowProgress(-1, -1);
            this.CheckCommands();
            if (this.closePending)
            {
                this.CloseInternal();
            }
            else
            {
                this.ShowWarnings();
                this.warningMessages = null;
            }
        }

        private void ExtractSpecificFiles(string outputDirectory, IFileInfo[] files, IImageSaver imageSaver, SoundExportFormat soundFormat)
        {
            this.txtProgress.Clear();
            this.lblCurrentAction.Text = "Extracting...";
            try
            {
                if ((imageSaver != null) || (soundFormat != null))
                {
                    this.extractor.ExtractAsync(this.currentFile, files, outputDirectory, this.imagePluginManager, imageSaver, this.soundPluginManager, soundFormat);
                }
                else
                {
                    this.extractor.ExtractAsync(this.currentFile, files, outputDirectory);
                    
                }
            }
            catch (Exception exception)
            {
                this.ShowError(exception.Message);
            }
        }

        private bool ExtractTemporaryFiles(IFileInfo[] files, out string destination)
        {
            if (this.currentFile == null)
            {
                throw new InvalidOperationException("No archive open.");
            }
            destination = this.GetTempArchiveLocation();
            BaseExtractor extractor = new BaseExtractor();
            bool flag2 = false;
            foreach (IFileInfo info in files)
            {
                bool flag;
            Label_0034:
                flag = false;
                flag2 = false;
                try
                {
                    Cursor.Current = Cursors.WaitCursor;
                    if (Monitor.TryEnter(this.currentFile, 0x4e20))
                    {
                        try
                        {
                            extractor.Extract(this.currentFile, info, destination);
                        }
                        finally
                        {
                            Monitor.Exit(this.currentFile);
                        }
                        if (!this.temporaryDirs.Contains(destination))
                        {
                            this.temporaryDirs.Add(destination);
                        }
                        Cursor.Current = Cursors.Default;
                    }
                    else
                    {
                        Cursor.Current = Cursors.Default;
                        throw new ApplicationException("Cannot lock the archive.");
                    }
                }
                catch (Exception exception)
                {
                    string message = "Unable to extract \"" + info.Name + "\"." + Environment.NewLine + Environment.NewLine + exception.Message;
                    DialogResult result = this.AskAbortRetryIgnore(message, "Error");
                    switch (result)
                    {
                        case DialogResult.Abort:
                            flag2 = true;
                            goto Label_0155;

                        case DialogResult.Retry:
                            flag = true;
                            goto Label_0155;

                        case DialogResult.Ignore:
                            goto Label_0155;
                    }
                    throw new ApplicationException("Unknown dialog result \"" + result.ToString() + "\".");
                }
            Label_0155:
                if (flag)
                {
                    goto Label_0034;
                }
                if (flag2)
                {
                    break;
                }
            }
            return !flag2;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.LoadConfig();
            this.closePending = false;
            this.archivePluginManager = new ArchivePluginManager();
            this.archivePluginManager.EnumeratePlugins();

            ArchiveBrowseFilter filter = this.archivePluginManager.CreateBrowseFilter();
            this.openFileDialog.Filter = filter.FilterString;
            this.openFileDialog.FilterIndex = filter.KnownTypesIndex;
            this.imagePluginManager = new ImagePluginManager();
            this.imagePluginManager.EnumeratePlugins();
            this.defaultImageSaveFilter = this.imagePluginManager.CreateSaverFilter();
            this.defaultImageSaveFilterIndex = 0;
            this.soundPluginManager = new SoundPluginManager();
            this.soundPluginManager.EnumeratePlugins();
            this.scanPluginManager = new ScanPluginManager();
            this.scanPluginManager.EnumeratePlugins();
            this.extractor = new BaseExtractor();
            this.extractor.ArchiveProgress += new ProgressEventHandler(this.extractor_ArchiveProgress);
            this.extractor.FileExtracting += new ExtractEventHandler(this.extractor_FileExtracting);
            this.extractor.FileExtracted += new ExtractEventHandler(this.extractor_FileExtracted);
            this.extractor.FileExtractError += new ExtractErrorEventHandler(this.extractor_FileExtractError);
            this.extractor.FileExtractWarning += new ExtractWarningEventHandler(this.extractor_FileExtractWarning);
            this.extractor.Finished += new EventHandler(this.extractor_Finished);
            this.fileListSortCriterionMaps = new SortCriterionMapCollection();
            this.fileListSortCriterionMaps.AddRange(new SortCriterionMap[] { new SortCriterionMap(SortCriterion.Name, this.colName, this.mnuSortItemsByName), new SortCriterionMap(SortCriterion.Type, this.colTypeName, this.mnuSortItemsByType), new SortCriterionMap(SortCriterion.Size, this.colSize, this.mnuSortItemsBySize), new SortCriterionMap(SortCriterion.CompressedSize, this.colCompressedSize, this.mnuSortItemsByCSize), new SortCriterionMap(SortCriterion.Compressed, this.colCompressed, this.mnuSortItemsByCompressed), new SortCriterionMap(SortCriterion.Path, this.colPath, this.mnuSortItemsByPath) });
            this.listViewTextSorter = new ListViewCaseInsensitiveSorter();
            this.listViewLongSorter = new ListViewDoubleSorter();
            this.listViewColumnSorter = this.listViewTextSorter;
            this.lvwInfo.ListViewItemSorter = this.listViewColumnSorter;
            this.lblArchiveType.Text = string.Empty;
            this.lblFileCount.Text = string.Empty;
            this.lblTotalSize.Text = string.Empty;
            this.lblCurrentAction.Text = string.Empty;
            WindowSettings.SetWindowSettings(this, this.config.Browser.WindowSettings);
            if (this.config.Browser.FileListView == ExplorerConfig.FileListView.Thumbnails)
            {
                this.lvwInfo.View = View.LargeIcon;
                this.mnuFileListDisplayThumbnails.Checked = true;
            }
            else
            {
                this.lvwInfo.View = View.Details;
                this.mnuFileListDisplayDetails.Checked = true;
            }
            int directoryTreeWidth = this.config.Browser.DirectoryTreeWidth;
            if (directoryTreeWidth > 0)
            {
                this.browserSplitter.SplitterDistance = directoryTreeWidth;
            }
            ExplorerConfig.DirectoryView directoryView = this.config.Browser.DirectoryView;
            this.pathToolStrip.Visible = directoryView == ExplorerConfig.DirectoryView.Hierarchical;
            this.browserSplitter.Panel1Collapsed = directoryView != ExplorerConfig.DirectoryView.Hierarchical;
            this.mnuDirectoryDisplayFlat.Checked = directoryView == ExplorerConfig.DirectoryView.Flat;
            this.mnuDirectoryDisplayHierarchical.Checked = directoryView == ExplorerConfig.DirectoryView.Hierarchical;
            ListViewExtensions.SetIconSpacing(this.lvwInfo, 150, 150);
            this.SetPreviewState(this.config.Browser.Preview.Active);
            this.SetPreviewWidth(this.config.Browser.Preview.Width);
            this.mnuTogglePreview.Checked = this.config.Browser.Preview.Active;
            this.mnuToggleSoundPreview.Checked = this.config.Browser.Preview.PreviewSounds;
            this.mnuAutoPlaySounds.Checked = this.config.Browser.Preview.AutoPlaySounds;
            this.mnuLockImageZoom.Checked = this.config.Browser.Preview.LockImageZoom;
            int height = this.config.Browser.Output.Height;
            if (height > 0)
            {
                this.browseOutputSplitter.SplitterDistance = this.browseOutputSplitter.Height - height;
            }
            this.ApplyFileListSortSettings(this.config.Browser.SortCriterion, this.config.Browser.SortOrder);
            this.SetNewStartupBehaviour(this.config.General.StartupBehaviour);
            this.mnuAllowScanning.Checked = this.config.UnknownFiles.AllowScanning;
            this.mnuAskBeforeScanning.Checked = this.config.UnknownFiles.AskBeforeScanning;
            this.CheckCommands();
            this.ClearPreview();
        }

        private void frmMain_DragDrop(object sender, DragEventArgs e)
        {
            if (this.IsDragDropOK(e))
            {
                try
                {
                    string[] data = (string[]) e.Data.GetData(DataFormats.FileDrop);
                    string path = data[0];
                    this.config.General.LastOpenDir = Path.GetDirectoryName(path);
                    new Thread(new ParameterizedThreadStart(this.DragDropCallbackThread)).Start(path);
                }
                catch (Exception exception)
                {
                    this.ShowError(exception.Message);
                }
            }
        }

        private void frmMain_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = this.IsDragDropOK(e) ? DragDropEffects.Copy : DragDropEffects.None;
        }

        private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.closePending = false;
            this.extractor.ArchiveProgress -= new ProgressEventHandler(this.extractor_ArchiveProgress);
            this.extractor.FileExtracting -= new ExtractEventHandler(this.extractor_FileExtracting);
            this.extractor.FileExtracted -= new ExtractEventHandler(this.extractor_FileExtracted);
            this.extractor.FileExtractError -= new ExtractErrorEventHandler(this.extractor_FileExtractError);
            this.extractor.FileExtractWarning -= new ExtractWarningEventHandler(this.extractor_FileExtractWarning);
            this.extractor.Finished -= new EventHandler(this.extractor_Finished);
            this.extractor = null;
            this.ClearPreview();
            if (this.currentFile != null)
            {
                this.currentFile.Close();
                this.currentFile = null;
            }
            this.CleanTempDirs(false);
            this.SaveConfig();
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.extractor.IsRunning)
            {
                if (this.AskYesNo("Abort extraction?", "Ravioli Explorer") == DialogResult.Yes)
                {
                    this.closePending = true;
                    this.extractor.Abort();
                }
                e.Cancel = true;
            }
            if (!e.Cancel)
            {
                this.config.Browser.WindowSettings = WindowSettings.GetWindowSettings(this);
                this.config.Browser.Preview.Active = this.IsPreviewActive();
                this.config.Browser.Preview.Width = this.GetPreviewWidth();
                this.config.Browser.Output.Height = this.browseOutputSplitter.Height - this.browseOutputSplitter.SplitterDistance;
                this.config.Browser.DirectoryTreeWidth = this.browserSplitter.SplitterDistance;
            }
        }

        private void frmMain_Resize(object sender, EventArgs e)
        {
            if ((this.previewImageZoomer != null) && (this.previewImageZoomer.Percent < 0))
            {
                this.previewImageZoomer.FitImage();
            }
        }

        private void frmMain_Shown(object sender, EventArgs e)
        {
            if (this.archivePluginManager.AvailableArchivePlugins.Length == 0)
            {
                this.ShowWarning("No archive plug-ins were found. At least one plug-in must be installed for this program to work.");
                base.Close();
            }
            else if (this.startDocument.Length > 0)
            {
                this.config.General.LastOpenDir = Path.GetDirectoryName(this.startDocument);
                this.OpenFile(this.startDocument);
            }
            else if (this.config.General.StartupBehaviour == ExplorerConfig.StartupBehaviour.OpenFile)
            {
                this.btnOpen_Click(this, EventArgs.Empty);
            }
            else if (this.config.General.StartupBehaviour == ExplorerConfig.StartupBehaviour.OpenGame)
            {
                this.btnOpenGame_Click(this, EventArgs.Empty);
            }
        }

        private Image GenerateSmallIcon(IFileSystemEntry entry)
        {
            Image icon;
            IconCache cache = new IconCache();
            string extensionWithPeriod = this.GetExtensionWithPeriod(entry.Name);
            if (entry is IDirectoryInfo)
            {
                icon = this.directoryImageList.Images[1];
            }
            else
            {
                icon = cache.GetIcon(extensionWithPeriod, IconSize.Small);
            }
            cache.Dispose();
            return icon;
        }

        private Bitmap GenerateThumbnail(IFileSystemEntry entry)
        {
            return this.GenerateThumbnail(entry, null);
        }

        private Bitmap GenerateThumbnail(IFileSystemEntry entry, ThumbnailGenerator thumbGen)
        {
            Bitmap bitmap;
            Bitmap errorThumbnail;
            if (entry == null)
            {
                throw new ArgumentNullException("entry");
            }
            if (entry is IDirectoryInfo)
            {
                Image image = PixelFormatConverter.GenerateThumbnail(Resources.Folder_Closed, 80, 80);
                bitmap = PixelFormatConverter.FrameImage(image, 0x60, 0x60, Color.Transparent, Color.LightGray);
                image.Dispose();
                return bitmap;
            }
            IFileInfo fileInfo = entry as IFileInfo;
            if (fileInfo == null)
            {
                throw new NotSupportedException("File System Entry of type " + entry.GetType().Name + " is not supported.");
            }
            bool flag = false;
            if (thumbGen == null)
            {
                thumbGen = new ThumbnailGenerator(this.imagePluginManager);
                flag = true;
            }
            Stopwatch stopwatch = new Stopwatch();
            if (!Monitor.TryEnter(this.currentFile, 0x4e20))
            {
                errorThumbnail = thumbGen.GetErrorThumbnail(80, 80);
            }
            else
            {
                try
                {
                    ThumbnailGenerator.ThumbnailOrigin origin;
                    stopwatch.Start();
                    errorThumbnail = thumbGen.GetThumbnail(this.currentFile, fileInfo, 80, 80, out origin);
                }
                finally
                {
                    stopwatch.Stop();
                    Monitor.Exit(this.currentFile);
                }
            }
            Stopwatch stopwatch2 = Stopwatch.StartNew();
            Bitmap bitmap4 = PixelFormatConverter.FrameImage(errorThumbnail, 0x60, 0x60, Color.Transparent, Color.LightGray);
            stopwatch2.Stop();
            bitmap = bitmap4;
            errorThumbnail.Dispose();
            if (flag)
            {
                thumbGen.Dispose();
            }
            TimeSpan span1 = stopwatch.Elapsed + stopwatch2.Elapsed;
            return bitmap;
        }

        private Image[] GenerateThumbnails(IList<ListViewItem> items)
        {
            Cursor.Current = Cursors.WaitCursor;
            Stopwatch stopwatch = Stopwatch.StartNew();
            Image[] imageArray = new Image[items.Count];
            ThumbnailGenerator thumbGen = new ThumbnailGenerator(this.imagePluginManager);
            try
            {
                int num = 0;
                foreach (ListViewItem item in items)
                {
                    imageArray[num++] = this.GenerateThumbnail(item.Tag as IFileSystemEntry, thumbGen);
                }
            }
            finally
            {
                thumbGen.Dispose();
                thumbGen = null;
            }
            stopwatch.Stop();
            Cursor.Current = Cursors.Default;
            return imageArray;
        }

        private string GetBinarySizeText(long size)
        {
            if (size >= 0x40000000L)
            {
                return (((int) (((double) size) / 1073741824.0)) + " GB");
            }
            if (size >= 0x100000L)
            {
                return (((int) (((double) size) / 1048576.0)) + " MB");
            }
            if (size >= 0x400L)
            {
                return (((int) (((double) size) / 1024.0)) + " KB");
            }
            return (size + " Bytes");
        }

        private Image GetCurrentPreviewDisplayImage()
        {
            return this.previewPictureBox.Image;
        }

        private Image GetCurrentPreviewImage()
        {
            return this.previewImageOriginal;
        }

        private ISoundExport GetCurrentPreviewSoundExport()
        {
            return (this.previewSoundPlayer as ISoundExport);
        }

        private string GetDisplayFileName()
        {
            IFileSystemAbstractor currentFile = this.currentFile as IFileSystemAbstractor;
            if (currentFile == null)
            {
                return Path.GetFileName(this.currentFile.FileName);
            }
            return currentFile.DisplayFileName;
        }

        private string GetDisplayFileNameNoExt()
        {
            IFileSystemAbstractor currentFile = this.currentFile as IFileSystemAbstractor;
            if (currentFile == null)
            {
                return Path.GetFileNameWithoutExtension(this.currentFile.FileName);
            }
            return currentFile.DisplayFileName;
        }

        private string GetExtensionWithPeriod(string path)
        {
            string extension = Path.GetExtension(path);
            if (extension.Length == 0)
            {
                extension = ".";
            }
            return extension;
        }

        private string GetImageDetailText(Image image, string imageTypeName)
        {
            return string.Format("{0} x {1} x {2} / {3}", new object[] { image.Size.Width, image.Size.Height, PixelFormatConverter.GetPixelFormatText(image.PixelFormat), imageTypeName });
        }

        private List<PluginMetadataWithCategory> GetPluginMetadata()
        {
            List<PluginMetadataWithCategory> list = new List<PluginMetadataWithCategory> {
                GetPluginMetadata("Archives", this.archivePluginManager.AvailableArchivePlugins),
                GetPluginMetadata("Game Viewers", this.archivePluginManager.AvailableGameViewerPlugins),
                GetPluginMetadata("Image Loaders", this.imagePluginManager.AvailableLoaderPlugins),
                GetPluginMetadata("Image Savers", this.imagePluginManager.AvailableSaverPlugins),
                GetPluginMetadata("Sound Players", this.soundPluginManager.AvailablePlayers)
            };
            list.AddRange(this.scanPluginManager.GetDetectorMetadata());
            return list;
        }

        private static PluginMetadataWithCategory GetPluginMetadata(string category, IEnumerable<PluginMapping> pluginMappings)
        {
            PluginMetadataWithCategory category2 = new PluginMetadataWithCategory {
                Category = category
            };
            foreach (PluginMapping mapping in pluginMappings)
            {
                PluginMetadata item = new PluginMetadata {
                    Name = mapping.TypeName,
                    Extensions = mapping.Extensions,
                    File = Path.GetFileName(mapping.PluginType.Assembly.Location)
                };
                category2.Data.Add(item);
            }
            return category2;
        }

        private int GetPreviewWidth()
        {
            return (this.browsePreviewSplitter.Width - this.browsePreviewSplitter.SplitterDistance);
        }

        private static string GetShortExtensionDisplay(string extensionDisplay)
        {
            if (extensionDisplay.Length <= 4)
            {
                return extensionDisplay;
            }
            return (extensionDisplay.Substring(0, 4) + "...");
        }

        private static string GetSoundDetailInfo(ISoundPlayer player)
        {
            if (player is ISoundPlayer2)
            {
                long length = ((ISoundPlayer2) player).GetLength();
                int num2 = (int) ((((double) length) / 1000.0) / 60.0);
                int num3 = (int) ((((double) length) / 1000.0) % 60.0);
                return string.Format("{0:D2}:{1:D2} / {2}", num2, num3, player.TypeName);
            }
            return player.TypeName;
        }

        private string GetTempArchiveLocation()
        {
            if (this.currentFile == null)
            {
                throw new InvalidOperationException("No archive open.");
            }
            return Path.Combine(Path.GetTempPath(), string.Concat(new object[] { "RExplorer", Process.GetCurrentProcess().Id, "_", this.ReplaceInvalidChars(this.GetDisplayFileName()) }));
        }

        private void HandleUnknownFile(string fileName, string errorMessage)
        {
            if (!this.config.UnknownFiles.AllowScanning)
            {
                this.ShowWarning(errorMessage);
            }
            else
            {
                string str = null;
                if (this.scanPluginManager.AvailableDetectors.Length > 0)
                {
                    bool flag;
                    if (this.config.UnknownFiles.AskBeforeScanning)
                    {
                        FileInfo info = new FileInfo(fileName);
                        string str2 = errorMessage + Environment.NewLine + Environment.NewLine + "Do you want to scan the file for known resources, like images and sounds?" + Environment.NewLine + Environment.NewLine + "Depending on the file size and how many items are found this can take some time. The file size is " + FileScanner.GetFriendlyFileSize(info.Length) + ".";
                        DialogResult result = new frmYesNoAlways { Owner = this, MessageCaption = "Ravioli Explorer", MessageText = str2 }.ShowDialog();
                        flag = (result == DialogResult.Yes) || (result == DialogResult.Retry);
                        switch (result)
                        {
                            case DialogResult.Retry:
                                this.config.UnknownFiles.AskBeforeScanning = false;
                                this.mnuAskBeforeScanning.Checked = this.config.UnknownFiles.AskBeforeScanning;
                                break;

                            case DialogResult.Ignore:
                                this.config.UnknownFiles.AllowScanning = false;
                                this.mnuAllowScanning.Checked = this.config.UnknownFiles.AllowScanning;
                                break;
                        }
                    }
                    else
                    {
                        flag = true;
                    }
                    if (flag)
                    {
                        string tempResultsFileInNewDir = ScanDefinitions.GetTempResultsFileInNewDir(fileName);
                        string directoryName = Path.GetDirectoryName(tempResultsFileInNewDir);
                        bool flag2 = false;
                        if (!Directory.Exists(directoryName))
                        {
                            Directory.CreateDirectory(directoryName);
                            flag2 = true;
                        }
                        try
                        {
                            bool flag3;
                            using (frmScan scan = new frmScan(this.scanPluginManager) { Owner = this })
                            {
                                scan.Scan(fileName, tempResultsFileInNewDir, out flag3);
                            }
                            if (!flag3)
                            {
                                this.OpenFile(tempResultsFileInNewDir, ScanDefinitions.ScanArchiveMapping);
                            }
                        }
                        finally
                        {
                            if (flag2 && !this.temporaryDirs.Contains(directoryName))
                            {
                                this.temporaryDirs.Add(directoryName);
                            }
                        }
                    }
                }
                else
                {
                    str = "Scanning cannot be performed because there are no scanning plug-ins available.";
                }
                if (str != null)
                {
                    this.ShowWarning(errorMessage + Environment.NewLine + Environment.NewLine + str);
                }
            }
        }

        private void InitializeComponent()
        {
            this.components = new Container();
            ComponentResourceManager manager = new ComponentResourceManager(typeof(frmMain));
            this.openFileDialog = new OpenFileDialog();
            this.archiveStatus = new StatusStrip();
            this.lblArchiveType = new ToolStripStatusLabel();
            this.lblCurrentAction = new ToolStripStatusLabel();
            this.toolStripProgressBar1 = new ToolStripProgressBar();
            this.lblFileCount = new ToolStripStatusLabel();
            this.lblTotalSize = new ToolStripStatusLabel();
            this.contextMenu = new ContextMenuStrip(this.components);
            this.mnuOpenFile = new ToolStripMenuItem();
            this.mnuPlayPreview = new ToolStripMenuItem();
            this.mnuStopPreview = new ToolStripMenuItem();
            this.mnuView = new ToolStripMenuItem();
            this.mnuViewAsText = new ToolStripMenuItem();
            this.mnuExtract = new ToolStripMenuItem();
            this.mainToolStrip = new ToolStrip();
            this.btnOpen = new ToolStripButton();
            this.btnOpenGame = new ToolStripButton();
            this.btnView = new ToolStripSplitButton();
            this.btnViewAsText = new ToolStripMenuItem();
            this.btnExtract = new ToolStripButton();
            this.btnStop = new ToolStripButton();
            this.btnFilter = new ToolStripDropDownButton();
            this.btnOptions = new ToolStripDropDownButton();
            this.previewToolStripMenuItem = new ToolStripMenuItem();
            this.mnuTogglePreview = new ToolStripMenuItem();
            this.mnuToggleSoundPreview = new ToolStripMenuItem();
            this.mnuAutoPlaySounds = new ToolStripMenuItem();
            this.mnuLockImageZoom = new ToolStripMenuItem();
            this.mnuSortItemsBy = new ToolStripMenuItem();
            this.mnuSortItemsByName = new ToolStripMenuItem();
            this.mnuSortItemsByType = new ToolStripMenuItem();
            this.mnuSortItemsBySize = new ToolStripMenuItem();
            this.mnuSortItemsByCSize = new ToolStripMenuItem();
            this.mnuSortItemsByCompressed = new ToolStripMenuItem();
            this.mnuSortItemsByPath = new ToolStripMenuItem();
            this.mnuFileListDisplayMode = new ToolStripMenuItem();
            this.mnuFileListDisplayDetails = new ToolStripMenuItem();
            this.mnuFileListDisplayThumbnails = new ToolStripMenuItem();
            this.mnuDirectoryDisplayMode = new ToolStripMenuItem();
            this.mnuDirectoryDisplayHierarchical = new ToolStripMenuItem();
            this.mnuDirectoryDisplayFlat = new ToolStripMenuItem();
            this.mnuStartupBehaviour = new ToolStripMenuItem();
            this.mnuStartupBehaviourDoNothing = new ToolStripMenuItem();
            this.mnuStartupBehaviourOpenFile = new ToolStripMenuItem();
            this.mnuStartupBehaviourOpenGame = new ToolStripMenuItem();
            this.mnuUnknownFiles = new ToolStripMenuItem();
            this.mnuAllowScanning = new ToolStripMenuItem();
            this.mnuAskBeforeScanning = new ToolStripMenuItem();
            this.btnAbout = new ToolStripButton();
            this.toolStripContainer1 = new ToolStripContainer();
            this.browseOutputSplitter = new SplitContainer();
            this.browserSplitter = new SplitContainer();
            this.tvwDirectories = new TreeView();
            this.browsePreviewSplitter = new SplitContainer();
            this.lvwInfo = new ListView();
            this.colName = new ColumnHeader();
            this.colTypeName = new ColumnHeader();
            this.colSize = new ColumnHeader();
            this.colCompressedSize = new ColumnHeader();
            this.colCompressed = new ColumnHeader();
            this.colPath = new ColumnHeader();
            this.lblZoomInfo = new Label();
            this.previewToolStrip = new ToolStrip();
            this.btnSavePreview = new ToolStripButton();
            this.previewSeparator = new ToolStripSeparator();
            this.btnZoomPreviewIn = new ToolStripButton();
            this.btnZoomPreviewOut = new ToolStripButton();
            this.btnZoomOriginalSize = new ToolStripButton();
            this.btnZoomToFit = new ToolStripButton();
            this.btnPlayPreview = new ToolStripButton();
            this.btnStopPreview = new ToolStripButton();
            this.previewStatus = new StatusStrip();
            this.lblPreviewStatus = new ToolStripStatusLabel();
            this.previewPictureBox = new PictureBox();
            this.previewRichTextBox = new RichTextBox();
            this.txtProgress = new TextBox();
            this.pathToolStrip = new ToolStripFixedKeys();
            this.lblPath = new ToolStripLabel();
            this.txtPath = new ToolStripSpringTextBox();
            this.btnGoTo = new ToolStripButton();
            this.saveFileDialog = new SaveFileDialog();
            this.filterImageList = new ImageList(this.components);
            this.fileTypeImageList = new ImageList(this.components);
            this.directoryImageList = new ImageList(this.components);
            this.archiveStatus.SuspendLayout();
            this.contextMenu.SuspendLayout();
            this.mainToolStrip.SuspendLayout();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.browseOutputSplitter.BeginInit();
            this.browseOutputSplitter.Panel1.SuspendLayout();
            this.browseOutputSplitter.Panel2.SuspendLayout();
            this.browseOutputSplitter.SuspendLayout();
            this.browserSplitter.BeginInit();
            this.browserSplitter.Panel1.SuspendLayout();
            this.browserSplitter.Panel2.SuspendLayout();
            this.browserSplitter.SuspendLayout();
            this.browsePreviewSplitter.BeginInit();
            this.browsePreviewSplitter.Panel1.SuspendLayout();
            this.browsePreviewSplitter.Panel2.SuspendLayout();
            this.browsePreviewSplitter.SuspendLayout();
            this.previewToolStrip.SuspendLayout();
            this.previewStatus.SuspendLayout();
            ((ISupportInitialize) this.previewPictureBox).BeginInit();
            this.pathToolStrip.SuspendLayout();
            base.SuspendLayout();
            this.openFileDialog.FilterIndex = 0;
            this.archiveStatus.GripStyle = ToolStripGripStyle.Visible;
            this.archiveStatus.Items.AddRange(new ToolStripItem[] { this.lblArchiveType, this.lblCurrentAction, this.toolStripProgressBar1, this.lblFileCount, this.lblTotalSize });
            this.archiveStatus.Location = new Point(0, 0x1a8);
            this.archiveStatus.Name = "archiveStatus";
            this.archiveStatus.Size = new Size(0x278, 0x16);
            this.archiveStatus.TabIndex = 0x1a;
            this.lblArchiveType.BorderSides = ToolStripStatusLabelBorderSides.All;
            this.lblArchiveType.BorderStyle = Border3DStyle.SunkenOuter;
            this.lblArchiveType.Name = "lblArchiveType";
            this.lblArchiveType.Size = new Size(0x73, 0x13);
            this.lblArchiveType.Spring = true;
            this.lblArchiveType.Text = "<<ArchiveType>>";
            this.lblArchiveType.TextAlign = ContentAlignment.MiddleLeft;
            this.lblArchiveType.Visible = false;
            this.lblCurrentAction.Name = "lblCurrentAction";
            this.lblCurrentAction.Size = new Size(0xe7, 0x13);
            this.lblCurrentAction.Spring = true;
            this.lblCurrentAction.Text = "<<CurrentAction>>";
            this.lblCurrentAction.TextAlign = ContentAlignment.MiddleLeft;
            this.lblCurrentAction.Visible = false;
            this.toolStripProgressBar1.AutoToolTip = true;
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new Size(200, 0x12);
            this.toolStripProgressBar1.Visible = false;
            this.lblFileCount.BorderSides = ToolStripStatusLabelBorderSides.All;
            this.lblFileCount.BorderStyle = Border3DStyle.SunkenOuter;
            this.lblFileCount.Name = "lblFileCount";
            this.lblFileCount.Size = new Size(0x5e, 0x13);
            this.lblFileCount.Text = "<<FileCount>>";
            this.lblFileCount.TextDirection = ToolStripTextDirection.Horizontal;
            this.lblFileCount.Visible = false;
            this.lblTotalSize.BorderSides = ToolStripStatusLabelBorderSides.All;
            this.lblTotalSize.BorderStyle = Border3DStyle.SunkenOuter;
            this.lblTotalSize.Name = "lblTotalSize";
            this.lblTotalSize.Size = new Size(90, 0x13);
            this.lblTotalSize.Text = "<<TotalSize>>";
            this.lblTotalSize.Visible = false;
            this.contextMenu.Items.AddRange(new ToolStripItem[] { this.mnuOpenFile, this.mnuPlayPreview, this.mnuStopPreview, this.mnuView, this.mnuViewAsText, this.mnuExtract });
            this.contextMenu.Name = "contextMenu";
            this.contextMenu.Size = new Size(0x8d, 0x88);
            this.mnuOpenFile.Font = new Font("Tahoma", 8.25f, FontStyle.Bold);
            this.mnuOpenFile.Name = "mnuOpenFile";
            this.mnuOpenFile.Size = new Size(140, 0x16);
            this.mnuOpenFile.Text = "&Open";
            this.mnuOpenFile.Click += new EventHandler(this.mnuOpenFile_Click);
            this.mnuPlayPreview.Image = Resources.PlayHS;
            this.mnuPlayPreview.Name = "mnuPlayPreview";
            this.mnuPlayPreview.Size = new Size(140, 0x16);
            this.mnuPlayPreview.Text = "&Play";
            this.mnuPlayPreview.Click += new EventHandler(this.mnuPlayPreview_Click);
            this.mnuStopPreview.Image = Resources.StopHS;
            this.mnuStopPreview.Name = "mnuStopPreview";
            this.mnuStopPreview.Size = new Size(140, 0x16);
            this.mnuStopPreview.Text = "&Stop";
            this.mnuStopPreview.Click += new EventHandler(this.mnuStopPreview_Click);
            this.mnuView.Image = Resources.PrintPreviewHS;
            this.mnuView.Name = "mnuView";
            this.mnuView.Size = new Size(140, 0x16);
            this.mnuView.Text = "&View";
            this.mnuView.Click += new EventHandler(this.mnuView_Click);
            this.mnuViewAsText.Image = Resources.PrintPreviewHS;
            this.mnuViewAsText.Name = "mnuViewAsText";
            this.mnuViewAsText.Size = new Size(140, 0x16);
            this.mnuViewAsText.Text = "View As &Text";
            this.mnuViewAsText.Click += new EventHandler(this.mnuViewAsText_Click);
            this.mnuExtract.Image = Resources.SaveHS;
            this.mnuExtract.Name = "mnuExtract";
            this.mnuExtract.Size = new Size(140, 0x16);
            this.mnuExtract.Text = "&Extract...";
            this.mnuExtract.Click += new EventHandler(this.mnuExtract_Click);
            this.mainToolStrip.Dock = DockStyle.None;
            this.mainToolStrip.GripStyle = ToolStripGripStyle.Hidden;
            this.mainToolStrip.Items.AddRange(new ToolStripItem[] { this.btnOpen, this.btnOpenGame, this.btnView, this.btnExtract, this.btnStop, this.btnFilter, this.btnOptions, this.btnAbout });
            this.mainToolStrip.Location = new Point(3, 0);
            this.mainToolStrip.Name = "mainToolStrip";
            this.mainToolStrip.Size = new Size(0x247, 0x19);
            this.mainToolStrip.TabIndex = 0x1d;
            this.mainToolStrip.Text = "toolStrip1";
            this.btnOpen.Image = Resources.OpenHS;
            this.btnOpen.ImageTransparentColor = Color.Magenta;
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new Size(0x56, 0x16);
            this.btnOpen.Text = "&Open File...";
            this.btnOpen.Click += new EventHandler(this.btnOpen_Click);
            this.btnOpenGame.Image = Resources.OpenHS;
            this.btnOpenGame.ImageTransparentColor = Color.Magenta;
            this.btnOpenGame.Name = "btnOpenGame";
            this.btnOpenGame.Size = new Size(0x63, 0x16);
            this.btnOpenGame.Text = "Open &Game...";
            this.btnOpenGame.Click += new EventHandler(this.btnOpenGame_Click);
            this.btnView.DropDownItems.AddRange(new ToolStripItem[] { this.btnViewAsText });
            this.btnView.Image = Resources.PrintPreviewHS;
            this.btnView.ImageTransparentColor = Color.Magenta;
            this.btnView.Name = "btnView";
            this.btnView.Size = new Size(0x40, 0x16);
            this.btnView.Text = "&View";
            this.btnView.ButtonClick += new EventHandler(this.btnView_ButtonClick);
            this.btnViewAsText.Image = Resources.PrintPreviewHS;
            this.btnViewAsText.Name = "btnViewAsText";
            this.btnViewAsText.Size = new Size(140, 0x16);
            this.btnViewAsText.Text = "View As &Text";
            this.btnViewAsText.Click += new EventHandler(this.btnViewAsText_Click);
            this.btnExtract.Image = Resources.SaveHS;
            this.btnExtract.ImageTransparentColor = Color.Magenta;
            this.btnExtract.Name = "btnExtract";
            this.btnExtract.Size = new Size(0x47, 0x16);
            this.btnExtract.Text = "&Extract...";
            this.btnExtract.Click += new EventHandler(this.btnExtract_Click);
            this.btnStop.Image = Resources.CriticalError;
            this.btnStop.ImageTransparentColor = Color.Magenta;
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new Size(0x33, 0x16);
            this.btnStop.Text = "&Stop";
            this.btnStop.Click += new EventHandler(this.btnStop_Click);
            this.btnFilter.Image = Resources.Filter2HS;
            this.btnFilter.ImageTransparentColor = Color.Magenta;
            this.btnFilter.Name = "btnFilter";
            this.btnFilter.Size = new Size(0x3e, 0x16);
            this.btnFilter.Text = "&Filter";
            this.btnFilter.DropDownItemClicked += new ToolStripItemClickedEventHandler(this.btnFilter_DropDownItemClicked);
            this.btnOptions.DropDownItems.AddRange(new ToolStripItem[] { this.previewToolStripMenuItem, this.mnuSortItemsBy, this.mnuFileListDisplayMode, this.mnuDirectoryDisplayMode, this.mnuStartupBehaviour, this.mnuUnknownFiles });
            //this.btnOptions.Image = (Image) manager.GetObject("btnOptions.Image");
            this.btnOptions.ImageTransparentColor = Color.Magenta;
            this.btnOptions.Name = "btnOptions";
            this.btnOptions.Size = new Size(0x4e, 0x16);
            this.btnOptions.Text = "Optio&ns";
            this.previewToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { this.mnuTogglePreview, this.mnuToggleSoundPreview, this.mnuAutoPlaySounds, this.mnuLockImageZoom });
            this.previewToolStripMenuItem.Name = "previewToolStripMenuItem";
            this.previewToolStripMenuItem.Size = new Size(0xa8, 0x16);
            this.previewToolStripMenuItem.Text = "&Preview";
            this.mnuTogglePreview.Name = "mnuTogglePreview";
            this.mnuTogglePreview.Size = new Size(0xd7, 0x16);
            this.mnuTogglePreview.Text = "&Enable Preview";
            this.mnuTogglePreview.Click += new EventHandler(this.mnuTogglePreview_Click);
            this.mnuToggleSoundPreview.Name = "mnuToggleSoundPreview";
            this.mnuToggleSoundPreview.Size = new Size(0xd7, 0x16);
            this.mnuToggleSoundPreview.Text = "&Preview Sounds";
            this.mnuToggleSoundPreview.Click += new EventHandler(this.mnuToggleSoundPreview_Click);
            this.mnuAutoPlaySounds.Name = "mnuAutoPlaySounds";
            this.mnuAutoPlaySounds.Size = new Size(0xd7, 0x16);
            this.mnuAutoPlaySounds.Text = "Pl&ay Sounds Automatically";
            this.mnuAutoPlaySounds.Click += new EventHandler(this.mnuAutoPlaySounds_Click);
            this.mnuLockImageZoom.Name = "mnuLockImageZoom";
            this.mnuLockImageZoom.Size = new Size(0xd7, 0x16);
            this.mnuLockImageZoom.Text = "&Lock Preview Image Zoom";
            this.mnuLockImageZoom.Click += new EventHandler(this.mnuLockImageZoom_Click);
            this.mnuSortItemsBy.DropDownItems.AddRange(new ToolStripItem[] { this.mnuSortItemsByName, this.mnuSortItemsByType, this.mnuSortItemsBySize, this.mnuSortItemsByCSize, this.mnuSortItemsByCompressed, this.mnuSortItemsByPath });
            this.mnuSortItemsBy.Name = "mnuSortItemsBy";
            this.mnuSortItemsBy.Size = new Size(0xa8, 0x16);
            this.mnuSortItemsBy.Text = "&Sort Items By";
            this.mnuSortItemsByName.Name = "mnuSortItemsByName";
            this.mnuSortItemsByName.Size = new Size(140, 0x16);
            this.mnuSortItemsByName.Text = "&Name";
            this.mnuSortItemsByName.Click += new EventHandler(this.mnuSortItemsByName_Click);
            this.mnuSortItemsByType.Name = "mnuSortItemsByType";
            this.mnuSortItemsByType.Size = new Size(140, 0x16);
            this.mnuSortItemsByType.Text = "&Type";
            this.mnuSortItemsByType.Click += new EventHandler(this.mnuSortItemsByType_Click);
            this.mnuSortItemsBySize.Name = "mnuSortItemsBySize";
            this.mnuSortItemsBySize.Size = new Size(140, 0x16);
            this.mnuSortItemsBySize.Text = "&Size";
            this.mnuSortItemsBySize.Click += new EventHandler(this.mnuSortItemsBySize_Click);
            this.mnuSortItemsByCSize.Name = "mnuSortItemsByCSize";
            this.mnuSortItemsByCSize.Size = new Size(140, 0x16);
            this.mnuSortItemsByCSize.Text = "P&acked";
            this.mnuSortItemsByCSize.Click += new EventHandler(this.mnuSortItemsByCSize_Click);
            this.mnuSortItemsByCompressed.Name = "mnuSortItemsByCompressed";
            this.mnuSortItemsByCompressed.Size = new Size(140, 0x16);
            this.mnuSortItemsByCompressed.Text = "&Compressed";
            this.mnuSortItemsByCompressed.Click += new EventHandler(this.mnuSortItemsByCompressed_Click);
            this.mnuSortItemsByPath.Name = "mnuSortItemsByPath";
            this.mnuSortItemsByPath.Size = new Size(140, 0x16);
            this.mnuSortItemsByPath.Text = "&Path";
            this.mnuSortItemsByPath.Click += new EventHandler(this.mnuSortItemsByPath_Click);
            this.mnuFileListDisplayMode.DropDownItems.AddRange(new ToolStripItem[] { this.mnuFileListDisplayDetails, this.mnuFileListDisplayThumbnails });
            this.mnuFileListDisplayMode.Name = "mnuFileListDisplayMode";
            this.mnuFileListDisplayMode.Size = new Size(0xa8, 0x16);
            this.mnuFileListDisplayMode.Text = "&File List View";
            this.mnuFileListDisplayDetails.Name = "mnuFileListDisplayDetails";
            this.mnuFileListDisplayDetails.Size = new Size(0x89, 0x16);
            this.mnuFileListDisplayDetails.Text = "&Details";
            this.mnuFileListDisplayDetails.Click += new EventHandler(this.mnuFileListDisplayDetails_Click);
            this.mnuFileListDisplayThumbnails.Name = "mnuFileListDisplayThumbnails";
            this.mnuFileListDisplayThumbnails.Size = new Size(0x89, 0x16);
            this.mnuFileListDisplayThumbnails.Text = "&Thumbnails";
            this.mnuFileListDisplayThumbnails.Click += new EventHandler(this.mnuFileListDisplayThumbnails_Click);
            this.mnuDirectoryDisplayMode.DropDownItems.AddRange(new ToolStripItem[] { this.mnuDirectoryDisplayHierarchical, this.mnuDirectoryDisplayFlat });
            this.mnuDirectoryDisplayMode.Name = "mnuDirectoryDisplayMode";
            this.mnuDirectoryDisplayMode.Size = new Size(0xa8, 0x16);
            this.mnuDirectoryDisplayMode.Text = "&Directory View";
            this.mnuDirectoryDisplayHierarchical.Name = "mnuDirectoryDisplayHierarchical";
            this.mnuDirectoryDisplayHierarchical.Size = new Size(0x89, 0x16);
            this.mnuDirectoryDisplayHierarchical.Text = "&Hierarchical";
            this.mnuDirectoryDisplayHierarchical.Click += new EventHandler(this.mnuDirectoryDisplayHierarchical_Click);
            this.mnuDirectoryDisplayFlat.Name = "mnuDirectoryDisplayFlat";
            this.mnuDirectoryDisplayFlat.Size = new Size(0x89, 0x16);
            this.mnuDirectoryDisplayFlat.Text = "&Flat";
            this.mnuDirectoryDisplayFlat.Click += new EventHandler(this.mnuDirectoryDisplayFlat_Click);
            this.mnuStartupBehaviour.DropDownItems.AddRange(new ToolStripItem[] { this.mnuStartupBehaviourDoNothing, this.mnuStartupBehaviourOpenFile, this.mnuStartupBehaviourOpenGame });
            this.mnuStartupBehaviour.Name = "mnuStartupBehaviour";
            this.mnuStartupBehaviour.Size = new Size(0xa8, 0x16);
            this.mnuStartupBehaviour.Text = "S&tartup Behaviour";
            this.mnuStartupBehaviourDoNothing.Name = "mnuStartupBehaviourDoNothing";
            this.mnuStartupBehaviourDoNothing.Size = new Size(0x89, 0x16);
            this.mnuStartupBehaviourDoNothing.Text = "Do &Nothing";
            this.mnuStartupBehaviourDoNothing.Click += new EventHandler(this.mnuStartupBehaviourDoNothing_Click);
            this.mnuStartupBehaviourOpenFile.Name = "mnuStartupBehaviourOpenFile";
            this.mnuStartupBehaviourOpenFile.Size = new Size(0x89, 0x16);
            this.mnuStartupBehaviourOpenFile.Text = "&Open File";
            this.mnuStartupBehaviourOpenFile.Click += new EventHandler(this.mnuStartupBehaviourOpen_Click);
            this.mnuStartupBehaviourOpenGame.Name = "mnuStartupBehaviourOpenGame";
            this.mnuStartupBehaviourOpenGame.Size = new Size(0x89, 0x16);
            this.mnuStartupBehaviourOpenGame.Text = "Open &Game";
            this.mnuStartupBehaviourOpenGame.Click += new EventHandler(this.mnuStartupBehaviourOpenGame_Click);
            this.mnuUnknownFiles.DropDownItems.AddRange(new ToolStripItem[] { this.mnuAllowScanning, this.mnuAskBeforeScanning });
            this.mnuUnknownFiles.Name = "mnuUnknownFiles";
            this.mnuUnknownFiles.Size = new Size(0xa8, 0x16);
            this.mnuUnknownFiles.Text = "&Unknown Files";
            this.mnuAllowScanning.Name = "mnuAllowScanning";
            this.mnuAllowScanning.Size = new Size(0xb6, 0x16);
            this.mnuAllowScanning.Text = "&Allow Scanning";
            this.mnuAllowScanning.Click += new EventHandler(this.mnuAllowScanning_Click);
            this.mnuAskBeforeScanning.Name = "mnuAskBeforeScanning";
            this.mnuAskBeforeScanning.Size = new Size(0xb6, 0x16);
            this.mnuAskBeforeScanning.Text = "Ask &Before Scanning";
            this.mnuAskBeforeScanning.Click += new EventHandler(this.mnuAskBeforeScanning_Click);
            this.btnAbout.Image = Resources.Help;
            this.btnAbout.ImageTransparentColor = Color.Magenta;
            this.btnAbout.Name = "btnAbout";
            this.btnAbout.Size = new Size(0x45, 0x16);
            this.btnAbout.Text = "&About...";
            this.btnAbout.Click += new EventHandler(this.btnAbout_Click);
            this.toolStripContainer1.ContentPanel.Controls.Add(this.browseOutputSplitter);
            this.toolStripContainer1.ContentPanel.Size = new Size(0x278, 0x18c);
            this.toolStripContainer1.Dock = DockStyle.Fill;
            this.toolStripContainer1.Location = new Point(0, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.Size = new Size(0x278, 0x1be);
            this.toolStripContainer1.TabIndex = 30;
            this.toolStripContainer1.Text = "toolStripContainer1";
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.mainToolStrip);
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.pathToolStrip);
            this.browseOutputSplitter.Dock = DockStyle.Fill;
            this.browseOutputSplitter.FixedPanel = FixedPanel.Panel2;
            this.browseOutputSplitter.Location = new Point(0, 0);
            this.browseOutputSplitter.Name = "browseOutputSplitter";
            this.browseOutputSplitter.Orientation = Orientation.Horizontal;
            this.browseOutputSplitter.Panel1.Controls.Add(this.browserSplitter);
            this.browseOutputSplitter.Panel2.Controls.Add(this.txtProgress);
            this.browseOutputSplitter.Size = new Size(0x278, 0x18c);
            this.browseOutputSplitter.SplitterDistance = 0x13c;
            this.browseOutputSplitter.TabIndex = 2;
            this.browserSplitter.Dock = DockStyle.Fill;
            this.browserSplitter.FixedPanel = FixedPanel.Panel1;
            this.browserSplitter.Location = new Point(0, 0);
            this.browserSplitter.Name = "browserSplitter";
            this.browserSplitter.Panel1.Controls.Add(this.tvwDirectories);
            this.browserSplitter.Panel2.Controls.Add(this.browsePreviewSplitter);
            this.browserSplitter.Size = new Size(0x278, 0x13c);
            this.browserSplitter.SplitterDistance = 210;
            this.browserSplitter.TabIndex = 4;
            this.tvwDirectories.Dock = DockStyle.Fill;
            this.tvwDirectories.HideSelection = false;
            this.tvwDirectories.Location = new Point(0, 0);
            this.tvwDirectories.Name = "tvwDirectories";
            this.tvwDirectories.ShowRootLines = false;
            this.tvwDirectories.Size = new Size(210, 0x13c);
            this.tvwDirectories.TabIndex = 0;
            this.tvwDirectories.AfterSelect += new TreeViewEventHandler(this.tvwDirectories_AfterSelect);
            this.browsePreviewSplitter.Dock = DockStyle.Fill;
            this.browsePreviewSplitter.Location = new Point(0, 0);
            this.browsePreviewSplitter.Name = "browsePreviewSplitter";
            this.browsePreviewSplitter.Panel1.Controls.Add(this.lvwInfo);
            this.browsePreviewSplitter.Panel2.Controls.Add(this.lblZoomInfo);
            this.browsePreviewSplitter.Panel2.Controls.Add(this.previewToolStrip);
            this.browsePreviewSplitter.Panel2.Controls.Add(this.previewStatus);
            this.browsePreviewSplitter.Panel2.Controls.Add(this.previewPictureBox);
            this.browsePreviewSplitter.Panel2.Controls.Add(this.previewRichTextBox);
            this.browsePreviewSplitter.Size = new Size(0x1a2, 0x13c);
            this.browsePreviewSplitter.SplitterDistance = 0xeb;
            this.browsePreviewSplitter.TabIndex = 3;
            this.lvwInfo.Columns.AddRange(new ColumnHeader[] { this.colName, this.colTypeName, this.colSize, this.colCompressedSize, this.colCompressed, this.colPath });
            this.lvwInfo.ContextMenuStrip = this.contextMenu;
            this.lvwInfo.Dock = DockStyle.Fill;
            this.lvwInfo.HideSelection = false;
            this.lvwInfo.Location = new Point(0, 0);
            this.lvwInfo.Name = "lvwInfo";
            this.lvwInfo.Size = new Size(0xeb, 0x13c);
            this.lvwInfo.TabIndex = 1;
            this.lvwInfo.UseCompatibleStateImageBehavior = false;
            this.lvwInfo.View = View.Details;
            this.lvwInfo.VirtualMode = true;
            this.lvwInfo.ColumnClick += new ColumnClickEventHandler(this.lvwInfo_ColumnClick);
            this.lvwInfo.ItemDrag += new ItemDragEventHandler(this.lvwInfo_ItemDrag);
            this.lvwInfo.RetrieveVirtualItem += new RetrieveVirtualItemEventHandler(this.lvwInfo_RetrieveVirtualItem);
            this.lvwInfo.SearchForVirtualItem += new SearchForVirtualItemEventHandler(this.lvwInfo_SearchForVirtualItem);
            this.lvwInfo.SelectedIndexChanged += new EventHandler(this.lvwInfo_SelectedIndexChanged);
            this.lvwInfo.QueryContinueDrag += new QueryContinueDragEventHandler(this.lvwInfo_QueryContinueDrag);
            this.lvwInfo.DoubleClick += new EventHandler(this.lvwInfo_DoubleClick);
            this.lvwInfo.KeyDown += new KeyEventHandler(this.lvwInfo_KeyDown);
            
            this.colName.Text = "Name";
            this.colName.Width = 230;
            this.colTypeName.Text = "Type";
            this.colTypeName.Width = 120;
            this.colSize.Text = "Size";
            this.colSize.TextAlign = HorizontalAlignment.Right;
            this.colSize.Width = 80;
            this.colCompressedSize.Text = "Packed";
            this.colCompressedSize.TextAlign = HorizontalAlignment.Right;
            this.colCompressedSize.Width = 80;
            this.colCompressed.Text = "Compressed";
            this.colCompressed.TextAlign = HorizontalAlignment.Right;
            this.colCompressed.Width = 50;
            this.colPath.Text = "Path";
            this.colPath.Width = 250;
            this.lblZoomInfo.AutoSize = true;
            this.lblZoomInfo.BackColor = Color.Black;
            this.lblZoomInfo.ForeColor = Color.White;
            this.lblZoomInfo.Location = new Point(2, 0x19);
            this.lblZoomInfo.Name = "lblZoomInfo";
            this.lblZoomInfo.Size = new Size(0x21, 13);
            this.lblZoomInfo.TabIndex = 4;
            this.lblZoomInfo.Text = "100%";
            this.lblZoomInfo.Visible = false;
            this.previewToolStrip.Items.AddRange(new ToolStripItem[] { this.btnSavePreview, this.previewSeparator, this.btnZoomPreviewIn, this.btnZoomPreviewOut, this.btnZoomOriginalSize, this.btnZoomToFit, this.btnPlayPreview, this.btnStopPreview });
            this.previewToolStrip.Location = new Point(0, 0);
            this.previewToolStrip.Name = "previewToolStrip";
            this.previewToolStrip.RenderMode = ToolStripRenderMode.System;
            this.previewToolStrip.Size = new Size(0xb3, 0x19);
            this.previewToolStrip.TabIndex = 3;
            this.previewToolStrip.Text = "toolStrip2";
            this.btnSavePreview.Image = Resources.SaveHS;
            this.btnSavePreview.ImageTransparentColor = Color.Magenta;
            this.btnSavePreview.Name = "btnSavePreview";
            this.btnSavePreview.Size = new Size(60, 0x16);
            this.btnSavePreview.Text = "&Save...";
            this.btnSavePreview.Click += new EventHandler(this.btnSavePreview_Click);
            this.previewSeparator.Name = "previewSeparator";
            this.previewSeparator.Size = new Size(6, 0x19);
            this.btnZoomPreviewIn.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.btnZoomPreviewIn.Image = Resources.ZoomIn;
            this.btnZoomPreviewIn.ImageTransparentColor = Color.Magenta;
            this.btnZoomPreviewIn.Name = "btnZoomPreviewIn";
            this.btnZoomPreviewIn.Size = new Size(0x17, 0x16);
            this.btnZoomPreviewIn.Text = "Zoom &in";
            this.btnZoomPreviewIn.ToolTipText = "Zoom in (Numpad +)";
            this.btnZoomPreviewIn.Click += new EventHandler(this.btnZoomPreviewIn_Click);
            this.btnZoomPreviewOut.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.btnZoomPreviewOut.Image = Resources.ZoomOut;
            this.btnZoomPreviewOut.ImageTransparentColor = Color.Magenta;
            this.btnZoomPreviewOut.Name = "btnZoomPreviewOut";
            this.btnZoomPreviewOut.Size = new Size(0x17, 0x16);
            this.btnZoomPreviewOut.Text = "Zoom &out";
            this.btnZoomPreviewOut.ToolTipText = "Zoom out (Numpad -)";
            this.btnZoomPreviewOut.Click += new EventHandler(this.btnZoomPreviewOut_Click);
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
            this.btnZoomToFit.Size = new Size(0x17, 20);
            this.btnZoomToFit.Text = "&Fit image";
            this.btnZoomToFit.ToolTipText = "Fit image (Numpad *)";
            this.btnZoomToFit.Click += new EventHandler(this.btnZoomToFit_Click);
            this.btnPlayPreview.Image = Resources.PlayHS;
            this.btnPlayPreview.ImageTransparentColor = Color.Magenta;
            this.btnPlayPreview.Name = "btnPlayPreview";
            this.btnPlayPreview.Size = new Size(0x31, 20);
            this.btnPlayPreview.Text = "P&lay";
            this.btnPlayPreview.Click += new EventHandler(this.btnPlayPreview_Click);
            this.btnStopPreview.Image = Resources.StopHS;
            this.btnStopPreview.ImageTransparentColor = Color.Magenta;
            this.btnStopPreview.Name = "btnStopPreview";
            this.btnStopPreview.Size = new Size(0x33, 20);
            this.btnStopPreview.Text = "S&top";
            this.btnStopPreview.Click += new EventHandler(this.btnStopPreview_Click);
            this.previewStatus.Items.AddRange(new ToolStripItem[] { this.lblPreviewStatus });
            this.previewStatus.Location = new Point(0, 0x124);
            this.previewStatus.Name = "previewStatus";
            this.previewStatus.Size = new Size(0xb3, 0x18);
            this.previewStatus.SizingGrip = false;
            this.previewStatus.TabIndex = 2;
            this.lblPreviewStatus.BorderSides = ToolStripStatusLabelBorderSides.All;
            this.lblPreviewStatus.BorderStyle = Border3DStyle.SunkenOuter;
            this.lblPreviewStatus.Name = "lblPreviewStatus";
            this.lblPreviewStatus.Size = new Size(0xa4, 0x13);
            this.lblPreviewStatus.Spring = true;
            this.lblPreviewStatus.Text = "<<PreviewStatus>>";
            this.lblPreviewStatus.TextAlign = ContentAlignment.MiddleLeft;
            this.previewPictureBox.BorderStyle = BorderStyle.Fixed3D;
            this.previewPictureBox.Dock = DockStyle.Fill;
            this.previewPictureBox.Location = new Point(0, 0);
            this.previewPictureBox.Name = "previewPictureBox";
            this.previewPictureBox.Size = new Size(0xb3, 0x13c);
            this.previewPictureBox.TabIndex = 1;
            this.previewPictureBox.TabStop = false;
            this.previewRichTextBox.Dock = DockStyle.Fill;
            this.previewRichTextBox.Location = new Point(0, 0);
            this.previewRichTextBox.Name = "previewRichTextBox";
            this.previewRichTextBox.ReadOnly = true;
            this.previewRichTextBox.Size = new Size(0xb3, 0x13c);
            this.previewRichTextBox.TabIndex = 0;
            this.previewRichTextBox.Text = "";
            this.txtProgress.Dock = DockStyle.Fill;
            this.txtProgress.Location = new Point(0, 0);
            this.txtProgress.Multiline = true;
            this.txtProgress.Name = "txtProgress";
            this.txtProgress.ReadOnly = true;
            this.txtProgress.ScrollBars = ScrollBars.Both;
            this.txtProgress.Size = new Size(0x278, 0x4c);
            this.txtProgress.TabIndex = 1;
            this.txtProgress.WordWrap = false;
            this.pathToolStrip.Dock = DockStyle.None;
            this.pathToolStrip.GripStyle = ToolStripGripStyle.Hidden;
            this.pathToolStrip.Items.AddRange(new ToolStripItem[] { this.lblPath, this.txtPath, this.btnGoTo });
            this.pathToolStrip.Location = new Point(0, 0x19);
            this.pathToolStrip.Name = "pathToolStrip";
            this.pathToolStrip.Size = new Size(0x278, 0x19);
            this.pathToolStrip.Stretch = true;
            this.pathToolStrip.TabIndex = 30;
            this.lblPath.Name = "lblPath";
            this.lblPath.Size = new Size(0x1f, 0x16);
            this.lblPath.Text = "&Path";
            this.txtPath.Name = "txtPath";
            this.txtPath.Size = new Size(0x1fc, 0x19);
            this.txtPath.KeyDown += new KeyEventHandler(this.txtPath_KeyDown);
            this.btnGoTo.Image = Resources.GoLtr;
            this.btnGoTo.ImageTransparentColor = Color.Magenta;
            this.btnGoTo.Name = "btnGoTo";
            this.btnGoTo.Size = new Size(0x3b, 0x16);
            this.btnGoTo.Text = "Go To";
            this.btnGoTo.Click += new EventHandler(this.btnGoTo_Click);
            this.filterImageList.ImageStream = (ImageListStreamer) manager.GetObject("filterImageList.ImageStream");
            this.filterImageList.TransparentColor = Color.Transparent;
            this.filterImageList.Images.SetKeyName(0, "Filter2HS.png");
            this.filterImageList.Images.SetKeyName(1, "Filter2HSFiltered.png");
            this.fileTypeImageList.ImageStream = (ImageListStreamer) manager.GetObject("fileTypeImageList.ImageStream");
            this.fileTypeImageList.TransparentColor = Color.Transparent;
            this.fileTypeImageList.Images.SetKeyName(0, "sound");
            this.directoryImageList.ImageStream = (ImageListStreamer) manager.GetObject("directoryImageList.ImageStream");
            this.directoryImageList.TransparentColor = Color.Magenta;
            this.directoryImageList.Images.SetKeyName(0, "book_active_directory.bmp");
            this.directoryImageList.Images.SetKeyName(1, "VSFolder_closed.bmp");
            this.directoryImageList.Images.SetKeyName(2, "VSFolder_open.bmp");
            this.AllowDrop = true;
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            //base.AutoScaleMode = AutoScaleMode.Font;
            base.ClientSize = new Size(0x278, 0x1be);
            base.Controls.Add(this.archiveStatus);
            base.Controls.Add(this.toolStripContainer1);
            base.Icon = (Icon) manager.GetObject("$this.Icon");
            base.Name = "frmMain";
            this.Text = "Ravioli Explorer";
            base.FormClosing += new FormClosingEventHandler(this.frmMain_FormClosing);
            base.FormClosed += new FormClosedEventHandler(this.frmMain_FormClosed);
            base.Load += new EventHandler(this.Form1_Load);
            base.Shown += new EventHandler(this.frmMain_Shown);
            base.DragDrop += new DragEventHandler(this.frmMain_DragDrop);
            base.DragEnter += new DragEventHandler(this.frmMain_DragEnter);
            base.Resize += new EventHandler(this.frmMain_Resize);
            this.archiveStatus.ResumeLayout(false);
            this.archiveStatus.PerformLayout();
            this.contextMenu.ResumeLayout(false);
            this.mainToolStrip.ResumeLayout(false);
            this.mainToolStrip.PerformLayout();
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.browseOutputSplitter.Panel1.ResumeLayout(false);
            this.browseOutputSplitter.Panel2.ResumeLayout(false);
            this.browseOutputSplitter.Panel2.PerformLayout();
            this.browseOutputSplitter.EndInit();
            this.browseOutputSplitter.ResumeLayout(false);
            this.browserSplitter.Panel1.ResumeLayout(false);
            this.browserSplitter.Panel2.ResumeLayout(false);
            this.browserSplitter.EndInit();
            this.browserSplitter.ResumeLayout(false);
            this.browsePreviewSplitter.Panel1.ResumeLayout(false);
            this.browsePreviewSplitter.Panel2.ResumeLayout(false);
            this.browsePreviewSplitter.Panel2.PerformLayout();
            this.browsePreviewSplitter.EndInit();
            this.browsePreviewSplitter.ResumeLayout(false);
            this.previewToolStrip.ResumeLayout(false);
            this.previewToolStrip.PerformLayout();
            this.previewStatus.ResumeLayout(false);
            this.previewStatus.PerformLayout();
            ((ISupportInitialize) this.previewPictureBox).EndInit();
            this.pathToolStrip.ResumeLayout(false);
            this.pathToolStrip.PerformLayout();
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        private bool IsDragDropOK(DragEventArgs e)
        {
            bool flag = false;
            if (((!e.Data.GetDataPresent(DataFormats.FileDrop) || !this.btnOpen.Enabled) || ((Application.OpenForms.Count != 1) || ((e.AllowedEffect & DragDropEffects.Copy) != DragDropEffects.Copy))) || (this.dragOutFileInfo != null))
            {
                return flag;
            }
            if (!this.config.UnknownFiles.AllowScanning)
            {
                string[] data = (string[]) e.Data.GetData(DataFormats.FileDrop);
                return (this.archivePluginManager.FindPlugins(Path.GetExtension(data[0])).Length > 0);
            }
            return true;
        }

        private bool IsPreviewActive()
        {
            return !this.browsePreviewSplitter.Panel2Collapsed;
        }

        private bool IsPreviewImageZoomLocked()
        {
            return this.config.Browser.Preview.LockImageZoom;
        }

        private bool IsSupportedImageFormat(IFileInfo file, Stream stream)
        {
            return (this.imagePluginManager.DetectImageFormat(file, stream) != null);
        }

        private bool IsSupportedSoundFormat(IFileInfo file)
        {
            return (this.soundPluginManager.FindPlayerPlugin(Path.GetExtension(file.Name)) != null);
        }

        private void LoadConfig()
        {
            
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "ExplorerConfig.xml");
            if (File.Exists(path))
            {
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(ExplorerConfig));
                    FileStream stream = new FileStream(path, FileMode.Open);
                    try
                    {
                        this.config = (ExplorerConfig) serializer.Deserialize(stream);
                    }
                    finally
                    {
                        stream.Close();
                    }
                }
                catch (Exception exception)
                {
                    this.ShowError("Unable to load configuration from \"" + path + "\"." + Environment.NewLine + Environment.NewLine + exception.Message);
                }
            }
            else
            {
                this.config = new ExplorerConfig();
            }
        }

        private void lvwInfo_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            this.ChangeFileListSort(this.fileListSortCriterionMaps.MapColumnToSortCriterion(e.Column));
        }

        private void lvwInfo_DoubleClick(object sender, EventArgs e)
        {
            if (this.lvwInfo.SelectedIndices.Count > 0)
            {
                this.OpenSelectedFiles();
            }
        }

        private void lvwInfo_ItemDrag(object sender, ItemDragEventArgs e)
        {
            this.dragOutFileInfo = new IFileInfo[this.lvwInfo.SelectedIndices.Count];
            string[] data = new string[this.lvwInfo.SelectedIndices.Count];
            string tempArchiveLocation = this.GetTempArchiveLocation();
            for (int i = 0; i < this.lvwInfo.SelectedIndices.Count; i++)
            {
                ListViewItem item = this.cachedListViewItems[this.lvwInfo.SelectedIndices[i]];
                this.dragOutFileInfo[i] = (IFileInfo) item.Tag;
                data[i] = Path.Combine(Path.Combine(tempArchiveLocation, Path.GetDirectoryName(this.dragOutFileInfo[i].Name)), Path.GetFileName(this.dragOutFileInfo[i].Name));
            }
            DataObject obj2 = new DataObject(DataFormats.FileDrop, data);
            this.lvwInfo.DoDragDrop(obj2, DragDropEffects.Copy);
        }

        private void lvwInfo_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Enter) && (this.lvwInfo.SelectedIndices.Count > 0))
            {
                this.OpenSelectedFiles();
                e.SuppressKeyPress = true;
            }
            else if ((this.previewImageZoomer != null) && this.allowPreviewZoom)
            {
                if (e.KeyCode == Keys.Add)
                {
                    this.previewImageZoomer.ZoomImageIn();
                    if (this.IsPreviewImageZoomLocked())
                    {
                        this.previewImageZoomDefault = this.previewImageZoomer.Percent;
                    }
                    e.SuppressKeyPress = true;
                }
                else if (e.KeyCode == Keys.Subtract)
                {
                    this.previewImageZoomer.ZoomImageOut();
                    if (this.IsPreviewImageZoomLocked())
                    {
                        this.previewImageZoomDefault = this.previewImageZoomer.Percent;
                    }
                    e.SuppressKeyPress = true;
                }
                else if (e.KeyCode == Keys.Multiply)
                {
                    this.previewImageZoomer.ZoomImageToFit();
                    if (this.IsPreviewImageZoomLocked())
                    {
                        this.previewImageZoomDefault = this.previewImageZoomer.Percent;
                    }
                    e.SuppressKeyPress = true;
                }
                else if (e.KeyCode == Keys.Divide)
                {
                    this.previewImageZoomer.ZoomImageOriginalSize();
                    if (this.IsPreviewImageZoomLocked())
                    {
                        this.previewImageZoomDefault = this.previewImageZoomer.Percent;
                    }
                    e.SuppressKeyPress = true;
                }
            }
        }

        private void lvwInfo_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            if (e.Action == DragAction.Drop)
            {
                string str;
                this.ExtractTemporaryFiles(this.dragOutFileInfo, out str);
                this.dragOutFileInfo = null;
            }
            else if (e.Action == DragAction.Cancel)
            {
                this.dragOutFileInfo = null;
            }
        }

        private void lvwInfo_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            string key = "Index" + e.ItemIndex.ToString();
            if (this.config.Browser.FileListView == ExplorerConfig.FileListView.Thumbnails)
            {
                if (!this.lvwInfo.LargeImageList.Images.ContainsKey(key))
                {
                    Image image = this.GenerateThumbnail(this.cachedListViewItems[e.ItemIndex].Tag as IFileSystemEntry);
                    this.lvwInfo.LargeImageList.Images.Add(key, image);
                }
                e.Item = this.cachedListViewItems[e.ItemIndex];
                e.Item.ImageIndex = this.lvwInfo.LargeImageList.Images.IndexOfKey(key);
            }
            else if (this.config.Browser.FileListView == ExplorerConfig.FileListView.Details)
            {
                if (!this.lvwInfo.SmallImageList.Images.ContainsKey(key))
                {
                    Image image2 = this.GenerateSmallIcon(this.cachedListViewItems[e.ItemIndex].Tag as IFileSystemEntry);
                    this.lvwInfo.SmallImageList.Images.Add(key, image2);
                }
                e.Item = this.cachedListViewItems[e.ItemIndex];
                e.Item.ImageIndex = this.lvwInfo.SmallImageList.Images.IndexOfKey(key);
            }
        }

        private void lvwInfo_SearchForVirtualItem(object sender, SearchForVirtualItemEventArgs e)
        {
            if (e.IsTextSearch)
            {
                if ((e.Direction == SearchDirectionHint.Down) || (e.Direction == SearchDirectionHint.Right))
                {
                    for (int i = e.StartIndex; i < this.cachedListViewItems.Length; i++)
                    {
                        ListViewItem item = this.cachedListViewItems[i];
                        if (item.Text.StartsWith(e.Text, StringComparison.OrdinalIgnoreCase))
                        {
                            e.Index = i;
                            return;
                        }
                    }
                }
                else if ((e.Direction == SearchDirectionHint.Up) || (e.Direction == SearchDirectionHint.Left))
                {
                    for (int j = e.StartIndex; j >= 0; j--)
                    {
                        ListViewItem item2 = this.cachedListViewItems[j];
                        if (item2.Text.StartsWith(e.Text, StringComparison.OrdinalIgnoreCase))
                        {
                            e.Index = j;
                            return;
                        }
                    }
                }
            }
        }

        private void lvwInfo_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.CheckCommands();
            if (this.IsPreviewActive())
            {
                if (this.lvwInfo.SelectedIndices.Count == 1)
                {
                    this.PreviewSelectedFiles();
                }
                else
                {
                    this.ClearPreview();
                }
            }
        }

        private static string MakeArrayString(StringCollection array)
        {
            StringBuilder builder = new StringBuilder();
            foreach (string str in array)
            {
                if (builder.Length > 0)
                {
                    builder.Append(", ");
                }
                builder.Append(str);
            }
            return builder.ToString();
        }

        private void mnuAllowScanning_Click(object sender, EventArgs e)
        {
            this.config.UnknownFiles.AllowScanning = !this.config.UnknownFiles.AllowScanning;
            this.mnuAllowScanning.Checked = this.config.UnknownFiles.AllowScanning;
        }

        private void mnuAskBeforeScanning_Click(object sender, EventArgs e)
        {
            this.config.UnknownFiles.AskBeforeScanning = !this.config.UnknownFiles.AskBeforeScanning;
            this.mnuAskBeforeScanning.Checked = this.config.UnknownFiles.AskBeforeScanning;
        }

        private void mnuAutoPlaySounds_Click(object sender, EventArgs e)
        {
            this.config.Browser.Preview.AutoPlaySounds = !this.config.Browser.Preview.AutoPlaySounds;
            this.mnuAutoPlaySounds.Checked = this.config.Browser.Preview.AutoPlaySounds;
        }

        private void mnuDirectoryDisplayFlat_Click(object sender, EventArgs e)
        {
            if (this.config.Browser.DirectoryView != ExplorerConfig.DirectoryView.Flat)
            {
                this.config.Browser.DirectoryView = ExplorerConfig.DirectoryView.Flat;
                this.mnuDirectoryDisplayHierarchical.Checked = false;
                this.mnuDirectoryDisplayFlat.Checked = true;
                this.browserSplitter.Panel1Collapsed = true;
                if (this.fileListFilters != null)
                {
                    this.fileListFilters.DirectoryFilter = null;
                }
                this.txtPath.Text = string.Empty;
                this.pathToolStrip.Visible = false;
                this.RefreshFileList(true);
            }
        }

        private void mnuDirectoryDisplayHierarchical_Click(object sender, EventArgs e)
        {
            if (this.config.Browser.DirectoryView != ExplorerConfig.DirectoryView.Hierarchical)
            {
                this.config.Browser.DirectoryView = ExplorerConfig.DirectoryView.Hierarchical;
                this.mnuDirectoryDisplayHierarchical.Checked = true;
                this.mnuDirectoryDisplayFlat.Checked = false;
                this.browserSplitter.Panel1Collapsed = false;
                ListViewItem item = null;
                if (this.lvwInfo.SelectedIndices.Count > 0)
                {
                    item = (ListViewItem) this.cachedListViewItems[this.lvwInfo.SelectedIndices[0]].Clone();
                }
                this.pathToolStrip.Visible = true;
                if (item != null)
                {
                    IFileInfo tag = (IFileInfo) item.Tag;
                    string directoryName = Path.GetDirectoryName(tag.Name);
                    if (!directoryName.StartsWith(this.treeViewRootKey))
                    {
                        directoryName = this.treeViewRootKey + directoryName;
                    }
                    TreeNode[] nodeArray = this.tvwDirectories.Nodes.Find(directoryName, true);
                    if (nodeArray.Length > 0)
                    {
                        this.NavigateTree(nodeArray[0], true);
                    }
                }
                else
                {
                    this.NavigateTree(this.treeViewRootKey);
                }
            }
        }

        private void mnuExtract_Click(object sender, EventArgs e)
        {
            this.ExtractFiles();
        }

        private void mnuFileListDisplayDetails_Click(object sender, EventArgs e)
        {
            if (this.config.Browser.FileListView != ExplorerConfig.FileListView.Details)
            {
                this.config.Browser.FileListView = ExplorerConfig.FileListView.Details;
                this.mnuFileListDisplayDetails.Checked = true;
                this.mnuFileListDisplayThumbnails.Checked = false;
                this.lvwInfo.View = View.Details;
            }
        }

        private void mnuFileListDisplayThumbnails_Click(object sender, EventArgs e)
        {
            if (this.config.Browser.FileListView != ExplorerConfig.FileListView.Thumbnails)
            {
                this.config.Browser.FileListView = ExplorerConfig.FileListView.Thumbnails;
                this.mnuFileListDisplayDetails.Checked = false;
                this.mnuFileListDisplayThumbnails.Checked = true;
                if (this.lvwInfo.LargeImageList == null)
                {
                    this.RefreshFileList(true);
                }
                else
                {
                    this.lvwInfo.View = View.LargeIcon;
                }
            }
        }

        private void mnuLockImageZoom_Click(object sender, EventArgs e)
        {
            if (!this.IsPreviewImageZoomLocked())
            {
                if (this.previewImageZoomer != null)
                {
                    this.previewImageZoomDefault = this.previewImageZoomer.Percent;
                    this.config.Browser.Preview.LockImageZoom = true;
                }
            }
            else
            {
                this.previewImageZoomDefault = -1;
                this.config.Browser.Preview.LockImageZoom = false;
            }
            this.mnuLockImageZoom.Checked = this.IsPreviewImageZoomLocked();
        }

        private void mnuOpenFile_Click(object sender, EventArgs e)
        {
            this.OpenSelectedFiles();
        }

        private void mnuPlayPreview_Click(object sender, EventArgs e)
        {
            if (this.previewSoundPlayer != null)
            {
                this.previewSoundPlayer.Play();
            }
        }

        private void mnuSortItemsByCompressed_Click(object sender, EventArgs e)
        {
            this.ChangeFileListSort(this.fileListSortCriterionMaps.MapMenuItemToSortCriterion(this.mnuSortItemsByCompressed));
        }

        private void mnuSortItemsByCSize_Click(object sender, EventArgs e)
        {
            this.ChangeFileListSort(this.fileListSortCriterionMaps.MapMenuItemToSortCriterion(this.mnuSortItemsByCSize));
        }

        private void mnuSortItemsByName_Click(object sender, EventArgs e)
        {
            this.ChangeFileListSort(this.fileListSortCriterionMaps.MapMenuItemToSortCriterion(this.mnuSortItemsByName));
        }

        private void mnuSortItemsByPath_Click(object sender, EventArgs e)
        {
            this.ChangeFileListSort(this.fileListSortCriterionMaps.MapMenuItemToSortCriterion(this.mnuSortItemsByPath));
        }

        private void mnuSortItemsBySize_Click(object sender, EventArgs e)
        {
            this.ChangeFileListSort(this.fileListSortCriterionMaps.MapMenuItemToSortCriterion(this.mnuSortItemsBySize));
        }

        private void mnuSortItemsByType_Click(object sender, EventArgs e)
        {
            this.ChangeFileListSort(this.fileListSortCriterionMaps.MapMenuItemToSortCriterion(this.mnuSortItemsByType));
        }

        private void mnuStartupBehaviourDoNothing_Click(object sender, EventArgs e)
        {
            this.SetNewStartupBehaviour(ExplorerConfig.StartupBehaviour.DoNothing);
        }

        private void mnuStartupBehaviourOpen_Click(object sender, EventArgs e)
        {
            this.SetNewStartupBehaviour(ExplorerConfig.StartupBehaviour.OpenFile);
        }

        private void mnuStartupBehaviourOpenGame_Click(object sender, EventArgs e)
        {
            this.SetNewStartupBehaviour(ExplorerConfig.StartupBehaviour.OpenGame);
        }

        private void mnuStopPreview_Click(object sender, EventArgs e)
        {
            if (this.previewSoundPlayer != null)
            {
                this.previewSoundPlayer.Stop();
            }
        }

        private void mnuTogglePreview_Click(object sender, EventArgs e)
        {
            this.TogglePreview();
        }

        private void mnuToggleSoundPreview_Click(object sender, EventArgs e)
        {
            bool flag = false;
            if (this.previewSoundPlayer != null)
            {
                this.previewSoundPlayer.Stop();
                flag = true;
            }
            this.config.Browser.Preview.PreviewSounds = !this.config.Browser.Preview.PreviewSounds;
            this.mnuToggleSoundPreview.Checked = this.config.Browser.Preview.PreviewSounds;
            if (this.config.Browser.Preview.PreviewSounds || flag)
            {
                this.PreviewSelectedFiles();
            }
        }

        private void mnuView_Click(object sender, EventArgs e)
        {
            this.ViewSelectedFiles();
        }

        private void mnuViewAsText_Click(object sender, EventArgs e)
        {
            this.ViewSelectedFiles(true);
        }

        private bool NavigateTree(string fullPath)
        {
            bool flag = false;
            TreeNode[] nodeArray = this.tvwDirectories.Nodes.Find(fullPath, true);
            if (nodeArray.Length > 0)
            {
                this.NavigateTree(nodeArray[0]);
                flag = true;
            }
            return flag;
        }

        private void NavigateTree(TreeNode node)
        {
            this.NavigateTree(node, false);
        }

        private void NavigateTree(TreeNode node, bool preserveFileSelections)
        {
            if (!preserveFileSelections)
            {
                this.ClearPreview();
            }
            string tag = (string) node.Tag;
            if (this.fileListFilters == null)
            {
                this.fileListFilters = new FileListFilters();
            }
            this.fileListFilters.DirectoryFilter = tag;
            this.txtPath.Text = tag;
            this.tvwDirectories.AfterSelect -= new TreeViewEventHandler(this.tvwDirectories_AfterSelect);
            try
            {
                if (this.tvwDirectories.SelectedNode != node)
                {
                    this.tvwDirectories.SelectedNode = node;
                }
            }
            finally
            {
                this.tvwDirectories.AfterSelect += new TreeViewEventHandler(this.tvwDirectories_AfterSelect);
            }
            this.RefreshFileList(preserveFileSelections);
            this.CheckCommands();
        }

        private void NavigateUserSpecifiedPath()
        {
            string text = this.txtPath.Text;
            if (((this.treeViewRootKey.Length == 0) || ((this.treeViewRootKey.Length > 0) && (text.Length > 0))) && !this.NavigateTree(text))
            {
                this.ShowWarning("\"" + text + "\" was not found.");
            }
        }

        private void OpenArchiveThreadMethod(object data)
        {
            OpenFileWaitData data2 = (OpenFileWaitData) data;
            data2.Archive.Open(data2.File);
        }

        private void OpenFile(string file)
        {
            if (!this.extractor.IsRunning && !string.IsNullOrEmpty(file))
            {
                if (!File.Exists(file))
                {
                    this.ShowWarning("File \"" + file + "\"does not exist.");
                }
                else
                {
                    try
                    {
                        Cursor.Current = Cursors.WaitCursor;
                        ArchivePluginMapping pluginMapping = null;
                        List<ArchivePluginMapping> list = new List<ArchivePluginMapping>();
                        ArchivePluginMapping[] mappingArray = this.archivePluginManager.FindPlugins(Path.GetExtension(file));
                        if ((mappingArray != null) && (mappingArray.Length > 0))
                        {
                            foreach (ArchivePluginMapping mapping2 in mappingArray)
                            {
                                try
                                {
                                    if (this.archivePluginManager.CreateInstance(mapping2).IsValidFormat(file))
                                    {
                                        list.Add(mapping2);
                                    }
                                }
                                catch (Exception exception)
                                {
                                    Cursor.Current = Cursors.Default;
                                    this.ShowWarning("The plug-in \"" + mapping2.TypeName + "\" caused an error while checking the file type: " + exception.Message);
                                }
                            }
                            if (list.Count > 0)
                            {
                                if (list.Count == 1)
                                {
                                    pluginMapping = list[0];
                                }
                                else
                                {
                                    pluginMapping = (ArchivePluginMapping) this.DisambiguateFileFormat(file, list.ToArray());
                                }
                                this.OpenFile(file, pluginMapping);
                            }
                            else
                            {
                                Cursor.Current = Cursors.Default;
                                string errorMessage = "Unknown file type.";
                                this.HandleUnknownFile(file, errorMessage);
                            }
                        }
                        else
                        {
                            Cursor.Current = Cursors.Default;
                            string str2 = "Unknown file type.";
                            this.HandleUnknownFile(file, str2);
                        }
                    }
                    catch (Exception exception2)
                    {
                        Cursor.Current = Cursors.Default;
                        string message = exception2.Message;
                        this.ShowError(message);
                    }
                }
            }
        }

        private void OpenFile(string file, ArchivePluginMapping pluginMapping)
        {
            try
            {
                if (pluginMapping != null)
                {
                    string str;
                    if (this.AskForRootDirectory(pluginMapping, file, out str))
                    {
                        Cursor.Current = Cursors.WaitCursor;
                        IArchive archive = this.archivePluginManager.CreateInstance(pluginMapping);
                        IRootDirectory directory = archive as IRootDirectory;
                        if (directory != null)
                        {
                            directory.RootDirectory = str;
                        }
                        IDataWriter writer = archive as IDataWriter;
                        if (writer != null)
                        {
                            string pluginDataDirectory = this.archivePluginManager.PluginDataDirectory;
                            if (!Directory.Exists(pluginDataDirectory))
                            {
                                Directory.CreateDirectory(pluginDataDirectory);
                            }
                            writer.DataDirectory = pluginDataDirectory;
                        }
                        OpenFileWaitData data = new OpenFileWaitData {
                            Archive = archive,
                            File = file
                        };
                        frmWait wait = new frmWait {
                            Owner = this,
                            Callback = new ParameterizedThreadStart(this.OpenArchiveThreadMethod),
                            CallbackData = data,
                            Message = "Loading..."
                        };
                        wait.ExecuteOnNewThread();
                        Exception exception = wait.Exception;
                        wait.Dispose();

                        if (exception != null)
                        {
                            throw exception;
                        }
                        this.ClearPreview();
                        this.txtProgress.Clear();
                        this.DestroyFilters();
                        if (this.currentFile != null)
                        {
                            this.currentFile.Close();
                            this.currentFile = null;
                        }

                        this.CleanTempDirs(true);
                        this.previewImageZoomDefault = -1;
                        this.currentFile = archive;
                        this.Text = "Ravioli Explorer - " + this.GetDisplayFileName();
                        this.CreateFilters();

                        if (this.config.Browser.DirectoryView == ExplorerConfig.DirectoryView.Hierarchical)
                        {
                            this.NavigateTree(this.treeViewRootKey);
                        }
                        else
                        {
                            this.RefreshFileList();
                        }

                        this.ApplyFileListSortSettings(this.config.Browser.SortCriterion, this.config.Browser.SortOrder);
                        this.lblArchiveType.Text = archive.TypeName;
                        this.lblArchiveType.Visible = true;
                        if (archive.Comment.Length > 0)
                        {
                            string comment = archive.Comment;
                            if (comment.Contains("\r") || comment.Contains("\n"))
                            {
                                MessageBox.Show(this, comment, "Archive Comment");
                            }
                            else
                            {
                                this.lblArchiveType.Text = this.lblArchiveType.Text + " (" + comment + ")";
                            }
                        }
                        this.CheckCommands();
                        Cursor.Current = Cursors.Default;
                    }
                }
                else
                {
                    this.ShowWarning("Unknown file type.");
                }
            }
            catch (Exception exception2)
            {
                Cursor.Current = Cursors.Default;
                string message = exception2.Message;
                this.ShowError(message);
            }
        }

        private void OpenGame(string directory, GameViewerPluginMapping pluginMapping)
        {
            try
            {
                string fileOfDirectory = FileSystemHelper.GetFileOfDirectory(directory);
                if (!this.archivePluginManager.CreateInstance(pluginMapping).IsValidFormat(fileOfDirectory))
                {
                    this.ShowWarning("\"" + pluginMapping.TypeName + "\" was not found at the specified location.");
                }
                else
                {
                    this.OpenFile(fileOfDirectory, pluginMapping);
                }
            }
            catch (Exception exception)
            {
                Cursor.Current = Cursors.Default;
                string message = exception.Message;
                this.ShowError(message);
            }
        }

        private void OpenSelectedFiles()
        {
            if (this.currentFile == null)
            {
                this.ShowWarning("No archive open.");
            }
            else if (this.lvwInfo.SelectedIndices.Count == 0)
            {
                this.ShowWarning("No files selected.");
            }
            else
            {
                string str;
                IFileInfo[] files = new IFileInfo[this.lvwInfo.SelectedIndices.Count];
                int num = 0;
                foreach (int num2 in this.lvwInfo.SelectedIndices)
                {
                    files[num++] = (IFileInfo) this.cachedListViewItems[num2].Tag;
                }
                if (this.ExtractTemporaryFiles(files, out str))
                {
                    if (this.previewSoundPlayer != null)
                    {
                        this.previewSoundPlayer.Stop();
                    }
                    Cursor.Current = Cursors.WaitCursor;
                    StringCollection strings = new StringCollection();
                    foreach (IFileInfo info in files)
                    {
                        try
                        {
                            string str2 = Path.Combine(Path.Combine(str, Path.GetDirectoryName(info.Name)), Path.GetFileName(info.Name));
                            ProcessStartInfo startInfo = new ProcessStartInfo {
                                FileName = str2,
                                UseShellExecute = true,
                                ErrorDialog = true,
                                ErrorDialogParentHandle = base.Handle
                            };
                            Process.Start(startInfo);
                        }
                        catch (Exception exception)
                        {
                            strings.Add(info.Name + ": " + exception.Message);
                        }
                    }
                    Cursor.Current = Cursors.Default;
                    if (strings.Count > 0)
                    {
                        frmMessages messages = new frmMessages();
                        messages.SetMessages(strings);
                        messages.ShowDialog(this);
                        messages.Dispose();
                    }
                }
            }
        }

        private void PreviewAsImage(IFileInfo file, Stream stream, ImagePluginMapping mapping)
        {
            if (mapping == null)
            {
                mapping = this.imagePluginManager.DetectImageFormat(file, stream);
            }
            if (mapping != null)
            {
                IImageLoader loader = this.imagePluginManager.CreateLoaderInstance(mapping);
                IPaletteConsumer consumer = loader as IPaletteConsumer;
                IPaletteProvider currentFile = this.currentFile as IPaletteProvider;
                if ((consumer != null) && (currentFile != null))
                {
                    consumer.Palette = currentFile.Palette;
                }
                Image image = loader.LoadImage(stream);
                this.previewFile = file;
                this.previewStream = stream;
                this.previewImageOriginal = image;
                this.previewImageLoader = loader;
                this.PreviewAsImageInternal(image, this.previewImageZoomDefault);
                this.lblPreviewStatus.Text = this.GetImageDetailText(image, loader.TypeName);
                this.btnSavePreview.Enabled = true;
                this.previewSeparator.Visible = true;
                this.btnZoomPreviewIn.Enabled = true;
                this.btnZoomPreviewIn.Visible = true;
                this.btnZoomPreviewOut.Enabled = true;
                this.btnZoomPreviewOut.Visible = true;
                this.btnZoomOriginalSize.Enabled = true;
                this.btnZoomOriginalSize.Visible = true;
                this.btnZoomToFit.Enabled = true;
                this.btnZoomToFit.Visible = true;
                this.allowPreviewZoom = true;
                loader = null;
            }
        }

        private void PreviewAsImageInternal(Image image, int zoomPercent)
        {
            this.previewPictureBox.BringToFront();
            this.previewImageZoomer = new ImageZoomer(image, this.previewPictureBox, this.lblZoomInfo, zoomPercent);
            this.allowPreviewZoom = false;
        }

        private void PreviewAsSound(IFileInfo file, Stream stream, bool autoPlay)
        {
            PluginMapping plugin = this.soundPluginManager.FindPlayerPlugin(Path.GetExtension(file.Name));
            if (plugin != null)
            {
                ISoundPlayer player = this.soundPluginManager.CreatePlayerInstance(plugin);
                player.LoadFromStream(stream);
                this.previewFile = file;
                this.previewStream = stream;
                this.previewSoundPlayer = player;
                this.lblPreviewStatus.Text = GetSoundDetailInfo(player);
                this.PreviewAsImageInternal(this.fileTypeImageList.Images["sound"], -1);
                this.btnSavePreview.Enabled = player is ISoundExport;
                this.previewSeparator.Visible = true;
                this.btnStopPreview.Visible = true;
                this.btnStopPreview.Enabled = true;
                this.btnPlayPreview.Visible = true;
                this.btnPlayPreview.Enabled = true;
                this.mnuPlayPreview.Visible = true;
                this.mnuPlayPreview.Enabled = true;
                this.mnuStopPreview.Visible = true;
                this.mnuStopPreview.Enabled = true;
                if (autoPlay)
                {
                    player.Play();
                }
            }
        }

        private void PreviewAsText(Stream stream)
        {
            string str2;
            bool flag = false;
            this.previewRichTextBox.BringToFront();
            Encoding currentEncoding = null;
            StreamReader reader = new StreamReader(stream, Encoding.Default, true);
            try
            {
                int num;
                char[] buffer = new char[0x4000];
                while ((num = reader.ReadBlock(buffer, 0, buffer.Length)) > 0)
                {
                    string text = new string(buffer, 0, num);
                    if (currentEncoding == null)
                    {
                        currentEncoding = reader.CurrentEncoding;
                    }
                    this.previewRichTextBox.AppendText(text);
                    if (text.Contains("\0"))
                    {
                        flag = true;
                        goto Label_007D;
                    }
                }
            }
            finally
            {
                reader.Dispose();
                stream.Dispose();
            }
        Label_007D:
            if (flag)
            {
                this.previewRichTextBox.BringToFront();
                this.previewRichTextBox.AppendText(Environment.NewLine + "(Binary file truncated in preview)");
                str2 = "Binary file";
            }
            else
            {
                str2 = "Text file";
                if (currentEncoding != null)
                {
                    str2 = str2 + " / " + currentEncoding.EncodingName;
                }
            }
            this.lblPreviewStatus.Text = str2;
            this.previewRichTextBox.SelectionStart = 0;
        }

        private void PreviewSelectedFiles()
        {
            if ((this.currentFile != null) && (this.lvwInfo.SelectedIndices.Count != 0))
            {
                IFileInfo tag = this.cachedListViewItems[this.lvwInfo.SelectedIndices[0]].Tag as IFileInfo;
                if (tag != null)
                {
                    this.ClearPreview();
                    if (this.IsSupportedSoundFormat(tag) && !this.config.Browser.Preview.PreviewSounds)
                    {
                        this.AppendPreviewText("(Sound previews are disabled)");
                    }
                    else if (tag.Size > 0x500000L)
                    {
                        this.AppendPreviewText("(Preview skipped - size limit exceeded)");
                    }
                    else
                    {
                        MemoryStream outputStream = null;
                        try
                        {
                            Cursor.Current = Cursors.WaitCursor;
                            if (!Monitor.TryEnter(this.currentFile, 0x4e20))
                            {
                                Cursor.Current = Cursors.Default;
                                this.AppendPreviewText("(Preview error: Cannot lock the archive.)");
                                return;
                            }
                            try
                            {
                                outputStream = new MemoryStream();
                                this.currentFile.ExtractFile(tag, outputStream, 0x500000L);
                            }
                            finally
                            {
                                Monitor.Exit(this.currentFile);
                            }
                            if (outputStream.Length >= 0x500000L)
                            {
                                Cursor.Current = Cursors.Default;
                                this.AppendPreviewText("(Preview skipped - size limit exceeded)");
                                outputStream.Dispose();
                                return;
                            }
                            if (outputStream.CanSeek && (outputStream.Position == outputStream.Length))
                            {
                                outputStream.Position = 0L;
                            }
                            ImagePluginMapping mapping = this.imagePluginManager.DetectImageFormat(tag, outputStream);
                            if (mapping != null)
                            {
                                this.PreviewAsImage(tag, outputStream, mapping);
                            }
                            else if (this.IsSupportedSoundFormat(tag))
                            {
                                bool autoPlaySounds = this.config.Browser.Preview.AutoPlaySounds;
                                this.PreviewAsSound(tag, outputStream, autoPlaySounds);
                            }
                            else
                            {
                                this.PreviewAsText(outputStream);
                            }
                        }
                        catch (Exception exception)
                        {
                            Cursor.Current = Cursors.Default;
                            if (outputStream != null)
                            {
                                outputStream.Dispose();
                                outputStream = null;
                            }
                            this.AppendPreviewText("(Preview error: " + exception.Message + ")");
                            return;
                        }
                        Cursor.Current = Cursors.Default;
                    }
                }
            }
        }

        private void RefreshFileList()
        {
            this.RefreshFileList(false);
        }

        private void RefreshFileList(bool preserveSelections)
        {
            ListViewItem[] itemArray = null;
            if (preserveSelections)
            {
                itemArray = new ListViewItem[this.lvwInfo.SelectedIndices.Count];
                for (int i = 0; i < this.lvwInfo.SelectedIndices.Count; i++)
                {
                    itemArray[i] = (ListViewItem) this.cachedListViewItems[this.lvwInfo.SelectedIndices[i]].Clone();
                }
            }
            if (this.currentFile != null)
            {
                if ((this.fileListFilters == null) || ((this.fileListFilters != null) && !this.fileListFilters.HasActiveFilters()))
                {
                    this.DisplayFiles(this.currentFile.Files);
                }
                else
                {
                    IDirectoryInfo[] collection = null;
                    List<IFileSystemEntry> files = new List<IFileSystemEntry>();
                    if (collection != null)
                    {
                        files.AddRange(collection);
                    }
                    IFileInfo[] infoArray2 = this.ApplyFileListFilters();
                    files.AddRange(infoArray2);
                    this.DisplayFiles(files);
                }
                if ((itemArray != null) && (itemArray.Length > 0))
                {
                    this.lvwInfo.SelectedIndexChanged -= new EventHandler(this.lvwInfo_SelectedIndexChanged);
                    try
                    {
                        bool flag = false;
                        for (int j = 0; j < this.cachedListViewItems.Length; j++)
                        {
                            ListViewItem item = this.cachedListViewItems[j];
                            foreach (ListViewItem item2 in itemArray)
                            {
                                if (((IFileInfo) item2.Tag).ID == ((IFileInfo) item.Tag).ID)
                                {
                                    this.lvwInfo.Items[j].Selected = true;
                                    if (!flag)
                                    {
                                        this.lvwInfo.EnsureVisible(j);
                                        this.lvwInfo.Items[j].Focused = true;
                                        flag = true;
                                    }
                                    break;
                                }
                            }
                        }
                    }
                    finally
                    {
                        this.lvwInfo.SelectedIndexChanged += new EventHandler(this.lvwInfo_SelectedIndexChanged);
                    }
                }
            }
        }

        private string ReplaceInvalidChars(string fileName)
        {
            StringBuilder builder = new StringBuilder(fileName);
            foreach (char ch in Path.GetInvalidPathChars())
            {
                builder.Replace(ch, '_');
            }
            builder.Replace(':', '_');
            builder.Replace('?', '_');
            return builder.ToString();
        }

        private void SaveConfig()
        {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "ExplorerConfig.xml");
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ExplorerConfig));
                FileStream stream = new FileStream(path, FileMode.Create);
                try
                {
                    serializer.Serialize((Stream) stream, this.config);
                }
                finally
                {
                    stream.Close();
                }
            }
            catch (Exception)
            {
            }
        }

        private void SaveImage(IFileInfo file, Image image, IImageLoader imageLoader)
        {
            if ((image == null) || image.Size.IsEmpty)
            {
                this.ShowWarning("No image to save.");
            }
            else
            {
                string name = file.Name;
                this.saveFileDialog.FileName = Path.GetFileNameWithoutExtension(name);
                if (this.config.Extract.LastSaveDir.Length > 0)
                {
                    this.saveFileDialog.InitialDirectory = this.config.Extract.LastSaveDir;
                }
                this.saveFileDialog.Filter = this.defaultImageSaveFilter;
                this.saveFileDialog.FilterIndex = this.defaultImageSaveFilterIndex;
                ImagePluginMapping mapping = this.imagePluginManager.FindSaverPlugin(this.config.Extract.TargetImageFormat);
                if (mapping != null)
                {
                    int num = -1;
                    for (int i = 0; i < this.imagePluginManager.AvailableSaverPlugins.Length; i++)
                    {
                        if (this.imagePluginManager.AvailableSaverPlugins[i] == mapping)
                        {
                            num = i;
                            break;
                        }
                    }
                    if (num >= 0)
                    {
                        this.saveFileDialog.FilterIndex = num + 1;
                    }
                }
                if (this.saveFileDialog.ShowDialog(this) == DialogResult.OK)
                {
                    this.config.Extract.LastSaveDir = Path.GetDirectoryName(this.saveFileDialog.FileName);
                    int index = this.saveFileDialog.FilterIndex - 1;
                    mapping = this.imagePluginManager.AvailableSaverPlugins[index];
                    this.config.Extract.TargetImageFormat = mapping.Extensions[0];
                    Cursor.Current = Cursors.WaitCursor;
                    string fileName = this.saveFileDialog.FileName;
                    ImagePluginMapping plugin = this.imagePluginManager.FindSaverPlugin(Path.GetExtension(fileName));
                    if (plugin != null)
                    {
                        Cursor.Current = Cursors.WaitCursor;
                        try
                        {
                            IImageSaver saver = this.imagePluginManager.CreateSaverInstance(plugin);
                            if (!this.imagePluginManager.IsSameFormat(imageLoader, saver))
                            {
                                saver.SaveImage(image, fileName);
                            }
                            else
                            {
                                this.currentFile.ExtractFile(file, fileName);
                            }
                            Cursor.Current = Cursors.Default;
                        }
                        catch (Exception exception)
                        {
                            Cursor.Current = Cursors.Default;
                            this.ShowError(exception.Message);
                        }
                    }
                    else
                    {
                        this.ShowWarning("Unknown file type.");
                    }
                }
            }
        }

        private void SetNewStartupBehaviour(ExplorerConfig.StartupBehaviour behaviour)
        {
            this.config.General.StartupBehaviour = behaviour;
            this.mnuStartupBehaviourDoNothing.Checked = behaviour == ExplorerConfig.StartupBehaviour.DoNothing;
            this.mnuStartupBehaviourOpenFile.Checked = behaviour == ExplorerConfig.StartupBehaviour.OpenFile;
            this.mnuStartupBehaviourOpenGame.Checked = behaviour == ExplorerConfig.StartupBehaviour.OpenGame;
        }

        private void SetPreviewState(bool active)
        {
            this.browsePreviewSplitter.Panel2Collapsed = !active;
        }

        private void SetPreviewWidth(int width)
        {
            if ((width > 0) && (width < this.browsePreviewSplitter.Width))
            {
                this.browsePreviewSplitter.SplitterDistance = this.browsePreviewSplitter.Width - width;
            }
        }

        private void SetSortMenuItem(ToolStripMenuItem newCheckedItem)
        {
            foreach (SortCriterionMap map in this.fileListSortCriterionMaps)
            {
                if (map.MenuItem != null)
                {
                    map.MenuItem.Checked = false;
                }
            }
            newCheckedItem.Checked = true;
        }

        private void ShowError(string message)
        {
            MessageBox.Show(this, message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
        }

        private void ShowProgress(int done, int total)
        {
            if (base.InvokeRequired)
            {
                base.BeginInvoke(new ShowProgressCallback(this.ShowProgress), new object[] { done, total });
            }
            else if ((done == -1) && (total == -1))
            {
                this.toolStripProgressBar1.Visible = false;
                this.lblCurrentAction.Visible = false;
                this.lblArchiveType.Visible = true;
            }
            else
            {
                int num = 0;
                if (total > 0)
                {
                    num = (int) ((((double) done) / ((double) total)) * 100.0);
                }
                this.toolStripProgressBar1.Value = num;
                this.toolStripProgressBar1.ToolTipText = num + "%";
                if (!this.toolStripProgressBar1.Visible)
                {
                    this.toolStripProgressBar1.Visible = true;
                }
                if (!this.lblCurrentAction.Visible)
                {
                    this.lblArchiveType.Visible = false;
                    this.lblCurrentAction.Visible = true;
                }
            }
        }

        private void ShowWarning(string message)
        {
            MessageBox.Show(this, message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        private void ShowWarnings()
        {
            if (base.InvokeRequired)
            {
                base.Invoke(new NotifyCallback(this.ShowWarnings));
            }
            else if ((this.warningMessages != null) && (this.warningMessages.Count > 0))
            {
                frmMessages messages = new frmMessages();
                messages.SetMessageType("Warnings");
                messages.SetMessages(this.warningMessages);
                messages.ShowDialog();
                messages.Dispose();
            }
        }

        private void TogglePreview()
        {
            if (!this.IsPreviewActive())
            {
                this.SetPreviewState(true);
                if (this.lvwInfo.SelectedIndices.Count == 1)
                {
                    this.PreviewSelectedFiles();
                }
            }
            else
            {
                this.SetPreviewState(false);
                this.ClearPreview();
            }
            this.mnuTogglePreview.Checked = this.IsPreviewActive();
        }

        private void tvwDirectories_AfterSelect(object sender, TreeViewEventArgs e)
        {
            this.NavigateTree(e.Node);
        }

        private void txtPath_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.NavigateUserSpecifiedPath();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private Image ViewAsImage(IFileInfo file, string filePath, frmView dlg, ImagePluginMapping mapping)
        {
            Image image = null;
            if (mapping == null)
            {
                mapping = this.imagePluginManager.DetectImageFormat(file, filePath);
            }
            if (mapping == null)
            {
                return image;
            }
            IImageLoader imageLoader = this.imagePluginManager.CreateLoaderInstance(mapping);
            IPaletteConsumer consumer = imageLoader as IPaletteConsumer;
            IPaletteProvider currentFile = this.currentFile as IPaletteProvider;
            if ((consumer != null) && (currentFile != null))
            {
                consumer.Palette = currentFile.Palette;
            }
            Image image2 = imageLoader.LoadImage(filePath);
            dlg.ShowImage(file, image2, this.GetImageDetailText(image2, imageLoader.TypeName), imageLoader);
            imageLoader = null;
            return image2;
        }

        private void ViewAsImage(IFileInfo file, Image image, frmView dlg, string detailInfo, IImageLoader imageLoader)
        {
            dlg.ShowImage(file, image, detailInfo, imageLoader);
        }

        private void ViewAsSound(IFileInfo file, string filePath, frmView dlg)
        {
            PluginMapping plugin = this.soundPluginManager.FindPlayerPlugin(Path.GetExtension(filePath));
            if (plugin != null)
            {
                ISoundPlayer player = this.soundPluginManager.CreatePlayerInstance(plugin);
                player.LoadFromFile(filePath);
                this.viewSoundPlayer = player;
                string soundDetailInfo = GetSoundDetailInfo(player);
                dlg.ShowSound(file, player, soundDetailInfo, this.fileTypeImageList.Images["sound"]);
            }
        }

        private void ViewAsText(string filePath, frmView dlg)
        {
            string str3;
            dlg.AppendText("");
            bool flag = false;
            Encoding currentEncoding = null;
            using (FileStream stream = new FileStream(filePath, FileMode.Open))
            {
                using (StreamReader reader = new StreamReader(stream, Encoding.Default, true))
                {
                    int num;
                    char[] chArray = new char[0x4000];
                    while ((num = reader.ReadBlock(chArray, 0, chArray.Length)) > 0)
                    {
                        if (currentEncoding == null)
                        {
                            currentEncoding = reader.CurrentEncoding;
                        }
                        string text = new string(chArray, 0, num);
                        if (text.Contains("\0"))
                        {
                            flag = true;
                            dlg.Clear();
                            currentEncoding = null;
                            goto Label_0095;
                        }
                        dlg.AppendText(text);
                    }
                }
            }
        Label_0095:
            if (flag)
            {
                using (FileStream stream2 = new FileStream(filePath, FileMode.Open))
                {
                    BinaryReader reader2 = new BinaryReader(stream2);
                    try
                    {
                        byte[] buffer = new byte[0x4000];
                        while (reader2.Read(buffer, 0, buffer.Length) > 0)
                        {
                            if (currentEncoding == null)
                            {
                                currentEncoding = Encoding.Default;
                            }
                            string str2 = currentEncoding.GetString(buffer);
                            dlg.AppendText(str2.Replace("\0", " "));
                        }
                    }
                    finally
                    {
                        reader2.Close();
                    }
                }
            }
            if (flag)
            {
                str3 = "Binary file";
            }
            else
            {
                str3 = "Text file";
                if (currentEncoding != null)
                {
                    str3 = str3 + " / " + currentEncoding.EncodingName;
                }
            }
            dlg.SetTextDetails(str3);
            dlg.ResetCursorPosition();
        }

        private void ViewSelectedFiles()
        {
            this.ViewSelectedFiles(false);
        }

        private void ViewSelectedFiles(bool forceText)
        {
            if (this.currentFile == null)
            {
                this.ShowWarning("No archive open.");
            }
            else if (this.lvwInfo.SelectedIndices.Count == 0)
            {
                this.ShowWarning("No files selected.");
            }
            else
            {
                frmView view;
                IFileInfo tag = (IFileInfo) this.cachedListViewItems[this.lvwInfo.SelectedIndices[0]].Tag;
                Image image = null;
                if ((!forceText && this.IsSupportedImageFormat(tag, null)) && (this.GetCurrentPreviewImage() != null))
                {
                    view = new frmView();
                    this.ViewAsImage(tag, this.GetCurrentPreviewImage(), view, this.lblPreviewStatus.Text, this.previewImageLoader);
                }
                else
                {
                    string str;
                    if (!this.ExtractTemporaryFiles(new IFileInfo[] { tag }, out str))
                    {
                        return;
                    }
                    view = new frmView();
                    try
                    {
                        Cursor.Current = Cursors.WaitCursor;
                        string filePath = Path.Combine(str, tag.Name);
                        ImagePluginMapping mapping = this.imagePluginManager.DetectImageFormat(tag, filePath);
                        if (!forceText && (mapping != null))
                        {
                            image = this.ViewAsImage(tag, filePath, view, mapping);
                        }
                        else if (!forceText && this.IsSupportedSoundFormat(tag))
                        {
                            if (this.previewSoundPlayer != null)
                            {
                                this.previewSoundPlayer.Stop();
                            }
                            this.ViewAsSound(tag, filePath, view);
                        }
                        else
                        {
                            this.ViewAsText(filePath, view);
                        }
                    }
                    catch (Exception exception)
                    {
                        view.Dispose();
                        Cursor.Current = Cursors.Default;
                        this.ShowError(exception.Message);
                        return;
                    }
                }
                view.Text = string.Format("View: {0}", Path.GetFileName(tag.Name));
                view.SetWindowSettings(this.config.Viewer.WindowSettings);
                view.SaveView += new frmView.SaveViewEventHandler(this.dlg_SaveView);
                try
                {
                    view.ShowDialog();
                }
                catch (Exception exception2)
                {
                    this.ShowError(exception2.Message);
                }
                view.SaveView -= new frmView.SaveViewEventHandler(this.dlg_SaveView);
                this.config.Viewer.WindowSettings = WindowSettings.GetWindowSettings(view);
                if (this.viewSoundPlayer != null)
                {
                    this.viewSoundPlayer.Stop();
                    this.viewSoundPlayer.Dispose();
                    this.viewSoundPlayer = null;
                }
                view.Dispose();
                if (image != null)
                {
                    image.Dispose();
                    image = null;
                }
            }
        }

        public string StartupDocument
        {
            get
            {
                return this.startDocument;
            }
            set
            {
                this.startDocument = value;
            }
        }

        private delegate void AppendTextCallback(string text);

        private delegate DialogResult AskAbortRetryIgnoreCallback(string message, string caption);

        private enum DirectoryImages
        {
            RootNode,
            ClosedFolder,
            OpenFolder
        }

        private delegate void NotifyCallback();

        private delegate void OpenFileCallback(string fileName);

        private delegate void ShowProgressCallback(int done, int total);
    }
}

