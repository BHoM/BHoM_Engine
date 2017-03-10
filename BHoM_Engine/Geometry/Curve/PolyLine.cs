using BHoM.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BHoM.Geometry
{


    public static class XPolyline
    {
        public static void CreateNurbForm(Polyline line)
        {
            line.SetDegree(1);
            line.SetKnots (new double[line.ControlPointVector.Length / (line.Dimensions + 1) + line.Order]);
            line.SetWeights(new double[line.ControlPointVector.Length / (line.Dimensions + 1)]);
            line.Knots[0] = 0;
            line.Knots[line.Knots.Length - 1] = line.Weights.Length - 1;
            for (int i = 0; i < line.Weights.Length; i++)
            {
                line.Knots[i + 1] = i;
                line.Weights[i] = 1;
            }
        }

        public static double Length(Polyline line)
        {
            double length = 0;
            for (int i = 0; i < line.ControlPointVector.Length / (line.Dimensions + 1) - (line.Dimensions + 1); i++)
            {
                length += ArrayUtils.Length(ArrayUtils.Sub(line.ControlPointVector, i, i + line.Dimensions + 1, line.Dimensions + 1));
            }
            return length;           
        }

        public static Point ClosestPoint(Polyline line, Point point)
        {
            List<Point> points = line.ControlPoints;

            double minDist = 1e10;
            Point closest = (points.Count > 0) ? points[0] : new Point(Double.PositiveInfinity, Double.PositiveInfinity, Double.PositiveInfinity);
            for (int i = 1; i < points.Count; i++)
            {
                double[] vector = ArrayUtils.Sub(line.ControlPointVector, (line.Dimensions + 1) * i, (line.Dimensions + 1) * (i + 1), line.Dimensions + 1);
                double[] dir = ArrayUtils.Normalise(vector);
                double t = ArrayUtils.DotProduct(dir, ArrayUtils.Sub(point, Utils.SubArray(line.ControlPointVector, (line.Dimensions + 1) * (i - 1), line.Dimensions + 1)));
                t = Math.Min(Math.Max(t, 0), ArrayUtils.Length(vector));

                Point cp = new Point(ArrayUtils.Add(line.StartPoint, ArrayUtils.Multiply(dir, t)));

                double dist = cp.SquareDistanceTo(point);
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
