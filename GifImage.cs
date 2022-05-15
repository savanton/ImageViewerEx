using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Timers;

namespace Savan
{
    public class GifImage : IDisposable
    {
        private ImageViewer imageViewer;
        private Image gif;
        private FrameDimension dimension;
        private int currentFrame;
        private bool updating;
        private Timer timer;
        private double framesPerSecond;
        private bool animationEnabled;

        public Size CurrentFrameSize { get; private set; }

        public void Dispose()
        {
            Lock();
            timer.Enabled = false;
            gif.Dispose();
            gif = null;
            Unlock();

            timer.Dispose();
        }

        public double FPS
        {
            get => (1000.0 / framesPerSecond);
            set
            {
                if (value <= 30.0 && value > 0.0)
                {
                    framesPerSecond = 1000.0 / value;

                    if (timer != null)
                    {
                        timer.Interval = framesPerSecond;
                    }
                }
            }
        }

        public bool AnimationEnabled
        {
            get => animationEnabled;
            set
            {
                animationEnabled = value;

                if (timer != null)
                {
                    timer.Enabled = animationEnabled;
                }
            }
        }

        public GifImage(ImageViewer viewer, Image img, bool animation, double fps)
        {
            this.updating = true;
            this.imageViewer = viewer;
            this.gif = img;
            this.dimension = new FrameDimension(gif.FrameDimensionsList[0]);
            this.FrameCount = gif.GetFrameCount(dimension);
            this.gif.SelectActiveFrame(dimension, 0);
            this.currentFrame = 0;
            this.animationEnabled = animation;

            this.timer = new Timer();

            this.updating = false;

            framesPerSecond = 1000.0 / fps; // 15 FPS
            this.timer.Enabled = animationEnabled;
            this.timer.Interval = framesPerSecond;
            this.timer.Elapsed += timer_Elapsed;

            this.CurrentFrame = (Bitmap)gif;
            this.CurrentFrameSize = new Size(CurrentFrame.Size.Width, CurrentFrame.Size.Height);
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            NextFrame();
        }

        public bool Lock()
        {
            if (updating == false)
            {
                while (updating)
                {
                    // Wait
                }

                return true;
            }

            return false;
        }

        public void Unlock()
        {
            updating = false;
        }

        public void NextFrame()
        {
            try
            {
                if (gif != null)
                {
                    if (Lock())
                    {
                        lock (gif)
                        {
                            gif.SelectActiveFrame(this.dimension, this.currentFrame);
                            currentFrame++;

                            if (currentFrame >= this.FrameCount)
                            {
                                currentFrame = 0;
                            }

                            OnFrameChanged();
                        }
                    }

                    Unlock();
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        public int Rotation { get; private set; }

        public void Rotate(int rotation)
        {
            Rotation = (Rotation + rotation) % 360;
        }

        private void OnFrameChanged()
        {
            CurrentFrame = (Bitmap)gif;
            CurrentFrameSize = new Size(CurrentFrame.Size.Width, CurrentFrame.Size.Height);

            imageViewer.InvalidatePanel();
        }

        public Bitmap CurrentFrame { get; private set; }

        public int FrameCount { get; }
    }
}
