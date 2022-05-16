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
        #region Fields

        private ImageViewerEx imageViewer;
        private Image gif;
        private FrameDimension dimension;
        private int currentFrame;
        private bool updating;
        private Timer timer;
        private double framesPerSecond;
        private bool animationEnabled;

        #endregion Fields

        #region C'tors

        public GifImage(ImageViewerEx viewer, Image img, bool animation, double fps)
        {
            updating = true;
            imageViewer = viewer;
            gif = img;
            dimension = new FrameDimension(gif.FrameDimensionsList[0]);
            FrameCount = gif.GetFrameCount(dimension);
            gif.SelectActiveFrame(dimension, 0);
            currentFrame = 0;
            animationEnabled = animation;

            timer = new Timer();

            updating = false;

            framesPerSecond = 1000.0 / fps; // 15 FPS
            timer.Enabled = animationEnabled;
            timer.Interval = framesPerSecond;
            timer.Elapsed += timer_Elapsed;

            CurrentFrame = (Bitmap)gif;
            CurrentFrameSize = new Size(CurrentFrame.Size.Width, CurrentFrame.Size.Height);
        }

        #endregion C'tors

        #region Public Members

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
                            gif.SelectActiveFrame(dimension, currentFrame);
                            currentFrame++;

                            if (currentFrame >= FrameCount)
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

        public Bitmap CurrentFrame { get; private set; }

        public int FrameCount { get; }


        #endregion Public Members

        #region Non Public Members

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            NextFrame();
        }

        private void OnFrameChanged()
        {
            CurrentFrame = (Bitmap)gif;
            CurrentFrameSize = new Size(CurrentFrame.Size.Width, CurrentFrame.Size.Height);

            imageViewer.InvalidatePanel();
        }

        #endregion Non Public Members

    }
}
