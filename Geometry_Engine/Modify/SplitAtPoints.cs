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
using BH.oM.Geometry.CoordinateSystem;
using System;
using System.Linq;
using System.Collections.Generic;
using BH.oM.Base.Attributes;
using BH.Engine.Base;
using System.ComponentModel;
using BH.oM.Quantities.Attributes;

namespace BH.Engine.Geometry
{
    public static partial class Modify
    {
        /***************************************************/
        /****          Split curve at points            ****/
        /***************************************************/

        [Description("Splits the curve at the provided Points and returns a list of curves corresponding to the segments. Points with a distance to the curve larger than the tolerance will be ignored.")]
        [Input("arc", "The curve to split.")]
        [Input("points", "The set of points to split the curve at. Method will ignore points that have a distance to the curve that is larger than the provided tolerance.")]
        [Input("tolerance", "Distance tolerance to be used in the method.", typeof(Length))]
        [Output("split", "The segments of the curve corresponding to the original curve split at the position of the provided points.")]
        public static List<Arc> SplitAtPoints(this Arc arc, List<Point> points, double tolerance = Tolerance.Distance)
        {
            if (!points.Any())
                return new List<Arc> { arc.DeepClone() };

            List<Arc> result = new List<Arc>();
            List<Point> cPts = new List<Point>();

            double sqTol = tolerance * tolerance;
            Plane arcPlane = arc.FitPlane();
            //Collect all points on the circle of the arc (points not on arc filtered out lower down)
            //Doing this half manually (Not calling IsOnCurve) due to wanting the projected points anyway, and this is more efficient.
            //Angles checked to be in domain while looping through the points further down the algorithm
            foreach (Point point in points)
            {
                Point projPt = point.Project(arcPlane);
                double projSqDist = projPt.SquareDistance(point);
                if (projSqDist > sqTol)
                    continue;   //Not in plane

                double inPlaneDist = Math.Abs(projPt.Distance(arcPlane.Origin) - arc.Radius);
                if (inPlaneDist > tolerance)
                    continue;   //Not within tolerance distance in plane

                if (projSqDist + inPlaneDist * inPlaneDist > sqTol) //Distance to the circle of the full arc (c^2=a^2+b^2 where a is out of plane distance and b is in plane distance) 
                    continue;   //Not on circle

                cPts.Add(projPt);   //Store projected point rather than original for more accurate angle calculation
            }

            //If any points on the full circle corresponding to the arc
            if (cPts.Count > 0)
            {
                if (arc.Radius * 2 < tolerance) //Angle tolerance calculation below fails if this is true, as it means computing ASin of a value larger than 1. Good check to make anyway, as a arc this small is a singularity within the tolerance.
                {
                    Base.Compute.RecordError("Arc has a radius smaller than half of the tolerance provided. All points on the arc deemed the same within tolerance. Unable to split arc at points.");
                    return null;
                }

                //Allowable difference in angle for the points on the arc to be tolerance distance away from each other
                double angleTolerance = 2 * Math.Asin(tolerance / 2 / arc.Radius); //This is the equivalent of straight line distance between two points rather than distance along the arc

                if (Math.Abs(arc.Angle()) > Math.PI * 2 + angleTolerance)
                {
                    //Record warning for invalid overlapping arcs. The method could still provide some potentially usefull outputs, so can still let it run, but with warning given.
                    Base.Compute.RecordWarning("Arc is overlapping itself (angle > PI*2). Points on the overlap will only split the arc once at points of overlap.");
                }

                //Arc currently only really valid if this start is smaller than end
                //An arc with a StartAngle larger than EndAngle gives negative lengths and have issues in other methods.
                //Handling currently invalid case in this method to atempt to future proof
                bool startSmaller = arc.StartAngle < arc.EndAngle;

                //Setup angle domain parameters
                double minAngle, maxAngle;
                if (startSmaller)
                {
                    minAngle = arc.StartAngle;
                    maxAngle = arc.EndAngle;
                }
                else
                {
                    minAngle = arc.EndAngle;
                    maxAngle = arc.StartAngle;
                }

                List<double> angles = new List<double>();
                foreach (Point p in cPts)
                {
                    //Calculate angles in relation to the coordinate system X axis for the point (this is the meassure used by the Angle class)
                    double angle = arc.CoordinateSystem.X.SignedAngle((p - arc.CoordinateSystem.Origin), arc.CoordinateSystem.Z);

                    //Ensure angle is in the domain of the arc
                    if (angle < minAngle)
                    {
                        do
                        {
                            if (minAngle - angle < angleTolerance)  //Within tolerance of start point.
                            {
                                angle = double.MaxValue;    //Set to max value to be caught by check in relation to max angle
                                break;  //Break out the while loop
                            }

                            angle += 2 * Math.PI;   //As long as angle is smaller than the minimum (StartAngle) increase by full lap
                        } while (angle < minAngle);

                        if (angle < maxAngle && maxAngle - angle > angleTolerance)   //Inside the domain and not to close to end point
                        {
                            angles.Add(angle);  //Store angle
                        }
                    }
                    else if (angle > maxAngle)
                    {
                        do
                        {
                            if (angle - maxAngle < angleTolerance)  //Within tolerance of end point
                            {
                                angle = double.MinValue;    //Set to min value to be caught by check in relation to min angle
                                break;  //Break out the while loop
                            }
                            angle -= 2 * Math.PI;   //As long as angle is larger than the maximum (EndAngle) decrease by full lap
                        } while (angle > maxAngle);

                        if (angle > minAngle && angle - minAngle > angleTolerance)   //Inside the domain and not close to startpoint
                        {
                            angles.Add(angle);  //Store angle
                        }
                    }
                    else if (angle - minAngle > angleTolerance && maxAngle - angle > angleTolerance)    //In the domain - check not to close to start/end points
                    {
                        angles.Add(angle);  //Store angle
                    }
                }

                if (angles.Count == 0)  //No angles in range
                    return new List<Arc> { arc.DeepClone() };

                angles.Sort();  //Equal to sort along curve

                //Remove duplicates within tolerance
                //Both more efficient and accurate to handles this with the angles compared to removing duplicate input points
                //Doing it this way ensures points on the curve are not within tolerance, rather than the input points, that could be
                //Outside tolerance in relation to each other, but ending up on the same point on the angle
                //The below does the equivalent of a DBScan (which is what the CullDuplicates for points relies on)
                List<List<double>> angleGroups = new List<List<double>>() { new List<double> { angles[0] } };
                int groupIndex = 0;
                for (int i = 1; i < angles.Count; i++)
                {
                    if (angles[i] - angleGroups[groupIndex].Last() < angleTolerance)    //Angles are sorted, so only need to compare to the one last added
                        angleGroups[groupIndex].Add(angles[i]);
                    else
                    {
                        angleGroups.Add(new List<double>() { angles[i] });
                        groupIndex++;
                    }
                }
                angles = angleGroups.Select(x => x.Average()).ToList();

                angles.Insert(0, minAngle);  //Insert minAngle (StartAngle) as first step
                angles.Add(maxAngle);  //Add maxAngle (EndAngle) to the end

                if (!startSmaller)  //As mentioned above, Arcs currently invalid if false, but handling here for completeness
                    angles.Reverse();

                for (int i = 1; i < angles.Count; i++)
                {
                    result.Add(new Arc
                    {
                        CoordinateSystem = arc.CoordinateSystem,
                        Radius = arc.Radius,
                        StartAngle = angles[i - 1],
                        EndAngle = angles[i]
                    });
                }
            }
            else
                result.Add(arc.DeepClone());
            return result;
        }

