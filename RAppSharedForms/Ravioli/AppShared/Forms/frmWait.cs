namespace Ravioli.AppShared.Forms
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Windows.Forms;

    public class frmWait : Form
    {
        private ParameterizedThreadStart callback;
        private object callbackData;
        private IContainer components;
        private System.Exception exception;
        private ManualResetEvent finishedEvent;
        private Label lblMessage;
        private string message;
        private ProgressBar progressBar1;
        private Thread thread;
        private string title;

        private event EventHandler finished;

        public frmWait()
        {
            this.InitializeComponent();
            this.title = "Please wait";
            this.message = "Processing...";
            this.thread = null;
            this.callbackData = null;
            this.exception = null;
            this.finished += new EventHandler(this.Finished);
        }

        private void CloseThreadSafe()
        {
            if (base.InvokeRequired)
            {
                base.Invoke(new ZeroParameterMethod(this.CloseThreadSafe));
            }
            else if (base.IsHandleCreated)
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

        public void ExecuteOnNewThread()
        {
            this.exception = null;
            this.finishedEvent = new ManualResetEvent(false);
            this.thread = new Thread(new ParameterizedThreadStart(this.MakeCallback));
            this.thread.Name = "WaitFormThread";
            this.thread.Start(this.callbackData);
            if (!this.finishedEvent.WaitOne(250))
            {
                base.ShowDialog();
                this.finishedEvent.WaitOne();
            }
            this.finishedEvent.Close();
            this.thread = null;
        }

        private void Finished(object sender, EventArgs e)
        {
            if (base.Modal)
            {
                this.CloseThreadSafe();
            }
            this.finishedEvent.Set();
        }

        private void frmWait_Load(object sender, EventArgs e)
        {
            this.Text = this.title;
            this.lblMessage.Text = this.message;
        }

        private void frmWait_Shown(object sender, EventArgs e)
        {
            if (this.finishedEvent.WaitOne(0))
            {
                this.CloseThreadSafe();
            }
        }

        private void InitializeComponent()
        {
            this.lblMessage = new Label();
            this.progressBar1 = new ProgressBar();
            base.SuspendLayout();
            this.lblMessage.Font = new Font("Microsoft Sans Serif", 12f, FontStyle.Bold, GraphicsUnit.Point, 0);
            this.lblMessage.Location = new Point(0, 0);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new Size(0x11c, 0x29);
            this.lblMessage.TabIndex = 0;
            this.lblMessage.Text = "<<message>>";
            this.lblMessage.TextAlign = ContentAlignment.MiddleCenter;
            this.progressBar1.Location = new Point(0x2a, 0x2c);
            this.progressBar1.MarqueeAnimationSpeed = 50;
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new Size(200, 0x17);
            this.progressBar1.Style = ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 1;
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.ClientSize = new Size(0x11c, 0x4f);
            base.ControlBox = false;
            base.Controls.Add(this.progressBar1);
            base.Controls.Add(this.lblMessage);
            base.FormBorderStyle = FormBorderStyle.FixedSingle;
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            base.Name = "frmWait";
            base.ShowInTaskbar = false;
            base.StartPosition = FormStartPosition.CenterParent;
            this.Text = "<<title>>";
            base.Load += new EventHandler(this.frmWait_Load);
            base.Shown += new EventHandler(this.frmWait_Shown);
            base.ResumeLayout(false);
        }

        private void MakeCallback(object data)
        {
            try
            {
                this.callback(data);
            }
            catch (System.Exception exception)
            {
                this.exception = exception;
            }
            this.finished(this, EventArgs.Empty);
        }

        public ParameterizedThreadStart Callback
        {
            get
            {
                return this.callback;
            }
            set
            {
                this.callback = value;
            }
        }

        public object CallbackData
        {
            get
            {
                return this.callbackData;
            }
            set
            {
                this.callbackData = value;
            }
        }

        public System.Exception Exception
        {
            get
            {
                return this.exception;
            }
        }

        public string Message
        {
            get
            {
                return this.message;
            }
            set
            {
                this.message = value;
            }
        }

        private delegate void ZeroParameterMethod();
    }
}

