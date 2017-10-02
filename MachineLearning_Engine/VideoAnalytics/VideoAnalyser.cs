using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Vision.Motion;
using Accord.Imaging;
using Accord.Video.DirectShow;
using System.Drawing;
using System.Threading;

namespace BH.Engine.MachineLearning
{
    public class VideoAnalyser
    {
        #region Variables
        private String _fileName;
        private FileVideoSource _video;
        private bool _firstFrameFlag;
        private Bitmap _previousFrame;
        private Bitmap _currentFrame;
        private AutoResetEvent _doneEvent;
        #endregion

        public VideoAnalyser()
        {
            FileName = "";
            _firstFrameFlag = false;
        }

        #region Properties
        public String FileName { set { _fileName = value; } }

        public Bitmap CurrentFrame { get { return _currentFrame; } }

        public Bitmap PreviousFrame { get { return _previousFrame; } }
        #endregion

        #region Methods
        public void LoadFile()
        {
            if (_fileName == null || _fileName.Equals("")) return;

            try
            {
                _doneEvent = new AutoResetEvent(false);

                _video = new FileVideoSource(_fileName);
                _video.NewFrame += new Accord.Video.NewFrameEventHandler(Video_NewFrame);
                _video.PlayingFinished += Video_PlayingFinished;
                
                _video.Start();

                _firstFrameFlag = true;

                _doneEvent.WaitOne();

                _firstFrameFlag = false;
            }
            catch { }
        }
        
        #endregion

        #region Events
        private void Video_NewFrame(object sender, Accord.Video.NewFrameEventArgs eventArgs)
        {
            if(_currentFrame != null) _previousFrame = _currentFrame; //Update the frames
            _currentFrame = new Bitmap(eventArgs.Frame); //Create a new copy so that when eventArgs is disposed it doesn't take the frame reference with it
            if(_firstFrameFlag)
                _video.SignalToStop(); //Get the first frame only to begin with for any preprocess necessary
        }

        private void Video_PlayingFinished(object sender, Accord.Video.ReasonToFinishPlaying reason)
        {
            _firstFrameFlag = false;
            _doneEvent.Set();
        }
        #endregion
    }
}
