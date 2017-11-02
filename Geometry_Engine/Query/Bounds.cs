using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** public Methods - Vectors                 ****/
        /***************************************************/

        public static BoundingBox GetBounds(this Plane plane)
        {
            double x = plane.Normal.X == 0 ? 0 : double.MaxValue;
            double y = plane.Normal.Y == 0 ? 0 : double.MaxValue;
            double z = plane.Normal.Z == 0 ? 0 : double.MaxValue;

            return new BoundingBox(new Point(-x, -y, -z), new Point(x, y, z));
        }

        /***************************************************/

        public static BoundingBox GetBounds(this Point pt)
        {
            return new BoundingBox(pt, pt);
        }

        /***************************************************/

        public static BoundingBox GetBounds(this Vector vector)
        {
            Point pt = new Point(vector.X, vector.Y, vector.Z);
            return new BoundingBox(pt, pt);
        }


        /***************************************************/
        /**** public Computation - Curves              ****/
        /***************************************************/

        public static BoundingBox GetBounds(this Arc arc)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        public static BoundingBox GetBounds(this Circle circle)
        {
            // SIMPLIFIED: Currently implemented only for XY!! To do: 3D.

            Point c = circle.Centre;
            double r = circle.Radius;
                  
            Point min = new Point(c.X - r, c.Y - r, c.Z);
            Point max = new Point(c.X + r, c.Y + r, c.Z);

            return new BoundingBox(min, max);

            //throw new NotImplementedException();
        }

        /***************************************************/

        public static BoundingBox GetBounds(this Ellipse ellipse)
        {
            // SIMPLIFIED: Currently implemented only for XY and not rotated!! To do: 3D.

            Point c = ellipse.Centre;
            double r1 = ellipse.Radius1;
            double r2 = ellipse.Radius2;
            //Vector v1 = ellipse.Vector1;
            //Vector v2 = ellipse.Vector2;


            Point min = new Point(c.X - r1, c.Y - r2, c.Z);
            Point max = new Point(c.X + r1, c.Y + r2, c.Z);

            return new BoundingBox(min, max);
        }

        /***************************************************/

        public static BoundingBox GetBounds(this Line line)
        {
            Point s = line.Start;
            Point e = line.End;
            Point min = new Point(Math.Min(s.X, e.X), Math.Min(s.Y, e.Y), Math.Min(s.Z, e.Z));
            Point max = new Point(Math.Max(s.X, e.X), Math.Max(s.Y, e.Y), Math.Max(s.Z, e.Z));
            return new BoundingBox(min, max);
        }

        /***************************************************/

        public static BoundingBox GetBounds(this NurbCurve curve)
        {
            return GetBounds(curve.ControlPoints); //TODO: Need a more accurate bounding box
        }

        /***************************************************/

        public static BoundingBox GetBounds(this PolyCurve curve)
        {
            List<ICurve> curves = curve.Curves;

            if (curves.Count == 0)
                return null;

            BoundingBox box = curves[0].IGetBounds();
            for (int i = 1; i < curves.Count; i++)
                box += curves[i].IGetBounds();

            return box;
        }

        /***************************************************/

        public static BoundingBox GetBounds(this Polyline line)
        {
            return GetBounds(line.ControlPoints);
        }


        /***************************************************/
        /**** public Computation - Surfaces            ****/
        /***************************************************/

        public static BoundingBox GetBounds(this Extrusion surface)
        {
            BoundingBox box = surface.Curve.IGetBounds();
            return box + (box + surface.Direction);
        }

        /***************************************************/

        public static BoundingBox GetBounds(this Loft surface)
        {
            List<ICurve> curves = surface.Curves;

            if (curves.Count == 0)
                return null;

            BoundingBox box = curves[0].IGetBounds();
            for (int i = 1; i < curves.Count; i++)
                box += curves[i].IGetBounds();

            return box;
        }

        /***************************************************/

        public static BoundingBox GetBounds(this NurbSurface surface)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        public static BoundingBox GetBounds(this Pipe surface)
        {
            BoundingBox box = surface.Centreline.IGetBounds();
            double radius = surface.Radius;

            box.Min -= new Vector(radius, radius, radius);  // TODO: more accurate bounding box needed
            box.Max += new Vector(radius, radius, radius);

            return box;
        }

        /***************************************************/

        public static BoundingBox GetBounds(this PolySurface surface)
        {
            List<ISurface> surfaces = surface.Surfaces;

            if (surfaces.Count == 0)
                return null;

            BoundingBox box = surfaces[0].IGetBounds();
            for (int i = 1; i < surfaces.Count; i++)
                box += surfaces[i].IGetBounds();

            return box;
        }


        /***************************************************/
        /**** public Methods - Others                  ****/
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

        public static BoundingBox GetBounds(this Mesh mesh)
        {
            return GetBounds(mesh.Vertices);
        }

        /***************************************************/

        public static BoundingBox GetBounds(this CompositeGeometry group)
        {
            List<IBHoMGeometry> elements = group.Elements;

            if (elements.Count == 0)
                return null;

            BoundingBox box = elements[0].IGetBounds();
            for (int i = 1; i < elements.Count; i++)
                box += elements[i].IGetBounds();

            return box;
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static BoundingBox IGetBounds(this IBHoMGeometry geometry)
        {
            return GetBounds(geometry as dynamic);
        }

    }
}
