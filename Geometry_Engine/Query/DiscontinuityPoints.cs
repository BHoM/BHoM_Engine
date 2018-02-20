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
            return curve.Curves.SelectMany(x => x.IDiscontinuityPoints()).ToList();
        }

        /***************************************************/

        public static List<Point> DiscontinuityPoints(this Polyline curve, double angletolerance = Tolerance.Angle)
        {
            bool isClosed = curve.IsClosed();
            List<Point> ctrlPts = curve.ControlPoints;
            if (ctrlPts.Count < 3) return ctrlPts;
            for (int i = 2; i < ctrlPts.Count; i++)
            {
                Vector v1 = ctrlPts[i - 1] - ctrlPts[i - 2];
                Vector v2 = ctrlPts[i] - ctrlPts[i - 1];
                double angle = v1.Angle(v2);             
                if (angle <= angletolerance || angle >= (2 * Math.PI) - angletolerance) //TODO: Dosn't work with IsColinear for small polylines              
                {
                    ctrlPts.RemoveAt(i - 1);
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
