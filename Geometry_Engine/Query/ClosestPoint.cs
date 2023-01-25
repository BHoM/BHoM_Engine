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

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods - Vectors                  ****/
        /***************************************************/

        public static Point ClosestPoint(this IEnumerable<Point> cloud, Point point)
        {
            double minDist = Double.PositiveInfinity;
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

        public static Point ClosestPoint(this Point point, IEnumerable<Point> points)
        {
            return points.ClosestPoint(point);
        }

        /***************************************************/

        public static Point ClosestPoint(this Vector vector, Point point)
        {
            return null;
        }

        /***************************************************/

        public static Point ClosestPoint(this Plane plane, Point point)
        {
            return point.Project(plane);
        }


        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

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

        public static Point ClosestPoint(this Ellipse ellipse, Point point, double tolerance = Tolerance.Distance)
        {
            //Tranform the point to the local coordinates of the ellipse
            //After tranformation can see it as the ellipse centre in the origin with first axis long global x and second axis along global y
            Cartesian coordinateSystem = Create.CartesianCoordinateSystem(ellipse.Centre, ellipse.Axis1, ellipse.Axis2);
            TransformMatrix transform = Create.OrientationMatrixLocalToGlobal(coordinateSystem);
            Point ptLoc = point.Transform(transform);

            //Algorithm from:
            //https://blog.chatfield.io/simple-method-for-distance-to-ellipse/
            //https://github.com/0xfaded/ellipse_demo/issues/1

            //Treat as point is in upper quadrant
            double px = Math.Abs(ptLoc.X);
            double py = Math.Abs(ptLoc.Y);

            double a = ellipse.Radius1;
            double b = ellipse.Radius2;

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
                    Base.Compute.RecordWarning("The aspect ratio of the provided Ellipse is to large to be able to accurately evaluate the Closest point. Point on line between vertex and co-vertex returned.");
                }

                Point closePt = new Line() { Start = new Point { X = a }, End = new Point { Y = b } }.ClosestPoint(new Point { X = px, Y = py });
                ptLoc = new Point { X = ptLoc.X > 0 ? closePt.X : -closePt.X, Y = ptLoc.Y > 0 ? closePt.Y : -closePt.Y };
            }
            else if (max / min < 5000) //Ratio of less than 1:5000 - Use the trig free optimised version that runs quicker
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
                if (ptLoc.X < 0)
                    tx = -tx;
                if (ptLoc.Y < 0)
                    ty = -ty;

                ptLoc = new Point { X = a * tx, Y = b * ty };
            }
            else
            {
                //Method essentially the same as above, but not optimised to avoid trig functions
                //This works a lot better for some extreme elipses, with a ratio of radii of over 1:5000
                //This ofc also works well for elipses with a more reasonable aspect ratio, but as most elipses in practice will have a more reasonable
                //aspect ratio, less than 1:5000, worth keeping the above as it runs quicker, and will be used by most cases.
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
                if (ptLoc.X < 0)
                    x = -x;
                if (ptLoc.Y < 0)
                    y = -y;

                ptLoc = new Point { X = x, Y = y };
            }
            //Tranform back to global coordinates
            transform = Create.OrientationMatrixGlobalToLocal(coordinateSystem);
            return ptLoc.Transform(transform);
        }

        /***************************************************/

        public static Point ClosestPoint(this Line line, Point point, bool infiniteSegment = false)
        {
            Vector dir = line.Direction();
            double t = dir * (point - line.Start);

            if (!infiniteSegment)
                t = Math.Min(Math.Max(t, 0), line.Length());

            return line.Start + t * dir;
        }

        /***************************************************/

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
    }
}


