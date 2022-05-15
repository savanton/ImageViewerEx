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
        private ImageViewer imageViewer;
        private	Rectangle boundingRect;
		private	Point dragPoint;
        private Bitmap bmp;
        private Bitmap bmpPreview;
        private MultiPageImage multiBmp;

        private int panelWidth => imageViewer.PanelWidth;
        private int panelHeight => imageViewer.PanelHeight;
        private int rotation;
        private bool multiFrame;

        public Rectangle BoundingBox => boundingRect;

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
                    if (multiFrame)
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

        public Size CurrentSize => new Size(boundingRect.Width, boundingRect.Height);

        public bool MultiPage { get; private set; }

        public int Pages { get; private set; } = 1;

        public int CurrentPage { get; private set; }

        public double Zoom { get; private set; } = 1.0;

        public int Rotation
        {
            get => rotation;
            set
            {
                // Making sure that the rotation is only 0, 90, 180 or 270 degrees!
                if (value == 90 || value == 180 || value == 270 || value == 0)
                {
                    rotation = value;
                }
            }
        }

        public Bitmap GetPage(int pageNumber)
        {
            var pages = multiBmp?.Image.GetFrameCount(FrameDimension.Page);
            if (pages > pageNumber && pageNumber >= 0)
            {
                multiBmp.Image.SelectActiveFrame(FrameDimension.Page, pageNumber);
                return new Bitmap(multiBmp.Image);
            }

            return null;
        }
        public int ImageWidth
        {
            get
            {
                if (multiFrame)
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
                if (multiFrame)
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
                if (multiFrame)
                    return Gif.CurrentFrame;

                return MultiPage ? multiBmp?.Page : bmp;
            }
            set
            {
                try
                {
                    if (value == null) return;

                    CurrentPage = 0;

                    // No memory leaks here!
                    bmp?.Dispose();
                    bmp = null;

                    multiBmp?.Dispose();
                    multiBmp = null;

                    Pages = 1;
                    MultiPage = false;
                    multiFrame = false;

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
                                multiFrame = gifFrames > 1;
                            }
                            catch
                            {
                                multiFrame = false;
                            }
                        }
                    }

                    if (multiFrame)
                    {
                        Gif = new GifImage(imageViewer, value, imageViewer.GifAnimation, imageViewer.GifFPS);
                    }
                    else if (MultiPage)
                    {
                        bmp = null;
                        multiBmp = new MultiPageImage(value);
                    }
                    else
                    {
                        bmp = value;
                        multiBmp = null;
                    }

                    // Initial rotation adjustments
                    if (rotation != 0)
                    {
                        if (rotation == 180)
                        {
                            Image.RotateFlip(RotateFlipType.Rotate180FlipNone);
                            boundingRect = new Rectangle(0, 0, (int)(this.ImageWidth * Zoom), (int)(this.ImageHeight * Zoom));
                        }
                        else
                        {
                            if (rotation == 90)
                            {
                                Image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                            }
                            else if (rotation == 270)
                            {
                                Image.RotateFlip(RotateFlipType.Rotate270FlipNone);
                            }

                            // Flip the X and Y values
                            boundingRect = new Rectangle(0, 0, (int)(ImageHeight * Zoom), (int)(ImageWidth * Zoom));
                        }
                    }
                    else
                    {
                        Image.RotateFlip(RotateFlipType.RotateNoneFlipNone);
                        boundingRect = new Rectangle(0, 0, (int)(ImageWidth * Zoom), (int)(ImageHeight * Zoom));
                    }

                    Zoom = 1.0;
                    bmpPreview = CreatePreviewImage();
                    FitToScreen();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("ImageViewer error: " + ex.ToString());
                }
            }
        }
        
        public Image PreviewImage => bmpPreview;

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

        public DrawObject(ImageViewer viewer, Bitmap bmp)
        {
            try
            {
                imageViewer = viewer;

                // Initial dragging to false and an Image.
                IsDragging = false;
                Image = bmp;
                Image.RotateFlip(RotateFlipType.RotateNoneFlipNone);

                boundingRect = new Rectangle(0, 0, (int)(ImageWidth * Zoom), (int)(ImageHeight * Zoom));
            }
            catch(Exception ex)
            {
                MessageBox.Show("ImageViewer error: " + ex.ToString());
            }
        }

        private ImageCodecInfo GetCodec(string type)
        {
            var info = ImageCodecInfo.GetImageEncoders();
            return info.FirstOrDefault(codecInfo => codecInfo.FormatDescription.Equals(type));
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

                    multiBmp.SetPage(p);
                    multiBmp.Rotate(this.rotation);

                    // No memory leaks here!
                    bmpPreview?.Dispose();
                    bmpPreview = null;

                    bmpPreview = CreatePreviewImage();
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

                    multiBmp.SetPage(CurrentPage);
                    multiBmp.Rotate(rotation);

                    // No memory leaks here!
                    bmpPreview?.Dispose();
                    bmpPreview = null;

                    bmpPreview = CreatePreviewImage();
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

                    multiBmp.SetPage(CurrentPage);
                    multiBmp.Rotate(rotation);

                    // No memory leaks here!
                    bmpPreview?.Dispose();
                    bmpPreview = null;

                    bmpPreview = CreatePreviewImage();
                    AvoidOutOfScreen();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ImageViewer error: " + ex.ToString());
            }
        }

        public DrawObject(ImageViewer viewer)
        {
            try
            {
                imageViewer = viewer;
                // Initial dragging to false and No image.
                IsDragging = false;
                bmp = null;
                multiBmp = null;
                Gif = null;
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

                int tempWidth = boundingRect.Width;
                int tempHeight = boundingRect.Height;

                boundingRect.Width = tempHeight;
                boundingRect.Height = tempWidth;

                rotation = (rotation + angle) % 360;

                if (multiFrame)
                {
                    Gif.Rotate(angle);
                }
                else if (MultiPage)
                {
                    multiBmp?.Rotate(angle);
                }
                else
                {
                    Image.RotateFlip(type);
                }

                AvoidOutOfScreen();

                // No memory leaks here!
                bmpPreview?.Dispose();
                bmpPreview = null;

                bmpPreview = CreatePreviewImage();
            }
            catch (Exception ex)
            {
                MessageBox.Show("ImageViewer error: " + ex.ToString());
            }
        }

        private Bitmap RotateCenter(Bitmap bmpSrc, float theta)
        {
            if(theta == 180.0f)
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

            if (multiFrame)
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
            var selectedX = (int)((double)(((double)boundingRect.X - ((double)boundingRect.X * 2)) + (double)x) / Zoom);
            var selectedY = (int)((double)(((double)boundingRect.Y - ((double)boundingRect.Y * 2)) + (double)y) / Zoom);
            var selectedWidth = width;
            var selectedHeight = height;

            // The selection width on the scale of the Original size!
            if (Zoom < 1.0 || Zoom > 1.0)
            {
                selectedWidth = Convert.ToInt32((double)width / Zoom);
                selectedHeight = Convert.ToInt32((double)height / Zoom);
            }

            // What is the highest possible zoomrate?
            var zoomX = ((double)panelWidth / (double)selectedWidth);
            var zoomY = ((double)panelHeight / (double)selectedHeight);

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
                if (selectedWidth < panelWidth)
                {
                    offsetX = (panelWidth - selectedWidth) / 2;
                }
                if (selectedHeight < panelHeight)
                {
                    offsetY = (panelHeight - selectedHeight) / 2;
                }

                boundingRect.X = (int)((int)((double)selectedX * newZoom) - ((int)((double)selectedX * newZoom) * 2)) + offsetX;
                boundingRect.Y = (int)((int)((double)selectedY * newZoom) - ((int)((double)selectedY * newZoom) * 2)) + offsetY;

                AvoidOutOfScreen();
            }
        }

        public void JumpToOrigin(int x, int y, int width, int height, int pWidth, int pHeight)
        {
            try
            {
                var zoom = (double)boundingRect.Width / (double)width;

                var originX = (int)(x * zoom);
                var originY = (int)(y * zoom);

                originX -= (originX * 2);
                originY -= (originY * 2);

                boundingRect.X = originX + (pWidth / 2);
                boundingRect.Y = originY + (pHeight / 2);

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
                boundingRect.X = (x - (width / 2)) - ((x - (width / 2)) * 2);
                boundingRect.Y = (y - (height / 2)) - ((y - (height / 2)) * 2);

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
                var zoomX = (double)width / (double)boundingRect.Width;
                var zoomY = (double)height / (double)boundingRect.Height;

                if (width > panelWidth)
                {
                    var oldX = (boundingRect.X - (boundingRect.X * 2)) + (panelWidth / 2);
                    var oldY = (boundingRect.Y - (boundingRect.Y * 2)) + (panelHeight / 2);

                    var newX = (int)(oldX * zoomX);
                    var newY = (int)(oldY * zoomY);

                    var originX = newX - (panelWidth / 2) - ((newX - (panelWidth / 2)) * 2);
                    var originY = newY - (panelHeight / 2) - ((newY - (panelHeight / 2)) * 2);

                    return new Point(originX, originY);
                }
                else
                {
                    if (height > panelHeight)
                    {
                        var oldY = (boundingRect.Y - (boundingRect.Y * 2)) + (panelHeight / 2);

                        var newY = (int)(oldY * zoomY);

                        var originY = newY - (panelHeight / 2) - ((newY - (panelHeight / 2)) * 2);

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
                    
                if(index != 0)
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
            boundingRect.X = -x;
            boundingRect.Y = -y;
        }

        public void SetPositionX(int x)
        {
            boundingRect.X = -x;
        }

        public void SetPositionY(int y)
        {
            boundingRect.Y = -y;
        }

        public void SetZoom(double z)
        {
            try
            {
                if (Image == null)
                    return;

                Zoom = z;

                var p = PointToOrigin(boundingRect.X, boundingRect.Y, (int)(ImageWidth * Zoom), (int)(ImageHeight * Zoom));

                boundingRect = new Rectangle(p.X, p.Y, (int)(ImageWidth * Zoom), (int)(ImageHeight * Zoom));
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

                var x_ratio = (double)panelWidth / (double)ImageWidth;
                var y_ratio = (double)panelHeight / (double)ImageHeight;

                if ((ImageWidth <= panelWidth) && (ImageHeight <= panelHeight))
                {
                    boundingRect.Width = ImageWidth;
                    boundingRect.Height = ImageHeight;
                }
                else if ((x_ratio * ImageHeight) < panelHeight)
                {
                    boundingRect.Height = Convert.ToInt32(Math.Ceiling(x_ratio * ImageHeight));
                    boundingRect.Width = panelWidth;
                }
                else
                {
                    boundingRect.Width = Convert.ToInt32(Math.Ceiling(y_ratio * ImageWidth));
                    boundingRect.Height = panelHeight;
                }

                boundingRect.X = 0;
                boundingRect.Y = 0;

                Zoom = ((double)boundingRect.Width / (double)ImageWidth);
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
                if (boundingRect.X >= 0)
                {
                    boundingRect.X = 0;
                }
                else if ((boundingRect.X <= (boundingRect.Width - panelWidth) - ((boundingRect.Width - panelWidth) * 2)))
                {
                    if ((boundingRect.Width - panelWidth) - ((boundingRect.Width - panelWidth) * 2) <= 0)
                    {
                        // I am too far to the left!
                        boundingRect.X = (boundingRect.Width - panelWidth) - ((boundingRect.Width - panelWidth) * 2);
                    }
                    else
                    {
                        // I am too far to the right!
                        boundingRect.X = 0;
                    }
                }

                // Am I lined out to the top?
                if (boundingRect.Y >= 0)
                {
                    boundingRect.Y = 0;
                }
                else if ((boundingRect.Y <= (boundingRect.Height - panelHeight) - ((boundingRect.Height - panelHeight) * 2)))
                {
                    if ((boundingRect.Height - panelHeight) - ((boundingRect.Height - panelHeight) * 2) <= 0)
                    {
                        // I am too far to the top!
                        boundingRect.Y = (boundingRect.Height - panelHeight) - ((boundingRect.Height - panelHeight) * 2);
                    }
                    else
                    {
                        // I am too far to the bottom!
                        boundingRect.Y = 0;
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
                if ((pt.X - dragPoint.X > (boundingRect.Width - panelWidth) - ((boundingRect.Width - panelWidth) * 2)) && (pt.X - dragPoint.X < 0))
                {
                    // No, everything is just fine
                    boundingRect.X = pt.X - dragPoint.X;
                }
                else if ((pt.X - dragPoint.X > 0))
                {
                    // Now don't drag it out of the panel please
                    boundingRect.X = 0;
                }
                else if((pt.X - dragPoint.X < (boundingRect.Width - panelWidth) - ((boundingRect.Width - panelWidth) * 2)))
                {
                    // I am dragging it out of my panel. How many pixels do I have left?
                    if ((boundingRect.Width - panelWidth) - ((boundingRect.Width - panelWidth) * 2) <= 0)
                    {
                        // Make it fit perfectly
                        boundingRect.X = (boundingRect.Width - panelWidth) - ((boundingRect.Width - panelWidth) * 2);
                    }
                }

                // Am I dragging it outside of the panel?
                if (pt.Y - dragPoint.Y > (boundingRect.Height - panelHeight) - ((boundingRect.Height - panelHeight) * 2) && (pt.Y - dragPoint.Y < 0))
                {
                    // No, everything is just fine
                    boundingRect.Y = pt.Y - dragPoint.Y;
                }
                else if ((pt.Y - dragPoint.Y > 0))
                {
                    // Now don't drag it out of the panel please
                    boundingRect.Y = 0;
                }
                else if (pt.Y - dragPoint.Y < (boundingRect.Height - panelHeight) - ((boundingRect.Height - panelHeight) * 2))
                {
                    // I am dragging it out of my panel. How many pixels do I have left?
                    if ((boundingRect.Height - panelHeight) - ((boundingRect.Height - panelHeight) * 2) <= 0)
                    {
                        // Make it fit perfectly
                        boundingRect.Y = (boundingRect.Height - panelHeight) - ((boundingRect.Height - panelHeight) * 2);
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
                dragPoint.X = pt.X - boundingRect.X;
                dragPoint.Y = pt.Y - boundingRect.Y;
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
                if (multiFrame)
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
                                    g.DrawImage(rotated, boundingRect);

                                    rotated.Dispose();
                                }
                                else
                                {
                                    g.DrawImage(Gif.CurrentFrame, boundingRect);
                                }
                            }
                        }
                        Gif.Unlock();
                    }
                }
                if (MultiPage)
                {
                    if (multiBmp?.Image != null)
                    {
                        g.DrawImage(multiBmp.Image, boundingRect);
                    }
                }
                else
                {
                    if (bmp != null)
                    {
                        g.DrawImage(bmp, boundingRect);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ImageViewer error: " + ex.ToString());
            }
		}
    }
}
