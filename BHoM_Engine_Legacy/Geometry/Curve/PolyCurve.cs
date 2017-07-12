using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BH.oM.Geometry
{

    public static class XPolyCurve
    {
        public static void CreateNurbFrom(PolyCurve curve)
        {
            Curve c = curve.Curves[0];
            for (int i = 1; i < curve.Curves.Count; i++)
            {
                c.Append(curve.Curves[i]);
            }
            curve.SetControlPoints(c.ControlPointVector);
            curve.SetKnots(c.Knots);
            curve.SetWeights(c.Weights);
            curve.SetDegree(c.Degree);
        }

        public static void Transform(PolyCurve curve, Transform t)
        {
            curve.Curves.Transform(t);
            curve.SetControlPoints(ArrayUtils.MultiplyMany(t, curve.ControlPointVector));
            curve.Update();
        }

        public static void Translate(PolyCurve curve, Vector v)
        {
            curve.Curves.Translate(v);
            curve.SetControlPoints(ArrayUtils.Add(curve.ControlPointVector, v));
        }

        public static void Mirror(PolyCurve curve, Plane p)
        {
            curve.Curves.Mirror(p);
            curve.SetControlPoints(ArrayUtils.Add(ArrayUtils.Multiply(p.ProjectionVectors(curve.ControlPointVector), 2), curve.ControlPointVector));
            curve.Update();
        }

        public static void Project(PolyCurve curve, Plane p)
        {
            curve.Project(p);
            curve.SetControlPoints(ArrayUtils.Add(p.ProjectionVectors(curve.ControlPointVector), curve.ControlPointVector));
            curve.Update();
        }


        public static Point ClosestPoint(PolyCurve curve, Point point)
        {
            List<Point> points = curve.ControlPoints;

            double minDist = 1e10;
            Point closest = curve.StartPoint;

            for (int i = 0; i < curve.Curves.Count; i++)
            {
                Point cp = curve.Curves[i].ClosestPoint(point);
                double dist = cp.DistanceTo(point);
                if (dist < minDist)
                {
                    closest = cp;
                    minDist = dist;
                }
            }

            return closest;
        }
    }
}
