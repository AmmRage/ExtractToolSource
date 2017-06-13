namespace Ravioli.AppShared.Forms
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    [Serializable]
    public class WindowSettings
    {
        private int height = 0;
        private int left = 0;
        private FormWindowState state = FormWindowState.Normal;
        private int top = 0;
        private int width = 0;

        public static WindowSettings GetWindowSettings(Form form)
        {
            WindowSettings settings = new WindowSettings();
            Rectangle rectangle = (form.WindowState == FormWindowState.Normal) ? form.Bounds : form.RestoreBounds;
            settings.Left = rectangle.Left;
            settings.Top = rectangle.Top;
            settings.Width = rectangle.Width;
            settings.Height = rectangle.Height;
            settings.State = form.WindowState;
            return settings;
        }

        public static void SetWindowSettings(Form form, WindowSettings settings)
        {
            if ((settings.Width > 0) && (settings.Height > 0))
            {
                form.SetBounds(settings.Left, settings.Top, settings.Width, settings.Height);
                form.WindowState = settings.State;
            }
        }

        public int Height
        {
            get
            {
                return this.height;
            }
            set
            {
                this.height = value;
            }
        }

        public int Left
        {
            get
            {
                return this.left;
            }
            set
            {
                this.left = value;
            }
        }

        public FormWindowState State
        {
            get
            {
                return this.state;
            }
            set
            {
                this.state = value;
            }
        }

        public int Top
        {
            get
            {
                return this.top;
            }
            set
            {
                this.top = value;
            }
        }

        public int Width
        {
            get
            {
                return this.width;
            }
            set
            {
                this.width = value;
            }
        }
    }
}

