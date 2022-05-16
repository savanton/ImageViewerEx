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

        private int _currentPage;

        private Bitmap _image;

        private Bitmap _bmp;

        #endregion Fields

        #region C'tors

        public MultiPageImage(Bitmap image)
        {
            Image = image;
            _currentPage = 0;
        }

        #endregion C'tors

        #region Public Members

        public int Rotation { get; private set; }

        public Bitmap Image
        {
            get => _bmp;
            set
            {
                if (_image != null)
                {
                    _image.Dispose();
                    _image = null;
                }

                _image = value;

                if (_bmp != null)
                {
                    _bmp.Dispose();
                    _bmp = null;
                }

                _bmp = new Bitmap(_image);
            }
        }

        public Bitmap Page => _bmp ?? (_bmp = new Bitmap(_image));

        public void Rotate(int rotation)
        {
            if (rotation != 90 && rotation != 180 && rotation != 270 && rotation != 0) return;

            Rotation = rotation;

            switch (this.Rotation)
            {
                case 90:
                    _bmp.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    break;
                case 180:
                    _bmp.RotateFlip(RotateFlipType.Rotate180FlipNone);
                    break;
                case 270:
                    _bmp.RotateFlip(RotateFlipType.Rotate270FlipNone);
                    break;
            }
        }

        public void SetPage(int pageNumber)
        {
            if (_image == null) return;
            if (_currentPage == pageNumber) return;

            int pages = _image.GetFrameCount(FrameDimension.Page);
            if (pages > pageNumber && pageNumber >= 0)
            {
                _currentPage = pageNumber;

                _image.SelectActiveFrame(FrameDimension.Page, pageNumber);

                if (_bmp != null)
                {
                    _bmp.Dispose();
                    _bmp = null;
                }

                _bmp = new Bitmap(_image);
            }
        }

        public Bitmap GetBitmap(int pageNumber)
        {
            if (_image == null)
            {
                return null;
            }

            if (_currentPage != pageNumber)
            {
                int pages = _image.GetFrameCount(FrameDimension.Page);
                if (pages > pageNumber && pageNumber >= 0)
                {
                    _currentPage = pageNumber;

                    _image.SelectActiveFrame(FrameDimension.Page, pageNumber);

                    if (_bmp != null)
                    {
                        _bmp.Dispose();
                        _bmp = null;
                    }

                    _bmp = new Bitmap(_image);
                }
            }

            return _bmp;
        }

        public void Dispose()
        {
            _image?.Dispose();
            _image = null;

            _bmp?.Dispose();
            _bmp = null;
        }

        #endregion Public Members
    }
}
