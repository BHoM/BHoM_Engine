using BH.oM.Geometry;
using System.Collections.Generic;
using System.Linq;
using System;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        public static List<Point> DiscontinuityPoints(this Arc curve)
        {
            return new List<Point> { curve.Start, curve.End };
        }

        /***************************************************/

        public static List<Point> DiscontinuityPoints(this Circle curve)
        {
            return new List<Point>();
        }

        /***************************************************/

        public static List<Point> DiscontinuityPoints(this Line curve)
        {
            return new List<Point> { curve.Start, curve.End };
        }

        /***************************************************/

        public static List<Point> DiscontinuityPoints(this NurbCurve curve)
        {
            if (curve.Degree() == 1)         //TODO: Check that this is correct
                return curve.ControlPoints;
            else
                return new List<Point> { curve.StartPoint(), curve.EndPoint() };
        }

        /***************************************************/

        public static List<Point> DiscontinuityPoints(this PolyCurve curve)
        {
            //TODO: need to check tangency between each two subsequent curves
            // return curve.Curves.SelectMany(x => x.IDiscontinuityPoints()).ToList();
            throw new NotImplementedException();
        }

        /***************************************************/

        public static List<Point> DiscontinuityPoints(this Polyline curve, double angletolerance = Tolerance.Angle)
        {
            List<Point> ctrlPts = new List<Point>(curve.ControlPoints);

            if (ctrlPts.Count < 3) return ctrlPts;
            int j = 0;
            if (!curve.IsClosed()) j += 2;
            for (int i = j; i < ctrlPts.Count; i++)
            {
                int cc = ctrlPts.Count;
                int i1 = (i - 1 + cc) % cc;
                int i2 = (i - 2 + cc) % cc;
                Vector v1 = ctrlPts[i1] - ctrlPts[i2];
                Vector v2 = ctrlPts[i] - ctrlPts[i1];
                double angle = v1.Angle(v2);
                if (angle <= angletolerance || angle >= (2 * Math.PI) - angletolerance || ctrlPts[i2].SquareDistance(ctrlPts[i1]) <= Tolerance.SqrtDist)
                {
                    ctrlPts.RemoveAt(i1);
                    i--;
                }
            }
            return ctrlPts;
        }

        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static List<Point> IDiscontinuityPoints(this ICurve curve)
        {
            return DiscontinuityPoints(curve as dynamic);
        }
    }
}