        /***************************************************/

        [Description("Splits the curve at the provided Points and returns a list of curves corresponding to the segments. Points with a distance to the curve larger than the tolerance will be ignored.")]
        [Input("circle", "The curve to split.")]
        [Input("points", "The set of points to split the curve at. Method will ignore points that have a distance to the curve that is larger than the provided tolerance.")]
        [Input("tolerance", "Distance tolerance to be used in the method.", typeof(Length))]
        [Output("split", "The segments of the curve corresponding to the original curve split at the position of the provided points.")]
        public static List<ICurve> SplitAtPoints(this Circle circle, List<Point> points, double tolerance = Tolerance.Distance)
        {

            List<ICurve> result = new List<ICurve>();
            List<Point> cPts = new List<Point>();

            foreach (Point point in points)
            {
                if (point.IsOnCurve(circle, tolerance))
                    cPts.Add(point);
            }

            cPts = cPts.CullDuplicates(tolerance);
            cPts = cPts.SortAlongCurve(circle);

            Point cirCen = circle.Centre;
            Vector startVector = circle.StartPoint() - circle.Centre;
            Vector normal = circle.Normal;
            Double rotAng;
            Cartesian system = new Cartesian(circle.Centre, startVector.Normalise(), (circle.PointAtParameter(0.25) - cirCen).Normalise(), normal);
            Arc mainArc = new Arc();
            Arc tmpArc = new Arc();

            mainArc.CoordinateSystem = system;
            mainArc.Radius = circle.Radius;

            Vector stVec;
            Vector enVec;
            Double enAng;

            if (cPts.Count == 0)
            {
                result.Add(circle);
                return result;
            }

            if (cPts.Count == 1)
            {
                tmpArc = mainArc;
                stVec = cPts[0] - circle.Centre;
                rotAng = ((2 * Math.PI + startVector.SignedAngle(stVec, normal)) % (2 * Math.PI));
                enAng = 2 * Math.PI;

                tmpArc = tmpArc.Rotate(cirCen, normal, rotAng);
                tmpArc.StartAngle = 0;
                tmpArc.EndAngle = enAng;
                result.Add(tmpArc.DeepClone());
            }
            else
            {
                for (int i = 0; i < cPts.Count; i++)
                {
                    tmpArc = mainArc;
                    stVec = cPts[i] - circle.Centre;
                    enVec = cPts[(i + 1) % cPts.Count] - circle.Centre;
                    rotAng = ((2 * Math.PI + startVector.SignedAngle(stVec, normal)) % (2 * Math.PI));
                    enAng = ((2 * Math.PI + stVec.SignedAngle(enVec, normal)) % (2 * Math.PI));

                    tmpArc = tmpArc.Rotate(cirCen, normal, rotAng);
                    tmpArc.StartAngle = 0;
                    tmpArc.EndAngle = enAng;
                    result.Add(tmpArc.DeepClone());
                }
            }
            return result;
        }

