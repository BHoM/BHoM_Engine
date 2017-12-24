using System;
using System.Collections.Generic;
using System.Linq;
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
        //private DataCollection _data;
        private List<ROI> _regionsOfInterest;
        private String _outputFolder;

        private TwoFramesDifferenceDetector _detector;
        private int _frameIndex;
        private Dictionary<int, Dictionary<int, double>> _motions;
        private int _maxFrame;
        #endregion

        public VideoAnalyser()
        {
            FileName = "";
            _firstFrameFlag = false;
            //_data = new DataCollection();
            _regionsOfInterest = new List<ROI>();
            _motions = new Dictionary<int, Dictionary<int, double>>();
            _maxFrame = 0;
        }

        #region Properties
        public String FileName { set { _fileName = value; } }

        public Bitmap CurrentFrame { get { return _currentFrame; } }

        public Bitmap PreviousFrame { get { return _previousFrame; } }

        public double FrameWidth { get { return (_currentFrame != null) ? _currentFrame.Width : 1; } }

        public double FrameHeight { get { return (_currentFrame != null) ? _currentFrame.Height : 1; } }

        public List<ROI> ROIs { get { return _regionsOfInterest; } }

        public double AverageResult(int timestep)
        {
            double avg = 0;

            foreach (ROI r in _regionsOfInterest)
                avg += r.Values[timestep];

            avg /= _regionsOfInterest.Count;

            return avg;
        }

        public int MaxFrame { get { return _maxFrame; } }
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

        public void AddROI(BH.oM.Geometry.Point start, BH.oM.Geometry.Point end)
        {
            _regionsOfInterest.Add(new ROI(start, end));
            _regionsOfInterest.Last().SetStartingImage(CutROI(_regionsOfInterest.Last()));
            //_regionsOfInterest.Last().SaveStartingImage(); //Testing the division purposes
        }

        public void IssueHumanIDs()
        {
            List<ROI> rois = _regionsOfInterest.OrderBy(x => x.StartPointS.X).ThenByDescending(x => x.StartPointS.Y).ToList(); //Order the ROIs in rows starting top left and moving across
            for (int x = 0; x < rois.Count; x++)
                rois[x].HumanID = (x + 1);
        }

        public void Analyse(String folderName)
        {
            _outputFolder = folderName;
            try
            {
                _doneEvent = new AutoResetEvent(false);
                _detector = new TwoFramesDifferenceDetector();
                _detector.SuppressNoise = true;
                _frameIndex = 0;

                _video = new FileVideoSource(_fileName);
                _video.NewFrame += new Accord.Video.NewFrameEventHandler(Video_NewFrame);
                _video.PlayingFinished += Video_PlayingFinished;

                _video.Start();

                _firstFrameFlag = false;

                _doneEvent.WaitOne();
            }
            catch { }
        }

        public void ExportResults(double fps)
        {
            String fileName = _outputFolder + "Output.json";
            //There's probably some library (external or in the BH.oM?) that would do this a lot better - but this proves the concept for now //FG 06.06.17
            List<ROI> rois = _regionsOfInterest.OrderBy(x => x.HumanID).ToList();

            System.IO.StreamWriter sw = new System.IO.StreamWriter(fileName);
            sw.WriteLine("{");
            sw.WriteLine("\t\"frames\":[");

            String output = "";

            foreach (KeyValuePair<int, Dictionary<int, double>> kvp in _motions)
            {
                output += "\t\t{\n";
                output += "\t\t\t\"timestep\":" + (kvp.Key / fps).ToString() + ",\n";
                output += "\t\t\t\"gridcells\":[\n";
                /*sw.WriteLine("\t\t{");
                sw.WriteLine("\t\t\t\"timestep\":" + (kvp.Key / fps).ToString() + ",");
                sw.WriteLine("\t\t\t\"gridcells\":[");*/

                foreach (KeyValuePair<int, double> kvp2 in kvp.Value)
                {
                    output += "\t\t\t\t{\n";
                    output += "\t\t\t\t\t\"ID\":" + kvp2.Key.ToString() + ",\n";
                    output += "\t\t\t\t\t\"Movement\":" + kvp2.Value.ToString() + ",\n";
                    output += "\t\t\t\t\t\"Occupancy\":true\n";
                    output += "\t\t\t\t},\n";
                    /*sw.WriteLine("\t\t\t\t{");
                    sw.WriteLine("\t\t\t\t\t\"ID\":" + kvp2.Key.ToString() + ",");
                    sw.WriteLine("\t\t\t\t\t\"Movement\":" + kvp2.Value.ToString() + ",");
                    sw.WriteLine("\t\t\t\t\t\"Occupancy\":true");
                    sw.WriteLine("\t\t\t\t}");*/
                }

                output = output.Remove(output.Length - 2);

                output += "\n\t\t\t]\n";
                output += "\t\t},\n";
                /*sw.WriteLine("\t\t\t]");
                sw.WriteLine("\t\t},");*/
            }

            output = output.Remove(output.Length - 2);
            sw.WriteLine(output);
            sw.WriteLine("\n\t]");
            sw.WriteLine("}");

            sw.Close();
        }

        private Bitmap CutROI(ROI roi)
        {
            Bitmap cut = new Bitmap(_currentFrame.Clone(roi.CutRectangle, _currentFrame.PixelFormat));
            return cut;
        }
        #endregion

        #region Events
        private void Video_NewFrame(object sender, Accord.Video.NewFrameEventArgs eventArgs)
        {
            if (_currentFrame != null) _previousFrame = _currentFrame; //Update the frames
            _currentFrame = new Bitmap(eventArgs.Frame); //Create a new copy so that when eventArgs is disposed it doesn't take the frame reference with it

            if (_firstFrameFlag)
                _video.SignalToStop(); //Get the first frame only to begin with for any preprocess necessary
            else
            {
                _detector.ProcessFrame(UnmanagedImage.FromManagedImage(_currentFrame));
                Bitmap motionFrame = _detector.MotionFrame.ToManagedImage();

                if (_frameIndex == 0)
                {
                    DrawGridLines(_currentFrame, false);
                    DrawGridLines(motionFrame, true);
                }
                else
                {
                    _currentFrame.Save(_outputFolder + "NA_" + _frameIndex.ToString() + ".png");
                    motionFrame.Save(_outputFolder + "AN_" + _frameIndex.ToString() + ".png");
                }

                if (!_motions.ContainsKey(_frameIndex))
                    _motions.Add(_frameIndex, new Dictionary<int, double>());

                foreach (ROI r in _regionsOfInterest)
                {
                    double motion = r.CompareMotionAgainstBase(_currentFrame, _frameIndex);
                    if (!_motions[_frameIndex].ContainsKey(r.HumanID))
                        _motions[_frameIndex].Add(r.HumanID, 0);
                    _motions[_frameIndex][r.HumanID] = motion;
                }
            }

            _frameIndex++;

            _maxFrame = Math.Max(_maxFrame, _frameIndex);
        }

        private void DrawGridLines(Bitmap image, bool analysis)
        {
            Pen pen = new Pen(Color.White);

            Bitmap result = new Bitmap(image.Width, image.Height);

            using (Graphics g = Graphics.FromImage(result))
            {
                g.DrawImage(image, 0, 0);

                foreach (ROI r in _regionsOfInterest)
                {
                    g.DrawLine(pen, r.StartPointS, new PointF(r.EndPointS.X, r.StartPointS.Y));
                    g.DrawLine(pen, r.StartPointS, new PointF(r.StartPointS.X, r.EndPointS.Y));
                    g.DrawLine(pen, new PointF(r.StartPointS.X, r.EndPointS.Y), r.EndPointS);
                    g.DrawLine(pen, new PointF(r.EndPointS.X, r.StartPointS.Y), r.EndPointS);
                }
            }

            if (analysis)
                result.Save(_outputFolder + "AN_" + _frameIndex.ToString() + ".png");
            else
                result.Save(_outputFolder + "NA_" + _frameIndex.ToString() + ".png");
        }

        private void Video_PlayingFinished(object sender, Accord.Video.ReasonToFinishPlaying reason)
        {
            _firstFrameFlag = false;
            _doneEvent.Set();
        }
        #endregion
    }
}