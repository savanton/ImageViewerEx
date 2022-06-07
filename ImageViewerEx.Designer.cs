namespace Savan
{
    partial class ImageViewerEx
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            DisposeControl();

            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panelPreview = new System.Windows.Forms.Panel();
            this.lblPreview = new System.Windows.Forms.Label();
            this.pbPanel = new System.Windows.Forms.PictureBox();
            this.tsLeft = new System.Windows.Forms.ToolStrip();
            this.tsbZoomIn = new System.Windows.Forms.ToolStripButton();
            this.tsbZoomOut = new System.Windows.Forms.ToolStripButton();
            this.tsbFitToScreen = new System.Windows.Forms.ToolStripButton();
            this.tsbRotate270 = new System.Windows.Forms.ToolStripButton();
            this.tsbRotate90 = new System.Windows.Forms.ToolStripButton();
            this.tsbMode = new System.Windows.Forms.ToolStripButton();
            this.tsbOpen = new System.Windows.Forms.ToolStripButton();
            this.tsbPreview = new System.Windows.Forms.ToolStripButton();
            this.tsTop = new System.Windows.Forms.ToolStrip();
            this.tscbZoom = new System.Windows.Forms.ToolStripComboBox();
            this.tsbNext = new System.Windows.Forms.ToolStripButton();
            this.tsbBack = new System.Windows.Forms.ToolStripButton();
            this.tslblNavigation = new System.Windows.Forms.ToolStripLabel();
            this.tstbNavigation = new System.Windows.Forms.ToolStripTextBox();
            this.layoutMain = new System.Windows.Forms.TableLayoutPanel();
            this.layoutImageView = new System.Windows.Forms.TableLayoutPanel();
            this.pbFull = new Savan.PanelDoubleBuffered();
            this.sbVert = new System.Windows.Forms.VScrollBar();
            this.sbHoriz = new System.Windows.Forms.HScrollBar();
            this.panelPreview.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbPanel)).BeginInit();
            this.tsLeft.SuspendLayout();
            this.tsTop.SuspendLayout();
            this.layoutMain.SuspendLayout();
            this.layoutImageView.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelPreview
            // 
            this.panelPreview.BackColor = System.Drawing.Color.LightSteelBlue;
            this.panelPreview.Controls.Add(this.lblPreview);
            this.panelPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelPreview.Location = new System.Drawing.Point(350, 0);
            this.panelPreview.Margin = new System.Windows.Forms.Padding(3, 0, 0, 2);
            this.panelPreview.Name = "panelPreview";
            this.panelPreview.Size = new System.Drawing.Size(150, 30);
            this.panelPreview.TabIndex = 12;
            // 
            // lblPreview
            // 
            this.lblPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblPreview.Font = new System.Drawing.Font("Verdana", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPreview.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.lblPreview.Location = new System.Drawing.Point(0, 0);
            this.lblPreview.Margin = new System.Windows.Forms.Padding(0);
            this.lblPreview.Name = "lblPreview";
            this.lblPreview.Size = new System.Drawing.Size(150, 30);
            this.lblPreview.TabIndex = 0;
            this.lblPreview.Text = "Preview";
            this.lblPreview.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pbPanel
            // 
            this.pbPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.pbPanel.Location = new System.Drawing.Point(350, 32);
            this.pbPanel.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.pbPanel.Name = "pbPanel";
            this.pbPanel.Size = new System.Drawing.Size(150, 117);
            this.pbPanel.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbPanel.TabIndex = 10;
            this.pbPanel.TabStop = false;
            this.pbPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbPanel_MouseDown);
            this.pbPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pbPanel_MouseMove);
            this.pbPanel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pbPanel_MouseUp);
            // 
            // tsLeft
            // 
            this.tsLeft.BackColor = System.Drawing.SystemColors.Control;
            this.tsLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tsLeft.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsLeft.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.tsLeft.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbZoomIn,
            this.tsbZoomOut,
            this.tsbFitToScreen,
            this.tsbRotate270,
            this.tsbRotate90,
            this.tsbMode,
            this.tsbOpen,
            this.tsbPreview});
            this.tsLeft.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.VerticalStackWithOverflow;
            this.tsLeft.Location = new System.Drawing.Point(0, 32);
            this.tsLeft.Name = "tsLeft";
            this.tsLeft.Padding = new System.Windows.Forms.Padding(0, 0, 2, 0);
            this.tsLeft.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.tsLeft.Size = new System.Drawing.Size(26, 318);
            this.tsLeft.TabIndex = 15;
            this.tsLeft.Text = "toolStrip1";
            // 
            // tsbZoomIn
            // 
            this.tsbZoomIn.AutoSize = false;
            this.tsbZoomIn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbZoomIn.Image = global::Savan.Properties.Resources.btnZoomIn;
            this.tsbZoomIn.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsbZoomIn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbZoomIn.Name = "tsbZoomIn";
            this.tsbZoomIn.Size = new System.Drawing.Size(24, 24);
            this.tsbZoomIn.Text = "tsbZoomIn";
            this.tsbZoomIn.Click += new System.EventHandler(this.tsbZoomIn_Click);
            // 
            // tsbZoomOut
            // 
            this.tsbZoomOut.AutoSize = false;
            this.tsbZoomOut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbZoomOut.Image = global::Savan.Properties.Resources.btnZoomOut;
            this.tsbZoomOut.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsbZoomOut.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbZoomOut.Name = "tsbZoomOut";
            this.tsbZoomOut.Size = new System.Drawing.Size(24, 24);
            this.tsbZoomOut.Text = "tsbZoomOut";
            this.tsbZoomOut.Click += new System.EventHandler(this.tsbZoomOut_Click);
            // 
            // tsbFitToScreen
            // 
            this.tsbFitToScreen.AutoSize = false;
            this.tsbFitToScreen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbFitToScreen.Image = global::Savan.Properties.Resources.btnFitToScreen;
            this.tsbFitToScreen.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsbFitToScreen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbFitToScreen.Name = "tsbFitToScreen";
            this.tsbFitToScreen.Size = new System.Drawing.Size(24, 24);
            this.tsbFitToScreen.Text = "tsbFitToScreen";
            this.tsbFitToScreen.Click += new System.EventHandler(this.tsbFitToScreen_Click);
            // 
            // tsbRotate270
            // 
            this.tsbRotate270.AutoSize = false;
            this.tsbRotate270.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbRotate270.Image = global::Savan.Properties.Resources.btnRotate270;
            this.tsbRotate270.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsbRotate270.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbRotate270.Name = "tsbRotate270";
            this.tsbRotate270.Size = new System.Drawing.Size(24, 24);
            this.tsbRotate270.Text = "tsbRotate270";
            this.tsbRotate270.Click += new System.EventHandler(this.tsbRotate270_Click);
            // 
            // tsbRotate90
            // 
            this.tsbRotate90.AutoSize = false;
            this.tsbRotate90.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbRotate90.Image = global::Savan.Properties.Resources.btnRotate90;
            this.tsbRotate90.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsbRotate90.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbRotate90.Name = "tsbRotate90";
            this.tsbRotate90.Size = new System.Drawing.Size(24, 24);
            this.tsbRotate90.Text = "tsbRotate90";
            this.tsbRotate90.Click += new System.EventHandler(this.tsbRotate90_Click);
            // 
            // tsbMode
            // 
            this.tsbMode.AutoSize = false;
            this.tsbMode.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbMode.Image = global::Savan.Properties.Resources.btnSelect;
            this.tsbMode.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsbMode.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbMode.Name = "tsbMode";
            this.tsbMode.Size = new System.Drawing.Size(24, 24);
            this.tsbMode.Text = "tsbMode";
            this.tsbMode.Click += new System.EventHandler(this.tsbMode_Click);
            // 
            // tsbOpen
            // 
            this.tsbOpen.AutoSize = false;
            this.tsbOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbOpen.Image = global::Savan.Properties.Resources.btnOpen;
            this.tsbOpen.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsbOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbOpen.Name = "tsbOpen";
            this.tsbOpen.Size = new System.Drawing.Size(24, 24);
            this.tsbOpen.Text = "tsbOpen";
            this.tsbOpen.Click += new System.EventHandler(this.tsbOpen_Click);
            // 
            // tsbPreview
            // 
            this.tsbPreview.AutoSize = false;
            this.tsbPreview.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbPreview.Image = global::Savan.Properties.Resources.btnPreview;
            this.tsbPreview.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsbPreview.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbPreview.Name = "tsbPreview";
            this.tsbPreview.Size = new System.Drawing.Size(24, 24);
            this.tsbPreview.Text = "tsbPreview";
            this.tsbPreview.Click += new System.EventHandler(this.tsbPreview_Click);
            // 
            // tsTop
            // 
            this.tsTop.BackColor = System.Drawing.Color.LightSteelBlue;
            this.tsTop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tsTop.GripMargin = new System.Windows.Forms.Padding(0);
            this.tsTop.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsTop.ImageScalingSize = new System.Drawing.Size(22, 22);
            this.tsTop.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tscbZoom,
            this.tsbNext,
            this.tsbBack,
            this.tslblNavigation,
            this.tstbNavigation});
            this.tsTop.Location = new System.Drawing.Point(26, 0);
            this.tsTop.Name = "tsTop";
            this.tsTop.Padding = new System.Windows.Forms.Padding(0);
            this.tsTop.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.tsTop.Size = new System.Drawing.Size(321, 32);
            this.tsTop.TabIndex = 24;
            this.tsTop.Text = "toolStrip2";
            // 
            // tscbZoom
            // 
            this.tscbZoom.AutoSize = false;
            this.tscbZoom.FlatStyle = System.Windows.Forms.FlatStyle.Standard;
            this.tscbZoom.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tscbZoom.Margin = new System.Windows.Forms.Padding(3, 0, 0, 9);
            this.tscbZoom.Name = "tscbZoom";
            this.tscbZoom.Size = new System.Drawing.Size(76, 23);
            this.tscbZoom.SelectedIndexChanged += new System.EventHandler(this.toolStripComboBox1_SelectedIndexChanged);
            this.tscbZoom.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tscbZoom_KeyPress);
            // 
            // tsbNext
            // 
            this.tsbNext.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tsbNext.AutoSize = false;
            this.tsbNext.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbNext.Image = global::Savan.Properties.Resources.btnNext;
            this.tsbNext.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsbNext.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbNext.Name = "tsbNext";
            this.tsbNext.Size = new System.Drawing.Size(24, 24);
            this.tsbNext.Text = "tsbNext";
            this.tsbNext.Click += new System.EventHandler(this.tsbNext_Click);
            // 
            // tsbBack
            // 
            this.tsbBack.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tsbBack.AutoSize = false;
            this.tsbBack.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbBack.Image = global::Savan.Properties.Resources.btnBack;
            this.tsbBack.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsbBack.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbBack.Name = "tsbBack";
            this.tsbBack.Size = new System.Drawing.Size(24, 24);
            this.tsbBack.Text = "tsbBack";
            this.tsbBack.Click += new System.EventHandler(this.tsbBack_Click);
            // 
            // tslblNavigation
            // 
            this.tslblNavigation.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tslblNavigation.Name = "tslblNavigation";
            this.tslblNavigation.Size = new System.Drawing.Size(21, 29);
            this.tslblNavigation.Text = "/ 0";
            // 
            // tstbNavigation
            // 
            this.tstbNavigation.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tstbNavigation.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tstbNavigation.MaxLength = 5;
            this.tstbNavigation.Name = "tstbNavigation";
            this.tstbNavigation.Size = new System.Drawing.Size(30, 32);
            this.tstbNavigation.Text = "0";
            this.tstbNavigation.TextBoxTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tstbNavigation.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tstbNavigation_KeyPress);
            // 
            // layoutMain
            // 
            this.layoutMain.BackColor = System.Drawing.SystemColors.Control;
            this.layoutMain.ColumnCount = 3;
            this.layoutMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.layoutMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.layoutMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.layoutMain.Controls.Add(this.layoutImageView, 1, 1);
            this.layoutMain.Controls.Add(this.tsLeft, 0, 1);
            this.layoutMain.Controls.Add(this.tsTop, 1, 0);
            this.layoutMain.Controls.Add(this.pbPanel, 2, 1);
            this.layoutMain.Controls.Add(this.panelPreview, 2, 0);
            this.layoutMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutMain.Location = new System.Drawing.Point(0, 0);
            this.layoutMain.Margin = new System.Windows.Forms.Padding(2);
            this.layoutMain.Name = "layoutMain";
            this.layoutMain.RowCount = 2;
            this.layoutMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.layoutMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.layoutMain.Size = new System.Drawing.Size(500, 350);
            this.layoutMain.TabIndex = 26;
            // 
            // layoutImageView
            // 
            this.layoutImageView.ColumnCount = 2;
            this.layoutImageView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.layoutImageView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.layoutImageView.Controls.Add(this.pbFull, 0, 0);
            this.layoutImageView.Controls.Add(this.sbVert, 1, 0);
            this.layoutImageView.Controls.Add(this.sbHoriz, 0, 1);
            this.layoutImageView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutImageView.Location = new System.Drawing.Point(26, 32);
            this.layoutImageView.Margin = new System.Windows.Forms.Padding(0);
            this.layoutImageView.Name = "layoutImageView";
            this.layoutImageView.RowCount = 2;
            this.layoutImageView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.layoutImageView.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.layoutImageView.Size = new System.Drawing.Size(321, 318);
            this.layoutImageView.TabIndex = 27;
            // 
            // pbFull
            // 
            this.pbFull.BackColor = System.Drawing.SystemColors.ControlLight;
            this.pbFull.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbFull.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbFull.Location = new System.Drawing.Point(0, 0);
            this.pbFull.Margin = new System.Windows.Forms.Padding(0);
            this.pbFull.Name = "pbFull";
            this.pbFull.Size = new System.Drawing.Size(304, 301);
            this.pbFull.TabIndex = 23;
            this.pbFull.Click += new System.EventHandler(this.pbFull_Click);
            this.pbFull.DragDrop += new System.Windows.Forms.DragEventHandler(this.pbFull_DragDrop);
            this.pbFull.DragEnter += new System.Windows.Forms.DragEventHandler(this.pbFull_DragEnter);
            this.pbFull.Paint += new System.Windows.Forms.PaintEventHandler(this.pbFull_Paint);
            this.pbFull.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.pbFull_MouseDoubleClick);
            this.pbFull.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbFull_MouseDown);
            this.pbFull.MouseEnter += new System.EventHandler(this.pbFull_MouseEnter);
            this.pbFull.MouseLeave += new System.EventHandler(this.pbFull_MouseLeave);
            this.pbFull.MouseHover += new System.EventHandler(this.pbFull_MouseHover);
            this.pbFull.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pbFull_MouseMove);
            this.pbFull.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pbFull_MouseUp);
            // 
            // sbVert
            // 
            this.sbVert.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.sbVert.Location = new System.Drawing.Point(304, 0);
            this.sbVert.Name = "sbVert";
            this.sbVert.Size = new System.Drawing.Size(17, 301);
            this.sbVert.TabIndex = 0;
            this.sbVert.Scroll += new System.Windows.Forms.ScrollEventHandler(this.sbVert_Scroll);
            // 
            // sbHoriz
            // 
            this.sbHoriz.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.sbHoriz.Location = new System.Drawing.Point(0, 301);
            this.sbHoriz.Name = "sbHoriz";
            this.sbHoriz.Size = new System.Drawing.Size(304, 17);
            this.sbHoriz.TabIndex = 1;
            this.sbHoriz.Scroll += new System.Windows.Forms.ScrollEventHandler(this.sbHoriz_Scroll);
            // 
            // ImageViewerEx
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.layoutMain);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Margin = new System.Windows.Forms.Padding(0);
            this.MinimumSize = new System.Drawing.Size(454, 157);
            this.Name = "ImageViewerEx";
            this.Size = new System.Drawing.Size(500, 350);
            this.Load += new System.EventHandler(this.ImageViewerEx_Load);
            this.Click += new System.EventHandler(this.ImageViewerEx_Click);
            this.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.ImageViewerEx_MouseWheel);
            this.panelPreview.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbPanel)).EndInit();
            this.tsLeft.ResumeLayout(false);
            this.tsLeft.PerformLayout();
            this.tsTop.ResumeLayout(false);
            this.tsTop.PerformLayout();
            this.layoutMain.ResumeLayout(false);
            this.layoutMain.PerformLayout();
            this.layoutImageView.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelPreview;
        private System.Windows.Forms.Label lblPreview;
        private System.Windows.Forms.PictureBox pbPanel;
        private PanelDoubleBuffered pbFull;
        private System.Windows.Forms.HScrollBar sbHoriz;
        private System.Windows.Forms.VScrollBar sbVert;
        private System.Windows.Forms.ToolStrip tsLeft;
        private System.Windows.Forms.ToolStripButton tsbZoomIn;
        private System.Windows.Forms.ToolStripButton tsbZoomOut;
        private System.Windows.Forms.ToolStripButton tsbFitToScreen;
        private System.Windows.Forms.ToolStripButton tsbRotate270;
        private System.Windows.Forms.ToolStripButton tsbRotate90;
        private System.Windows.Forms.ToolStripButton tsbMode;
        private System.Windows.Forms.ToolStripButton tsbOpen;
        private System.Windows.Forms.ToolStripButton tsbPreview;
        private System.Windows.Forms.ToolStrip tsTop;
        private System.Windows.Forms.ToolStripComboBox tscbZoom;
        private System.Windows.Forms.TableLayoutPanel layoutMain;
        private System.Windows.Forms.ToolStripButton tsbNext;
        private System.Windows.Forms.ToolStripButton tsbBack;
        private System.Windows.Forms.ToolStripLabel tslblNavigation;
        private System.Windows.Forms.ToolStripTextBox tstbNavigation;
        private System.Windows.Forms.TableLayoutPanel layoutImageView;
    }
}
