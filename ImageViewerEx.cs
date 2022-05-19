using System;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Savan
{
    public partial class ImageViewerEx : UserControl
    {
        #region Types/Enums/Delegates

        public enum ZoomType
        {
            In,
            Out
        }

        public delegate void ImageViewerRotationEventHandler(object sender, ImageViewerRotationEventArgs e);
        public delegate void ImageViewerZoomEventHandler(object sender, ImageViewerZoomEventArgs e);

        #endregion Types/Enums/Delegates

        #region Interops

        [DllImport("user32.dll")]
        private static extern short GetKeyState(int key);

        #endregion Interops

        #region Events

        public event ImageViewerRotationEventHandler AfterRotation;
        public event ImageViewerZoomEventHandler AfterZoom;

        #endregion Events

        #region Fields

        private DrawObject _drawing;

        private bool _isScrolling;
        private bool _scrollbars;
        private double _fps = 15.0;
        private bool _animationEnabled;
        private bool _selectMode;
        private bool _shiftSelecting;
        private Point _ptSelectionStart;
        private Point _ptSelectionEnd;

        private bool _panelDragging;
        private bool _showPreview = true;
        private Cursor _grabCursor;
        private Cursor _dragCursor;
        private Bitmap _preview;

        private static readonly Pen SelectionPen = new Pen(Color.Blue, 1);
        private static readonly Brush SelectionBrush = new SolidBrush(Color.FromArgb(60, SelectionPen.Color.R, SelectionPen.Color.G, SelectionPen.Color.B));

        #endregion Fields

        #region C'tors

        public ImageViewerEx()
        {
            // DrawObject initialization
            _drawing = new DrawObject(this);

            try
            {
                var a = Assembly.GetExecutingAssembly();

                // Stream to initialize the cursors.
                using (var imgStream = a.GetManifestResourceStream("Savan.Resources.Grab.cur"))
                {
                    if (imgStream != null)
                    {
                        _grabCursor = new Cursor(imgStream);
                    }
                }

                using (var imgStream = a.GetManifestResourceStream("Savan.Resources.Drag.cur"))
                {
                    if (imgStream != null)
                    {
                        _dragCursor = new Cursor(imgStream);
                    }
                }
            }
            catch
            {
                // Cursors could not be found
            }

            InitializeComponent();

            InitControl();

            Preview();
        }


        #endregion

        #region Public Members

        public int PanelWidth => pbFull.Width;

        public int PanelHeight => pbFull.Height;

        public void InvalidatePanel()
        {
            pbFull.Invalidate();
        }

        public bool Scrollbars
        {
            get => _scrollbars;
            set
            {
                _scrollbars = value;
                DisplayScrollbars();
                SetScrollbarValues();
            }
        }

        public double GifFPS
        {
            get => _fps;
            set
            {
                if (value <= 30.0 && value > 0.0)
                {
                    _fps = value;
                    if (_drawing.Gif != null)
                    {
                        _drawing.Gif.FPS = _fps;
                    }
                }
            }
        }

        public bool GifAnimation
        {
            get => _animationEnabled;
            set
            {
                _animationEnabled = value;
                if (_drawing.Gif != null)
                {
                    _drawing.Gif.AnimationEnabled = _animationEnabled;
                }
            }
        }

        public bool OpenButton
        {
            get => tsbOpen.Visible;
            set => tsbOpen.Visible = value;
        }

        public bool PreviewButton
        {
            get => tsbPreview.Visible;
            set => tsbPreview.Visible = value;
        }

        public override bool AllowDrop
        {
            get => base.AllowDrop;
            set
            {
                pbFull.AllowDrop = value;
                base.AllowDrop = value;
            }
        }

        public double Zoom
        {
            get => Math.Round(_drawing.Zoom * 100, 0);
            set
            {
                if (value > 0)
                {
                    // Make it a double!
                    var zoomDouble = (double)value / (double)100;

                    _drawing.SetZoom(zoomDouble);
                    UpdatePanels(true);
                }
            }
        }

        public Size OriginalSize => _drawing.OriginalSize;

        public Size CurrentSize => _drawing.CurrentSize;

        public Color HeaderColor
        {
            get => tsTop.BackColor;
            set
            {
                tsTop.BackColor = value;
                panelPreview.BackColor = value;
            }
        }

        public Color PreviewPanelColor
        {
            get => panelPreview.BackColor;
            set => panelPreview.BackColor = value;
        }

        public Color TextColor
        {
            get => lblPreview.ForeColor;
            set => lblPreview.ForeColor = value;
        }

        public Color PreviewTextColor
        {
            get => lblPreview.ForeColor;
            set => lblPreview.ForeColor = value;
        }

        public Color BackgroundColor
        {
            get => pbFull.BackColor;
            set => pbFull.BackColor = value;
        }

        public string PreviewText
        {
            get => lblPreview.Text;
            set => lblPreview.Text = value;
        }

        public string ImagePath
        {
            set
            {
                _drawing.ImagePath = value;

                UpdatePanels(true);
                ToggleMultiPage();

                // scrollbars
                DisplayScrollbars();
                SetScrollbarValues();
            }
        }

        public Bitmap Image
        {
            get => _drawing.Image;
            set
            {
                _drawing.Image = value;

                UpdatePanels(true);
                ToggleMultiPage();

                // scrollbars
                DisplayScrollbars();
                SetScrollbarValues();
            }
        }

        public int Rotation
        {
            get => _drawing.Rotation;
            set
            {
                // Making sure the rotation is 0, 90, 180 or 270 degrees!
                if (value == 90 || value == 180 || value == 270 || value == 0)
                {
                    _drawing.Rotation = value;
                }
            }
        }

        public bool ShowPreview
        {
            get => _showPreview;
            set
            {
                if (_showPreview != value)
                {
                    _showPreview = value;
                    Preview();
                }
            }
        }

        public void InitControl()
        {
            if (!_scrollbars)
            {
                sbHoriz.Visible = false;
                sbVert.Visible = false;
                sbPanel.Visible = false;
            }
        }

        public void Rotate(RotateFlipType rotateType)
        {
            if (_drawing == null)
                return;

            _drawing.Rotate(rotateType);

            // AfterRotation Event
            OnRotation(new ImageViewerRotationEventArgs(_drawing.Rotation));
            UpdatePanels(true);
            ToggleMultiPage();
        }

        public void FitToScreen()
        {
            _drawing.FitToScreen();
            UpdatePanels(true);
        }

        #endregion Public Members

        #region Non Public Members

        protected virtual void OnRotation(ImageViewerRotationEventArgs e)
        {
            AfterRotation?.Invoke(this, e);
        }

        protected virtual void OnZoom(ImageViewerZoomEventArgs e)
        {
            AfterZoom?.Invoke(this, e);
        }

        private bool IsKeyPressed(int key)
        {
            bool keyPressed;
            var result = GetKeyState(key);

            switch (result)
            {
                case 0:
                    // Not pressed and not toggled
                    keyPressed = false;
                    break;

                case 1:
                    // Not presses but toggled
                    keyPressed = false;
                    break;

                default:
                    // Pressed
                    keyPressed = true;
                    break;
            }

            return keyPressed;
        }

        private void Preview()
        {
            // Hide preview panel mechanics
            // Making sure that UpdatePanels doesn't get called when it's hidden!

            if (_showPreview != pbPanel.Visible)
            {
                panelPreview.Visible = _showPreview;
                pbPanel.Visible = _showPreview;

                InitControl();
                _drawing.AvoidOutOfScreen();

                if (_showPreview)
                    UpdatePanels(true);
            }
        }

        private void DisposeControl()
        {
            // No memory leaks here
            _drawing?.Dispose();
            _preview?.Dispose();
        }

        private void FocusOnMe()
        {
            // Do not lose focus! ("Fix" for the Scrolling issue)
            this.Focus();
        }

        private void DisplayScrollbars()
        {
            if (_scrollbars)
            {
                if (this.Image != null)
                {
                    int perPercent = this.CurrentSize.Width / 100;

                    if (this.CurrentSize.Width - perPercent > this.pbFull.Width)
                    {
                        this.sbHoriz.Visible = true;
                    }
                    else
                    {
                        this.sbHoriz.Visible = false;
                    }

                    if (this.CurrentSize.Height - perPercent > this.pbFull.Height)
                    {
                        this.sbVert.Visible = true;
                    }
                    else
                    {
                        this.sbVert.Visible = false;
                    }

                    if (this.sbVert.Visible == true && this.sbHoriz.Visible == true)
                    {
                        this.sbPanel.Visible = true;
                        this.sbVert.Height = this.pbFull.Height - 18;
                        this.sbHoriz.Width = this.pbFull.Width - 18;
                    }
                    else
                    {
                        this.sbPanel.Visible = false;

                        if (this.sbVert.Visible)
                        {
                            this.sbVert.Height = this.pbFull.Height;
                        }
                        else
                        {
                            this.sbHoriz.Width = this.pbFull.Width;
                        }
                    }
                }
                else
                {
                    this.sbHoriz.Visible = false;
                    this.sbVert.Visible = false;
                    this.sbPanel.Visible = false;
                }
            }
            else
            {
                this.sbHoriz.Visible = false;
                this.sbVert.Visible = false;
                this.sbPanel.Visible = false;
            }
        }

        private void SetScrollbarValues()
        {
            if (_scrollbars)
            {
                if (sbHoriz.Visible)
                {
                    _isScrolling = true;
                    double perPercent = (double)this.CurrentSize.Width / 101.0;
                    double totalPercent = (double)this.pbFull.Width / perPercent;

                    sbHoriz.Minimum = 0;
                    sbHoriz.Maximum = 100;
                    sbHoriz.LargeChange = Convert.ToInt32(Math.Round(totalPercent, 0));

                    double value = (double)((-this._drawing.BoundingBox.X) / perPercent);

                    if (value > sbHoriz.Maximum) { sbHoriz.Value = (sbHoriz.Maximum - sbHoriz.LargeChange) + ((sbHoriz.LargeChange > 0) ? 1 : 0); }
                    else if (value < 0) { sbHoriz.Value = 0; }
                    else
                    {
                        sbHoriz.Value = Convert.ToInt32(Math.Round(value, 0));
                    }
                    _isScrolling = false;
                }

                if (sbVert.Visible)
                {
                    _isScrolling = true;
                    double perPercent = (double)this.CurrentSize.Height / 101.0;
                    double totalPercent = (double)this.pbFull.Height / perPercent;

                    sbVert.Minimum = 0;
                    sbVert.Maximum = 100;
                    sbVert.LargeChange = Convert.ToInt32(Math.Round(totalPercent, 0));

                    double value = (double)((-this._drawing.BoundingBox.Y) / perPercent);

                    if (value > sbVert.Maximum) { sbVert.Value = (sbVert.Maximum - sbVert.LargeChange) + ((sbVert.LargeChange > 0) ? 1 : 0); }
                    else if (value < 0) { sbVert.Value = 0; }
                    else
                    {
                        sbVert.Value = Convert.ToInt32(Math.Round(value, 0));
                    }
                    _isScrolling = false;
                }
            }
            else
            {
                sbHoriz.Visible = false;
                sbVert.Visible = false;
            }
        }

        private void ImageViewerEx_Load(object sender, EventArgs e)
        {
            // Loop for ComboBox Items! Increments by 25%
            for (double z = 0.25; z <= 4.0; z = z + 0.25)
            {
                tscbZoom.Items.Add(z * 100 + "%");
            }

            tscbZoom.SelectedIndex = 3;
        }

        private void ToggleMultiPage()
        {
            if (_drawing.MultiPage)
            {
                ShowNavigationControls(true);
                tslblNavigation.Text = "/ " + _drawing.Pages.ToString();
                tstbNavigation.Text = (_drawing.CurrentPage + 1).ToString();
            }
            else
            {
                ShowNavigationControls(false);
                tslblNavigation.Text = "/ 0";
                tstbNavigation.Text = "0";
            }
        }

        private void ShowNavigationControls(bool show)
        {
            tstbNavigation.Visible = show;
            tslblNavigation.Visible = show;
            tsbBack.Visible = show;
            tsbNext.Visible = show;
        }

        private void pbFull_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.FillRectangle(new SolidBrush(pbFull.BackColor), e.ClipRectangle.X, e.ClipRectangle.Y, e.ClipRectangle.Width, e.ClipRectangle.Height);
            _drawing.Draw(e.Graphics);

            // Draw selection rectangle
            if (_shiftSelecting)
            {
                var rect = new Rectangle()
                {
                    X = Math.Min(_ptSelectionStart.X, _ptSelectionEnd.X),
                    Y = Math.Min(_ptSelectionStart.Y, _ptSelectionEnd.Y),
                    Width = Math.Abs(_ptSelectionStart.X - _ptSelectionEnd.X),
                    Height = Math.Abs(_ptSelectionStart.Y - _ptSelectionEnd.Y)

                };

                e.Graphics.FillRectangle(SelectionBrush, rect);
            }
        }

        private void pbFull_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // Left Shift or Right Shift pressed? Or is select mode one?
                if (this.IsKeyPressed(0xA0) || this.IsKeyPressed(0xA1) || _selectMode == true)
                {
                    // Fancy cursor
                    pbFull.Cursor = Cursors.Cross;

                    _shiftSelecting = true;

                    // Initial selection
                    _ptSelectionStart.X = e.X;
                    _ptSelectionStart.Y = e.Y;

                    // No selection end
                    _ptSelectionEnd.X = -1;
                    _ptSelectionEnd.Y = -1;
                }
                else
                {
                    // Start dragging
                    _drawing.BeginDrag(new Point(e.X, e.Y));

                    // Fancy cursor
                    if (_grabCursor != null)
                    {
                        pbFull.Cursor = _grabCursor;
                    }
                }
            }
        }

        private void pbFull_MouseUp(object sender, MouseEventArgs e)
        {
            // Am i dragging or selecting?
            if (_shiftSelecting == true)
            {
                // Calculate my selection rectangle
                Rectangle rect = CalculateSelectionRectangle(_ptSelectionStart, _ptSelectionEnd);

                // Clear the selection rectangle
                _ptSelectionEnd.X = -1;
                _ptSelectionEnd.Y = -1;
                _ptSelectionStart.X = -1;
                _ptSelectionStart.Y = -1;

                // Stop selecting
                _shiftSelecting = false;

                // Position of the panel to the screen
                Point ptPbFull = PointToScreen(pbFull.Location);

                // Zoom to my selection
                _drawing.ZoomToSelection(rect, ptPbFull);

                // Refresh my screen & update my preview panel
                pbFull.Invalidate();
                UpdatePanels(true);
            }
            else
            {
                // Stop dragging and update my panels
                _drawing.EndDrag();
                UpdatePanels(true);

                // Fancy cursor
                if (_dragCursor != null)
                {
                    pbFull.Cursor = _dragCursor;
                }
            }
        }

        private void pbFull_MouseMove(object sender, MouseEventArgs e)
        {
            // Am I dragging or selecting?
            if (_shiftSelecting == true)
            {
                // Keep selecting
                _ptSelectionEnd.X = e.X;
                _ptSelectionEnd.Y = e.Y;

                pbFull.Invalidate();
            }
            else
            {
                // Keep dragging
                _drawing.Drag(new Point(e.X, e.Y));
                if (_drawing.IsDragging)
                {
                    UpdatePanels(false);
                }
                else
                {
                    // I'm not dragging OR selecting
                    // Make sure if left or right shift is pressed to change cursor

                    if (this.IsKeyPressed(0xA0) || this.IsKeyPressed(0xA1) || _selectMode == true)
                    {
                        // Fancy Cursor
                        if (pbFull.Cursor != Cursors.Cross)
                        {
                            pbFull.Cursor = Cursors.Cross;
                        }
                    }
                    else
                    {
                        // Fancy Cursor
                        if (pbFull.Cursor != _dragCursor)
                        {
                            pbFull.Cursor = _dragCursor;
                        }
                    }
                }
            }
        }

        private void ImageViewerEx_MouseWheel(object sender, MouseEventArgs e)
        {
            _drawing.Scroll(sender, e);

            if (_drawing.Image != null)
            {
                if (e.Delta < 0)
                {
                    OnZoom(new ImageViewerZoomEventArgs(_drawing.Zoom, ZoomType.Out));
                }
                else
                {
                    OnZoom(new ImageViewerZoomEventArgs(_drawing.Zoom, ZoomType.In));
                }
            }

            UpdatePanels(true);
        }

        public void Open(byte[] imageContent)
        {
            this.Image = new Bitmap(new MemoryStream(imageContent), true);
        }

        private void UpdatePanels(bool updatePreview)
        {
            if (_drawing.CurrentSize.Width > 0 && _drawing.OriginalSize.Width > 0)
            {
                // scrollbars
                DisplayScrollbars();
                SetScrollbarValues();

                // Make sure panel is up to date
                pbFull.Invalidate();

                // Calculate zoom
                double zoom = Math.Round(((double)_drawing.CurrentSize.Width / (double)_drawing.OriginalSize.Width), 2);

                // Display zoom in percentages
                tscbZoom.Text = (int)(zoom * 100) + "%";

                if (updatePreview && _drawing.PreviewImage != null && pbPanel.Visible == true)
                {
                    // No memory leaks here
                    if (_preview != null)
                    {
                        _preview.Dispose();
                        _preview = null;
                    }

                    // New preview
                    _preview = new Bitmap(_drawing.PreviewImage.Size.Width, _drawing.PreviewImage.Size.Height);

                    // Make sure panel is the same size as the bitmap
                    if (pbPanel.Size != _drawing.PreviewImage.Size)
                    {
                        pbPanel.Size = _drawing.PreviewImage.Size;
                    }

                    // New Graphics from the new bitmap we created (Empty)
                    using (Graphics g = Graphics.FromImage(_preview))
                    {
                        // Draw the image on the bitmap
                        g.DrawImage(_drawing.PreviewImage, 0, 0, _drawing.PreviewImage.Size.Width, _drawing.PreviewImage.Size.Height);

                        double ratioX = (double)_drawing.PreviewImage.Size.Width / (double)_drawing.CurrentSize.Width;
                        double ratioY = (double)_drawing.PreviewImage.Size.Height / (double)_drawing.CurrentSize.Height;

                        double boxWidth = pbFull.Width * ratioX;
                        double boxHeight = pbFull.Height * ratioY;
                        double positionX = ((_drawing.BoundingBox.X - (_drawing.BoundingBox.X * 2)) * ratioX);
                        double positionY = ((_drawing.BoundingBox.Y - (_drawing.BoundingBox.Y * 2)) * ratioY);

                        if (boxHeight >= _drawing.PreviewImage.Size.Height)
                        {
                            boxHeight = _drawing.PreviewImage.Size.Height - 1;
                        }
                        else if ((boxHeight + positionY) > _drawing.PreviewImage.Size.Height)
                        {
                            boxHeight = _drawing.PreviewImage.Size.Height - (positionY);
                        }

                        if (boxWidth >= _drawing.PreviewImage.Size.Width)
                        {
                            boxWidth = _drawing.PreviewImage.Size.Width - 1;
                        }
                        else if ((boxWidth + positionX) > _drawing.PreviewImage.Size.Width)
                        {
                            boxWidth = _drawing.PreviewImage.Size.Width - (positionX);
                        }

                        // Draw the rectangle on the bitmap
                        g.DrawRectangle(SelectionPen, new Rectangle((int)positionX, (int)positionY, (int)boxWidth, (int)boxHeight));

                        // Display the bitmap
                        pbPanel.Image = _preview;
                    }
                }
            }
        }

        private void pbPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (_panelDragging == false)
            {
                _drawing.JumpToOrigin(e.X, e.Y, pbPanel.Width, pbPanel.Height, pbFull.Width, pbFull.Height);
                UpdatePanels(true);

                _panelDragging = true;
            }
        }

        private void pbFull_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            _drawing.JumpToOrigin(e.X + (_drawing.BoundingBox.X - (_drawing.BoundingBox.X * 2)), e.Y + (_drawing.BoundingBox.Y - (_drawing.BoundingBox.Y * 2)), pbFull.Width, pbFull.Height);
            UpdatePanels(true);
        }

        private void pbFull_MouseHover(object sender, EventArgs e)
        {
            // Left shift or Right shift!
            if (this.IsKeyPressed(0xA0) || this.IsKeyPressed(0xA1))
            {
                // Fancy cursor
                pbFull.Cursor = Cursors.Cross;
            }
            else
            {
                // Fancy cursor if not dragging
                if (!_drawing.IsDragging)
                {
                    pbFull.Cursor = _dragCursor;
                }
            }
        }

        private void ImageViewerEx_Click(object sender, EventArgs e)
        {
            FocusOnMe();
        }

        private void pbFull_Click(object sender, EventArgs e)
        {
            FocusOnMe();
        }

        private void pbPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (_panelDragging)
            {
                _drawing.JumpToOrigin(e.X, e.Y, pbPanel.Width, pbPanel.Height, pbFull.Width, pbFull.Height);
                UpdatePanels(true);
            }
        }

        private void pbPanel_MouseUp(object sender, MouseEventArgs e)
        {
            _panelDragging = false;
        }

        private void pbFull_MouseEnter(object sender, EventArgs e)
        {
            if (this.IsKeyPressed(0xA0) || this.IsKeyPressed(0xA1) || _selectMode == true)
            {
                pbFull.Cursor = Cursors.Cross;
            }
            else
            {
                if (_dragCursor != null)
                {
                    pbFull.Cursor = _dragCursor;
                }
            }
        }

        private void pbFull_MouseLeave(object sender, EventArgs e)
        {
            pbFull.Cursor = Cursors.Default;
        }

        private Rectangle CalculateSelectionRectangle(Point ptSelectStart, Point ptSelectEnd)
        {
            ptSelectStart = pbFull.PointToScreen(ptSelectStart);
            ptSelectEnd = pbFull.PointToScreen(ptSelectEnd);

            var rect = new Rectangle()
            {
                X = Math.Min(ptSelectStart.X, ptSelectEnd.X),
                Y = Math.Min(ptSelectStart.Y, ptSelectEnd.Y),
                Width = Math.Abs(ptSelectStart.X - ptSelectEnd.X),
                Height = Math.Abs(ptSelectStart.Y - ptSelectEnd.Y)

            };

            return rect;
        }

        private void pbFull_DragDrop(object sender, DragEventArgs e)
        {
            // Get The file(s) you dragged into an array. (We'll just pick the first image anyway)
            var fileList = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            foreach (var file in fileList)
            {
                // Make sure the file exists!
                if (System.IO.File.Exists(file))
                {
                    string ext = (System.IO.Path.GetExtension(file)).ToLower();

                    // Checking the extensions to be Image formats
                    if (ext == ".jpg" || ext == ".jpeg" || ext == ".gif" || ext == ".wmf" || ext == ".emf" || ext == ".bmp" || ext == ".png" || ext == ".tif" || ext == ".tiff")
                    {
                        try
                        {
                            // Try to load it into a bitmap
                            //newBmp = Bitmap.FromFile(FileList[f]);
                            this.ImagePath = file;

                            // If succeeded stop the loop
                            if (this.Image != null)
                            {
                                break;
                            }
                        }
                        catch
                        {
                            // Not an image?
                        }
                    }
                }
            }
        }

        private void pbFull_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
        }

        private void sbVert_Scroll(object sender, ScrollEventArgs e)
        {
            if (!_isScrolling)
            {
                double perPercent = (double)this.CurrentSize.Height / 101.0;

                double value = e.NewValue * perPercent;

                this._drawing.SetPositionY(Convert.ToInt32(Math.Round(value, 0)));

                this._drawing.AvoidOutOfScreen();

                pbFull.Invalidate();

                UpdatePanels(true);
            }
        }

        private void sbHoriz_Scroll(object sender, ScrollEventArgs e)
        {
            if (!_isScrolling)
            {
                double perPercent = (double)this.CurrentSize.Width / 101.0;

                double value = e.NewValue * perPercent;

                this._drawing.SetPositionX(Convert.ToInt32(Math.Round(value, 0)));

                this._drawing.AvoidOutOfScreen();

                pbFull.Invalidate();

                UpdatePanels(true);
            }
        }

        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            double zoom = (tscbZoom.SelectedIndex + 1) * 0.25;
            double originalZoom = _drawing.Zoom;

            if (_drawing.Zoom != zoom)
            {
                _drawing.SetZoom(zoom);

                if (_drawing.Image != null)
                {
                    if (zoom > originalZoom)
                    {
                        OnZoom(new ImageViewerZoomEventArgs(_drawing.Zoom, ZoomType.In));
                    }
                    else
                    {
                        OnZoom(new ImageViewerZoomEventArgs(_drawing.Zoom, ZoomType.Out));
                    }
                }

                UpdatePanels(true);
            }
        }

        private void tsbZoomIn_Click(object sender, EventArgs e)
        {
            _drawing.ZoomIn();

            // AfterZoom Event
            if (_drawing.Image != null)
            {
                OnZoom(new ImageViewerZoomEventArgs(_drawing.Zoom, ZoomType.In));
            }
            UpdatePanels(true);
        }

        private void tsbZoomOut_Click(object sender, EventArgs e)
        {
            _drawing.ZoomOut();

            // AfterZoom Event
            if (_drawing.Image != null)
            {
                OnZoom(new ImageViewerZoomEventArgs(_drawing.Zoom, ZoomType.Out));
            }
            UpdatePanels(true);

        }

        private void tsbFitToScreen_Click(object sender, EventArgs e)
        {
            _drawing.FitToScreen();
            UpdatePanels(true);
        }

        private void tsbRotate270_Click(object sender, EventArgs e)
        {
            Rotate(RotateFlipType.Rotate270FlipNone);
        }

        private void tsbRotate90_Click(object sender, EventArgs e)
        {
            Rotate(RotateFlipType.Rotate90FlipNone);
        }

        private void tsbMode_Click(object sender, EventArgs e)
        {
            if (_selectMode == false)
            {
                _selectMode = true;
                tsbMode.Image = Properties.Resources.btnDrag;
            }
            else
            {
                _selectMode = false;
                tsbMode.Image = Properties.Resources.btnSelect;
            }
        }

        private void tsbOpen_Click(object sender, EventArgs e)
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter =
                    "Image Files|*.jpg;*.jpeg;*.gif;*.bmp;*.png;*.tif;*.tiff;*.wmf;*.emf|JPEG Files (*.jpg)|*.jpg;*.jpeg|GIF Files (*.gif)|*.gif|BMP Files (*.bmp)|*.bmp|PNG Files (*.png)|*.png|TIF files (*.tif;*.tiff)|*.tif;*.tiff|EMF/WMF Files (*.wmf;*.emf)|*.wmf;*.emf|All files (*.*)|*.*";

                if (openFileDialog.ShowDialog(this) == DialogResult.OK)
                {
                    this.ImagePath = openFileDialog.FileName;
                }

                UpdatePanels(true);
            }
        }

        private void tsbPreview_Click(object sender, EventArgs e)
        {
            ShowPreview = !ShowPreview;
        }

        private void tscbZoom_KeyPress(object sender, KeyPressEventArgs e)
        {
            // If it's not a digit, delete or backspace then make sure the input is being handled with. (Suppressed)
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Delete && e.KeyChar != (char)Keys.Back)
            {
                // If enter is pressed apply the entered zoom
                if (e.KeyChar == (char)Keys.Return)
                {
                    int zoom = 0;

                    // Make sure the percent sign is out of the tscbZoom.Text
                    int.TryParse(tscbZoom.Text.Replace("%", ""), out zoom);

                    // If zoom is higher than zero
                    if (zoom > 0)
                    {
                        // Make it a double!
                        double zoomDouble = (double)zoom / (double)100;

                        _drawing.SetZoom(zoomDouble);
                        UpdatePanels(true);

                        //btnZoomIn.Focus();
                    }
                }

                e.Handled = true;
            }
        }

        private void tstbNavigation_KeyPress(object sender, KeyPressEventArgs e)
        {
            // If it's not a digit, delete or backspace then make sure the input is being handled with. (Suppressed)
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Delete && e.KeyChar != (char)Keys.Back)
            {
                // If enter is pressed apply the entered zoom
                if (e.KeyChar == (char)Keys.Return)
                {
                    int page = 0;

                    int.TryParse(tstbNavigation.Text, out page);

                    // If zoom is higher than zero
                    if (page > 0 && page <= _drawing.Pages)
                    {
                        _drawing.SetPage(page);
                        UpdatePanels(true);

                        //btnZoomIn.Focus();
                    }
                    else
                    {
                        tstbNavigation.Text = _drawing.CurrentPage.ToString();
                    }
                }

                e.Handled = true;
            }
        }

        private void tsbBack_Click(object sender, EventArgs e)
        {
            _drawing.PreviousPage();
            tstbNavigation.Text = (_drawing.CurrentPage + 1).ToString();

            pbFull.Invalidate();
            UpdatePanels(true);
        }

        private void tsbNext_Click(object sender, EventArgs e)
        {
            _drawing.NextPage();
            tstbNavigation.Text = (_drawing.CurrentPage + 1).ToString();

            pbFull.Invalidate();
            UpdatePanels(true);
        }

        #endregion Non Public Members
    }

    public class ImageViewerRotationEventArgs : EventArgs
    {
        public int Rotation { get; }

        public ImageViewerRotationEventArgs(int rotation)
        {
            Rotation = rotation;
        }
    }

    public class ImageViewerZoomEventArgs : EventArgs
    {
        public int Zoom { get; }
        public ImageViewerEx.ZoomType InOut { get; }

        public ImageViewerZoomEventArgs(double zoom, ImageViewerEx.ZoomType inOut)
        {
            Zoom = Convert.ToInt32(Math.Round((zoom * 100), 0));
            InOut = inOut;
        }
    }
}
