using System;
using BH.oM.Geometry;
using BH.oM.Reflection.Attributes;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        public static Vector TangentAtParameter(this Arc curve, double parameter, double tolerance = Tolerance.Distance)
        {
            double paramTol = tolerance / curve.Length();
            if (parameter > 1 + paramTol || parameter < 0 - paramTol) return null;
            
            return curve.CoordinateSystem.Y.Rotate(curve.StartAngle + (curve.EndAngle - curve.StartAngle) * parameter, curve.CoordinateSystem.Z);
        }

        /***************************************************/

        public static Vector TangentAtParameter(this Circle curve, double parameter, double tolerance = Tolerance.Distance)
        {
            double paramTol = tolerance / curve.Length();
            if (parameter > 1 + paramTol || parameter < 0 - paramTol) return null;

            Vector n = curve.Normal;
            Vector refVector = 1 - Math.Abs(n.DotProduct(Vector.XAxis)) > Tolerance.Angle ? Vector.XAxis : Vector.ZAxis;
            Vector localX = n.CrossProduct(refVector).Normalise();
            return n.CrossProduct(localX).Rotate(parameter * 2 * Math.PI, n);
        }

        /***************************************************/

        public static Vector TangentAtParameter(this Line curve, double parameter, double tolerance = Tolerance.Distance)
        {
            double paramTol = tolerance / curve.Length();
            if (parameter > 1 + paramTol || parameter < 0 - paramTol) return null;

            return curve.Direction();
        }

        /***************************************************/

        [NotImplemented]
        public static Vector TangentAtParameter(this NurbCurve curve, double parameter, double tolerance = Tolerance.Distance)
        {
            //Vector sumNwP = new Vector { X = 0, Y = 0, Z = 0 };
            //Vector sumNwPDer = new Vector { X = 0, Y = 0, Z = 0 };
            //double sumNw = 0;
            //double sumNwDer = 0;

            //int degree = curve.Degree();

            //for (int i = 0; i < curve.ControlPoints.Count; i++)
            //{
            //    double Nt = curve.BasisFunction(i, degree, parameter);
            //    double Nder = curve.DerivativeFunction(i, degree, parameter);
            //    Vector p = Create.Vector(curve.ControlPoints[i]);
            //    sumNwP += p * Nt * curve.Weights[i];
            //    sumNwPDer += p * Nder * curve.Weights[i];
            //    sumNw += Nt * curve.Weights[i];
            //    sumNwDer += Nder * curve.Weights[i];
            //}
            //Vector tangent = sumNwPDer * sumNw - sumNwP * sumNwDer;
            //return tangent.Normalise();

            // The above code does not work for the current implementation of Nurbs curve - TBC.
            throw new NotImplementedException();
        }

        /***************************************************/

        public static Vector TangentAtParameter(this PolyCurve curve, double parameter, double tolerance = Tolerance.Distance)
        {
            double length = curve.Length();
            double paramTol = tolerance / length;
            if (parameter > 1 + paramTol || parameter < 0 - paramTol) return null;

            double cLength = parameter * length;
            foreach (ICurve c in curve.SubParts())
            {
                double l = c.ILength();
                if (l >= cLength) return c.ITangentAtParameter(cLength / l);
                cLength -= l;
            }
            return curve.EndDir();
        }

        /***************************************************/

        public static Vector TangentAtParameter(this Polyline curve, double parameter, double tolerance = Tolerance.Distance)
        {
            double length = curve.Length();
            double paramTol = tolerance / length;
            if (parameter > 1 + paramTol || parameter < 0 - paramTol) return null;

            double cLength = parameter * length;
            double sum = 0;
            foreach (Line line in curve.SubParts())
            {
                sum += line.Length();
                if (cLength <= sum)
                {
                    return line.Direction();
                }
            }
            return curve.EndDir();
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static Vector ITangentAtParameter(this ICurve curve, double parameter, double tolerance = Tolerance.Distance)
        {
            return TangentAtParameter(curve as dynamic, parameter, tolerance);
        }

        /***************************************************/
    }
}
