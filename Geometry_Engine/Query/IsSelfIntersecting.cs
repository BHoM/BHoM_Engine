/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
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
using System.Linq;
using System.ComponentModel;
using BH.oM.Quantities.Attributes;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        [Description("Checks if any part of the the Curve is intersecting with any other part of the curve. A Line is by definition never self intersecting, hence this method always returns false.")]
        [Input("curve", "The curve to check for self intersection. A for a Line, this method always returns false.")]
        [Input("tolerance", "Distance tolerance to be used by the method. For a Line this in unused.", (typeof(Length)))]
        [Output("isIntersecting", "Returns true if the Curve is self intersecting. For a Line this always returns false.")]
        public static bool IsSelfIntersecting(this Line curve, double tolerance = Tolerance.Distance)
        {
            return false;
        }

        /***************************************************/

        [Description("Checks if any part of the the Curve is intersecting with any other part of the curve. For an Arc this is true if the angle range is larger than 2 PI, i.e. if the curve is overlapping itself.")]
        [Input("curve", "The curve to check for self intersection.")]
        [Input("tolerance", "Distance tolerance to be used by the method. For an Arc this in unused.", (typeof(Length)))]
        [Output("isIntersecting", "Returns true if the Curve is self intersecting.")]
        public static bool IsSelfIntersecting(this Arc curve, double tolerance = Tolerance.Distance)
        {
            return Math.Abs(curve.StartAngle - curve.EndAngle) > 2 * Math.PI;
        }

        /***************************************************/

        [Description("Checks if any part of the the Curve is intersecting with any other part of the curve. A Circle is by definition never self intersecting, hence this method always returns false.")]
        [Input("curve", "The curve to check for self intersection. A for a Circle, this method always returns false.")]
        [Input("tolerance", "Distance tolerance to be used by the method. For a Circle this in unused.", (typeof(Length)))]
        [Output("isIntersecting", "Returns true if the Curve is self intersecting. For a Circle this always returns false.")]
        public static bool IsSelfIntersecting(this Circle curve, double tolerance = Tolerance.Distance)
        {
            return false;
        }

        /***************************************************/

        [Description("Checks if any part of the the Curve is intersecting with any other part of the curve. An Ellipse is by definition never self intersecting, hence this method always returns false.")]
        [Input("curve", "The curve to check for self intersection. A for an Ellipse, this method always returns false.")]
        [Input("tolerance", "Distance tolerance to be used by the method. For an Ellipse this in unused.", (typeof(Length)))]
        [Output("isIntersecting", "Returns true if the Curve is self intersecting. For an Ellipse this always returns false.")]
        public static bool IsSelfIntersecting(this Ellipse curve, double tolerance = Tolerance.Distance)
        {
            return false;
        }

        /***************************************************/

        [Description("Checks if any part of the the Polyline is intersecting with any other part of the curve.")]
        [Input("curve", "The Polyline to check for self intersection.")]
        [Input("tolerance", "Distance tolerance to be used by the method.", (typeof(Length)))]
        [Output("isIntersecting", "Returns true if the Curve is self intersecting.")]
        public static bool IsSelfIntersecting(this Polyline curve, double tolerance = Tolerance.Distance)
        {
            double sqTolerance = tolerance * tolerance;

            List<Line> curves = curve.SubParts().Where(x => x.SquareLength() > sqTolerance).ToList();
            if (curves.Count < 2)
                return false;

            List<BoundingBox> boxes = curves.Select(x => x.Bounds()).ToList();
            bool closed = curve.IsClosed(tolerance);
            for (int i = 0; i < curves.Count - 1; i++)
            {
                for (int j = i + 1; j < curves.Count; j++)
                {
                    if (boxes[i].IsInRange(boxes[j], tolerance))
                    {
                        foreach (Point intPt in curves[i].LineIntersections(curves[j], false, tolerance))
                        {
                            if (j == i + 1 && intPt.SquareDistance(curves[i].End) <= sqTolerance && intPt.SquareDistance(curves[j].Start) <= sqTolerance)
                                continue;
                            else if (closed && i == 0 && j == curves.Count - 1 && intPt.SquareDistance(curves[i].Start) <= sqTolerance && intPt.SquareDistance(curves[j].End) <= sqTolerance)
                                continue;
                            else
                                return true;
                        }
                    }
                }
            }

            return false;
        }

        /***************************************************/

        [Description("Checks if any part of the the PolyCurve is intersecting with any other part of the curve.")]
        [Input("curve", "The PolyCurve to check for self intersection.")]
        [Input("tolerance", "Distance tolerance to be used by the method.", (typeof(Length)))]
        [Output("isIntersecting", "Returns true if the Curve is self intersecting.")]
        public static bool IsSelfIntersecting(this PolyCurve curve, double tolerance = Tolerance.Distance)
        {
            List<ICurve> curves = curve.SubParts().Where(x => x.ILength() > tolerance).ToList();
            if (curves.Count == 0)
                return false;

            if (curves.Count == 1)
                return curves[0].IIsSelfIntersecting(tolerance);

            List<BoundingBox> boxes = curves.Select(x => x.IBounds()).ToList();
            bool closed = curve.IsClosed(tolerance);
            double sqTolerance = tolerance * tolerance;
            for (int i = 0; i < curves.Count - 1; i++)
            {
                for (int j = i + 1; j < curves.Count; j++)
                {
                    if (boxes[i].IsInRange(boxes[j]))
                    {
                        foreach (Point intPt in curves[i].ICurveIntersections(curves[j], tolerance))
                        {
                            if (j == i + 1 && intPt.SquareDistance(curves[i].IEndPoint()) <= sqTolerance && intPt.SquareDistance(curves[j].IStartPoint()) <= sqTolerance)
                                continue;
                            else if (closed && i == 0 && j == curves.Count - 1 && intPt.SquareDistance(curves[i].IStartPoint()) <= sqTolerance && intPt.SquareDistance(curves[j].IEndPoint()) <= sqTolerance)
                                continue;
                            else
                                return true;
                        }
                    }
                }
            }

            return false;
        }

        /***************************************************/

        [Description("Checks if any part of the the Polygon is intersecting with any other part of the curve. A Polygon is checked to not be self intersecting at creation, hence this method always returns false.")]
        [Input("curve", "The curve to check for self intersection. A for a Polygon, this method always returns false.")]
        [Input("tolerance", "Distance tolerance to be used by the method. For a Polygon this in unused.", (typeof(Length)))]
        [Output("isIntersecting", "Returns true if the Polygon is self intersecting. For a Polygon this always returns false.")]
        public static bool IsSelfIntersecting(this Polygon curve, double tolerance = Tolerance.Distance)
        {
            return false;
        }

        /***************************************************/

        [Description("Checks if any part of the the BoundaryCurve is intersecting with any other part of the curve. A BoundaryCurve is checked to not be self intersecting at creation, hence this method always returns false.")]
        [Input("curve", "The curve to check for self intersection. A for a Polygon, this method always returns false.")]
        [Input("tolerance", "Distance tolerance to be used by the method. For a Polygon this in unused.", (typeof(Length)))]
        [Output("isIntersecting", "Returns true if the BoundaryCurve is self intersecting. For a BoundaryCurve this always returns false.")]
        public static bool IsSelfIntersecting(this BoundaryCurve curve, double tolerance = Tolerance.Distance)
        {
            return false;
        }

        /***************************************************/
        /**** Public Methods = Interfaces               ****/
        /***************************************************/

        [Description("Checks if any part of the the ICurve is intersecting with any other part of the curve.")]
        [Input("curve", "The ICurve to check for self intersection.")]
        [Input("tolerance", "Distance tolerance to be used by the method.", (typeof(Length)))]
        [Output("isIntersecting", "Returns true if the Curve is self intersecting.")]
        public static bool IIsSelfIntersecting(this ICurve curve, double tolerance = Tolerance.Distance)
        {
            return IsSelfIntersecting(curve as dynamic, tolerance);
        }


        /***************************************************/
        /**** Private Fallback Methods                  ****/
        /***************************************************/

        private static bool IsSelfIntersecting(this ICurve curve, double tolerance = Tolerance.Distance)
        {
            throw new NotImplementedException($"IsSelfIntersecting is not implemented for ICurves of type: {curve.GetType().Name}.");
        }

        /***************************************************/
    }
}