        /***************************************************/

        [Description("Splits the curve at the provided Points and returns a list of curves corresponding to the segments. Points with a distance to the curve larger than the tolerance will be ignored.")]
        [Input("line", "The curve to split.")]
        [Input("points", "The set of points to split the curve at. Method will ignore points that have a distance to the curve that is larger than the provided tolerance.")]
        [Input("tolerance", "Distance tolerance to be used in the method.", typeof(Length))]
        [Output("split", "The segments of the curve corresponding to the original curve split at the position of the provided points.")]
        public static List<Line> SplitAtPoints(this Line line, List<Point> points, double tolerance = Tolerance.Distance)
        {
            List<Line> result = new List<Line>();
            List<Point> cPts = new List<Point> { line.Start, line.End };
            double sqTol = tolerance * tolerance;

            foreach (Point point in points)
            {
                if (point.SquareDistance(line.Start) > sqTol && point.SquareDistance(line.End) > sqTol && point.SquareDistance(line) <= sqTol)
                    cPts.Add(point);
            }

            if (cPts.Count > 2)
            {
                cPts = cPts.CullDuplicates(tolerance);
                cPts = cPts.SortAlongCurve(line, tolerance);
                for (int i = 0; i < cPts.Count - 1; i++)
                {
                    result.Add(new Line { Start = cPts[i], End = cPts[i + 1] });
                }
            }
            else
                result.Add(line.DeepClone());

            return result;
        }

        /***************************************************/

