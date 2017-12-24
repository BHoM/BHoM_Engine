using BH.oM.Geometry;
using System.Collections.Generic;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Vector GetDirection(this Line line)
        {
            return new Vector(line.End.X - line.Start.X, line.End.Y - line.Start.Y, line.End.Z - line.Start.Z).GetNormalised();
        }

        /***************************************************/

        public static Vector GetNormal(this Mesh mesh, Face face)
        {
            List<Point> vertices = mesh.Vertices;

            Point p1 = vertices[(face.A)];
            Point p2 = vertices[(face.B)];
            Point p3 = vertices[(face.C)];
            return Query.GetCrossProduct(p2 - p1, p3 - p1);
        }

        /***************************************************/
        public static Vector GetTangentAt(this NurbCurve curve, double t)
        {

            Vector sumNwP = new Vector(0,0,0);
            Vector sumNwPDer = new Vector(0,0,0);
            double sumNw = 0;
            double sumNwDer = 0;

            int degree = curve.GetDegree();

            for (int i = 0; i < curve.ControlPoints.Count; i++)
            {
                double Nt = curve.GetBasisFunction(i, degree, t);
                double Nder = curve.GetDerivativeFunction(i, degree, t);
                Vector p = new Vector(curve.ControlPoints[i]);
                sumNwP += p * Nt * curve.Weights[i];
                sumNwPDer += p * Nder * curve.Weights[i];
                sumNw += Nt * curve.Weights[i];
                sumNwDer += Nder * curve.Weights[i];
            }
            Vector tangent = sumNwPDer * sumNw - sumNwP * sumNwDer;
            return tangent.GetNormalised();

        }


    }
}
