using System;
using System.Collections.Generic;
using System.Drawing;
using Accord.Vision.Motion;
using Accord.Imaging;

namespace BH.Engine.MachineLearning
{
    public class ROI
    {
        private BH.oM.Geometry.Point mStartCorner;
        private BH.oM.Geometry.Point mEndCorner;
        private Guid _id;
        private Bitmap _startingImage;
        private RectangleF _cutRect;
        private Dictionary<int, double> _motionLevels;
        private int _humanID;

        private TwoFramesDifferenceDetector _detector;

        public ROI(BH.oM.Geometry.Point start, BH.oM.Geometry.Point end)
        {
            mStartCorner = start;
            mEndCorner = end;
            _cutRect = new RectangleF(StartPointS.X, StartPointS.Y, (float)Width, (float)Height);
            _id = Guid.NewGuid();
            _motionLevels = new Dictionary<int, double>();

            _detector = new TwoFramesDifferenceDetector();
            _detector.SuppressNoise = true;

            _humanID = -1;
        }

        public void SetStartingImage(System.Drawing.Bitmap image)
        {
            _startingImage = new System.Drawing.Bitmap(image);
        }

        public void SaveStartingImage()
        {
            //Testing purposes of division of the ROI
            String fileName = @"C:\Users\fgreenro\Documents\BH Project Work\Stadia Analytics\ROI Division Test\" + _id.ToString() + ".png";
            _startingImage.Save(fileName);
        }

        public PointF StartPointS { get { return new PointF((float)mStartCorner.X, (float)mStartCorner.Y); } }
        public PointF EndPointS { get { return new PointF((float)mEndCorner.X, (float)mEndCorner.Y); } }
        public BH.oM.Geometry.Point StartPoint { get { return mStartCorner; } }
        public BH.oM.Geometry.Point EndPoint { get { return mEndCorner; } }
        public RectangleF CutRectangle { get { return _cutRect; } }
        public double Width { get { return Math.Abs(mEndCorner.X - mStartCorner.X); } }
        public double Height { get { return Math.Abs(mEndCorner.Y - mStartCorner.Y); } }
        public int HumanID { get { return _humanID; } set { _humanID = value; } }
        public Dictionary<int, double> Values { get { return _motionLevels; } }

        public double CompareMotionAgainstBase(Bitmap newFrame, int frameIndex)
        {
            _detector.ProcessFrame(UnmanagedImage.FromManagedImage(newFrame.Clone(_cutRect, newFrame.PixelFormat)));
            if (!_motionLevels.ContainsKey(frameIndex))
                _motionLevels.Add(frameIndex, 0);

            _motionLevels[frameIndex] = _detector.MotionLevel;

            return _motionLevels[frameIndex];
            /*Bitmap source = new Bitmap(newFrame.Clone(_cutRect, newFrame.PixelFormat));
            double motion = 0;

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

            // Accumulate the motion for each cell
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    motion += pixels[((y * width) + x) * step];
                }

            }

            // Divide by the number of pixels for each cell
            motion /= width * height;

            // Unlock bitmap data
            source.UnlockBits(bitmapData);

            _motionLevels.Add(motion);

            //return motionLevels;*/
        }
    }
}