        [Description("Splits the curve at the provided Points and returns a list of curves corresponding to the segments. Points with a distance to the curve larger than the tolerance will be ignored.")]
        [Input("curve", "The curve to split.")]
        [Input("points", "The set of points to split the curve at. Method will ignore points that have a distance to the curve that is larger than the provided tolerance.")]
        [Input("tolerance", "Distance tolerance to be used in the method.", typeof(Length))]
        [Output("split", "The segments of the curve corresponding to the original curve split at the position of the provided points.")]
        public static List<ICurve> SplitAtPoints(this PolyCurve curve, List<Point> points, double tolerance = Tolerance.Distance)
        {
            if (curve == null || points == null)
                return null;

            if (points.Count == 0)
                return new List<ICurve> { curve.DeepClone() };

            List<ICurve> result = new List<ICurve>();

            List<ICurve> subParts = curve.ISubParts().ToList(); //Subparts of PolyCurve
            List<Point> nonDuplicatePoints = points.CullDuplicates(tolerance);

            PolyCurve prev = null;  //Polycurve to collect parts across segments
            double sqTol = tolerance * tolerance;
            for (int k = 0; k < subParts.Count; k++)
            {
                ICurve crv = subParts[k];

                List<ICurve> split = crv.ISplitAtPoints(nonDuplicatePoints, tolerance); //Split with all point. Methods called filters out points on the particular curve

                if (split.Count == 0)   //Should never happen
                {
                    Base.Compute.RecordWarning($"Unable to split a segment of type {crv.GetType()} in a Polycurve. Segment ignored in returned split curve.");
                    continue;
                }

                //Check if split at start or end of current segment by checking if the start and/or end point of the first resp last segment is within tolerance of the split points.
                Point stPt = split.First().IStartPoint();
                Point enPt = split.Last().IEndPoint();
                bool splitAtStart = nonDuplicatePoints.Any(x => x.SquareDistance(stPt) < sqTol);
                bool splitAtEnd = nonDuplicatePoints.Any(x => x.SquareDistance(enPt) < sqTol);

                for (int i = 0; i < split.Count; i++)
                {
                    if (i == 0 && splitAtStart) //If first split segment and split at start
                    {
                        if (prev != null)   //Check if previous exist and if so, add it
                        {
                            if (prev.Curves.Count == 1)     //If prev is single segment curve
                                result.Add(prev.Curves[0]); //Only add the segment rather than the wrapping PolyCurve
                            else                            //Prev more than single segment
                                result.Add(prev);           //Add prev to return list
                        }

                        prev = null;    //Prev handled -> set to null
                    }

                    if (i != split.Count - 1 || splitAtEnd)   //If not last split or if split at end. Only need special case for last segment if _not_ splitting at endpoint
                    {
                        if (prev != null)   //Prev set - only true if this is the first split and not splitting at start
                        {
                            prev.Curves.Add(split[i]);  //Add full split segment to prev
                            result.Add(prev);   //Add prev to output list
                            prev = null;    //Prev handled, set to null. If splitting at end, nothing to bring over
                        }
                        else    //Prev not set
                        {
                            result.Add(split[i]);   //Add full split segment to output list
                        }
                    }
                    else    //Last split and not splitting at end
                    {
                        if (prev != null)   //If prev is set - should only be true for single split segment not split at start
                            prev.Curves.Add(split[i]);   //Add current split segment to prev
                        else
                            prev = new PolyCurve { Curves = new List<ICurve> { split[i] } }; //If prev is not set, set it with current split segment added
                    }
                }

            }

            if (prev != null)   //If prev is set
            {
                if (curve.StartPoint().SquareDistance(curve.EndPoint()) < sqTol && result.Count != 0) //If curve is closed (Not using curve.IsClosed() here due to it checking if closed and not disjointed. Here we only want to check if endpoints match for the full curve. This way disjointed curves will work as well)
                {
                    prev.Curves.AddRange(result[0].ISubParts());    //Apply all of the parts of the first return to prev
                    result[0] = prev;                               //Replace first part with prev
                }
                else //If not, simply add the prev to the return list
                {
                    if (prev.Curves.Count == 1)     //If prev is single segment curve
                        result.Add(prev.Curves[0]); //Only add the segment rather than the wrapping PolyCurve
                    else                            //Prev more than single segment
                        result.Add(prev);           //Add prev to return list
                }
            }

            return result;
        }

