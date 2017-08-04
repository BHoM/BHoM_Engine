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

        public static BoundingBox GetBounds(List<Point> pts)
        {
            Point pt = pts[0];
            double minX = pt.X; double minY = pt.Y; double minZ = pt.Z;
            double maxX = minX; double maxY = minY; double maxZ = minZ;

            for (int i = pts.Count - 1; i > 0; i--)
            {
                pt = pts[i];
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
        /**** Private Methods - Vectors                 ****/
        /***************************************************/

        public static BoundingBox _GetBounds(this Plane plane)
        {
            double x = plane.Normal.X == 0 ? 0 : double.MaxValue;
            double y = plane.Normal.Y == 0 ? 0 : double.MaxValue;
            double z = plane.Normal.Z == 0 ? 0 : double.MaxValue;

            return new BoundingBox(new Point(-x, -y, -z), new Point(x, y, z));
        }

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
            return GetBounds(curve.ControlPoints); //TODO: Need a more accurate bounding box
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
            return GetBounds(line.ControlPoints);
        }


        /***************************************************/
        /**** Private Computation - Surfaces            ****/
        /***************************************************/

        private static BoundingBox _GetBounds(this Extrusion surface)
        {
            BoundingBox box = surface.Curve.GetBounds();
            return box + (box + surface.Direction);
        }

        /***************************************************/

        private static BoundingBox _GetBounds(this Loft surface)
        {
            List<ICurve> curves = surface.Curves;

            if (curves.Count == 0)
                return null;

            BoundingBox box = curves[0].GetBounds();
            for (int i = 1; i < curves.Count; i++)
                box += curves[i].GetBounds();

            return box;
        }

        /***************************************************/

        private static BoundingBox _GetBounds(this NurbSurface surface)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        private static BoundingBox _GetBounds(this Pipe surface)
        {
            BoundingBox box = surface.Centreline.GetBounds();
            double radius = surface.Radius;

            box.Min -= new Vector(radius, radius, radius);  // TODO: more accurate bounding box needed
            box.Max += new Vector(radius, radius, radius);

            return box;
        }

        /***************************************************/

        private static BoundingBox _GetBounds(this PolySurface surface)
        {
            List<ISurface> surfaces = surface.Surfaces;

            if (surfaces.Count == 0)
                return null;

            BoundingBox box = surfaces[0].GetBounds();
            for (int i = 1; i < surfaces.Count; i++)
                box += surfaces[i].GetBounds();

            return box;
        }


        /***************************************************/
        /**** Private Methods - Others                  ****/
        /***************************************************/

        private static BoundingBox _GetBounds(this Mesh mesh)
        {
            return GetBounds(mesh.Vertices);
        }

    }
}
