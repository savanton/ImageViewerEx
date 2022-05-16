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

        private ImageViewerEx _imageViewer;
        private Image _gif;
        private FrameDimension _dimension;
        private int _currentFrame;
        private bool _updating;
        private Timer _timer;
        private double _framesPerSecond;
        private bool _animationEnabled;

        #endregion Fields

        #region C'tors

        public GifImage(ImageViewerEx viewer, Image img, bool animation, double fps)
        {
            _updating = true;
            _imageViewer = viewer;
            _gif = img;
            _dimension = new FrameDimension(_gif.FrameDimensionsList[0]);
            FrameCount = _gif.GetFrameCount(_dimension);
            _gif.SelectActiveFrame(_dimension, 0);
            _currentFrame = 0;
            _animationEnabled = animation;

            _timer = new Timer();

            _updating = false;

            _framesPerSecond = 1000.0 / fps; // 15 FPS
            _timer.Enabled = _animationEnabled;
            _timer.Interval = _framesPerSecond;
            _timer.Elapsed += timer_Elapsed;

            CurrentFrame = (Bitmap)_gif;
            CurrentFrameSize = new Size(CurrentFrame.Size.Width, CurrentFrame.Size.Height);
        }

        #endregion C'tors

        #region Public Members

        public Size CurrentFrameSize { get; private set; }

        public void Dispose()
        {
            Lock();
            _timer.Enabled = false;
            _gif.Dispose();
            _gif = null;
            Unlock();

            _timer.Dispose();
        }

        public double FPS
        {
            get => (1000.0 / _framesPerSecond);
            set
            {
                if (value <= 30.0 && value > 0.0)
                {
                    _framesPerSecond = 1000.0 / value;

                    if (_timer != null)
                    {
                        _timer.Interval = _framesPerSecond;
                    }
                }
            }
        }

        public bool AnimationEnabled
        {
            get => _animationEnabled;
            set
            {
                _animationEnabled = value;

                if (_timer != null)
                {
                    _timer.Enabled = _animationEnabled;
                }
            }
        }

        public bool Lock()
        {
            if (_updating == false)
            {
                while (_updating)
                {
                    // Wait
                }

                return true;
            }

            return false;
        }

        public void Unlock()
        {
            _updating = false;
        }

        public void NextFrame()
        {
            if (_gif != null)
            {
                if (Lock())
                {
                    lock (_gif)
                    {
                        _gif.SelectActiveFrame(_dimension, _currentFrame);
                        _currentFrame++;

                        if (_currentFrame >= FrameCount)
                        {
                            _currentFrame = 0;
                        }

                        OnFrameChanged();
                    }
                }

                Unlock();
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
            CurrentFrame = (Bitmap)_gif;
            CurrentFrameSize = new Size(CurrentFrame.Size.Width, CurrentFrame.Size.Height);

            _imageViewer.InvalidatePanel();
        }

        #endregion Non Public Members

    }
}
