/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
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
using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using BH.oM.Geometry.CoordinateSystem;
using Humanizer;
using BH.oM.Quantities.Attributes;
using System.ComponentModel;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods - Vectors                  ****/
        /***************************************************/

        [Description("Finds the closest point in the provided set of points in relation to the provided Point.")]
        [Input("cloud", "The set of points from which to find the closest to the provided point.")]
        [Input("point", "The reference point. The point in the cloud with the smallest distance to this point will be returned.")]
        [Output("closestPt", "The point in the cloud with the smallest distance to the provided point.")]
        public static Point ClosestPoint(this IEnumerable<Point> cloud, Point point)
        {
            double minDist = double.PositiveInfinity;
            double sqDist = 0;
            Point cp = null;

            foreach (Point pt in cloud)
            {
                sqDist = pt.SquareDistance(point);
                if (sqDist < minDist)
                {
                    minDist = sqDist;
                    cp = pt;
                }
            }

            return cp;
        }

        /***************************************************/

        [Description("Finds the closest point in the provided set of points in relation to the provided Point.")]
        [Input("points", "The set of points from which to find the closest to the provided point.")]
        [Input("point", "The reference point. The point in the set of points with the smallest distance to this point will be returned.")]
        [Output("closestPt", "The point in list of points with the smallest distance to the provided point.")]
        public static Point ClosestPoint(this Point point, IEnumerable<Point> points)
        {
            return points.ClosestPoint(point);
        }

        /***************************************************/

        [Description("Method only defined to satisfy IGeometry interface. A Vector does not have a set location in space, why null always is returned by this method.")]
        [Input("vector", "Unused for this method. A Vector does not have a set location in space, why null always is returned by this method.")]
        [Input("point", "Unused for this method. A Vector does not have a set location in space, why null always is returned by this method.")]
        [Output("closestPt", " A Vector does not have a set location in space, why null always is returned by this method.")]
        public static Point ClosestPoint(this Vector vector, Point point)
        {
            return null;
        }

        /***************************************************/

        [Description("Finds the closest point on the Plane in relation to the provided Point.")]
        [Input("plane", "The Plane on which to find the closest point in relation to the provided point.")]
        [Input("point", "The reference point. The point on the Plane with the smallest distance to this point will be returned.")]
        [Output("closestPt", "The point on the Plane with the smallest distance to the provided point.")]
        public static Point ClosestPoint(this Plane plane, Point point)
        {
            return point.Project(plane);
        }


        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        [Description("Finds the closest point on the Arc in relation to the provided Point.")]
        [Input("curve", "The Arc on which to find the closest point in relation to the provided point.")]
        [Input("point", "The reference point. The point on the Arc with the smallest distance to this point will be returned.")]
        [Input("tolerance", "Distance tolerance to be used in the method.", typeof(Length))]
        [Output("closestPt", "The point on the Arc with the smallest distance to the provided point.")]
        public static Point ClosestPoint(this Arc curve, Point point, double tolerance = Tolerance.Distance)
        {
            if (point.SquareDistance(curve.Centre()) <= tolerance * tolerance)
                return curve.StartPoint();

            Point center = curve.Centre();
            Point midPoint = curve.PointAtParameter(0.5);
            Plane p = curve.FitPlane();

            Point onCircle = center + (point.Project(p) - center).Normalise() * curve.Radius;
            double sqrd = midPoint.SquareDistance(curve.StartPoint());
            return midPoint.SquareDistance(onCircle) <= sqrd ? onCircle : onCircle.ClosestPoint(new List<Point> { curve.StartPoint(), curve.EndPoint() });
        }

        /***************************************************/

        [Description("Finds the closest point on the Circle in relation to the provided Point.")]
        [Input("circle", "The Circle on which to find the closest point in relation to the provided point.")]
        [Input("point", "The reference point. The point on the Circle with the smallest distance to this point will be returned.")]
        [Input("tolerance", "Distance tolerance to be used in the method.", typeof(Length))]
        [Output("closestPt", "The point on the Circle with the smallest distance to the provided point.")]
        public static Point ClosestPoint(this Circle circle, Point point, double tolerance = Tolerance.Distance)
        {
            Plane p = new Plane { Origin = circle.Centre, Normal = circle.Normal };
            Vector closestDir = point.Project(p) - circle.Centre;
            if (closestDir.SquareLength() <= tolerance * tolerance)
                return circle.StartPoint();
            else
                return circle.Centre + (point.Project(p) - circle.Centre).Normalise() * circle.Radius;
        }

        /***************************************************/

        [Description("Finds the closest point on the Ellipse in relation to the provided Point.")]
        [Input("ellipse", "The Ellipse on which to find the closest point in relation to the provided point.")]
        [Input("point", "The reference point. The point on the Ellipse with the smallest distance to this point will be returned.")]
        [Input("tolerance", "Distance tolerance to be used in the method.", typeof(Length))]
        [Output("closestPt", "The point on the Ellipse with the smallest distance to the provided point.")]
        public static Point ClosestPoint(this Ellipse ellipse, Point point, double tolerance = Tolerance.Distance)
        {
            if (ellipse.IsNull() || point.IsNull())
                return null;

            //Tranform the point to the local coordinates of the ellipse
            //After tranformation can see it as the ellipse centred in the origin with first axis long global x and second axis along global y
            Cartesian coordinateSystem = Create.CartesianCoordinateSystem(ellipse.Centre, ellipse.Axis1, ellipse.Axis2);
            TransformMatrix transform = Create.OrientationMatrixLocalToGlobal(coordinateSystem);
            Point ptLoc = point.Transform(transform);

            //Get the closest point in local coordinates of the ellipse
            Point closePt = ClosestPointEllipseLocal(ellipse.Radius1, ellipse.Radius2, ptLoc, tolerance);

            //Tranform back to global coordinates
            transform = Create.OrientationMatrixGlobalToLocal(coordinateSystem);
            return closePt.Transform(transform);
        }

        /***************************************************/

        [Description("Finds the closest point on the Line in relation to the provided Point.")]
        [Input("line", "The Line on which to find the closest point in relation to the provided point.")]
        [Input("point", "The reference point. The point on the Line with the smallest distance to this point will be returned.")]
        [Input("infiniteSegment", "If true, the returned point will be the closest point on the infinite line. If false, returned point will be the closest point on the finite line segment.")]
        [Output("closestPt", "The point on the Line with the smallest distance to the provided point.")]
        public static Point ClosestPoint(this Line line, Point point, bool infiniteSegment = false)
        {
            Vector dir = line.Direction();
            double t = dir * (point - line.Start);

            if (!infiniteSegment)
                t = Math.Min(Math.Max(t, 0), line.Length());

            return line.Start + t * dir;
        }

        /***************************************************/

        [Description("Finds the closest point on the PolyCurve in relation to the provided Point.")]
        [Input("curve", "The PolyCurve on which to find the closest point in relation to the provided point.")]
        [Input("point", "The reference point. The point on the PolyCurve with the smallest distance to this point will be returned.")]
        [Output("closestPt", "The point on the PolyCurve with the smallest distance to the provided point.")]
        public static Point ClosestPoint(this PolyCurve curve, Point point)
        {
            double minDist = double.PositiveInfinity;
            Point closest = null;
            List<ICurve> curves = curve.Curves;

            foreach (ICurve c in curve.SubParts())
            {
                Point cp = c.IClosestPoint(point);
                double dist = cp.Distance(point);
                if (dist < minDist)
                {
                    closest = cp;
                    minDist = dist;
                }
            }

            return closest;
        }

        /***************************************************/

        [Description("Finds the closest point on the Polyline in relation to the provided Point.")]
        [Input("curve", "The Polyline on which to find the closest point in relation to the provided point.")]
        [Input("point", "The reference point. The point on the Polyline with the smallest distance to this point will be returned.")]
        [Output("closestPt", "The point on the Polyline with the smallest distance to the provided point.")]
        public static Point ClosestPoint(this Polyline curve, Point point)
        {
            double minDist = double.PositiveInfinity;
            double sqDist = 0;
            Point closest = null;

            foreach (Line l in curve.SubParts())
            {
                Point cp = l.ClosestPoint(point);
                sqDist = cp.SquareDistance(point);
                if (sqDist < minDist)
                {
                    closest = cp;
                    minDist = sqDist;
                }
            }

            return closest;
        }

        /***************************************************/
        /**** Public Methods - Surfaces                 ****/
        /***************************************************/

        [Description("Finds the closest point on the PlanarSurface in relation to the provided Point.")]
        [Input("surface", "The PlanarSurface on which to find the closest point in relation to the provided point.")]
        [Input("point", "The reference point. The point on the PlanarSurface with the smallest distance to this point will be returned.")]
        [Output("closestPt", "The point on the PlanarSurface with the smallest distance to the provided point.")]
        public static Point ClosestPoint(this PlanarSurface surface, Point point)
        {
            Plane panelPlane = surface.FitPlane();
            List<Point> cPt = new List<Point> { point.Project(panelPlane) };

            foreach (ICurve outline in surface.InternalBoundaries)
            {
                if (outline.IIsContaining(cPt))
                    return outline.IClosestPoint(cPt[0]);
            }

            ICurve panelOutline = surface.ExternalBoundary;
            return (panelOutline.IIsContaining(cPt)) ? cPt[0] : panelOutline.IClosestPoint(cPt[0]);
        }

        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        [Description("Finds the closest point on the geometry in relation to the provided Point.")]
        [Input("geometry", "The geometry on which to find the closest point in relation to the provided point.")]
        [Input("point", "The reference point. The point on the geometry with the smallest distance to this point will be returned.")]
        [Output("closestPt", "The point on the geometry with the smallest distance to the provided point.")]
        public static Point IClosestPoint(this IGeometry geometry, Point point)
        {
            return ClosestPoint(geometry as dynamic, point);
        }


        /***************************************************/
        /**** Private Fallback Methods                  ****/
        /***************************************************/

        private static Point ClosestPoint(this IGeometry geometry, Point point)
        {
            Base.Compute.RecordError($"ClosestPoint is not implemented for IGeometry of type: {geometry.GetType().Name}.");
            return null;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        [Description("Gets the closest point on an ellipse centred around the global origin with first axis along global X and second axis along global Y.")]
        [Input("radX", "Radius along the global X-axis.")]
        [Input("radY", "Radius along the global Y axis.")]
        [Input("pt", "Point to find closest match on the ellipse for. The z-component of this point will be ignored as the ellipse evaluation is made in the global XY plane.")]
        [Input("tolerance", "Tolerance used for checking convergence of numeric search forthe closest point.")]
        [Output("pt", "The closest point on the ellipse in the global coordinates in relation to the proivided point.")]
        private static Point ClosestPointEllipseLocal(double radX, double radY, Point pt, double tolerance)
        {
            //Algorithm from:
            //https://blog.chatfield.io/simple-method-for-distance-to-ellipse/
            //https://github.com/0xfaded/ellipse_demo/issues/1

            //Treat as point is in upper quadrant
            double px = Math.Abs(pt.X);
            double py = Math.Abs(pt.Y);

            double a = radX;
            double b = radY;

            //Check ratio - if ratio more than a certain degree, treat as line
            double max = Math.Max(a, b);
            double min = Math.Min(a, b);
            double aspectRatio = max / min;

            //When h is equal to 1, the ellipse is a line
            //The algorithm below will not be able to handle to elongated ellipses, hence 
            //pointless to evaluate.
            if (min == 0 || aspectRatio > 1e20)
            {
                //Raise a warning when b is not exactly equal to 0
                if (b != 0)
                {
                    Base.Compute.RecordWarning("The aspect ratio of the provided Ellipse is too large to be able to accurately evaluate the Closest point. Point on line between vertex and co-vertex returned.");
                }

                Point closePt = new Line() { Start = new Point { X = a }, End = new Point { Y = b } }.ClosestPoint(new Point { X = px, Y = py });
                return new Point { X = pt.X > 0 ? closePt.X : -closePt.X, Y = pt.Y > 0 ? closePt.Y : -closePt.Y };
            }
            else if (max / min < 3000) //Ratio of less than 1:3000 - Use the trig free optimised version that runs quicker
            {
                double tx = 0.707;
                double ty = 0.707;

                double t;

                tolerance = tolerance * min / (max * 2);

                int c = 0;

                do
                {
                    double x = a * tx;
                    double y = b * ty;

                    double ex = (a * a - b * b) * (tx * tx * tx) / a;
                    double ey = (b * b - a * a) * (ty * ty * ty) / b;

                    double rx = x - ex;
                    double ry = y - ey;

                    double qx = px - ex;
                    double qy = py - ey;

                    double r = Math.Sqrt(ry * ry + rx * rx);
                    double q = Math.Sqrt(qy * qy + qx * qx);

                    tx = Math.Min(1, Math.Max(0, (qx * r / q + ex) / a));
                    ty = Math.Min(1, Math.Max(0, (qy * r / q + ey) / b));
                    t = Math.Sqrt(ty * ty + tx * tx);
                    tx /= t;
                    ty /= t;
                    c++;
                } while ((Math.Abs(1 - t) > tolerance || c < 5) && c < 100);

                //Get to correct quadrant
                if (pt.X < 0)
                    tx = -tx;
                if (pt.Y < 0)
                    ty = -ty;

                return new Point { X = a * tx, Y = b * ty };
            }
            else
            {
                //Method essentially the same as above, but not optimised to avoid trig functions
                //This works a lot better for some extreme elipses, with a ratio of radii of over 1:3000
                //This ofc also works well for elipses with a more reasonable aspect ratio, but as most elipses in practice will have a more reasonable
                //aspect ratio, less than 1:3000, worth keeping the above as it runs quicker, and will be used by most cases.
                double t = Math.PI / 4;
                double deltaT = 0;

                tolerance = tolerance * min / (max * 2);
                tolerance = Math.Max(tolerance, 1e-16); //Pointless to use a tolerance less than this when using floating points
                int c = 0;

                double x, y;

                do
                {
                    double tx = Math.Cos(t);
                    double ty = Math.Sin(t);
                    x = a * tx;
                    y = b * ty;

                    double ex = (a * a - b * b) * (tx * tx * tx) / a;
                    double ey = (b * b - a * a) * (ty * ty * ty) / b;

                    double rx = x - ex;
                    double ry = y - ey;

                    double qx = px - ex;
                    double qy = py - ey;

                    double r = Math.Sqrt(ry * ry + rx * rx);
                    double q = Math.Sqrt(qy * qy + qx * qx);

                    double deltaC = r * Math.Asin((rx * qy - ry * qx) / (r * q));
                    deltaT = deltaC / Math.Sqrt(a * a + b * b - x * x - y * y);

                    t += deltaT;
                    t = Math.Min(Math.PI / 2, Math.Max(0, t));
                    c++;
                } while ((Math.Abs(deltaT) > tolerance) && c < 20);

                //Get to correct quadrant
                if (pt.X < 0)
                    x = -x;
                if (pt.Y < 0)
                    y = -y;

                return new Point { X = x, Y = y };
            }
        }

        /***************************************************/
    }
}


