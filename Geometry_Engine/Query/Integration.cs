using System.Collections.Generic;
using BH.oM.Geometry;


namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                           ****/
        /***************************************************/

        public static double GetCurveIntegration(this ICurve fx, Vector direction, double from, double to, ref double centroid, double increment = 0.001)
        {
            double result = 0;
            double max = System.Math.Max(from, to);
            double min = System.Math.Min(from, to);
            double sumAreaLength = 0;
            int segments = (int)((max - min) / increment);
            increment = (max - min) / (double)(segments + 1);
            Point origin = Point.Origin;
            Plane plane = new Plane(origin, direction);
            for (double dx = min; dx < max; dx += increment)
            {
                double currentCentre = dx + increment / 2;
                double sliceWidth = (increment);
                plane.Origin = (origin + plane.Normal * currentCentre);
                List<Point> points = fx.GetIntersections(plane, 0.001);
                double currentValue = 0;
                if (points.Count == 2)
                {
                    currentValue = System.Math.Abs(points[0].Y - points[1].Y);
                }
                else if (points.Count == 1)
                {
                    currentValue = points[0].Y;
                }
                result += currentValue * sliceWidth;
                sumAreaLength += currentValue * sliceWidth * currentCentre;
            }
            centroid = result != 0 ? sumAreaLength / result : 0;
            return result;
        }

        /***************************************************/

        /// <summary>
        /// 
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="curve">f(x) -> n (where n is a constant value) integrated in the x direction</param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="centroid"></param>
        /// <returns></returns>
        public static double GetAreaIntegration(List<IntegrationSlice> slices, double curve, double from, double to, ref double centroid)
        {
            double result = 0;
            double max = System.Math.Max(from, to);
            double min = System.Math.Min(from, to);

            double sumAreaLength = 0;
            for (int i = 0; i < slices.Count; i++)
            {
                IntegrationSlice slice = slices[i];
                if (slice.Centre + slice.Width / 2 > min && slice.Centre - slice.Width / 2 < max)
                {
                    double botSlice = System.Math.Max(min, slice.Centre - slice.Width / 2);
                    double topSlice = System.Math.Min(max, slice.Centre + slice.Width / 2);
                    double currentCentre = (topSlice + botSlice) / 2;
                    double currentValue = curve;
                    double sliceWidth = (topSlice - botSlice);
                    result += currentValue * slice.Length * sliceWidth;
                    sumAreaLength += currentValue * slice.Length * sliceWidth * currentCentre;
                }
            }
            centroid = result != 0 ? sumAreaLength / result : 0;
            return result;
        }

        /***************************************************/


        /// <summary>
        /// 
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="curve">f(x) -> n (where n is a constant value) integrated in the x direction</param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="centroid"></param>
        /// <returns></returns>

        public static double GetAreaIntegration(List<IntegrationSlice> slices, double constant, double xPower, double yPower, double origin = 0)
        {
            double result = 0;

            for (int i = 0; i < slices.Count; i++)
            {
                IntegrationSlice slice = slices[i];
                double dx = slice.Width;
                result += constant * System.Math.Pow(slice.Centre - origin, xPower) * System.Math.Pow(slice.Length, yPower) * dx;
            }
            return result;
        }

        /***************************************************/



        /// <summary>
        /// 
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="curve">f(x) -> n (where n is a constant value) integrated in the x direction</param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="centroid"></param>
        /// <returns></returns>
        public static double GetAreaIntegration(List<IntegrationSlice> slices, double constant, double xPower, double yPower, double from = double.MinValue, double to = double.MaxValue, double origin = 0)
        {
            double result = 0;
            double max = System.Math.Max(from, to);
            double min = System.Math.Min(from, to);


            for (int i = 0; i < slices.Count; i++)
            {
                IntegrationSlice slice = slices[i];
                if (slice.Centre + slice.Width / 2 > min && slice.Centre - slice.Width / 2 < max)
                {
                    double botSlice = System.Math.Max(min, slice.Centre - slice.Width / 2);
                    double topSlice = System.Math.Min(max, slice.Centre + slice.Width / 2);
                    double sliceCentre = (topSlice + botSlice) / 2;
                    double dx = (topSlice - botSlice);
                    result += constant * System.Math.Pow(sliceCentre - origin, xPower) * System.Math.Pow(slice.Length, yPower) * dx;
                }
            }
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="curve">f(x) -> integrated in the x direction</param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="centroid"></param>
        /// <returns></returns>
        public static double GetAreaIntegration(List<IntegrationSlice> slices, Vector direction, ICurve curve, double from, double to, ref double centroid)
        {
            double result = 0;
            double max = System.Math.Max(from, to);
            double min = System.Math.Min(from, to);
            double sumAreaLength = 0;
            Point origin = Point.Origin;
            Plane plane = new Plane(origin, direction);

            for (int i = 0; i < slices.Count; i++)
            {
                IntegrationSlice slice = slices[i];
                if (slice.Centre + slice.Width / 2 > min && slice.Centre - slice.Width / 2 < max)
                {
                    double botSlice = System.Math.Max(min, slice.Centre - slice.Width / 2);
                    double topSlice = System.Math.Min(max, slice.Centre + slice.Width / 2);
                    double currentCentre = (topSlice + botSlice) / 2;
                    double sliceWidth = (topSlice - botSlice);
                    plane.Origin = (origin + plane.Normal * currentCentre);
                    List<Point> points = curve.GetIntersections(plane, 0.001);
                    double currentValue = 0;
                    if (points.Count == 2)
                    {
                        currentValue = System.Math.Abs(points[0].Y - points[1].Y);
                    }
                    else if (points.Count == 1)
                    {
                        currentValue = points[0].Y;
                    }
                    result += currentValue * slice.Length * sliceWidth;
                    sumAreaLength += currentValue * slice.Length * sliceWidth * currentCentre;
                }
            }
            centroid = result != 0 ? sumAreaLength / result : 0;
            return result;
        }

        /***************************************************/

        public static double GetAreaIntegration(List<IntegrationSlice> solid, List<IntegrationSlice> voids, Vector direction, ICurve curve, double from, double to, ref double centroid)
        {
            double centroidSolid = 0;
            double centroidVoid = 0;

            double intSolid = GetAreaIntegration(solid, direction, curve, from, to, ref centroidSolid);
            double intVoid = GetAreaIntegration(voids, direction, curve, from, to, ref centroidVoid);

            centroid = (intSolid * centroidSolid - intVoid * centroidVoid) / (intSolid - intVoid);

            return intSolid - intVoid;
        }

        /***************************************************/


        /***************************************************/


        /***************************************************/

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/


        /***************************************************/
        /**** Private Definitions                       ****/
        /***************************************************/


    }
}
