/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2018, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or          
 * (at your option) any later version.                                          
 *                                                                              
 * The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License     
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */

using BH.oM.Geometry;
using BH.oM.Geometry.CoordinateSystem;
using BH.oM.Reflection.Attributes;
using System;
using System.Collections.Generic;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** public Methods - Vectors                 ****/
        /***************************************************/

        public static BoundingBox Bounds(this Plane plane)
        {
            double x = plane.Normal.X == 0 ? 0 : double.MaxValue;
            double y = plane.Normal.Y == 0 ? 0 : double.MaxValue;
            double z = plane.Normal.Z == 0 ? 0 : double.MaxValue;

            return new BoundingBox { Min = new Point { X = -x, Y = -y, Z = -z }, Max = new Point { X = x, Y = y, Z = z } };
        }

        /***************************************************/

        public static BoundingBox Bounds(this Point pt)
        {
            return new BoundingBox { Min = pt, Max = pt };
        }

        /***************************************************/

        public static BoundingBox Bounds(this Vector vector)
        {
            Point pt = new Point { X = vector.X, Y = vector.Y, Z = vector.Z };
            return new BoundingBox { Min = pt, Max = pt };
        }

        /***************************************************/

        public static BoundingBox Bounds(this Cartesian coordinateSystem)
        {
            return ((Plane)coordinateSystem).Bounds();
        }

        /***************************************************/

        public static BoundingBox Bounds(this BoundingBox boundingBox)
        {
            return boundingBox.Clone();
        }

        /***************************************************/
        /**** public Computation - Curves              ****/
        /***************************************************/

        public static BoundingBox Bounds(this Arc arc)
        {

            if (!arc.IsValid())
                throw new Exception("Invalid Arc");

            Circle circle = new Circle { Centre = arc.CoordinateSystem.Origin, Normal = arc.CoordinateSystem.Z, Radius = arc.Radius };


            Point start = arc.StartPoint();
            Point end = arc.EndPoint();

            double xMax, xMin, yMax, yMin, zMax, zMin;

            //Get m in and max values from start and end points
            if (start.X > end.X)
            {
                xMax = start.X;
                xMin = end.X;
            }
            else
            {
                xMax = end.X;
                xMin = start.X;
            }

            if (start.Y > end.Y)
            {
                yMax = start.Y;
                yMin = end.Y;
            }
            else
            {
                yMax = end.Y;
                yMin = start.Y;
            }

            if (start.Z > end.Z)
            {
                zMax = start.Z;
                zMin = end.Z;
            }
            else
            {
                zMax = end.Z;
                zMin = start.Z;
            }


            //Circular arc parameterised to
            //A(theta) = C+r*cos(theta)*xloc+r*sin(theta)*yloc
            //where: C = centre point
            //r - radius
            //xloc - local x-axis unit vector
            //yloc - local y-axis unit vector
            //A - point on the circle


            Vector x = start - circle.Centre;
            Vector y = circle.Normal.CrossProduct(x);

            Vector endV = end - circle.Centre;

            double angle = x.SignedAngle(endV, circle.Normal);

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
            double theta = Math.Abs(a1) > Tolerance.Angle ? Math.Atan(b1 / a1) : Math.PI / 2;
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
            theta = Math.Abs(a2) > Tolerance.Angle ? Math.Atan(b2 / a2) : Math.PI / 2;
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
            theta = Math.Abs(a3) > Tolerance.Angle ? Math.Atan(b3 / a3) : Math.PI / 2;
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
            
            return new BoundingBox { Min = new Point { X = xMin, Y = yMin, Z = zMin }, Max = new Point { X = xMax, Y = yMax, Z = zMax } };
        }

        /***************************************************/

        public static BoundingBox Bounds(this Circle circle)
        {
            Vector normal = circle.Normal;

            if (normal == new Vector { X = 0, Y = 0, Z = 0 })
                throw new InvalidOperationException("Method trying to operate on an invalid circle");

            double ax = Angle(normal, new Vector { X = 1, Y = 0, Z = 0 });
            double ay = Angle(normal, new Vector { X = 0, Y = 1, Z = 0 });
            double az = Angle(normal, new Vector { X = 0, Y = 0, Z = 1 });

            Vector R = new Vector { X = Math.Sin(ax), Y = Math.Sin(ay), Z = Math.Sin(az) };
            R *= circle.Radius;

            return new BoundingBox { Min = circle.Centre - R, Max = circle.Centre + R };
        }

        /***************************************************/

        public static BoundingBox Bounds(this Ellipse ellipse)
        {
            if (ellipse.Axis1 == new Vector { X = 0, Y = 0, Z = 0 } || ellipse.Axis2 == new Vector { X = 0, Y = 0, Z = 0 })
                throw new InvalidOperationException("Method trying to operate on an invalid ellipse");

            Point centre = ellipse.Centre;
            double cx = centre.X, cy = centre.Y, cz = centre.Z;

            Vector u = ellipse.Axis2.Normalise() * ellipse.Radius2;
            double ux = u.X, uy = u.Y, uz = u.Z;

            Vector v = ellipse.Axis1.Normalise() * ellipse.Radius1;
            double vx = v.X, vy = v.Y, vz = v.Z;

            ux *= ux; uy *= uy; uz *= uz;
            vx *= vx; vy *= vy; vz *= vz;

            return new BoundingBox
            {
                Min = new Point { X = (cx - Math.Sqrt(ux + vx)), Y = (cy - Math.Sqrt(uy + vy)), Z = (cz - Math.Sqrt(uz + vz)) },
                Max = new Point { X = (cx + Math.Sqrt(ux + vx)), Y = (cy + Math.Sqrt(uy + vy)), Z = (cz + Math.Sqrt(uz + vz)) }
            };
        }

        /***************************************************/

        public static BoundingBox Bounds(this Line line)
        {
            Point s = line.Start;
            Point e = line.End;
            Point min = new Point { X = Math.Min(s.X, e.X), Y = Math.Min(s.Y, e.Y), Z = Math.Min(s.Z, e.Z) };
            Point max = new Point { X = Math.Max(s.X, e.X), Y = Math.Max(s.Y, e.Y), Z = Math.Max(s.Z, e.Z) };
            return new BoundingBox { Min = min, Max = max };
        }

        /***************************************************/

        public static BoundingBox Bounds(this NurbsCurve curve)
        {
            return curve.ControlPoints.Bounds();
        }

        /***************************************************/

        public static BoundingBox Bounds(this PolyCurve curve)
        {
            List<ICurve> curves = curve.Curves;

            if (curves.Count == 0)
                return null;

            BoundingBox box = curves[0].IBounds();
            for (int i = 1; i < curves.Count; i++)
                box += curves[i].IBounds();

            return box;
        }

        /***************************************************/

        public static BoundingBox Bounds(this Polyline curve)
        {
            return curve.ControlPoints.Bounds();
        }


        /***************************************************/
        /**** public Computation - Surfaces            ****/
        /***************************************************/

        public static BoundingBox Bounds(this Extrusion surface)
        {
            BoundingBox box = surface.Curve.IBounds();
            return box + (box + surface.Direction);
        }

        /***************************************************/

        public static BoundingBox Bounds(this Loft surface)
        {
            List<ICurve> curves = surface.Curves;

            if (curves.Count == 0)
                return null;

            BoundingBox box = curves[0].IBounds();
            for (int i = 1; i < curves.Count; i++)
                box += curves[i].IBounds();

            return box;
        }

        /***************************************************/

        public static BoundingBox Bounds(this NurbsSurface surface)
        {
            return surface.ControlPoints.Bounds();
        }

        /***************************************************/

        public static BoundingBox Bounds(this Pipe surface)
        {
            BoundingBox box = surface.Centreline.IBounds();
            double radius = surface.Radius;

            box.Min -= new Vector { X = radius, Y = radius, Z = radius };  // TODO: more accurate bounding box needed
            box.Max += new Vector { X = radius, Y = radius, Z = radius };

            return box;
        }

        /***************************************************/

        public static BoundingBox Bounds(this PolySurface surface)
        {
            List<ISurface> surfaces = surface.Surfaces;

            if (surfaces.Count == 0)
                return null;

            BoundingBox box = surfaces[0].IBounds();
            for (int i = 1; i < surfaces.Count; i++)
                box += surfaces[i].IBounds();

            return box;
        }

        /***************************************************/

        public static BoundingBox Bounds(this PlanarSurface surface)
        {
            BoundingBox box = surface.ExternalBoundary.IBounds();
            if (surface.InternalBoundaries != null)
            {
                for (int i = 1; i < surface.InternalBoundaries.Count; i++)
                    box += surface.InternalBoundaries[i].IBounds();
            }
            return box;
        }


        /***************************************************/
        /**** public Methods - Others                  ****/
        /***************************************************/

        public static BoundingBox Bounds(this List<Point> pts)
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

            return new BoundingBox { Min = new Point { X = minX, Y = minY, Z = minZ }, Max = new Point { X = maxX, Y = maxY, Z = maxZ } };
        }

        /***************************************************/

        public static BoundingBox Bounds(this Mesh mesh)
        {
            return Bounds(mesh.Vertices);
        }

        /***************************************************/

        public static BoundingBox Bounds(this CompositeGeometry group)
        {
            List<IGeometry> elements = group.Elements;

            if (elements.Count == 0)
                return null;

            BoundingBox box = elements[0].IBounds();
            for (int i = 1; i < elements.Count; i++)
                box += elements[i].IBounds();

            return box;
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static BoundingBox IBounds(this IGeometry geometry)
        {
            if (geometry == null)
                return null;

            return Bounds(geometry as dynamic);
        }


        /***************************************************/
        /**** Private Methods - Fallback                ****/
        /***************************************************/

        private static BoundingBox Bounds(IGeometry geometry)
        {
            return null;
        }

        /***************************************************/
    }
}
