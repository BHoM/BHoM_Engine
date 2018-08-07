using BH.oM.Geometry;
using BH.oM.Reflection.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public  Methods - Curves                  ****/
        /***************************************************/

        public static NurbCurve ToNurbCurve(this Arc arc)
        {
            double angle = arc.EndAngle - arc.StartAngle;
            Point centre = arc.Centre();
            int nbPts = 1 + 2 * (int)Math.Ceiling(2 * angle / Math.PI);
            double factor = Math.Cos(angle / (nbPts - 1));

            // Create the points
            List<Point> points = new List<Point>();
            for (int i = 0; i < nbPts; i++)
            {
                double t =  i * 1.0 / (nbPts - 1);
                Point pt = arc.PointAtParameter(t);
                if (i % 2 == 1)
                    pt = centre + (pt - centre) / factor;
                points.Add(pt);
            }

            // Create the knots
            double knotStep = 2.0 / (nbPts - 1);
            List<double> knots = new List<double>();
            for (int i = 0; i < (nbPts + 1) / 2; i++)
            {
                knots.Add(i * knotStep);
                knots.Add(i * knotStep);
            }

            // Create the weights
            List<double> weights = new List<double>();
            for (int i = 0; i < nbPts; i++)
            {
                double w = (i % 2 == 0) ? 1.0 : factor;
                weights.Add(w);
            }

            return new NurbCurve { ControlPoints = points, Knots = knots, Weights = weights };
        }

        /***************************************************/

        public static NurbCurve ToNurbCurve(this Circle circle)
        {
            double angle = 2 * Math.PI;
            Point centre = circle.Centre;
            int nbPts = 9;
            double factor = Math.Cos(angle / (nbPts - 1));

            // Create the points
            List<Point> points = new List<Point>();
            for (int i = 0; i < nbPts; i++)
            {
                double t = i * 1.0 / (nbPts - 1);
                Point pt = circle.PointAtParameter(t);
                if (i % 2 == 1)
                    pt = centre + (pt - centre) / factor;
                points.Add(pt);
            }

            // Create the knots
            double knotStep = 2.0 / (nbPts - 1);
            List<double> knots = new List<double>();
            for (int i = 0; i < (nbPts + 1) / 2; i++)
            {
                knots.Add(i * knotStep);
                knots.Add(i * knotStep);
            }

            // Create the weights
            List<double> weights = new List<double>();
            for (int i = 0; i < nbPts; i++)
            {
                double w = (i % 2 == 0) ? 1.0 : factor;
                weights.Add(w);
            }

            return new NurbCurve { ControlPoints = points, Knots = knots, Weights = weights };
        }

        /***************************************************/

        public static NurbCurve ToNurbCurve(this Line line)
        {
            return Create.NurbCurve(new List<Point> { line.Start, line.End }, new double[] { 1, 1 }, new double[] { 0, 1 });

        }

        /***************************************************/

        public static NurbCurve ToNurbCurve(this NurbCurve curve)
        {
            return curve.Clone();
        }

        /***************************************************/

        [NotImplemented]
        public static NurbCurve ToNurbCurve(this PolyCurve curve)
        {
            //Curve c = curve.Curves[0];
            //for (int i = 1; i < curve.Curves.Count; i++)
            //{
            //    c.Append(curve.Curves[i]);
            //}
            //curve.SetControlPoints(c.ControlPointVector);
            //curve.SetKnots(c.Knots);
            //curve.SetWeights(c.Weights);
            //curve.SetDegree(c.Degree);

            throw new NotImplementedException();
        }

        /***************************************************/

        public static NurbCurve ToNurbCurve(this Polyline curve)
        {
            List<Point> points = curve.ControlPoints;
            List<double> weights = curve.ControlPoints.Select(x => 1.0).ToList();
            List<double> knots = new List<double> { 0 };

            double t = 0;
            for ( int i = 1; i < points.Count; i++)
            {
                t += points[i].Distance(points[i - 1]);
                knots.Add(t);
            }
            knots = knots.Select(x => x / t).ToList();

            return new NurbCurve { ControlPoints = points, Weights = weights, Knots = knots };
        }


        /***************************************************/
        /**** Public Methods - Interaces                ****/
        /***************************************************/

        public static NurbCurve IToNurbCurve(this ICurve geometry)
        {
            return ToNurbCurve(geometry as dynamic);
        }

    }
}
