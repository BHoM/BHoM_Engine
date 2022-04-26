/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2022, the respective contributors. All rights reserved.
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
using System;
using System.Linq;
using System.Collections.Generic;
using BH.oM.Base.Attributes;
using BH.Engine.Base;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods - BoundingBox              ****/
        /***************************************************/

        public static bool IsContaining(this BoundingBox box1, BoundingBox box2, bool acceptOnEdge = true, double tolerance = Tolerance.Distance)
        {
            Point max1 = box1.Max;
            Point min1 = box1.Min;
            Point max2 = box2.Max;
            Point min2 = box2.Min;

            if (acceptOnEdge)
            {
                return (min2.X >= min1.X - tolerance && max2.X <= max1.X + tolerance &&
                        min2.Y >= min1.Y - tolerance && max2.Y <= max1.Y + tolerance &&
                        min2.Z >= min1.Z - tolerance && max2.Z <= max1.Z + tolerance);
            }
            else
            {
                return (min2.X > min1.X + tolerance && max2.X < max1.X - tolerance &&
                        min2.Y > min1.Y + tolerance && max2.Y < max1.Y - tolerance &&
                        min2.Z > min1.Z + tolerance && max2.Z < max1.Z - tolerance);
            }
        }

        /***************************************************/

        public static bool IsContaining(this BoundingBox box, Point pt, bool acceptOnEdge = true, double tolerance = Tolerance.Distance)
        {
            Point max = box.Max;
            Point min = box.Min;

            if (acceptOnEdge)
            {
                return (pt.X >= min.X - tolerance && pt.X <= max.X + tolerance &&
                        pt.Y >= min.Y - tolerance && pt.Y <= max.Y + tolerance &&
                        pt.Z >= min.Z - tolerance && pt.Z <= max.Z + tolerance);
            }
            else
            {
                return (pt.X > min.X + tolerance && pt.X < max.X - tolerance &&
                        pt.Y > min.Y + tolerance && pt.Y < max.Y - tolerance &&
                        pt.Z > min.Z + tolerance && pt.Z < max.Z - tolerance);
            }
        }

        /***************************************************/

        public static bool IsContaining(this BoundingBox box, List<Point> pts, bool acceptOnEdge = true, double tolerance = Tolerance.Distance)
        {
            if (pts.Count == 0)
                return false;


            foreach (Point pt in pts)
            {
                if (!box.IsContaining(pt, acceptOnEdge, tolerance))
                    return false;
            }

            return true;
        }

        /***************************************************/

        public static bool IsContaining(this BoundingBox box, IGeometry geometry, bool acceptOnEdge = true, double tolerance = Tolerance.Distance)
        {
            return box.IsContaining(geometry.IBounds(), acceptOnEdge, tolerance);
        }


        /***************************************************/
        /**** Public Methods - Curve / points           ****/
        /***************************************************/

        public static bool IsContaining(this Arc curve, List<Point> points, bool acceptOnEdge = true, double tolerance = Tolerance.Distance)
        {
            if (!curve.IsClosed(tolerance)) return false;
            Circle circle = new Circle { Centre = curve.Centre(), Radius = curve.Radius, Normal = curve.FitPlane().Normal };
            return circle.IsContaining(points, acceptOnEdge, tolerance);
        }

        /***************************************************/

        public static bool IsContaining(this Circle curve, List<Point> points, bool acceptOnEdge = true, double tolerance = Tolerance.Distance)
        {
            Plane p = new Plane { Origin = curve.Centre, Normal = curve.Normal };
            foreach (Point pt in points)
            {
                if (pt.Distance(p) > tolerance) return false;
                if ((acceptOnEdge && pt.Distance(curve.Centre) - curve.Radius - tolerance > 0) || (!acceptOnEdge && pt.Distance(curve.Centre) - curve.Radius + tolerance >= 0)) return false;
            }
            return true;
        }

        /***************************************************/

        public static bool IsContaining(this Line curve1, List<Point> points, bool acceptOnEdge = true, double tolerance = Tolerance.Distance)
        {
            return false;
        }

        /***************************************************/

        public static bool IsContaining(this Polyline curve, List<Point> points, bool acceptOnEdge = true, double tolerance = Tolerance.Distance)
        {
            // Todo:
            // project to 2D & rewrite methods to 2D to improve performance
            // - to be replaced with a general method for a nurbs curve?
            // - could be done with a ray instead of an infinite line!

            BoundingBox box = curve.Bounds();
            if (points.Any(x => !box.IsContaining(x, true, tolerance)))
                return false;

            if (curve.IsClosed(tolerance))
            {
                Plane p = curve.FitPlane(tolerance);
                double sqTol = tolerance * tolerance;

                if (p == null)
                {
                    if (acceptOnEdge)
                    {
                        foreach (Point pt in points)
                        {
                            if (curve.ClosestPoint(pt).SquareDistance(pt) > sqTol)
                                return false;
                        }
                        return true;
                    }
                    else
                        return false;
                }
                else
                {
                    List<Line> subParts = curve.SubParts();
                    List<Vector> edgeDirections = subParts.Select(c => c.Direction()).ToList();
                    foreach (Point pt in points)
                    {
                        Point pPt = pt.Project(p);
                        if (pPt.SquareDistance(pt) <= sqTol)
                        {
                            Point end = p.Origin.DeepClone();
                            Vector direction = (end - pPt).Normalise();
                            while (direction.SquareLength() <= 0.5 || edgeDirections.Any(e => 1 - Math.Abs(e.DotProduct(direction)) <= Tolerance.Angle))
                            {
                                end = p.Origin.Translate(Create.RandomVectorInPlane(p, true));
                                direction = (end - pPt).Normalise();
                            }

                            Line ray = new Line { Start = pPt, End = end };
                            ray.Infinite = true;
                            List<Point> intersects = new List<Point>();
                            List<Point> extraIntersects = new List<Point>();

                            Func<double, double, double> ToFactor = (t, n) => (1 - t * t) / (1 - n * n);

                            Line current = subParts[1];
                            double prevTolFactor = ToFactor(subParts[0].Direction().DotProduct(direction), current.Direction().DotProduct(direction));

                            for (int i = 1; i < subParts.Count + 1; i++)
                            {
                                Line next = subParts[(i + 1) % subParts.Count];

                                double nextTolFactor = ToFactor(next.Direction().DotProduct(direction), current.Direction().DotProduct(direction));

                                Point iPt = current.LineIntersection(ray, false, tolerance);
                                if (iPt != null)
                                {
                                    double signedAngle = direction.SignedAngle(current.Direction(), p.Normal);
                                    if ((current.Start.SquareDistance(iPt) <= sqTol * prevTolFactor)) // Will we get a point on the previous line
                                    {
                                        if (signedAngle > Tolerance.Angle)
                                            intersects.Add(iPt);
                                        else
                                            extraIntersects.Add(iPt);
                                    }
                                    else if ((current.End.SquareDistance(iPt) <= sqTol * nextTolFactor))  // Will we get a point on the next line
                                    {
                                        if (signedAngle < -Tolerance.Angle)
                                            intersects.Add(iPt);
                                        else
                                            extraIntersects.Add(iPt);
                                    }
                                    else
                                        intersects.Add(iPt);
                                }
                                prevTolFactor = 1 / nextTolFactor;
                                current = next;
                            }

                            if (intersects.Count == 0)
                                return false;

                            if ((pPt.ClosestPoint(intersects.Union(extraIntersects)).SquareDistance(pPt) <= sqTol))
                            {
                                if (acceptOnEdge)
                                    continue;
                                else
                                    return false;
                            }

                            intersects.Add(pPt);
                            intersects = intersects.SortCollinear(tolerance);
                            for (int j = 0; j < intersects.Count; j++)
                            {
                                if (j % 2 == 0 && intersects[j] == pPt)
                                    return false;
                            }
                        }
                        else
                            return false;
                    }
                    return true;
                }
            }
            return false;
        }

        /***************************************************/

        public static bool IsContaining(this PolyCurve curve, List<Point> points, bool acceptOnEdge = true, double tolerance = Tolerance.Distance)
        {
            // Todo:
            // - to be replaced with a general method for a nurbs curve?
            // - this is very problematic for edge cases (cutting line going through a sharp corner, to be superseded?

            BoundingBox box = curve.Bounds();
            if (points.Any(x => !box.IsContaining(x, true, tolerance)))
                return false;

            if (!curve.IsClosed(tolerance))
                return false;

            if (curve.Curves.Count == 1 && curve.Curves[0] is Circle)
                return IsContaining(curve.Curves[0] as Circle, points, acceptOnEdge, tolerance);

            Plane p = curve.FitPlane(tolerance);
            double sqTol = tolerance * tolerance;

            if (p == null)
            {
                if (acceptOnEdge)
                {
                    foreach (Point pt in points)
                    {
                        if (curve.ClosestPoint(pt).SquareDistance(pt) > sqTol)
                            return false;
                    }
                    return true;
                }
                else
                    return false;
            }

            List<ICurve> subParts = curve.SubParts();
            List<Vector> edgeDirections = subParts.Where(s => s is Line).Select(c => (c as Line).Direction()).ToList();
            foreach (Point pt in points)
            {
                Point pPt = pt.Project(p);
                if (pPt.SquareDistance(pt) > sqTol) // not on the same plane
                    return false;

                Point end = p.Origin.DeepClone();   // Avrage of control points
                Vector direction = (end - pPt).Normalise();     // Gets a line cutting through the curves and the point
                while (direction.SquareLength() <= 0.5 || edgeDirections.Any(e => 1 - Math.Abs(e.DotProduct(direction)) <= Tolerance.Angle)) // not zeroa or parallel to edges
                {
                    end = p.Origin.Translate(Create.RandomVectorInPlane(p, true));
                    direction = (end - pPt).Normalise();
                }

                Line ray = new Line { Start = pPt, End = end };
                ray.Infinite = true;
                List<Point> intersects = new List<Point>();
                List<Point> extraIntersects = new List<Point>();

                foreach (ICurve subPart in subParts)
                {
                    List<Point> iPts = subPart.ILineIntersections(ray, false, tolerance);   // LineIntersection ignores the `false`
                    foreach (Point iPt in iPts)
                    {
                        double signedAngle = direction.SignedAngle(subPart.ITangentAtPoint(iPt, tolerance), p.Normal);
                        if ((subPart.IStartPoint().SquareDistance(iPt) <= sqTol))   // Keep intersections from beeing counted twice?
                        {
                            if (signedAngle >= -Tolerance.Angle)    // tangent is to the left of the direction
                                intersects.Add(iPt);
                            else
                                extraIntersects.Add(iPt);
                        }
                        else if ((subPart.IEndPoint().SquareDistance(iPt) <= sqTol))
                        {
                            if (signedAngle <= Tolerance.Angle)     // tangent is to the rigth of the direction
                                intersects.Add(iPt);
                            else
                                extraIntersects.Add(iPt);
                        }
                        else if (Math.Abs(signedAngle) <= Tolerance.Angle)  // They are parallel
                            extraIntersects.Add(iPt);
                        else
                            intersects.Add(iPt);
                    }
                }

                if (intersects.Count == 0)  // did not intersect the curve (strange)
                    return false;

                if ((pPt.ClosestPoint(intersects.Union(extraIntersects)).SquareDistance(pPt) <= sqTol)) // if any intersection point is the point
                {
                    if (acceptOnEdge)
                        continue;
                    else
                        return false;
                }

                intersects.Add(pPt);
                intersects = intersects.SortCollinear(tolerance);
                for (int j = 0; j < intersects.Count; j++)  // Even indecies on a colinerar sort is outside the region
                {
                    if (j % 2 == 0 && intersects[j] == pPt)
                        return false;
                }
            }
            return true;
        }

        /***************************************************/
        /**** Public Methods - Curve / curve            ****/
        /***************************************************/

        public static bool IsContaining(this Arc curve1, ICurve curve2, bool acceptOnEdge = true, double tolerance = Tolerance.Distance)
        {
            if (!curve1.IsClosed(tolerance)) return false;
            Circle circle = new Circle { Centre = curve1.Centre(), Radius = curve1.Radius, Normal = curve1.FitPlane().Normal };
            return circle.IsContaining(curve2);
        }

        /***************************************************/

        public static bool IsContaining(this Circle curve1, ICurve curve2, bool acceptOnEdge = true, double tolerance = Tolerance.Distance)
        {
            if (curve2 is Line || curve2 is Polyline) return curve1.IsContaining(curve2.IControlPoints(), acceptOnEdge, tolerance);

            List<Point> iPts = curve1.ICurveIntersections(curve2, tolerance);
            if (!acceptOnEdge && iPts.Count > 0) return false;

            List<double> cParams = new List<double> { 0, 1 };
            foreach (Point iPt in iPts)
            {
                cParams.Add(curve2.IParameterAtPoint(iPt, tolerance));
            }
            cParams.Sort();

            for (int i = 0; i < cParams.Count - 1; i++)
            {
                iPts.Add(curve2.IPointAtParameter((cParams[i] + cParams[i + 1]) * 0.5));
            }
            return curve1.IsContaining(iPts, acceptOnEdge, tolerance);
        }

        /***************************************************/

        public static bool IsContaining(this Line curve1, ICurve curve2, bool acceptOnEdge = true, double tolerance = Tolerance.Distance)
        {
            return false;
        }

        /***************************************************/

        public static bool IsContaining(this Polyline curve1, ICurve curve2, bool acceptOnEdge = true, double tolerance = Tolerance.Distance)
        {
            if (!curve1.IsClosed(tolerance)) return false;

            List<Point> iPts = curve1.ICurveIntersections(curve2, tolerance);
            if (!acceptOnEdge && iPts.Count > 0) return false;

            List<double> cParams = new List<double> { 0, 1 };
            iPts = iPts.CullDuplicates(tolerance);
            foreach (Point iPt in iPts)
            {
                cParams.Add(curve2.IParameterAtPoint(iPt, tolerance));
            }
            cParams.Sort();

            for (int i = 0; i < cParams.Count - 1; i++)
            {
                iPts.Add(curve2.IPointAtParameter((cParams[i] + cParams[i + 1]) * 0.5));
            }
            return curve1.IsContaining(iPts, acceptOnEdge, tolerance);
        }

        /***************************************************/

        public static bool IsContaining(this PolyCurve curve1, ICurve curve2, bool acceptOnEdge = true, double tolerance = Tolerance.Distance)
        {
            if (!curve1.IsClosed(tolerance)) return false;

            List<Point> iPts = curve1.ICurveIntersections(curve2, tolerance);
            if (!acceptOnEdge && iPts.Count > 0) return false;

            List<double> cParams = new List<double> { 0, 1 };
            foreach (Point iPt in iPts)
            {
                cParams.Add(curve2.IParameterAtPoint(iPt, tolerance));
            }
            cParams.Sort();

            for (int i = 0; i < cParams.Count - 1; i++)
            {
                iPts.Add(curve2.IPointAtParameter((cParams[i] + cParams[i + 1]) * 0.5));
            }
            return curve1.IsContaining(iPts, acceptOnEdge, tolerance);
        }

        /***************************************************/
        /**** Public Methods - Cuboid                   ****/
        /***************************************************/

        public static bool IsContaining(this Cuboid cuboid, IGeometry geometry, bool acceptOnEdge = true, double tolerance = Tolerance.Distance)
        {
            TransformMatrix transform = Create.OrientationMatrixLocalToGlobal(cuboid.CoordinateSystem);
            IGeometry globalGeo = geometry.ITransform(transform);
            BoundingBox geoBox = globalGeo.IBounds();
            BoundingBox cuboidBox = cuboid.BoundingBox();
            return cuboidBox.IsContaining(geoBox, acceptOnEdge, tolerance);
        }

        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static bool IIsContaining(this ICurve curve, List<Point> points, bool acceptOnEdge = true, double tolerance = Tolerance.Distance)
        {
            return IsContaining(curve as dynamic, points, acceptOnEdge, tolerance);
        }

        /***************************************************/

        public static bool IIsContaining(this ICurve curve1, ICurve curve2, bool acceptOnEdge = true, double tolerance = Tolerance.Distance)
        {
            return IsContaining(curve1 as dynamic, curve2 as dynamic, acceptOnEdge, tolerance);
        }


        /***************************************************/
        /**** Private Fallback Methods                  ****/
        /***************************************************/

        private static bool IsContaining(this ICurve curve, List<Point> points, bool acceptOnEdge = true, double tolerance = Tolerance.Distance)
        {
            throw new NotImplementedException($"IsContaining is not implemented for ICurves of type: {curve.GetType().Name}.");
        }

        /***************************************************/

        private static bool IsContaining(this ICurve curve1, ICurve curve2, bool acceptOnEdge = true, double tolerance = Tolerance.Distance)
        {
            throw new NotImplementedException($"IsContaining is not implemented for a combination of {curve1.GetType().Name} and {curve2.GetType().Name}.");
        }

        /***************************************************/
    }
}

