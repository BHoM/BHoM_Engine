using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Geometry;
using BH.oM.DataStructure;
using BH.oM.Base;
using BH.Engine.DataStructure;
using BH.Engine.Geometry;


namespace ModelLaundry_Engine
{
    public static class Diagnostic
    {
        public static List<Point> CheckSnappedPoints(List<object> elements, double tolerance, double minDist = 1e-12) //TODO: do we need to re-add #min dist' in bh.om.base?
        {
            PointMatrix matrix = new PointMatrix(tolerance);

            // Get the control points in the matrix
            List<Point> refPoints = new List<Point>();
            List<ICurve> refGeom = Util.GetGeometries(elements);
            foreach (ICurve curve in refGeom)
            {
                foreach (Point pt in curve.GetControlPoints())
                    matrix.AddPoint(pt);
            }

            // Get all the errors
            HashSet<Point> errors = new HashSet<Point>();
            foreach (Tuple<PointMatrix.CompositeValue, PointMatrix.CompositeValue> tuple in matrix.GetRelatedPairs(minDist, tolerance))
            {
                errors.Add(tuple.Item1.Point);
            }

            return errors.ToList();
        }
    }
}
