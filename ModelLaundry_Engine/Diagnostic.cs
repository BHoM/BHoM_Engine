using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BHoM.Geometry;

namespace ModelLaundry_Engine
{
    public static class Diagnostic
    {
        public static List<Point> CheckSnappedPoints(List<object> elements, double tolerance)
        {
            PointMatrix matrix = new PointMatrix(tolerance);

            // Get the control points in the matrix
            List<Point> refPoints = new List<Point>();
            List<Curve> refGeom = Util.GetGeometries(elements);
            foreach (Curve curve in refGeom)
            {
                foreach (Point pt in curve.ControlPoints)
                    matrix.AddPoint(pt);
            }

            // Get all the errors
            HashSet<Point> errors = new HashSet<Point>();
            foreach (Tuple<PointMatrix.CompositeValue, PointMatrix.CompositeValue> tuple in matrix.GetRelatedPairs(0, tolerance))
            {
                errors.Add(tuple.Item1.Point);
            }

            return errors.ToList();
        }
    }
}
