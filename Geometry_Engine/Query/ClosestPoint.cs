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
using BH.oM.Reflection.Attributes;
using System;
using System.Collections.Generic;

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

        [DeprecatedAttribute("2.3", "Replaced with method checking if point is arc.centre due to issue 896", null, "ClosestPoint")]
        public static Point ClosestPoint(this Arc arc, Point point)
        {
            return arc.ClosestPoint(point, Tolerance.Distance);
        }

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

        public static Point ClosestPoint(this Circle circle, Point point)
        {
            Plane p = new Plane { Origin = circle.Centre, Normal = circle.Normal };
            return circle.Centre + (point.Project(p) - circle.Centre).Normalise() * circle.Radius;
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

        [NotImplemented]
        public static Point ClosestPoint(this NurbsCurve curve, Point point)
        {
            throw new NotImplementedException();
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

        [NotImplemented]
        public static Point ClosestPoint(this Extrusion surface, Point point)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        [NotImplemented]
        public static Point ClosestPoint(this Loft surface, Point point)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        [NotImplemented]
        public static Point ClosestPoint(this NurbsSurface surface, Point point)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        [NotImplemented]
        public static Point ClosestPoint(this Pipe surface, Point point)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        [NotImplemented]
        public static Point ClosestPoint(this PolySurface surface, Point point)
        {
            throw new NotImplementedException();
        }


        /***************************************************/
        /**** Public Methods - Others                   ****/
        /***************************************************/

        [NotImplemented]
        public static Point ClosestPoint(this Mesh mesh, Point point)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        [NotImplemented]
        public static Point ClosestPoint(this CompositeGeometry group, Point point)
        {
            throw new NotImplementedException();
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static Point IClosestPoint(this IGeometry geometry, Point point)
        {
            return ClosestPoint(geometry as dynamic, point);
        }

        /***************************************************/
    }
}
