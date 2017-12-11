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

            if (!arc.IsValid())
                throw new Exception("Invalid Arc");

            Circle circle = Create.Circle(arc.Start, arc.Middle, arc.End);


            double xMax, xMin, yMax, yMin, zMax, zMin;

            //Get m in and max values from start and end points
            if (arc.Start.X > arc.End.X)
            {
                xMax = arc.Start.X;
                xMin = arc.End.X;
            }
            else
            {
                xMax = arc.End.X;
                xMin = arc.Start.X;
            }

            if (arc.Start.Y > arc.End.Y)
            {
                yMax = arc.Start.Y;
                yMin = arc.End.Y;
            }
            else
            {
                yMax = arc.End.Y;
                yMin = arc.Start.Y;
            }

            if (arc.Start.Z > arc.End.Z)
            {
                zMax = arc.Start.Z;
                zMin = arc.End.Z;
            }
            else
            {
                zMax = arc.End.Z;
                zMin = arc.Start.Z;
            }


            //Circular arc parameterised to
            //A(theta) = C+r*cos(theta)*xloc+r*sin(theta)*yloc
            //where: C = centre point
            //r - radius
            //xloc - local x-axis unit vector
            //yloc - local y-axis unit vector
            //A - point on the circle


            Vector x = arc.Start - circle.Centre;
            Vector y = circle.Normal.GetCrossProduct(x);

            Vector end = arc.End - circle.Centre;

            double angle = x.GetSignedAngle(end, circle.Normal);

            angle = angle < 0 ? angle + Math.PI * 2 : angle;

            double a1, a2, a3, b1, b2, b3;

            a1 = x.X;
            a2 = x.Y;
            a3 = x.Z;
            b1 = y.X;
            b2 = y.Y;
            b3 = y.Z;


            //Finding potential extreme values for x, y and z. Solving for A' = 0

            //Extreme x
            double theta = Math.Abs(a1) > Tolerance.Distance ? Math.Atan(b1 / a1) : Math.PI/2;
            while (theta < angle)
            {
                if (theta > 0)
                {
                    double xTemp = circle.Centre.X + Math.Cos(theta) * a1 + Math.Sin(theta) * b1;
                    xMax = Math.Max(xMax, xTemp);
                    xMin = Math.Min(xMin, xTemp);
                }
                theta += Math.PI;
            }

            //Extreme y
            theta = Math.Abs(a2) > Tolerance.Distance ? Math.Atan(b2 / a2) : Math.PI/2;
            while (theta < angle)
            {
                if (theta > 0)
                {
                    double yTemp = circle.Centre.Y + Math.Cos(theta) * a2 + Math.Sin(theta) * b2;
                    yMax = Math.Max(yMax, yTemp);
                    yMin = Math.Min(yMin, yTemp);
                }
                theta += Math.PI;
            }

            //Extreme z
            theta = Math.Abs(a3) > Tolerance.Distance ? Math.Atan(b3 / a3) : Math.PI/2;
            while (theta < angle)
            {
                if (theta > 0)
                {
                    double zTemp = circle.Centre.Z + Math.Cos(theta) * a3 + Math.Sin(theta) * b3;
                    zMax = Math.Max(zMax, zTemp);
                    zMin = Math.Min(zMin, zTemp);
                }
                theta += Math.PI;
            }
            

            return new BoundingBox(new Point(xMin, yMin, zMin), new Point(xMax, yMax, zMax));
        }

        /***************************************************/

        public static BoundingBox GetBounds(this Circle circle)
        {
            Vector normal = circle.Normal;

            if (normal == new Vector(0, 0, 0))
                throw new InvalidOperationException("Method trying to operate on an invalid circle");

            double ax = GetAngle(normal, new Vector(1, 0, 0));
            double ay = GetAngle(normal, new Vector(0, 1, 0));
            double az = GetAngle(normal, new Vector(0, 0, 1));

            Vector R = new Vector(Math.Sin(ax), Math.Sin(ay), Math.Sin(az));
            R *= circle.Radius;

            return new BoundingBox(circle.Centre - R, circle.Centre + R);
        }

        /***************************************************/

        public static BoundingBox GetBounds(this Ellipse ellipse)
        {
            if (ellipse.Axis1 == new Vector(0, 0, 0) || ellipse.Axis2 == new Vector(0, 0, 0))
                throw new InvalidOperationException("Method trying to operate on an invalid ellipse");

            Point centre = ellipse.Centre;
            double cx = centre.X, cy = centre.Y, cz = centre.Z;

            Vector u = ellipse.Axis2.GetNormalised() * ellipse.Radius2;
            double ux = u.X, uy = u.Y, uz = u.Z;

            Vector v = ellipse.Axis1.GetNormalised() * ellipse.Radius1;
            double vx = v.X, vy = v.Y, vz = v.Z;

            ux *= ux; uy *= uy; uz *= uz;
            vx *= vx; vy *= vy; vz *= vz;

            return new BoundingBox(new Point((cx - Math.Sqrt(ux + vx)), (cy - Math.Sqrt(uy + vy)), (cz - Math.Sqrt(uz + vz))),
                                   new Point((cx + Math.Sqrt(ux + vx)), (cy + Math.Sqrt(uy + vy)), (cz + Math.Sqrt(uz + vz))));
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
