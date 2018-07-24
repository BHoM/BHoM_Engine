using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Compute
    {
        /***************************************************/
        /**** public Methods - Vectors                  ****/
        /***************************************************/

        public static Plane FitPlane(this List<Point> points, double tolerance = Tolerance.Distance)
        {
            if (points.Count < 3) return null;
            Plane result = null;

            Point origin = points.Average();
            double[,] MTM = new double[3, 3];
            double[,] normalizedPoints = new double[points.Count, 3];
            for (int i = 0; i < points.Count; i++)
            {
                normalizedPoints[i, 0] = points[i].X - origin.X;
                normalizedPoints[i, 1] = points[i].Y - origin.Y;
                normalizedPoints[i, 2] = points[i].Z - origin.Z;
            }

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    double value = 0;
                    for (int k = 0; k < points.Count; k++)
                    {
                        value += normalizedPoints[k, i] * normalizedPoints[k, j];
                    }
                    MTM[i, j] = value;
                }
            }
            
            Vector[] eigenvectors = MTM.Eigenvectors(tolerance);
            if (eigenvectors == null) return null;

            double leastSquares = double.PositiveInfinity;
            foreach (Vector eigenvector in eigenvectors)
            {
                double a = eigenvector.X / eigenvector.Z;
                double b = eigenvector.Y / eigenvector.Z;
                double C = -(1 / (a * origin.X + b * origin.Y + origin.Z));
                double B = C * b;
                double A = C * a;

                double squares = 0;
                double S = 1 / (A * A + B * B + C * C);
                foreach (Point pt in points)
                {
                    squares += S * (Math.Pow(A * pt.X + B * pt.Y + C * pt.Z + 1, 2));
                }
                
                if (squares <= leastSquares)
                {
                    leastSquares = squares;
                    result = new Plane { Origin = origin, Normal = eigenvector };
                }
            }
            return result;
        }


        /***************************************************/
        /**** public Methods - Curves                   ****/
        /***************************************************/

        public static Plane FitPlane(this Arc curve, double tolerance = Tolerance.Distance)
        {
            return (Plane)curve.CoordinateSystem;
        }

        /***************************************************/

        public static Plane FitPlane(this Circle curve, double tolerance = Tolerance.Distance)
        {
            return new Plane { Origin = curve.Centre, Normal = curve.Normal };
        }

        /***************************************************/

        public static Plane FitPlane(this Line curve, double tolerance = Tolerance.Distance)
        {
            return null;
        }

        /***************************************************/

        public static Plane FitPlane(this NurbCurve curve, double tolerance = Tolerance.Distance)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        public static Plane FitPlane(this PolyCurve curve, double tolerance = Tolerance.Distance)
        {
            return FitPlane(curve.Curves.SelectMany(x => x.IControlPoints()).ToList(), tolerance);
        }

        /***************************************************/

        public static Plane FitPlane(this Polyline curve, double tolerance = Tolerance.Distance)
        {
            return FitPlane(curve.ControlPoints, tolerance);
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static Plane IFitPlane(this ICurve curve, double tolerance = Tolerance.Distance)
        {
            return FitPlane(curve as dynamic, tolerance);
        }

        /***************************************************/
    }
}
