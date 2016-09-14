using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Video;
using System.Drawing;
using Accord.Vision.Motion;
using Accord.Imaging;
using System.Threading;

namespace MachineLearning_Engine
{
    public class MotionLevelAnalyser
    {
        public MotionLevelAnalyser()
        {
        }


        /****************************************/
        /****  Public Methods                ****/
        /****************************************/

        public async Task<Dictionary<int, double>> Run(string videoFileName, int startFrame = 0, int endFrame = int.MaxValue, string outFolder = "")
        {
            m_MotionLevel = new Dictionary<int, double>();
            m_Detector = new TwoFramesDifferenceDetector();
            m_OutFolder = outFolder;
            if (!outFolder.EndsWith("\\"))
                m_OutFolder += "\\";

            m_FrameIndex = 0;
            m_StartFrame = startFrame;
            m_EndFrame = endFrame;
            m_DoneEvent = new AutoResetEvent(false);

            m_VideoSource = new Accord.Video.DirectShow.FileVideoSource(videoFileName);
            m_VideoSource.NewFrame += NewFrame;
            m_VideoSource.PlayingFinished += PlayingFinished;
            m_VideoSource.Start();

            /*m_FVideoSource = new Accord.Video.FFMPEG.VideoFileSource(videoFileName);
            m_FVideoSource.NewFrame += NewFrame;
            m_FVideoSource.PlayingFinished += PlayingFinished;
            m_FVideoSource.Start();*/


            m_DoneEvent.WaitOne();

            return m_MotionLevel;
        }


        /****************************************/
        /****  Private Fields & Methods      ****/
        /****************************************/

        private Accord.Video.DirectShow.FileVideoSource m_VideoSource;
        //private Accord.Video.FFMPEG.VideoFileSource m_FVideoSource;

        private Dictionary<int, double> m_MotionLevel;
        private string m_OutFolder;
        private TwoFramesDifferenceDetector m_Detector;
        private int m_FrameIndex;
        private int m_StartFrame;
        private int m_EndFrame;
        private AutoResetEvent m_DoneEvent;


        /****************************************/

        private void PlayingFinished(object sender, ReasonToFinishPlaying reason)
        {
            Console.WriteLine("Saving data");
            if (m_OutFolder != "")
            {
                System.IO.StreamWriter file = new System.IO.StreamWriter(m_OutFolder + "motion.csv");
                file.WriteLine("frame, motion level");

                foreach (KeyValuePair<int, double> kvp in m_MotionLevel)
                {
                    file.WriteLine(kvp.Key.ToString() + "," + kvp.Value.ToString());
                }

                file.Close();
            }
            
            m_DoneEvent.Set();
        }

        /****************************************/

        private void NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            if (m_FrameIndex > m_EndFrame)
            {
                Console.WriteLine("ready to stop");
                m_VideoSource.SignalToStop();
                Console.WriteLine("last frame");
                return;
            }

            if (m_FrameIndex >= m_StartFrame)
            {
                Console.WriteLine("Frame " + m_FrameIndex);
                Bitmap frame = eventArgs.Frame;
                m_Detector.ProcessFrame(UnmanagedImage.FromManagedImage(frame));
                m_MotionLevel[m_FrameIndex] = m_Detector.MotionLevel;

                if (m_OutFolder != "")
                {
                    string number = (100000 + m_FrameIndex).ToString().Substring(1);
                    string filename = m_OutFolder + "motion_" + number + ".jpeg";
                    m_Detector.MotionFrame.ToManagedImage().Save(filename);
                }
            }

            m_FrameIndex++;
        }
        
    }
}
