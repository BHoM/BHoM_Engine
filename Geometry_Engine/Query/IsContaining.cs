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
using System.ComponentModel;
using BH.oM.Quantities.Attributes;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods - BoundingBox              ****/
        /***************************************************/

        [Description("Checks if a BoundingBox if fully contained within another BoundingBox within tolerance.")]
        [Input("box1", "The outer BoundingBox, e.g. the BoundingBox to check if it is containing the second BoundingBox.")]
        [Input("box2", "The inner BoundingBox, e.g. that BoundingBox to check if it is contained in the first BoundingBox.")]
        [Input("acceptOnEdge", "If true, faces of the box overlapping within tolerance are accepted. If false, the second box needs to be strictly smaller in all dimensions compared to the first box.")]
        [Input("tolerance", "Tolerance to be used to check if faces of the boxes overlap.\n" +
               "If accept on edge is true, the minimum values of box2 needs to be larger or equal to the minimum values of box1 - tolerance and the maximum values of box2 needs to be smaller or equal to the maximum values of box1 + tolerance.\n" +
               "If accept on edge is false, the minimum values of box2 needs to be larger than the minimum values of box1 + tolerance and the maximum values of box2 needs to be smaller than the maximum values of box1 - tolerance.", typeof(Length))]
        [Output("isContaining", "Returns true if the second box is fully inside the first box within tolerance.")]
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

        [Description("Checks if a Point is inside a BoundingBox within tolerance.")]
        [Input("box", "The BoundingBox to check for point containment.")]
        [Input("pt", "The Point to check if it is inside the box.")]
        [Input("acceptOnEdge", "If true, the Point is deemed to be contained if it is on the edge of the BoundingBox. If false, the point needs to be fully inside the box, not allowing it to touch any of the faces within tolerance.")]
        [Input("tolerance", "Tolerance to be used to check if the point is on the edge of the box. A point within tolerance distance away from one of the faces of the box is deemed to be on the edge.", typeof(Length))]
        [Output("isContaining", "Returns true if the Point is inside the BoundingBox.")]
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

        [Description("Checks if a collection of Points are all inside a Bounding box within tolerance. If a single point is outside the box, the method will return false.")]
        [Input("box", "The BoundingBox to check for point containment.")]
        [Input("pts", "The points to check all are inside the box.")]
        [Input("acceptOnEdge", "If true, a point is deemed to be contained if it is on the edge of the box. If false, the point needs to be fully inside the box, not allowing it to touch any of the faces within tolerance.")]
        [Input("tolerance", "Tolerance to be used to check if a point is on the edge of the box. A point within tolerance distance away from one of the faces of the box is deemed to be on the edge.", typeof(Length))]
        [Output("isContaining", "Returns true if all of the Points are inside the BoundingBox.")]
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

        [Description("Checks if the geometry is inside the BoundingBox within tolerance by checking if the Bounds of the geometry is inside the provided BoundingBox.")]
        [Input("box", "The BoundingBox to check for point containment.")]
        [Input("geometry", "The Geometry to check if it is inside the box.")]
        [Input("acceptOnEdge", "If true, the Geometry is deemed to be contained if it is touching the edge of the BoundingBox. If false, the geometry needs to be fully inside the box, not allowing it to touch any of the faces within tolerance.")]
        [Input("tolerance", "Tolerance to be used to check if the geometry is on the edge of the box. A geometry within tolerance distance away from one of the faces of the box is deemed to be on the edge.", typeof(Length))]
        [Output("isContaining", "Returns true if the geometry is inside the BoundingBox.")]
        public static bool IsContaining(this BoundingBox box, IGeometry geometry, bool acceptOnEdge = true, double tolerance = Tolerance.Distance)
        {
            return box.IsContaining(geometry.IBounds(), acceptOnEdge, tolerance);
        }


        /***************************************************/
        /**** Public Methods - Curve / points           ****/
        /***************************************************/

        [Description("Checks if the colleciton of Points are all contained within the curve. If a single Point is outside the curve, the method will return false. Points not in the plane of the curve are deemed to be outside.")]
        [Input("curve", "The Arc to check if it is containing all of the provided points. If the Arc is not closed, i.e. not a Circle, the method will return false.")]
        [Input("points", "The points to check if they are all contained within the curve. If a single point is outside the curve or not in the plane of the curve the method will return false.")]
        [Input("acceptOnEdge", "If true, points that are within the tolerance distance away from the curve are demmed to be inside it. If false, only points that are inside and not within tolerance distance away from the curve are deemed to be inside.")]
        [Input("tolerance", "Distance tolerance to be used in the method. Points are deemed to be on the edge of the curve if they are within this distance from the curve.", typeof(Length))]
        [Output("isContaining", "Returns true if all of the provided points are inside the curve.")]
        public static bool IsContaining(this Arc curve, List<Point> points, bool acceptOnEdge = true, double tolerance = Tolerance.Distance)
        {
            if (!curve.IsClosed(tolerance)) return false;
            Circle circle = new Circle { Centre = curve.Centre(), Radius = curve.Radius, Normal = curve.FitPlane().Normal };
            return circle.IsContaining(points, acceptOnEdge, tolerance);
        }

        /***************************************************/

        [Description("Checks if the colleciton of Points are all contained within the curve. If a single Point is outside the curve, the method will return false. Points not in the plane of the curve are deemed to be outside.")]
        [Input("curve", "The Circle to check if it is containing all of the provided points.")]
        [Input("points", "The points to check if they are all contained within the curve. If a single point is outside the curve or not in the plane of the curve the method will return false.")]
        [Input("acceptOnEdge", "If true, points that are within the tolerance distance away from the curve are demmed to be inside it. If false, only points that are inside and not within tolerance distance away from the curve are deemed to be inside.")]
        [Input("tolerance", "Distance tolerance to be used in the method. Points are deemed to be on the edge of the curve if they are within this distance from the curve.", typeof(Length))]
        [Output("isContaining", "Returns true if all of the provided points are inside the curve.")]
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

        [PreviousInputNames("curve","curve1")]
        [Description("Checks if the colleciton of Points are all contained within the curve. For a Line this will always return false.")]
        [Input("curve", "The Line to check. For a Line this methods will always return false.")]
        [Input("points", "The points to check. For a line this method will always return false.")]
        [Input("acceptOnEdge", "Not used by this method. A Line is not an enclosed region, hence even points that are on the curve will be deemed to be outside.")]
        [Input("tolerance", "Not used by this method. A Line is not an enclosed region, hence even points that are on the curve will be deemed to be outside.", typeof(Length))]
        [Output("isContaining", "Returns true if all of the provided points are inside the curve. For a Line this always returns false.")]
        public static bool IsContaining(this Line curve, List<Point> points, bool acceptOnEdge = true, double tolerance = Tolerance.Distance)
        {
            return false;
        }

        /***************************************************/

        [Description("Checks if the colleciton of Points are all contained within the curve. If a single Point is outside the curve, the method will return false. Points not in the plane of the curve are deemed to be outside.")]
        [Input("curve", "The Polyline to check if it is containing all of the provided points. If the Polyline is not closed or planar, the method will return false.")]
        [Input("points", "The points to check if they are all contained within the curve. If a single point is outside the curve or not in the plane of the curve the method will return false.")]
        [Input("acceptOnEdge", "If true and the curve is closed, points that are within the tolerance distance away from the curve are demmed to be inside it. If false, only points that are inside and not within tolerance distance away from the curve are deemed to be inside.")]
        [Input("tolerance", "Distance tolerance to be used in the method. Points are deemed to be on the edge of the curve if they are within this distance from the curve.", typeof(Length))]
        [Output("isContaining", "Returns true if all of the provided points are inside the curve.")]
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
                            Point end = p.Origin;
                            Vector direction = (end - pPt).Normalise();
                            while (direction.SquareLength() <= 0.5 || edgeDirections.Any(e => 1 - Math.Abs(e.DotProduct(direction)) <= Tolerance.Angle))
                            {
                                direction = Create.RandomVectorInPlane(p, true);
                            }

                            end = pPt.Translate(direction);
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

        [Description("Checks if the colleciton of Points are all contained within the curve. If a single Point is outside the curve, the method will return false. Points not in the plane of the curve are deemed to be outside.")]
        [Input("curve", "The PolyCurve to check if it is containing all of the provided points. If the PolyCurve is not closed or planar, the method will return false.")]
        [Input("points", "The points to check if they are all contained within the curve. If a single point is outside the curve or not in the plane of the curve the method will return false.")]
        [Input("acceptOnEdge", "If true and the curve is closed, points that are within the tolerance distance away from the curve are demmed to be inside it. If false, only points that are inside and not within tolerance distance away from the curve are deemed to be inside.")]
        [Input("tolerance", "Distance tolerance to be used in the method. Points are deemed to be on the edge of the curve if they are within this distance from the curve.", typeof(Length))]
        [Output("isContaining", "Returns true if all of the provided points are inside the curve.")]
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

                double distance = pPt.Distance(curve);
                if (distance < tolerance)
                {
                    if (acceptOnEdge)
                        continue;
                    else
                        return false;
                }

                Point end = p.Origin;  // Avrage of control points
                Vector direction = (end - pPt).Normalise();     // Gets a line cutting through the curves and the point
                while (direction.SquareLength() <= 0.5 || edgeDirections.Any(e => 1 - Math.Abs(e.DotProduct(direction)) <= Tolerance.Angle)) // not zeroa or parallel to edges
                {
                    direction = Create.RandomVectorInPlane(p, true);
                }

                end = pPt.Translate(direction);
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

        [Description("Checks if a curve is contained within another the curve. The curves need to be co-planar for the method to be able to return true.")]
        [Input("curve1", "The Arc to check if it is containing the second curve. Needs to be coplanar with the second curve. If the Arc is not closed, i.e. not a Circle, the method will return false.")]
        [Input("curve2", "The curve to check if it is contained within the the first curve. Needs to be coplanar with the first curve.")]
        [Input("acceptOnEdge", "If true, a the inner curve is allowed to touch the outer curve within tolerance. If false, all points of the second curve needs to be fully inside the first curve and are not allowed to be within tolerance distance from the first curve.")]
        [Input("tolerance", "Distance tolerance to be used in the method. Points on the second are deemed to be on the edge of the curve if they are within this distance from the curve.", typeof(Length))]
        [Output("isContaining", "Returns true if the second curve is inside the first curve.")]
        public static bool IsContaining(this Arc curve1, ICurve curve2, bool acceptOnEdge = true, double tolerance = Tolerance.Distance)
        {
            if (!curve1.IsClosed(tolerance)) return false;
            Circle circle = new Circle { Centre = curve1.Centre(), Radius = curve1.Radius, Normal = curve1.FitPlane().Normal };
            return circle.IsContaining(curve2, acceptOnEdge, tolerance);
        }

        /***************************************************/

        [Description("Checks if a curve is contained within another the curve. The curves need to be co-planar for the method to be able to return true.")]
        [Input("curve1", "The Circle to check if it is containing the second curve. Needs to be coplanar with the second curve.")]
        [Input("curve2", "The curve to check if it is contained within the the first curve. Needs to be coplanar with the first curve.")]
        [Input("acceptOnEdge", "If true, a the inner curve is allowed to touch the outer curve within tolerance. If false, all points of the second curve needs to be fully inside the first curve and are not allowed to be within tolerance distance from the first curve.")]
        [Input("tolerance", "Distance tolerance to be used in the method. Points on the second are deemed to be on the edge of the curve if they are within this distance from the curve.", typeof(Length))]
        [Output("isContaining", "Returns true if the second curve is inside the first curve.")]
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

        [Description("Checks if curve is contained within the Curve. For a Line this will always return false.")]
        [Input("curve1", "The Line to check if it is containing the other curve. For a Line this methods will always return false.")]
        [Input("curve2", "The curve to check if it inside the first curve. For a line this method will always return false.")]
        [Input("acceptOnEdge", "Not used by this method. A Line is not an enclosed region, hence even a overlapping line will be deemed to be outside.")]
        [Input("tolerance", "Not used by this method. A Line is not an enclosed region, hence even a overlapping line will be deemed to be outside.", typeof(Length))]
        [Output("isContaining", "Returns true if the second curve is inside the first curve. For a Line this always returns false.")]

        public static bool IsContaining(this Line curve1, ICurve curve2, bool acceptOnEdge = true, double tolerance = Tolerance.Distance)
        {
            return false;
        }

        /***************************************************/

        [Description("Checks if a curve is contained within another the curve. The curves need to be co-planar for the method to be able to return true.")]
        [Input("curve1", "The Polyline to check if it is containing the second curve. Needs to be closed and coplanar with the second curve.")]
        [Input("curve2", "The curve to check if it is contained within the the first curve. Needs to be coplanar with the first curve.")]
        [Input("acceptOnEdge", "If true, a the inner curve is allowed to touch the outer curve within tolerance. If false, all points of the second curve needs to be fully inside the first curve and are not allowed to be within tolerance distance from the first curve.")]
        [Input("tolerance", "Distance tolerance to be used in the method. Points on the second are deemed to be on the edge of the curve if they are within this distance from the curve.", typeof(Length))]
        [Output("isContaining", "Returns true if the second curve is inside the first curve.")]
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

        [Description("Checks if a curve is contained within another the curve. The curves need to be co-planar for the method to be able to return true.")]
        [Input("curve1", "The PolyCurve to check if it is containing the second curve. Needs to be closed and coplanar with the second curve.")]
        [Input("curve2", "The curve to check if it is contained within the the first curve. Needs to be coplanar with the first curve.")]
        [Input("acceptOnEdge", "If true, a the inner curve is allowed to touch the outer curve within tolerance. If false, all points of the second curve needs to be fully inside the first curve and are not allowed to be within tolerance distance from the first curve.")]
        [Input("tolerance", "Distance tolerance to be used in the method. Points on the second are deemed to be on the edge of the curve if they are within this distance from the curve.", typeof(Length))]
        [Output("isContaining", "Returns true if the second curve is inside the first curve.")]
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

        [Description("Checks if the geometry is inside the Cuboid within tolerance by checking if the Bounds of the geometry in the coordinate system of the Cuboid is inside the provided Cuboid.")]
        [Input("cuboid", "The Cuboid to check for point containment.")]
        [Input("geometry", "The Geometry to check if it is inside the Cuboid.")]
        [Input("acceptOnEdge", "If true, the Geometry is deemed to be contained if it is touching the edge of the Cuboid. If false, the geometry needs to be fully inside the Cuboid, not allowing it to touch any of the faces within tolerance.")]
        [Input("tolerance", "Tolerance to be used to check if the geometry is on the edge of the Cuboid. A geometry within tolerance distance away from one of the faces of the Cuboid is deemed to be on the edge.", typeof(Length))]
        [Output("isContaining", "Returns true if the geometry is inside the Cuboid.")]
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

        [Description("Checks if the colleciton of Points are all contained within the curve. If a single Point is outside the curve, the method will return false. Points not in the plane of the curve are deemed to be outside.")]
        [Input("curve", "The PolyCurve to check if it is containing all of the provided points. If the PolyCurve is not closed or planar, the method will return false.")]
        [Input("points", "The points to check if they are all contained within the curve. If a single point is outside the curve or not in the plane of the curve the method will return false.")]
        [Input("acceptOnEdge", "If true and the curve is closed, points that are within the tolerance distance away from the curve are demmed to be inside it. If false, only points that are inside and not within tolerance distance away from the curve are deemed to be inside.")]
        [Input("tolerance", "Distance tolerance to be used in the method. Points are deemed to be on the edge of the curve if they are within this distance from the curve.", typeof(Length))]
        [Output("isContaining", "Returns true if all of the provided points are inside the curve.")]
        public static bool IIsContaining(this ICurve curve, List<Point> points, bool acceptOnEdge = true, double tolerance = Tolerance.Distance)
        {
            return IsContaining(curve as dynamic, points, acceptOnEdge, tolerance);
        }

        /***************************************************/

        [Description("Checks if a curve is contained within another the curve. The curves need to be co-planar for the method to be able to return true.")]
        [Input("curve1", "The curve to check if it is containing the second curve. Needs to be closed and coplanar with the second curve.")]
        [Input("curve2", "The curve to check if it is contained within the the first curve. Needs to be coplanar with the first curve.")]
        [Input("acceptOnEdge", "If true, a the inner curve is allowed to touch the outer curve within tolerance. If false, all points of the second curve needs to be fully inside the first curve and are not allowed to be within tolerance distance from the first curve.")]
        [Input("tolerance", "Distance tolerance to be used in the method. Points on the second are deemed to be on the edge of the curve if they are within this distance from the curve.", typeof(Length))]
        [Output("isContaining", "Returns true if the second curve is inside the first curve.")]
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