        /***************************************************/

        [Description("Splits the curve at the provided Points and returns a list of curves corresponding to the segments. Points with a distance to the curve larger than the tolerance will be ignored.")]
        [Input("curve", "The curve to split.")]
        [Input("points", "The set of points to split the curve at. Method will ignore points that have a distance to the curve that is larger than the provided tolerance.")]
        [Input("tolerance", "Distance tolerance to be used in the method.", typeof(Length))]
        [Output("split", "The segments of the curve corresponding to the original curve split at the position of the provided points.")]
        public static List<Polyline> SplitAtPoints(this Polyline curve, List<Point> points, double tolerance = Tolerance.Distance)
        {
            if (points.Count == 0)
                return new List<Polyline> { curve.DeepClone() };

            List<Polyline> result = new List<Polyline>();
            List<Line> segments = curve.SubParts();
            Polyline section = new Polyline { ControlPoints = new List<Point>() };
            bool closed = curve.IsClosed(tolerance);
            double sqTol = tolerance * tolerance;

            for (int i = 0; i < segments.Count; i++)
            {
                Line l = segments[i];
                bool intStart = false;
                List<Point> iPts = new List<Point>();

                foreach (Point point in points)
                {
                    if (point.SquareDistance(l.Start) <= sqTol)
                    {
                        intStart = true;
                        if (i == 0)
                            closed = false;
                    }
                    else if (point.SquareDistance(l.End) > sqTol && point.SquareDistance(l) <= sqTol)
                        iPts.Add(point);
                }

                section.ControlPoints.Add(l.Start);
                if (intStart && section.ControlPoints.Count > 1)
                {
                    result.Add(section);
                    section = new Polyline { ControlPoints = new List<Point> { l.Start.DeepClone() } };
                }

                if (iPts.Count > 0)
                {
                    iPts = iPts.CullDuplicates(tolerance);
                    iPts = iPts.SortAlongCurve(l, tolerance);
                    foreach (Point iPt in iPts)
                    {
                        section.ControlPoints.Add(iPt);
                        result.Add(section);
                        section = new Polyline { ControlPoints = new List<Point> { iPt } };
                    }
                }
            }
            section.ControlPoints.Add(curve.ControlPoints.Last());
            result.Add(section);

            if (result.Count > 1 && closed)
            {
                result[0].ControlPoints.RemoveAt(0);
                result[0].ControlPoints.InsertRange(0, result.Last().ControlPoints);
                result.RemoveAt(result.Count - 1);
            }

            return result;
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        [Description("Splits the curve at the provided Points and returns a list of curves corresponding to the segments. Points with a distance to the curve larger than the tolerance will be ignored.")]
        [Input("curve", "The curve to split.")]
        [Input("points", "The set of points to split the curve at. Method will ignore points that have a distance to the curve that is larger than the provided tolerance.")]
        [Input("tolerance", "Distance tolerance to be used in the method.", typeof(Length))]
        [Output("split", "The segments of the curve corresponding to the original curve split at the position of the provided points.")]
        public static List<ICurve> ISplitAtPoints(this ICurve curve, List<Point> points, double tolerance = Tolerance.Distance)
        {
            List<ICurve> result = new List<ICurve>();
            System.Collections.IList splitCurves = SplitAtPoints(curve as dynamic, points, tolerance);
            for (int i = 0; i < splitCurves.Count; i++)
            {
                result.Add(splitCurves[i] as ICurve);
            }

            return result;
        }

        /***************************************************/
        /**** Private Fallback Methods                  ****/
        /***************************************************/

        private static List<ICurve> SplitAtPoints(this ICurve curve, List<Point> points, double tolerance = Tolerance.Distance)
        {
            Base.Compute.RecordError($"SplitAtPoints is not implemented for ICurves of type: {curve.GetType().Name}.");
            return null;
        }

        /***************************************************/
    }
}


