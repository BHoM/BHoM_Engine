/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
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

using System;
using BH.oM.Geometry;
using BH.oM.Dimensional;
using BH.oM.Analytical.Elements;
using BH.Engine.Geometry;
using BH.oM.Base;
using System.Collections.Generic;
using BH.oM.Base.Attributes;
using System.ComponentModel;

namespace BH.Engine.Facade
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns if lines are adjacent.")]
        [Input("curve1", "First crv to check adjacency for.")]
        [Input("curve2", "Second crv to check adjacency for.")]
        [Input("tolerance", "Tolerance to apply to adjacency check.")]
        [Output("bool", "True if provided lines are adjacent.")]
        public static bool IsAdjacent(this Line curve1, Line curve2, double tolerance = Tolerance.Distance)
        {
            if(curve1 == null || curve2 == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the adjacency of two lines if either line is null.");
                return false;
            }

            List<Point> testPts = new List<Point> { curve1.Start, curve1.End, curve2.Start, curve2.End };
            if (testPts.IsCollinear())
            {
                Point s1 = curve1.Start;
                Point e1 = curve1.End;
                Point s2 = curve2.Start;
                Point e2 = curve2.End;

                // Check if lines have matching end points
                if ((s1.Distance(s2) <= tolerance && e1.Distance(e2) <= tolerance) ||
                    (s1.Distance(e2) <= tolerance && e1.Distance(s2) <= tolerance))
                {
                    return true;
                }

                // Check that lines overlap using domain bounds
                foreach (Point pt in curve1.ControlPoints())
                {
                    if (pt.X <= Math.Max(s2.X, e2.X) && pt.X >= Math.Min(s2.X, e2.X) &&
                        pt.Y <= Math.Max(s2.Y, e2.Y) && pt.Y >= Math.Min(s2.Y, e2.Y) &&
                        pt.Z <= Math.Max(s2.Z, e2.Z) && pt.Z >= Math.Min(s2.Z, e2.Z) &&
                        pt.Distance(s2) > tolerance &&
                        pt.Distance(e2) > tolerance)
                    {
                        return true;
                    }
                }

                foreach (Point pt in curve2.ControlPoints())
                {
                    if (pt.X <= Math.Max(s1.X, e1.X) && pt.X >= Math.Min(s1.X, e1.X) &&
                        pt.Y <= Math.Max(s1.Y, e1.Y) && pt.Y >= Math.Min(s1.Y, e1.Y) &&
                        pt.Z <= Math.Max(s1.Z, e1.Z) && pt.Z >= Math.Min(s1.Z, e1.Z) &&
                        pt.Distance(s1) > tolerance &&
                        pt.Distance(e1) > tolerance)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /***************************************************/

        [Description("Returns if lines are adjacent.")]
        [Input("curve1", "First crv to check adjacency for.")]
        [Input("curve2", "Second crv to check adjacency for.")]
        [Input("tolerance", "Minimum overlap length to be considered adjacent (0 = curves only touching at endpoints are included).")]
        [Output("bool", "True if provided lines are adjacent.")]
        public static bool IsAdjacentApprox(this Line curve1, Line curve2, double tolerance = Tolerance.Distance)
        {
            if (curve1 == null || curve2 == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the approximate adjacency of two lines if either line is null.");
                return false;
            }

            bool isAdj = false;

            BH.oM.Base.Output<Point, Point> results = curve1.CurveProximity(curve2);
            double distance = results.Item1.Distance(results.Item2);
            if (distance < Tolerance.Distance)
            {
                List<Point> testPts = new List<Point> { curve1.Start, curve1.End, curve2.Start, curve2.End };
                if (testPts.IsCollinear())
                {
                    Vector scaleVec = new Vector { X = tolerance, Y = tolerance, Z = tolerance };

                    // Check that adjacency is not only at touching endpoints
                    if (results.Item1.Distance(curve1.Start) < tolerance)
                    {
                        Point tempPt = results.Item1.Translate(curve1.Direction().Normalise().Scale(Point.Origin, scaleVec));
                        if (tempPt.Distance(curve2.Start) < results.Item1.Distance(curve2.Start) || tempPt.Distance(curve2.End) < results.Item1.Distance(curve2.End))
                            isAdj = true;
                    }
                    else if (results.Item1.Distance(curve1.End) < tolerance)
                    {
                        Point tempPt = results.Item1.Translate(curve1.Direction().Reverse().Normalise().Scale(Point.Origin, scaleVec));
                        if (tempPt.Distance(curve2.Start) < results.Item1.Distance(curve2.Start) || tempPt.Distance(curve2.End) < results.Item1.Distance(curve2.End))
                            isAdj = true;
                    }
                    else isAdj = true;
                }
            }
            return isAdj;
        }

        /***************************************************/

        [Description("Returns if lines are adjacent.")]
        [Input("curve1", "First crv to check adjacency for.")]
        [Input("curve2", "Second crv to check adjacency for.")]
        [Input("tolerance", "Minimum overlap length to be considered adjacent (0 = curves only touching at endpoints are included).")]
        [Output("bool", "True if provided lines are adjacent.")]
        public static bool IsAdjacent(this Line curve1, Polyline curve2, double tolerance = Tolerance.Distance)
        {
            if (curve1 == null || curve2 == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the adjacency of two curves if either curve is null.");
                return false;
            }
            
            foreach (Line crv in curve2.SubParts())
                if (crv.IsAdjacent(curve2, tolerance))
                    return true;
            return false;
        }

        /***************************************************/

        [Description("Returns if lines are adjacent.")]
        [Input("curve1", "First crv to check adjacency for.")]
        [Input("curve2", "Second crv to check adjacency for.")]
        [Input("tolerance", "Minimum overlap length to be considered adjacent (0 = curves only touching at endpoints are included).")]
        [Output("bool", "True if provided lines are adjacent.")]
        public static bool IsAdjacent(this Polyline curve1, Line curve2, double tolerance = Tolerance.Distance)
        {
            if (curve1 == null || curve2 == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the adjacency of a PolyLine and a Curve if either is null.");
                return false;
            }
            
            foreach (Line crv in curve1.SubParts())
                if (crv.IsAdjacent(curve2, tolerance))
                    return true;
            return false;
        }

        /***************************************************/

        [Description("Returns if lines are adjacent.")]
        [Input("curve1", "First crv to check adjacency for.")]
        [Input("curve2", "Second crv to check adjacency for.")]
        [Input("tolerance", "Minimum overlap length to be considered adjacent (0 = curves only touching at endpoints are included).")]
        [Output("bool", "True if provided lines are adjacent.")]
        public static bool IsAdjacent(this Polyline curve1, Polyline curve2, double tolerance = Tolerance.Distance)
        {
            if (curve1 == null || curve2 == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the adjacency of two PolyLines if either PolyLine is null.");
                return false;
            }
            
            foreach (Line crv2 in curve2.SubParts())
               if (curve1.IsAdjacent(crv2, tolerance))
                   return true;
            return false;
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        [Description("Returns if curves are adjacent.")]
        [Input("curve1", "First curve to check adjacency for.")]
        [Input("curve2", "Second curve to check adjacency for.")]
        [Input("tolerance", "Minimum overlap length to be considered adjacent (0 = curves only touching at endpoints are included).")]
        [Output("bool", "True if provided curves are adjacent.")]
        public static bool IIsAdjacent(this ICurve curve1, ICurve curve2, double tolerance = Tolerance.Distance)
        {
            if (curve1 == null || curve2 == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the adjacency of two ICurves if either ICurve is null.");
                return false;
            }
            
            return IsAdjacent(new Polyline { ControlPoints = curve1.IControlPoints() } as dynamic, new Polyline { ControlPoints = curve2.IControlPoints() } as dynamic);
        }

        [Description("Returns if curves are adjacent.")]
        [Input("elem1", "First element to check adjacency for.")]
        [Input("elem2", "Second element to check adjacency for.")]
        [Input("tolerance", "Minimum overlap length to be considered adjacent (0 = curves only touching at endpoints are included).")]
        [Output("bool", "True if provided elements are adjacent.")]
        public static bool IIsAdjacent(this IElement1D elem1, IElement1D elem2, double tolerance = Tolerance.Distance)
        {
            if (elem1 == null || elem2 == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the adjacency of two IElement1Ds if either is null.");
                return false;
            }
            
            return IsAdjacent(elem1 as dynamic, elem2 as dynamic);
        }


        /***************************************************/
        /**** Private Fallback Methods                  ****/
        /***************************************************/

        private static bool IsAdjacent(this ICurve curve1, ICurve curve2, double tolerance = Tolerance.Distance)
        {
            if (curve1 == null || curve2 == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the adjacency of two curves if either is null.");
                return false;
            }

            Base.Compute.RecordWarning($"IsAdjacent is not implemented for a combination of {curve1.GetType().Name} and {curve2.GetType().Name}.");
            return false;
        }

        /***************************************************/

        private static bool IsAdjacent(this IElement1D elem1, IElement1D elem2, double tolerance = Tolerance.Distance)
        {
            if(elem1 == null || elem2 == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the adjacency of two IElement1Ds if either is null.");
                return false;
            }

            Base.Compute.RecordWarning($"IsAdjacent is not implemented for a combination of {elem1.GetType().Name} and {elem2.GetType().Name}.");
            return false;
        }

        /***************************************************/
        /**** Private Helper Methods                  ****/
        /***************************************************/

        [Description("Returns if elements are adjacent.")]
        [Input("edge1", "First element to check adjacency for.")]
        [Input("edge2", "Second element to check adjacency for.")]
        [Input("tolerance", "Minimum overlap length to be considered adjacent (0 = curves only touching at endpoints are included).")]
        [Output("bool", "True if provided lines are adjacent.")]
        private static bool IsAdjacent(this IEdge edge1, IEdge edge2, double tolerance = Tolerance.Distance)
        {
            if (edge1 == null || edge2 == null)
            {
                return false;
            }
            
            ICurve crv1 = edge1.Curve;
            ICurve crv2 = edge2.Curve;

            if (crv1 != null && crv2 != null)
                return Query.IIsAdjacent(crv1, crv2);
            else
                return false;
        }

        /***************************************************/

        [Description("Returns if elements are adjacent.")]
        [Input("edge1", "First element to check adjacency for.")]
        [Input("crv2", "Second element to check adjacency for.")]
        [Input("tolerance", "Minimum overlap length to be considered adjacent (0 = curves only touching at endpoints are included).")]
        [Output("bool", "True if provided lines are adjacent.")]
        private static bool IsAdjacent(this IEdge edge1, ICurve crv2, double tolerance = Tolerance.Distance)
        {
            if (edge1 == null || crv2 == null)
            {
                return false;
            }
            
            ICurve crv1 = edge1.Curve;

            if (crv1 != null && crv2 != null)
                return Query.IIsAdjacent(crv1, crv2);
            else
                return false;
        }

        /***************************************************/

        [Description("Returns if elements are adjacent.")]
        [Input("crv1", "Second element to check adjacency for.")]
        [Input("edge2", "First element to check adjacency for.")]
        [Input("tolerance", "Minimum overlap length to be considered adjacent (0 = curves only touching at endpoints are included).")]
        [Output("bool", "True if provided lines are adjacent.")]
        private static bool IsAdjacent(this ICurve crv1, IEdge edge2, double tolerance = Tolerance.Distance)
        {
            if (crv1 == null || edge2 == null)
            {
                return false;
            }
            
            ICurve crv2 = edge2.Curve;

            if (crv1 != null && crv2 != null)
                return Query.IIsAdjacent(crv1, crv2);
            else
                return false;
        }

        /***************************************************/
    }
}



