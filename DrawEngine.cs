using System.Drawing;
using System;

namespace Savan
{
    class DrawEngine
    {
        /// <summary>
        /// Original class to implement Double Buffering by
        /// NT Almond 
        /// 24 July 2003
        /// 
        /// Extended and adjusted by
        /// Jordy "Kaiwa" Ruiter
        /// </summary>
        /// 
        private Bitmap memoryBitmap;
		private	int	width;
		private	int	height;

        public void Dispose()
        {
            Graphics?.Dispose();
            memoryBitmap?.Dispose();
        }

        public DrawEngine()
		{
            try
            {
			    width = 0;
			    height = 0;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("ImageViewer error: " + ex.ToString());
            }
		}

		public bool CreateDoubleBuffer(Graphics g, int width, int height)
        {
            try
            {
                if (memoryBitmap != null)
                {
                    memoryBitmap.Dispose();
                    memoryBitmap = null;
                }

                if (this.Graphics != null)
                {
                    this.Graphics.Dispose();
                    this.Graphics = null;
                }

                if (width == 0 || height == 0)
                    return false;

                this.width = width;
                this.height = height;

                memoryBitmap = new Bitmap(width, height);
                this.Graphics = Graphics.FromImage(memoryBitmap);

                return true;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("ImageViewer error: " + ex.ToString());
                return false;
            }
		}

		public void Render(Graphics g)
		{
            try
            {
                if (memoryBitmap != null)
                {
                    g.DrawImage(memoryBitmap, new Rectangle(0, 0, width, height), 0, 0, width, height, GraphicsUnit.Pixel);
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("ImageViewer error: " + ex.ToString());
            }
		}

		public bool CanDoubleBuffer()
		{
            try
            {
                return Graphics != null;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("ImageViewer error: " + ex.ToString());
                return false;
            }
		}

        public Graphics Graphics { get; private set; }
    }
}
