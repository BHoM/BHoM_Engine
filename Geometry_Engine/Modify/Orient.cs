using BH.oM.Geometry;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods - Vectors                  ****/
        /***************************************************/

        public static Point Orient(this Point point, Plane fromPlane, Plane toPlane)
        {
            Point o1 = fromPlane.Origin;
            Vector n1 = fromPlane.Normal;
            Point o2 = toPlane.Origin;
            Vector n2 = toPlane.Normal;

            if (n1.IsParallel(n2) == 0)
            {
                TransformMatrix T1, T2, R1, R2;
                if (n1.IsParallel(Vector.ZAxis) == 0)
                {
                    Vector vy1 = n1.CrossProduct(Vector.ZAxis).Normalise();
                    Vector vx1 = n1.CrossProduct(vy1).Normalise();
                    Vector vz1 = n1;

                    double[,] r1 = new double[,]
                    {
                    {  vx1.X,  vx1.Y,  vx1.Z,  -o1.X  },
                    {  vy1.X,  vy1.Y,  vy1.Z,  -o1.Y  },
                    {  vz1.X,  vz1.Y,  vz1.Z,  -o1.Z  },
                    {  0,  0,  0,  1  }
                    };

                    T1 = Create.TranslationMatrix(new Vector { X = o1.X, Y = o1.Y, Z = o1.Z });
                    T2 = Create.TranslationMatrix(new Vector { X = -o1.X, Y = -o1.Y, Z = -o1.Z });
                    R1 = new TransformMatrix { Matrix = r1 };
                }
                else
                {
                    T1 = Create.IdentityMatrix();
                    T2 = Create.IdentityMatrix();
                    R1 = Create.IdentityMatrix();
                }

                if (n2.IsParallel(Vector.ZAxis) == 0)
                {
                    Vector vy2 = n2.CrossProduct(Vector.ZAxis).Normalise();
                    Vector vx2 = n2.CrossProduct(vy2).Normalise();
                    Vector vz2 = n2;

                    double[,] r2 = new double[,]
                    {
                    {  vx2.X,  vy2.X,  vz2.X,  o2.X  },
                    {  vx2.Y,  vy2.Y,  vz2.Y,  o2.Y  },
                    {  vx2.Z,  vy2.Z,  vz2.Z,  o2.Z   },
                    {  0,  0,  0,  1  }
                    };
                    R2 = new TransformMatrix { Matrix = r2 };
                }
                else
                {
                    R2 = Create.IdentityMatrix();
                }

                return point.Transform(T1 * R1 * T2).Transform(R2);
            }
            return point.Clone();
        }

        /***************************************************/

        public static Vector Orient(this Vector vector, Plane fromPlane, Plane toPlane)
        {
            throw new System.NotImplementedException();
            //Point o1 = fromPlane.Origin;
            //Vector n1 = fromPlane.Normal;
            //Point o2 = toPlane.Origin;
            //Vector n2 = toPlane.Normal;

            //if (n1.IsParallel(n2) == 0)
            //{
            //    TransformMatrix R1, R2;
            //    if (n1.IsParallel(Vector.ZAxis) == 0)
            //    {
            //        Vector vy1 = n1.CrossProduct(Vector.ZAxis).Normalise();
            //        Vector vx1 = n1.CrossProduct(vy1).Normalise();
            //        Vector vz1 = n1;

            //        double[,] r1 = new double[,]
            //        {
            //        {  vx1.X,  vx1.Y,  vx1.Z,  0  },
            //        {  vy1.X,  vy1.Y,  vy1.Z,  0  },
            //        {  vz1.X,  vz1.Y,  vz1.Z,  0  },
            //        {  0,  0,  0,  1  }
            //        };
                    
            //        R1 = new TransformMatrix { Matrix = r1 };
            //    }
            //    else
            //    {
            //        R1 = Create.IdentityMatrix();
            //    }

            //    if (n2.IsParallel(Vector.ZAxis) == 0)
            //    {
            //        Vector vy2 = n2.CrossProduct(Vector.ZAxis).Normalise();
            //        Vector vx2 = n2.CrossProduct(vy2).Normalise();
            //        Vector vz2 = n2;

            //        double[,] r2 = new double[,]
            //        {
            //        {  vx2.X,  vy2.X,  vz2.X,  0  },
            //        {  vx2.Y,  vy2.Y,  vz2.Y,  0  },
            //        {  vx2.Z,  vy2.Z,  vz2.Z,  0  },
            //        {  0,  0,  0,  1  }
            //        };
            //        R2 = new TransformMatrix { Matrix = r2 };
            //    }
            //    else
            //    {
            //        R2 = Create.IdentityMatrix();
            //    }

            //    return vector.Transform(R1).Transform(R2);
            //}
            //return vector.Clone();
        }

        /***************************************************/

        public static Plane Orient(this Plane plane, Plane fromPlane, Plane toPlane)
        {
            throw new System.NotImplementedException();
            // return new Plane { Origin = plane.Origin.Orient(fromPlane, toPlane), Normal = plane.Normal.Orient(fromPlane, toPlane) };
        }


        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        public static Arc Orient(this Arc arc, Plane fromPlane, Plane toPlane)
        {
            return new Arc { Start = arc.Start.Orient(fromPlane,toPlane), Middle = arc.Middle.Orient(fromPlane, toPlane), End = arc.End.Orient(fromPlane, toPlane) };
        }

        /***************************************************/

        public static Circle Orient(this Circle circle, Plane fromPlane, Plane toPlane)
        {
            return new Circle { Centre = circle.Centre.Orient(fromPlane, toPlane), Normal = circle.Normal.Orient(fromPlane, toPlane), Radius = circle.Radius };
        }

        /***************************************************/

        public static Line Orient(this Line line, Plane fromPlane, Plane toPlane)
        {
            return new Line { Start = line.Start.Orient(fromPlane, toPlane), End = line.End.Orient(fromPlane, toPlane) };
        }

        /***************************************************/

        public static NurbCurve Orient(this NurbCurve curve, Plane fromPlane, Plane toPlane)
        {
            return new NurbCurve { ControlPoints = curve.ControlPoints.Select(x => x.Orient(fromPlane, toPlane)).ToList(), Weights = curve.Weights.ToList(), Knots = curve.Knots.ToList() };
        }


        /***************************************************/

        public static PolyCurve Orient(this PolyCurve curve, Plane fromPlane, Plane toPlane)
        {
            return new PolyCurve { Curves = curve.Curves.Select(x => x.IOrient(fromPlane, toPlane)).ToList() };
        }

        /***************************************************/

        public static Polyline Orient(this Polyline curve, Plane fromPlane, Plane toPlane)
        {
            return new Polyline { ControlPoints = curve.ControlPoints.Select(x => x.Orient(fromPlane, toPlane)).ToList() };
        }


        /***************************************************/
        /**** Public Methods - Surfaces                 ****/
        /***************************************************/

        public static Extrusion Orient(this Extrusion surface, Plane fromPlane, Plane toPlane)
        {
            return new Extrusion { Curve = surface.Curve.IOrient(fromPlane, toPlane), Direction = surface.Direction.Orient(fromPlane, toPlane), Capped = surface.Capped };
        }

        /***************************************************/

        public static Loft Orient(this Loft surface, Plane fromPlane, Plane toPlane)
        {
            return new Loft { Curves = surface.Curves.Select(x => x.IOrient(fromPlane, toPlane)).ToList() };
        }

        /***************************************************/

        public static NurbSurface Orient(this NurbSurface surface, Plane fromPlane, Plane toPlane)
        {
            return new NurbSurface { ControlPoints = surface.ControlPoints.Select(x => x.Orient(fromPlane, toPlane)).ToList(), Weights = surface.Weights.ToList(), UKnots = surface.UKnots.ToList(), VKnots = surface.VKnots.ToList() };
        }

        /***************************************************/

        public static Pipe Orient(this Pipe surface, Plane fromPlane, Plane toPlane)
        {
            return new Pipe { Centreline = surface.Centreline.IOrient(fromPlane, toPlane), Radius = surface.Radius, Capped = surface.Capped };
        }

        /***************************************************/

        public static PolySurface Orient(this PolySurface surface, Plane fromPlane, Plane toPlane)
        {
            return new PolySurface { Surfaces = surface.Surfaces.Select(x => x.IOrient(fromPlane, toPlane)).ToList() };
        }


        /***************************************************/
        /**** Public Methods - Others                   ****/
        /***************************************************/

        public static Mesh Orient(this Mesh mesh, Plane fromPlane, Plane toPlane)
        {
            return new Mesh { Vertices = mesh.Vertices.Select(x => x.Orient(fromPlane, toPlane)).ToList(), Faces = mesh.Faces.Select(x => x.Clone() as Face).ToList() };
        }

        /***************************************************/

        public static CompositeGeometry Orient(this CompositeGeometry group, Plane fromPlane, Plane toPlane)
        {
            return new CompositeGeometry { Elements = group.Elements.Select(x => x.IOrient(fromPlane, toPlane)).ToList() };
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static IBHoMGeometry IOrient(this IBHoMGeometry geometry, Plane fromPlane, Plane toPlane)
        {
            return Orient(geometry as dynamic, fromPlane, toPlane);
        }

        /***************************************************/

        public static ICurve IOrient(this ICurve geometry, Plane fromPlane, Plane toPlane)
        {
            return Orient(geometry as dynamic, fromPlane, toPlane);
        }

        /***************************************************/

        public static ISurface IOrient(this ISurface geometry, Plane fromPlane, Plane toPlane)
        {
            return Orient(geometry as dynamic, fromPlane, toPlane);
        }
    }
}
