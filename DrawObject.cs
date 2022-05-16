using System;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace Savan
{
    public class DrawObject
    {
        #region Fields

        private ImageViewerEx _imageViewer;
        private	Rectangle _boundingRect;
		private	Point _dragPoint;
        private Bitmap _bmp;
        private Bitmap _bmpPreview;
        private MultiPageImage _multiBmp;
        private bool _multiFrame;
        private int _rotation;

        #endregion Fields

        #region C'tors

        public DrawObject(ImageViewerEx viewer, Bitmap bmp)
        {
            try
            {
                _imageViewer = viewer;

                // Initial dragging to false and an Image.
                IsDragging = false;
                Image = bmp;
                Image.RotateFlip(RotateFlipType.RotateNoneFlipNone);

                _boundingRect = new Rectangle(0, 0, (int)(ImageWidth * Zoom), (int)(ImageHeight * Zoom));
            }
            catch (Exception ex)
            {
                MessageBox.Show("ImageViewer error: " + ex.ToString());
            }
        }

        public DrawObject(ImageViewerEx viewer)
        {
            try
            {
                _imageViewer = viewer;
                // Initial dragging to false and No image.
                IsDragging = false;
                _bmp = null;
                _multiBmp = null;
                Gif = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("ImageViewer error: " + ex.ToString());
            }
        }

        #endregion

        #region Public Members

        public Rectangle BoundingBox => _boundingRect;

        public void Dispose()
        {
            Image?.Dispose();
        }

        public bool IsDragging { get; private set; }

        public GifImage Gif { get; private set; }

        public Size OriginalSize
        {
            get
            {
                if (Image != null)
                {
                    if (_multiFrame)
                    {
                        if (Gif != null)
                        {
                            if (Gif.Rotation == 0 || Gif.Rotation == 180)
                            {
                                return Gif.CurrentFrameSize;
                            }

                            return new Size(Gif.CurrentFrameSize.Height, Gif.CurrentFrameSize.Width);
                        }

                        return Size.Empty;
                    }

                    return Image.Size;
                }

                return Size.Empty;
            }
        }

        public Size CurrentSize => new Size(_boundingRect.Width, _boundingRect.Height);

        public bool MultiPage { get; private set; }

        public int Pages { get; private set; } = 1;

        public int CurrentPage { get; private set; }

        public double Zoom { get; private set; } = 1.0;

        public int Rotation
        {
            get => _rotation;
            set
            {
                // Making sure that the rotation is only 0, 90, 180 or 270 degrees!
                if (value == 90 || value == 180 || value == 270 || value == 0)
                {
                    _rotation = value;
                }
            }
        }

        public Bitmap GetPage(int pageNumber)
        {
            var pages = _multiBmp?.Image.GetFrameCount(FrameDimension.Page);
            if (pages > pageNumber && pageNumber >= 0)
            {
                _multiBmp.Image.SelectActiveFrame(FrameDimension.Page, pageNumber);
                return new Bitmap(_multiBmp.Image);
            }

            return null;
        }

        public int ImageWidth
        {
            get
            {
                if (_multiFrame)
                {
                    if (Gif != null)
                    {
                        if (Gif.Rotation == 0 || Gif.Rotation == 180)
                        {
                            return Gif.CurrentFrameSize.Width;
                        }

                        return Gif.CurrentFrameSize.Height;
                    }

                    return 0;
                }

                return Image.Width;
            }
        }

        public int ImageHeight
        {
            get
            {
                if (_multiFrame)
                {
                    if (Gif != null)
                    {
                        if (Gif.Rotation == 0 || Gif.Rotation == 180)
                        {
                            return Gif.CurrentFrameSize.Height;
                        }

                        return Gif.CurrentFrameSize.Width;
                    }

                    return 0;
                }

                return Image.Height;
            }
        }

        public Bitmap Image
        {
            get
            {
                if (_multiFrame)
                    return Gif.CurrentFrame;

                return MultiPage ? _multiBmp?.Page : _bmp;
            }
            set
            {
                try
                {
                    if (value == null) return;

                    CurrentPage = 0;

                    // No memory leaks here!
                    _bmp?.Dispose();
                    _bmp = null;

                    _multiBmp?.Dispose();
                    _multiBmp = null;

                    Pages = 1;
                    MultiPage = false;
                    _multiFrame = false;

                    if (value.RawFormat.Equals(ImageFormat.Tiff))
                    {
                        try
                        {
                            //Gets the total number of frames in the .tiff file
                            Pages = value.GetFrameCount(FrameDimension.Page);
                            MultiPage = Pages > 1;
                        }
                        catch
                        {
                            MultiPage = false;
                            Pages = 1;
                        }
                    }
                    else if (value.RawFormat.Equals(ImageFormat.Gif))
                    {
                        if (!MultiPage)
                        {
                            try
                            {
                                var gifDimension = new FrameDimension(value.FrameDimensionsList[0]);
                                var gifFrames = value.GetFrameCount(gifDimension);
                                _multiFrame = gifFrames > 1;
                            }
                            catch
                            {
                                _multiFrame = false;
                            }
                        }
                    }

                    if (_multiFrame)
                    {
                        Gif = new GifImage(_imageViewer, value, _imageViewer.GifAnimation, _imageViewer.GifFPS);
                    }
                    else if (MultiPage)
                    {
                        _bmp = null;
                        _multiBmp = new MultiPageImage(value);
                    }
                    else
                    {
                        _bmp = value;
                        _multiBmp = null;
                    }

                    // Initial rotation adjustments
                    if (_rotation != 0)
                    {
                        if (_rotation == 180)
                        {
                            Image.RotateFlip(RotateFlipType.Rotate180FlipNone);
                            _boundingRect = new Rectangle(0, 0, (int)(this.ImageWidth * Zoom), (int)(this.ImageHeight * Zoom));
                        }
                        else
                        {
                            if (_rotation == 90)
                            {
                                Image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                            }
                            else if (_rotation == 270)
                            {
                                Image.RotateFlip(RotateFlipType.Rotate270FlipNone);
                            }

                            // Flip the X and Y values
                            _boundingRect = new Rectangle(0, 0, (int)(ImageHeight * Zoom), (int)(ImageWidth * Zoom));
                        }
                    }
                    else
                    {
                        Image.RotateFlip(RotateFlipType.RotateNoneFlipNone);
                        _boundingRect = new Rectangle(0, 0, (int)(ImageWidth * Zoom), (int)(ImageHeight * Zoom));
                    }

                    Zoom = 1.0;
                    _bmpPreview = CreatePreviewImage();
                    FitToScreen();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("ImageViewer error: " + ex.ToString());
                }
            }
        }

        public Image PreviewImage => _bmpPreview;

        public string ImagePath
        {
            set
            {
                Bitmap temp;

                // Make sure it does not crash on incorrect image formats
                try
                {
                    temp = new Bitmap(value);
                }
                catch
                {
                    MessageBox.Show("ImageViewer error: Incorrect image format!");
                    return;
                }

                Image = temp;
            }
        }

        public void SetPage(int page)
        {
            var p = page - 1;

            try
            {
                if (Image == null || MultiPage != true)
                    return;

                if (p < Pages && p >= 0)
                {
                    CurrentPage = p;

                    _multiBmp.SetPage(p);
                    _multiBmp.Rotate(this._rotation);

                    // No memory leaks here!
                    _bmpPreview?.Dispose();
                    _bmpPreview = null;

                    _bmpPreview = CreatePreviewImage();
                    AvoidOutOfScreen();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ImageViewer error: " + ex.ToString());
            }
        }

        public void NextPage()
        {
            try
            {
                if (Image == null || MultiPage != true)
                    return;

                var nextPage = CurrentPage + 1;

                if (nextPage < Pages)
                {
                    CurrentPage = nextPage;

                    _multiBmp.SetPage(CurrentPage);
                    _multiBmp.Rotate(_rotation);

                    // No memory leaks here!
                    _bmpPreview?.Dispose();
                    _bmpPreview = null;

                    _bmpPreview = CreatePreviewImage();
                    AvoidOutOfScreen();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ImageViewer error: " + ex.ToString());
            }
        }

        public void PreviousPage()
        {
            try
            {
                if (Image == null || MultiPage != true)
                    return;

                var prevPage = CurrentPage - 1;

                if (prevPage >= 0)
                {
                    CurrentPage = prevPage;

                    _multiBmp.SetPage(CurrentPage);
                    _multiBmp.Rotate(_rotation);

                    // No memory leaks here!
                    _bmpPreview?.Dispose();
                    _bmpPreview = null;

                    _bmpPreview = CreatePreviewImage();
                    AvoidOutOfScreen();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ImageViewer error: " + ex.ToString());
            }
        }

        public void Rotate(RotateFlipType type)
        {
            try
            {
                if (this.Image == null)
                    return;

                var angle = 0;

                switch (type)
                {
                    case RotateFlipType.Rotate90FlipNone:
                        angle = 90;
                        break;
                    case RotateFlipType.Rotate180FlipNone:
                        angle = 180;
                        break;
                    case RotateFlipType.Rotate270FlipNone:
                        angle = 270;
                        break;
                }

                int tempWidth = _boundingRect.Width;
                int tempHeight = _boundingRect.Height;

                _boundingRect.Width = tempHeight;
                _boundingRect.Height = tempWidth;

                _rotation = (_rotation + angle) % 360;

                if (_multiFrame)
                {
                    Gif.Rotate(angle);
                }
                else if (MultiPage)
                {
                    _multiBmp?.Rotate(angle);
                }
                else
                {
                    Image.RotateFlip(type);
                }

                AvoidOutOfScreen();

                // No memory leaks here!
                _bmpPreview?.Dispose();
                _bmpPreview = null;

                _bmpPreview = CreatePreviewImage();
            }
            catch (Exception ex)
            {
                MessageBox.Show("ImageViewer error: " + ex.ToString());
            }
        }

        private Bitmap RotateCenter(Bitmap bmpSrc, float theta)
        {
            if (theta == 180.0f)
            {
                var bmpDest = new Bitmap(bmpSrc.Width, bmpSrc.Height);
                var gDest = Graphics.FromImage(bmpDest);

                gDest.DrawImage(bmpSrc, new Point(0, 0));

                bmpDest.RotateFlip(RotateFlipType.Rotate180FlipNone);

                return bmpDest;
            }
            else
            {
                var mRotate = new Matrix();
                mRotate.Translate(bmpSrc.Width / -2, bmpSrc.Height / -2, MatrixOrder.Append);
                mRotate.RotateAt(theta, new Point(0, 0), MatrixOrder.Append);

                var gp = new GraphicsPath();
                // transform image points by rotation matrix
                gp.AddPolygon(new Point[] { new Point(0, 0), new Point(bmpSrc.Width, 0), new Point(0, bmpSrc.Height) });
                gp.Transform(mRotate);
                var pts = gp.PathPoints;

                // create destination bitmap sized to contain rotated source image
                var bbox = RotateBoundingBox(bmpSrc, mRotate);
                var bmpDest = new Bitmap(bbox.Width, bbox.Height);

                var gDest = Graphics.FromImage(bmpDest);
                var mDest = new Matrix();
                mDest.Translate(bmpDest.Width / 2, bmpDest.Height / 2, MatrixOrder.Append);
                gDest.Transform = mDest;
                gDest.DrawImage(bmpSrc, pts);
                gDest.DrawRectangle(Pens.Red, bbox);
                gDest.Dispose();
                gp.Dispose();

                return bmpDest;
            }
        }

        private static Rectangle RotateBoundingBox(Image img, System.Drawing.Drawing2D.Matrix matrix)
        {
            var gu = new GraphicsUnit();
            var rImg = Rectangle.Round(img.GetBounds(ref gu));

            // Transform the four points of the image, to get the resized bounding box.
            var topLeft = new Point(rImg.Left, rImg.Top);
            var topRight = new Point(rImg.Right, rImg.Top);
            var bottomRight = new Point(rImg.Right, rImg.Bottom);
            var bottomLeft = new Point(rImg.Left, rImg.Bottom);
            var points = new Point[] { topLeft, topRight, bottomRight, bottomLeft };
            var gp = new GraphicsPath(points, new byte[] { (byte)PathPointType.Start, (byte)PathPointType.Line, (byte)PathPointType.Line, (byte)PathPointType.Line });
            gp.Transform(matrix);
            return Rectangle.Round(gp.GetBounds());
        }

        private Bitmap CreatePreviewImage()
        {
            // 148 && 117 as initial and default size for the preview panel.
            var previewRect = new Rectangle(0, 0, 148, 117);

            var x_ratio = (double)previewRect.Width / (double)BoundingBox.Width;
            var y_ratio = (double)previewRect.Height / (double)BoundingBox.Height;

            if ((BoundingBox.Width <= previewRect.Width) && (BoundingBox.Height <= previewRect.Height))
            {
                previewRect.Width = BoundingBox.Width;
                previewRect.Height = BoundingBox.Height;
            }
            else if ((x_ratio * BoundingBox.Height) < previewRect.Height)
            {
                previewRect.Height = Convert.ToInt32(Math.Ceiling(x_ratio * BoundingBox.Height));
                previewRect.Width = previewRect.Width;
            }
            else
            {
                previewRect.Width = Convert.ToInt32(Math.Ceiling(y_ratio * BoundingBox.Width));
                previewRect.Height = previewRect.Height;
            }

            var previewBmp = new Bitmap(previewRect.Width, previewRect.Height);

            if (_multiFrame)
            {
                if (Gif != null)
                {
                    using (var g = Graphics.FromImage(previewBmp))
                    {
                        if (Gif.Lock())
                        {
                            lock (Gif.CurrentFrame)
                            {
                                g.DrawImage(Gif.Rotation != 0 ? RotateCenter(Gif.CurrentFrame, Gif.Rotation) : Gif.CurrentFrame, previewRect);
                            }
                        }

                        Gif.Unlock();
                    }
                }
            }
            else
            {
                using (var g = Graphics.FromImage(previewBmp))
                {
                    if (Image != null)
                    {
                        g.DrawImage(Image, previewRect);
                    }
                }
            }

            return previewBmp;
        }

        public void ZoomToSelection(Rectangle selection, Point ptPbFull)
        {
            var x = (selection.X - ptPbFull.X);
            var y = (selection.Y - ptPbFull.Y);
            var width = selection.Width;
            var height = selection.Height;

            // So, where did my selection start on the entire picture?
            var selectedX = (int)((double)(((double)_boundingRect.X - ((double)_boundingRect.X * 2)) + (double)x) / Zoom);
            var selectedY = (int)((double)(((double)_boundingRect.Y - ((double)_boundingRect.Y * 2)) + (double)y) / Zoom);
            var selectedWidth = width;
            var selectedHeight = height;

            // The selection width on the scale of the Original size!
            if (Zoom < 1.0 || Zoom > 1.0)
            {
                selectedWidth = Convert.ToInt32((double)width / Zoom);
                selectedHeight = Convert.ToInt32((double)height / Zoom);
            }

            // What is the highest possible zoomrate?
            var zoomX = ((double)PanelWidth / (double)selectedWidth);
            var zoomY = ((double)PanelHeight / (double)selectedHeight);

            var newZoom = Math.Min(zoomX, zoomY);

            // Avoid Int32 crashes!
            if (newZoom * 100 < Int32.MaxValue && newZoom * 100 > Int32.MinValue)
            {
                SetZoom(newZoom);

                selectedWidth = (int)((double)selectedWidth * newZoom);
                selectedHeight = (int)((double)selectedHeight * newZoom);

                // Center the selected area
                var offsetX = 0;
                var offsetY = 0;
                if (selectedWidth < PanelWidth)
                {
                    offsetX = (PanelWidth - selectedWidth) / 2;
                }
                if (selectedHeight < PanelHeight)
                {
                    offsetY = (PanelHeight - selectedHeight) / 2;
                }

                _boundingRect.X = (int)((int)((double)selectedX * newZoom) - ((int)((double)selectedX * newZoom) * 2)) + offsetX;
                _boundingRect.Y = (int)((int)((double)selectedY * newZoom) - ((int)((double)selectedY * newZoom) * 2)) + offsetY;

                AvoidOutOfScreen();
            }
        }

        public void JumpToOrigin(int x, int y, int width, int height, int pWidth, int pHeight)
        {
            try
            {
                var zoom = (double)_boundingRect.Width / (double)width;

                var originX = (int)(x * zoom);
                var originY = (int)(y * zoom);

                originX -= (originX * 2);
                originY -= (originY * 2);

                _boundingRect.X = originX + (pWidth / 2);
                _boundingRect.Y = originY + (pHeight / 2);

                AvoidOutOfScreen();
            }
            catch (Exception ex)
            {
                MessageBox.Show("ImageViewer error: " + ex.ToString());
            }
        }

        public void JumpToOrigin(int x, int y, int width, int height)
        {
            try
            {
                _boundingRect.X = (x - (width / 2)) - ((x - (width / 2)) * 2);
                _boundingRect.Y = (y - (height / 2)) - ((y - (height / 2)) * 2);

                AvoidOutOfScreen();
            }
            catch (Exception ex)
            {
                MessageBox.Show("ImageViewer error: " + ex.ToString());
            }
        }

        public Point PointToOrigin(int x, int y, int width, int height)
        {
            try
            {
                var zoomX = (double)width / (double)_boundingRect.Width;
                var zoomY = (double)height / (double)_boundingRect.Height;

                if (width > PanelWidth)
                {
                    var oldX = (_boundingRect.X - (_boundingRect.X * 2)) + (PanelWidth / 2);
                    var oldY = (_boundingRect.Y - (_boundingRect.Y * 2)) + (PanelHeight / 2);

                    var newX = (int)(oldX * zoomX);
                    var newY = (int)(oldY * zoomY);

                    var originX = newX - (PanelWidth / 2) - ((newX - (PanelWidth / 2)) * 2);
                    var originY = newY - (PanelHeight / 2) - ((newY - (PanelHeight / 2)) * 2);

                    return new Point(originX, originY);
                }
                else
                {
                    if (height > PanelHeight)
                    {
                        var oldY = (_boundingRect.Y - (_boundingRect.Y * 2)) + (PanelHeight / 2);

                        var newY = (int)(oldY * zoomY);

                        var originY = newY - (PanelHeight / 2) - ((newY - (PanelHeight / 2)) * 2);

                        return new Point(0, originY);
                    }

                    return new Point(0, 0);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ImageViewer error: " + ex.ToString());
                return new Point(0, 0);
            }
        }

        public void ZoomIn()
        {
            try
            {
                if (Image == null)
                    return;

                // Make sure zoom steps are with 25%
                var index = 0.25 - (Zoom % 0.25);

                if (index != 0)
                {
                    Zoom += index;
                }
                else
                {
                    Zoom += 0.25;
                }

                SetZoom(Zoom);
            }
            catch (Exception ex)
            {
                MessageBox.Show("ImageViewer error: " + ex.ToString());
            }
        }

        public void ZoomOut()
        {
            try
            {
                if (Image == null)
                    return;

                // Make sure zoom steps are with 25% and higher than 0%
                if (Zoom - 0.25 > 0)
                {
                    if (((Zoom - 0.25) % 0.25) != 0)
                    {
                        Zoom -= Zoom % 0.25;
                    }
                    else
                    {
                        Zoom -= 0.25;
                    }
                }

                SetZoom(Zoom);
            }
            catch (Exception ex)
            {
                MessageBox.Show("ImageViewer error: " + ex.ToString());
            }
        }

        public void SetPosition(int x, int y)
        {
            _boundingRect.X = -x;
            _boundingRect.Y = -y;
        }

        public void SetPositionX(int x)
        {
            _boundingRect.X = -x;
        }

        public void SetPositionY(int y)
        {
            _boundingRect.Y = -y;
        }

        public void SetZoom(double z)
        {
            try
            {
                if (Image == null)
                    return;

                Zoom = z;

                var p = PointToOrigin(_boundingRect.X, _boundingRect.Y, (int)(ImageWidth * Zoom), (int)(ImageHeight * Zoom));

                _boundingRect = new Rectangle(p.X, p.Y, (int)(ImageWidth * Zoom), (int)(ImageHeight * Zoom));
                AvoidOutOfScreen();
            }
            catch (Exception ex)
            {
                MessageBox.Show("ImageViewer error: " + ex.ToString());
            }
        }

        public void Scroll(object sender, MouseEventArgs e)
        {
            try
            {
                if (Image == null)
                    return;

                if (e.Delta < 0)
                {
                    ZoomOut();
                }
                else
                {
                    ZoomIn();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ImageViewer error: " + ex.ToString());
            }
        }

        public void FitToScreen()
        {
            try
            {
                if (Image == null)
                    return;

                var x_ratio = (double)PanelWidth / (double)ImageWidth;
                var y_ratio = (double)PanelHeight / (double)ImageHeight;

                if ((ImageWidth <= PanelWidth) && (ImageHeight <= PanelHeight))
                {
                    _boundingRect.Width = ImageWidth;
                    _boundingRect.Height = ImageHeight;
                }
                else if ((x_ratio * ImageHeight) < PanelHeight)
                {
                    _boundingRect.Height = Convert.ToInt32(Math.Ceiling(x_ratio * ImageHeight));
                    _boundingRect.Width = PanelWidth;
                }
                else
                {
                    _boundingRect.Width = Convert.ToInt32(Math.Ceiling(y_ratio * ImageWidth));
                    _boundingRect.Height = PanelHeight;
                }

                _boundingRect.X = 0;
                _boundingRect.Y = 0;

                Zoom = ((double)_boundingRect.Width / (double)ImageWidth);
            }
            catch (Exception ex)
            {
                MessageBox.Show("ImageViewer error: " + ex.ToString());
            }
        }

        public void AvoidOutOfScreen()
        {
            try
            {
                // Am I lined out to the left?
                if (_boundingRect.X >= 0)
                {
                    _boundingRect.X = 0;
                }
                else if ((_boundingRect.X <= (_boundingRect.Width - PanelWidth) - ((_boundingRect.Width - PanelWidth) * 2)))
                {
                    if ((_boundingRect.Width - PanelWidth) - ((_boundingRect.Width - PanelWidth) * 2) <= 0)
                    {
                        // I am too far to the left!
                        _boundingRect.X = (_boundingRect.Width - PanelWidth) - ((_boundingRect.Width - PanelWidth) * 2);
                    }
                    else
                    {
                        // I am too far to the right!
                        _boundingRect.X = 0;
                    }
                }

                // Am I lined out to the top?
                if (_boundingRect.Y >= 0)
                {
                    _boundingRect.Y = 0;
                }
                else if ((_boundingRect.Y <= (_boundingRect.Height - PanelHeight) - ((_boundingRect.Height - PanelHeight) * 2)))
                {
                    if ((_boundingRect.Height - PanelHeight) - ((_boundingRect.Height - PanelHeight) * 2) <= 0)
                    {
                        // I am too far to the top!
                        _boundingRect.Y = (_boundingRect.Height - PanelHeight) - ((_boundingRect.Height - PanelHeight) * 2);
                    }
                    else
                    {
                        // I am too far to the bottom!
                        _boundingRect.Y = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ImageViewer error: " + ex.ToString());
            }
        }

        public void Drag(Point pt)
        {
            try
            {
                if (this.Image == null || IsDragging != true)
                    return;

                // Am I dragging it outside of the panel?
                if ((pt.X - _dragPoint.X > (_boundingRect.Width - PanelWidth) - ((_boundingRect.Width - PanelWidth) * 2)) && (pt.X - _dragPoint.X < 0))
                {
                    // No, everything is just fine
                    _boundingRect.X = pt.X - _dragPoint.X;
                }
                else if ((pt.X - _dragPoint.X > 0))
                {
                    // Now don't drag it out of the panel please
                    _boundingRect.X = 0;
                }
                else if ((pt.X - _dragPoint.X < (_boundingRect.Width - PanelWidth) - ((_boundingRect.Width - PanelWidth) * 2)))
                {
                    // I am dragging it out of my panel. How many pixels do I have left?
                    if ((_boundingRect.Width - PanelWidth) - ((_boundingRect.Width - PanelWidth) * 2) <= 0)
                    {
                        // Make it fit perfectly
                        _boundingRect.X = (_boundingRect.Width - PanelWidth) - ((_boundingRect.Width - PanelWidth) * 2);
                    }
                }

                // Am I dragging it outside of the panel?
                if (pt.Y - _dragPoint.Y > (_boundingRect.Height - PanelHeight) - ((_boundingRect.Height - PanelHeight) * 2) && (pt.Y - _dragPoint.Y < 0))
                {
                    // No, everything is just fine
                    _boundingRect.Y = pt.Y - _dragPoint.Y;
                }
                else if ((pt.Y - _dragPoint.Y > 0))
                {
                    // Now don't drag it out of the panel please
                    _boundingRect.Y = 0;
                }
                else if (pt.Y - _dragPoint.Y < (_boundingRect.Height - PanelHeight) - ((_boundingRect.Height - PanelHeight) * 2))
                {
                    // I am dragging it out of my panel. How many pixels do I have left?
                    if ((_boundingRect.Height - PanelHeight) - ((_boundingRect.Height - PanelHeight) * 2) <= 0)
                    {
                        // Make it fit perfectly
                        _boundingRect.Y = (_boundingRect.Height - PanelHeight) - ((_boundingRect.Height - PanelHeight) * 2);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ImageViewer error: " + ex.ToString());
            }
        }

        public void BeginDrag(Point pt)
        {
            try
            {
                if (Image == null)
                    return;

                // Initial drag position
                _dragPoint.X = pt.X - _boundingRect.X;
                _dragPoint.Y = pt.Y - _boundingRect.Y;
                IsDragging = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("ImageViewer error: " + ex.ToString());
            }
        }

        public void EndDrag()
        {
            try
            {
                if (Image != null)
                {
                    IsDragging = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ImageViewer error: " + ex.ToString());
            }
        }

        public void Draw(Graphics g)
        {
            try
            {
                if (_multiFrame)
                {
                    if (Gif.CurrentFrame != null)
                    {
                        if (Gif.Lock())
                        {
                            lock (Gif.CurrentFrame)
                            {
                                if (Gif.Rotation != 0)
                                {
                                    var rotated = RotateCenter(Gif.CurrentFrame, Gif.Rotation);
                                    g.DrawImage(rotated, _boundingRect);

                                    rotated.Dispose();
                                }
                                else
                                {
                                    g.DrawImage(Gif.CurrentFrame, _boundingRect);
                                }
                            }
                        }
                        Gif.Unlock();
                    }
                }
                if (MultiPage)
                {
                    if (_multiBmp?.Image != null)
                    {
                        g.DrawImage(_multiBmp.Image, _boundingRect);
                    }
                }
                else
                {
                    if (_bmp != null)
                    {
                        g.DrawImage(_bmp, _boundingRect);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ImageViewer error: " + ex.ToString());
            }
        }

        #endregion Public Members

        #region Non Public Members

        private int PanelWidth => _imageViewer.PanelWidth;

        private int PanelHeight => _imageViewer.PanelHeight;

        private ImageCodecInfo GetCodec(string type)
        {
            var info = ImageCodecInfo.GetImageEncoders();
            return info.FirstOrDefault(codecInfo => codecInfo.FormatDescription.Equals(type));
        }

        #endregion Non Public Members

    }
}
