using System;
using System.Collections.Generic;
using Accord.Video;
using System.Drawing;
using Accord.Vision.Motion;
using Accord.Imaging;
using System.Threading;
using System.Drawing.Imaging;


namespace BH.Engine.MachineLearning
{
    public class MotionLevelAnalyser
    {
        public class Config
        {
            public Config()
            {
                StartFrame = 0;
                EndFrame = int.MaxValue;
                FrameStep = 1;
                OutFolder = "";
                NbRows = 1;
                NbColumns = 1;
            }

            public int StartFrame;
            public int EndFrame;
            public int FrameStep;
            public string OutFolder;
            public int NbRows;
            public int NbColumns;
        }

        public delegate void ResultEvent(Dictionary<int, List<double>> result);


        public MotionLevelAnalyser()
        {
        }


        /****************************************/
        /****  Public Methods                ****/
        /****************************************/

        public Dictionary<int, List<double>> Run(string videoFileName, Config config)
        {
            m_MotionLevel = new Dictionary<int, List<double>>();
            m_Detector = new TwoFramesDifferenceDetector();
            m_Detector.SuppressNoise = true;
            m_OutFolder = config.OutFolder;
            if (!m_OutFolder.EndsWith("\\"))
                m_OutFolder = m_OutFolder + "\\";

            m_FrameIndex = 0;
            m_Config = config;
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

        public ResultEvent ResultReceived;


        /****************************************/
        /****  Private Fields & Methods      ****/
        /****************************************/

        private Accord.Video.DirectShow.FileVideoSource m_VideoSource;
        //private Accord.Video.FFMPEG.VideoFileSource m_FVideoSource;

        private Dictionary<int, List<double>> m_MotionLevel;
        private TwoFramesDifferenceDetector m_Detector;
        private int m_FrameIndex;
        private Config m_Config;
        private string m_OutFolder;
        private AutoResetEvent m_DoneEvent;


        /****************************************/

        private void PlayingFinished(object sender, ReasonToFinishPlaying reason)
        {
            Console.WriteLine("Saving data");
            if (m_OutFolder != "")
            {
                System.IO.StreamWriter file = new System.IO.StreamWriter(m_OutFolder + "motion.csv");
                string headers = "frame";
                for (int y = 1; y <= m_Config.NbRows; y++)
                    for (int x = 1; x <= m_Config.NbColumns; x++)
                        headers += ",motion(" + y + ";" + x + ")";
                file.WriteLine(headers);

                foreach (KeyValuePair<int, List<double>> kvp in m_MotionLevel)
                {
                    string line = kvp.Key.ToString();
                    foreach (double val in kvp.Value)
                        line += "," + val;
                    file.WriteLine(line);
                }

                file.Close();
            }
            
            m_DoneEvent.Set();

            if (ResultReceived != null)
                ResultReceived.Invoke(m_MotionLevel);
        }

        /****************************************/

        private void NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            if (m_FrameIndex > m_Config.EndFrame)
            {
                Console.WriteLine("ready to stop");
                m_VideoSource.SignalToStop();
                Console.WriteLine("last frame");
                return;
            }

            if (m_FrameIndex >= m_Config.StartFrame)
            {
                // Get the motion frame
                Bitmap frame = eventArgs.Frame;
                m_Detector.ProcessFrame(UnmanagedImage.FromManagedImage(frame));
                Bitmap motionFrame = m_Detector.MotionFrame.ToManagedImage();

                // Get the level of motion
                if (m_Config.NbRows == 1 && m_Config.NbColumns == 1)
                    m_MotionLevel[m_FrameIndex] = new List<double> { m_Detector.MotionLevel };
                else
                    m_MotionLevel[m_FrameIndex] = GetMotionLevel(motionFrame, m_Config.NbRows, m_Config.NbColumns);

                // Save output motion image
                if (m_OutFolder != "" && (m_FrameIndex - m_Config.StartFrame) % m_Config.FrameStep == 0)
                {
                    Console.WriteLine("Frame " + m_FrameIndex);
                    string number = (100000 + m_FrameIndex).ToString().Substring(1);
                    string filename = m_OutFolder + "motion_" + number + ".jpeg";
                    Bitmap outImg = DrawCellLines(motionFrame, m_Config.NbRows, m_Config.NbColumns);
                    outImg.Save(filename);
                }
            }

            m_FrameIndex++;
        }

        /****************************************/

        private List<double> GetMotionLevel(Bitmap source, int nbRows, int nbColumns)
        {
            List<double> motionLevels = new List<double>(new double[nbRows * nbColumns]);

            // Lock bitmap and return bitmap data
            int width = source.Width;
            int height = source.Height;
            BitmapData bitmapData = source.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, source.PixelFormat);

            // Get source bitmap pixel format size
            int depth = Bitmap.GetPixelFormatSize(source.PixelFormat);
            if (depth != 8 && depth != 24 && depth != 32)
                throw new ArgumentException("Only 8, 24 and 32 bpp images are supported.");

            // Copy data from pointer to array
            int step = depth / 8;
            byte[] pixels = new byte[width * height * step];
            IntPtr ptr = bitmapData.Scan0;
            System.Runtime.InteropServices.Marshal.Copy(ptr, pixels, 0, pixels.Length);

            // Get cell sizes and adjust final image section
            int columnSize = width / nbColumns;
            int rowSize = height / nbRows;
            width = columnSize * nbColumns;
            height = rowSize * nbRows;
            int nbCellPixels = rowSize * columnSize;

            // Accumulate the motion for each cell
            for (int y = 0; y < height; y++)
            {
                int i = y / rowSize;
                for (int x = 0; x < width; x++)
                {
                    motionLevels[i * nbColumns + x / columnSize] += pixels[((y * width) + x) * step];
                }
                    
            }

            // Divide by the number of pixels for each cell
            for (int i = 0; i < motionLevels.Count; i++)
                motionLevels[i] /= nbCellPixels;

            // Unlock bitmap data
            source.UnlockBits(bitmapData);

            return motionLevels;
        }

        /****************************************/

        private Bitmap DrawCellLines(Bitmap image, int nbRows, int nbColumns)
        {
            int width = image.Width;
            int height = image.Height;
            int columnSize = width / nbColumns;
            int rowSize = height / nbRows;

            Pen pen = new Pen(Color.White);
            SolidBrush brush = new SolidBrush(Color.White);
            Font font = new Font("Arial", 16);

            Bitmap result = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(result))
            {
                g.DrawImage(image, 0, 0);
                for (int i = 1; i <= nbRows; i++)
                {
                    int y = i * rowSize;
                    g.DrawString("("+i+";1)", font, brush, new Point(5, y - rowSize/2));
                    if (i < nbRows)
                        g.DrawLine(pen, new Point(0, y), new Point(width - 1, y));
                }
                    

                for (int j = 1; j <= nbColumns; j++)
                {
                    int x = j * columnSize;
                    g.DrawString("(1;"+j+")", font, brush, new Point(x-columnSize/2, 5));
                    if (j < nbColumns)
                        g.DrawLine(pen, new Point(x, 0), new Point(x, height - 1));
                }
                    
            }

            return result;
        }

    }
}

