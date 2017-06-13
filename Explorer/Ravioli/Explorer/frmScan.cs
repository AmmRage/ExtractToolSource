namespace Ravioli.Explorer
{
    using Ravioli.AppShared;
    using Ravioli.Scanner;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Windows.Forms;

    public class frmScan : Form
    {
        private bool allowClose;
        private Button btnCancel;
        private IContainer components;
        private string fileName;
        private ManualResetEvent finishedEvent;
        private Label lblMessage;
        private Label lblProgress;
        private ProgressBar progressBar1;
        private ScanCompletedEventArgs scanCompletedEventArgs;
        private FileScanner scanner;
        private ScanPluginManager scanPluginManager;

        public frmScan(ScanPluginManager scanPluginManager)
        {
            this.InitializeComponent();
            this.scanPluginManager = scanPluginManager;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.allowClose = true;
            this.scanner.ScanCancelAsync();
        }

        private void CloseThreadSafe()
        {
            if (base.InvokeRequired)
            {
                base.Invoke(new ZeroParameterMethod(this.CloseThreadSafe));
            }
            else
            {
                base.DialogResult = DialogResult.OK;
                base.Close();
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

        private void frmScan_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = !this.allowClose;
        }

        private void InitializeComponent()
        {
            this.progressBar1 = new ProgressBar();
            this.lblMessage = new Label();
            this.lblProgress = new Label();
            this.btnCancel = new Button();
            base.SuspendLayout();
            this.progressBar1.Location = new Point(0x2a, 0x2c);
            this.progressBar1.MarqueeAnimationSpeed = 50;
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new Size(200, 0x17);
            this.progressBar1.Style = ProgressBarStyle.Continuous;
            this.progressBar1.TabIndex = 3;
            this.lblMessage.Font = new Font("Microsoft Sans Serif", 12f, FontStyle.Bold, GraphicsUnit.Point, 0);
            this.lblMessage.Location = new Point(0, 0);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new Size(0x11c, 0x29);
            this.lblMessage.TabIndex = 2;
            this.lblMessage.Text = "Scanning...";
            this.lblMessage.TextAlign = ContentAlignment.MiddleCenter;
            this.lblProgress.Location = new Point(12, 70);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new Size(260, 0x17);
            this.lblProgress.TabIndex = 4;
            this.lblProgress.Text = "0%";
            this.lblProgress.TextAlign = ContentAlignment.MiddleCenter;
            this.btnCancel.DialogResult = DialogResult.Cancel;
            this.btnCancel.Location = new Point(0x69, 0x69);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new Size(0x4b, 0x17);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new EventHandler(this.btnCancel_Click);
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.CancelButton = this.btnCancel;
            base.ClientSize = new Size(0x11c, 0x90);
            base.ControlBox = false;
            base.Controls.Add(this.btnCancel);
            base.Controls.Add(this.lblProgress);
            base.Controls.Add(this.progressBar1);
            base.Controls.Add(this.lblMessage);
            base.FormBorderStyle = FormBorderStyle.FixedSingle;
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            base.Name = "frmScan";
            base.ShowInTaskbar = false;
            base.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Please wait";
            base.FormClosing += new FormClosingEventHandler(this.frmScan_FormClosing);
            base.ResumeLayout(false);
        }

        public void Scan(string fileName, string resultsFileName, out bool cancelled)
        {
            this.finishedEvent = new ManualResetEvent(false);
            this.scanner = new FileScanner(this.scanPluginManager);
            this.fileName = fileName;
            this.scanner.ScanProgressChanged += (this.scanner_ScanProgressChanged);
            this.scanner.ScanCompleted += new ScanCompletedEventHandler(this.scanner_ScanCompleted);
            this.scanner.ScanAsync(this.fileName);
            this.allowClose = false;
            if (!this.finishedEvent.WaitOne(250))
            {
                base.ShowDialog();
                this.finishedEvent.WaitOne();
            }
            this.finishedEvent.Close();
            if (!this.scanCompletedEventArgs.Cancelled && (this.scanCompletedEventArgs.Error == null))
            {
                try
                {
                    FileOperations.SaveScanResults(this.scanCompletedEventArgs.Results, resultsFileName);
                }
                catch (Exception)
                {
                    if (File.Exists(resultsFileName))
                    {
                        File.Delete(resultsFileName);
                    }
                    throw;
                }
            }
            cancelled = this.scanCompletedEventArgs.Cancelled;
        }

        private void scanner_ScanCompleted(object sender, ScanCompletedEventArgs e)
        {
            this.allowClose = true;
            this.scanCompletedEventArgs = e;
            if (base.Modal && !e.Cancelled)
            {
                this.CloseThreadSafe();
            }
            this.finishedEvent.Set();
        }

        private void scanner_ScanProgressChanged(object sender, ScanProgressChangedEventArgs e)
        {
            this.UpdateProgressThreadSafe(e.ProgressPercentage, e.ItemsFound);
        }

        private void UpdateProgressThreadSafe(int percent, int itemsFound)
        {
            if (this.lblProgress.InvokeRequired)
            {
                this.lblProgress.Invoke(new TwoIntParameters(this.UpdateProgressThreadSafe), new object[] { percent, itemsFound });
            }
            else
            {
                this.progressBar1.Value = percent;
                this.lblProgress.Text = string.Format("{0}% completed, {1} {2}.", percent, itemsFound, (itemsFound == 1) ? "item found" : "items found");
            }
        }

        private delegate void TwoIntParameters(int percent, int itemsFound);

        private delegate void ZeroParameterMethod();
    }
}

