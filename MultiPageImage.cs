using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace Savan
{
    public class MultiPageImage
    {
        #region Fields

        private int currentPage;

        private Bitmap image;

        private Bitmap bmp;

        #endregion Fields

        #region C'tors

        public MultiPageImage(Bitmap image)
        {
            Image = image;
            currentPage = 0;
        }

        #endregion C'tors

        #region Public Members

        public int Rotation { get; private set; }

        public Bitmap Image
        {
            get => bmp;
            set
            {
                if (image != null)
                {
                    image.Dispose();
                    image = null;
                }

                image = value;

                if (bmp != null)
                {
                    bmp.Dispose();
                    bmp = null;
                }

                bmp = new Bitmap(image);
            }
        }

        public Bitmap Page => bmp ?? (bmp = new Bitmap(image));

        public void Rotate(int rotation)
        {
            if (rotation != 90 && rotation != 180 && rotation != 270 && rotation != 0) return;

            Rotation = rotation;

            switch (this.Rotation)
            {
                case 90:
                    bmp.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    break;
                case 180:
                    bmp.RotateFlip(RotateFlipType.Rotate180FlipNone);
                    break;
                case 270:
                    bmp.RotateFlip(RotateFlipType.Rotate270FlipNone);
                    break;
            }
        }

        public void SetPage(int pageNumber)
        {
            if (image == null) return;
            if (currentPage == pageNumber) return;

            int pages = image.GetFrameCount(FrameDimension.Page);
            if (pages > pageNumber && pageNumber >= 0)
            {
                currentPage = pageNumber;

                image.SelectActiveFrame(FrameDimension.Page, pageNumber);

                if (bmp != null)
                {
                    bmp.Dispose();
                    bmp = null;
                }

                bmp = new Bitmap(image);
            }
        }

        public Bitmap GetBitmap(int pageNumber)
        {
            if (image == null)
            {
                return null;
            }

            if (currentPage != pageNumber)
            {
                int pages = image.GetFrameCount(FrameDimension.Page);
                if (pages > pageNumber && pageNumber >= 0)
                {
                    currentPage = pageNumber;

                    image.SelectActiveFrame(FrameDimension.Page, pageNumber);

                    if (bmp != null)
                    {
                        bmp.Dispose();
                        bmp = null;
                    }

                    bmp = new Bitmap(image);
                }
            }

            return bmp;
        }

        public void Dispose()
        {
            image?.Dispose();
            image = null;

            bmp?.Dispose();
            bmp = null;
        }

        #endregion Public Members
    }
}
