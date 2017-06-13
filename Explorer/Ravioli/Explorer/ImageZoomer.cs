namespace Ravioli.Explorer
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    internal class ImageZoomer
    {
        private PictureBox displayBox;
        private int imageZoomPercent;
        private int maxPercent;
        private int minPercent;
        private Image originalImage;
        private Label zoomInfoLabel;
        private static readonly int[] zoomLevels = new int[] { 10, 0x19, 50, 100, 150, 200, 300, 500, 800, 0x3e8 };

        public ImageZoomer(Image originalImage, PictureBox displayBox, Label zoomInfoLabel, int imageZoomPercent)
        {
            this.originalImage = originalImage;
            this.displayBox = displayBox;
            this.zoomInfoLabel = zoomInfoLabel;
            this.imageZoomPercent = imageZoomPercent;
            this.maxPercent = 0x3e8;
            this.minPercent = 10;
            if (imageZoomPercent < 0)
            {
                this.ZoomImageToFit();
            }
            else
            {
                this.ZoomImage(originalImage, imageZoomPercent);
            }
        }

        public void FitImage()
        {
            if (this.displayBox.Image != null)
            {
                if ((this.displayBox.Image.Width > this.displayBox.Width) || (this.displayBox.Image.Height > this.displayBox.Height))
                {
                    this.displayBox.SizeMode = PictureBoxSizeMode.Zoom;
                }
                else
                {
                    this.displayBox.SizeMode = PictureBoxSizeMode.CenterImage;
                }
                this.zoomInfoLabel.Visible = false;
                this.imageZoomPercent = -1;
            }
        }

        private Image GetCurrentDisplayImage()
        {
            return this.displayBox.Image;
        }

        private Image GetCurrentImage()
        {
            return this.originalImage;
        }

        private int GetLevelForPercent(int percent)
        {
            for (int i = 0; i < zoomLevels.Length; i++)
            {
                if (zoomLevels[i] == percent)
                {
                    return i;
                }
            }
            return -1;
        }

        private void ZoomImage(Image original, int percent)
        {
            int width = (int) (original.Width * (((double) percent) / 100.0));
            int height = (int) (original.Height * (((double) percent) / 100.0));
            Image image = PixelFormatConverter.ResizeImage(original, width, height);
            if (this.displayBox.Image != null)
            {
                this.displayBox.Image.Dispose();
                this.displayBox.Image = null;
            }
            this.displayBox.SizeMode = PictureBoxSizeMode.CenterImage;
            this.displayBox.Image = image;
            this.imageZoomPercent = percent;
            this.zoomInfoLabel.Text = this.imageZoomPercent + "%";
            this.zoomInfoLabel.BringToFront();
            this.zoomInfoLabel.Visible = true;
        }

        public void ZoomImageIn()
        {
            Image currentDisplayImage = this.GetCurrentDisplayImage();
            Image currentImage = this.GetCurrentImage();
            if ((currentDisplayImage != null) && (currentImage != null))
            {
                int imageZoomPercent = this.imageZoomPercent;
                if (this.imageZoomPercent < 0)
                {
                    int levelForPercent = this.GetLevelForPercent(100);
                    imageZoomPercent = zoomLevels[++levelForPercent];
                }
                else
                {
                    for (int i = 0; i < zoomLevels.Length; i++)
                    {
                        if ((this.imageZoomPercent == zoomLevels[i]) && (i < (zoomLevels.Length - 1)))
                        {
                            imageZoomPercent = zoomLevels[i + 1];
                            break;
                        }
                    }
                }
                if (imageZoomPercent <= this.maxPercent)
                {
                    this.ZoomImage(currentImage, imageZoomPercent);
                }
            }
        }

        public void ZoomImageOriginalSize()
        {
            Image currentImage = this.GetCurrentImage();
            if (currentImage != null)
            {
                this.ZoomImage(currentImage, 100);
            }
        }

        public void ZoomImageOut()
        {
            Image currentImage = this.GetCurrentImage();
            if (currentImage != null)
            {
                int imageZoomPercent = this.imageZoomPercent;
                if (this.imageZoomPercent < 0)
                {
                    int levelForPercent = this.GetLevelForPercent(100);
                    imageZoomPercent = zoomLevels[--levelForPercent];
                }
                else
                {
                    for (int i = 0; i < zoomLevels.Length; i++)
                    {
                        if ((this.imageZoomPercent == zoomLevels[i]) && (i > 0))
                        {
                            imageZoomPercent = zoomLevels[i - 1];
                            break;
                        }
                    }
                }
                if (imageZoomPercent >= this.minPercent)
                {
                    this.ZoomImage(currentImage, imageZoomPercent);
                }
            }
        }

        public void ZoomImageToFit()
        {
            this.ZoomImageOriginalSize();
            this.FitImage();
        }

        public int MaxPercent
        {
            get
            {
                return this.maxPercent;
            }
            set
            {
                this.maxPercent = value;
                Image currentImage = this.GetCurrentImage();
                if ((currentImage != null) && (this.maxPercent < this.imageZoomPercent))
                {
                    this.ZoomImage(currentImage, this.maxPercent);
                }
            }
        }

        public int MinPercent
        {
            get
            {
                return this.minPercent;
            }
            set
            {
                this.minPercent = value;
                Image currentImage = this.GetCurrentImage();
                if ((currentImage != null) && (this.minPercent > this.imageZoomPercent))
                {
                    this.ZoomImage(currentImage, this.minPercent);
                }
            }
        }

        public int Percent
        {
            get
            {
                return this.imageZoomPercent;
            }
        }
    }
}

