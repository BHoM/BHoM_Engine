using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Geometry
{
    public static partial class Measure
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static BoundingBox GetBounds(this IBHoMGeometry geometry)
        {
            return _GetBounds(geometry as dynamic);
        }


        /***************************************************/
        /**** Private Methods - Vectors                 ****/
        /***************************************************/
        public static BoundingBox _GetBounds(this Point pt)
        {
            return new BoundingBox(pt, pt);
        }

        /***************************************************/

        public static BoundingBox _GetBounds(this Vector vector)
        {
            Point pt = new Point(vector.X, vector.Y, vector.Z);
            return new BoundingBox(pt, pt);
        }


        /***************************************************/
        /**** Private Computation - Curves              ****/
        /***************************************************/

        private static BoundingBox _GetBounds(this Arc arc)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        private static BoundingBox _GetBounds(this Circle circle)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        private static BoundingBox _GetBounds(this Line line)
        {
            Point s = line.Start;
            Point e = line.End;
            Point min = new Point(Math.Min(s.X, e.X), Math.Min(s.Y, e.Y), Math.Min(s.Z, e.Z));
            Point max = new Point(Math.Max(s.X, e.X), Math.Max(s.Y, e.Y), Math.Max(s.Z, e.Z));
            return new BoundingBox(min, max);
        }

        /***************************************************/

        private static BoundingBox _GetBounds(this NurbCurve curve)
        {
            Point pt = curve.ControlPoints[0];
            double minX = pt.X;
            double minY = pt.Y;
            double minZ = pt.Z;
            double maxX = minX;
            double maxY = minY;
            double maxZ = minZ;

            for (int i = curve.ControlPoints.Count - 1; i > 0; i--)
            {
                pt = curve.ControlPoints[i];
                if (pt.X < minX) minX = pt.X;
                if (pt.Y < minY) minY = pt.Y;
                if (pt.Z < minZ) minZ = pt.Z;
                if (pt.X > maxX) maxX = pt.X;
                if (pt.Y > maxY) maxY = pt.Y;
                if (pt.Z > maxZ) maxZ = pt.Z;
            }

            return new BoundingBox(new Point(minX, minY, minZ), new Point(maxX, maxY, maxZ));
        }

        /***************************************************/

        private static BoundingBox _GetBounds(this PolyCurve curve)
        {
            List<ICurve> curves = curve.Curves;

            if (curves.Count == 0)
                return null;

            BoundingBox box = curves[0].GetBounds();
            for (int i = 1; i < curves.Count; i++)
                box += curves[i].GetBounds();

            return box;
        }

        /***************************************************/

        private static BoundingBox _GetBounds(this Polyline line)
        {
            Point pt = line.ControlPoints[0];
            double minX = pt.X;
            double minY = pt.Y;
            double minZ = pt.Z;
            double maxX = minX;
            double maxY = minY;
            double maxZ = minZ;

            for (int i = line.ControlPoints.Count - 1; i > 0; i--)
            {
                pt = line.ControlPoints[i];
                if (pt.X < minX) minX = pt.X;
                if (pt.Y < minY) minY = pt.Y;
                if (pt.Z < minZ) minZ = pt.Z;
                if (pt.X > maxX) maxX = pt.X;
                if (pt.Y > maxY) maxY = pt.Y;
                if (pt.Z > maxZ) maxZ = pt.Z;
            }

            return new BoundingBox(new Point(minX, minY, minZ), new Point(maxX, maxY, maxZ));
        }

    }
}
