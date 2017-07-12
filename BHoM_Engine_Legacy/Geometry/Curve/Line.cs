using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.oM.Geometry
{
    public static class XLine
    {
        public static void CreateNurbForm(this Line line)
        {
            line.SetKnots(new double[] { 0, 0, 1, 1 });
            line.SetWeights(new double[] { 1, 1 });
            line.SetDegree(1);
        }

        public static double Length(this Line line)
        {
            return ArrayUtils.Length(ArrayUtils.Sub(line.ControlPointVector, 0, line.Dimensions + 1, line.Dimensions));
        }

        public static Point ProjectOnInfiniteLine(this Line line, Point pt)
        {
            double[] dir = ArrayUtils.Normalise(ArrayUtils.Sub(line.ControlPointVector, line.Dimensions + 1, 0, line.Dimensions));
            double t = ArrayUtils.DotProduct(dir,  ArrayUtils.Sub(pt, line.StartPoint));
            return new Point(ArrayUtils.Add(line.StartPoint, ArrayUtils.Multiply(dir, t)));
        }

        public static Point ClosestPoint(this Line line, Point pt)
        {
            Vector dir = (line.EndPoint - line.StartPoint) / line.Length();
            double t = Math.Min(Math.Max(dir * (pt - line.StartPoint), 0), line.Length());
            return line.StartPoint + t * dir;
        }

        public static double DistanceTo(this Line line, Line other)
        {
            Point intersection = Intersect.LineLine(line, other);
            if (intersection != null && (intersection.DistanceTo(line.ClosestPoint(intersection)) < BH.oM.Base.Tolerance.MIN_DIST) && (intersection.DistanceTo(other.ClosestPoint(intersection)) < BH.oM.Base.Tolerance.MIN_DIST))
            {
                return 0;
            }
            else
            {
                List<double> distances = new List<double>();
                distances.Add(line.StartPoint.DistanceTo(other.ClosestPoint(line.StartPoint)));
                distances.Add(line.EndPoint.DistanceTo(other.ClosestPoint(line.EndPoint)));
                distances.Add(other.StartPoint.DistanceTo(line.ClosestPoint(other.StartPoint)));
                distances.Add(other.EndPoint.DistanceTo(line.ClosestPoint(other.EndPoint)));

                return distances.Min();
            }
        }
    }
}
