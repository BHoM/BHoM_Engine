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

using System;
using System.Collections.Generic;
using BH.oM.Geometry;
using System.ComponentModel;
using BH.oM.Base.Attributes;

namespace BH.Engine.Geometry
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Collapses a PolyCurve to a polyline where curved segments are split into Polyline segments for computations. Where their partitioning is based on both their curvature and size, ergo; shorter segment less partitions, very curved segment more partitions")]
        [Input("curve", "The curve to collapse")]
        [Input("relativeAngleTolerance", "relativeAngleTolerance is the angleTolerance for a unit circle, the tolerance value will vary by: toleranceScaleFactor over curvature")]
        [Input("toleranceScaleFactor", "toleranceScaleFactor is the value that decides how much the angleTolerance varies due to curvature")]
        [Input("maxDivisionsPerSegment", "The maximum number of segment each sub-curve can be broken into")]
        [Input("scale", "the radius of a baseline circle for the angle tolerance, smaller circles will have less segments and bigger more. Default of 0 means the program will determine an appropriate radius for the PolyCurve")]
        [Output("C", "A polyline approximating the provided curve")]
        public static Polyline CollapseToScaledPolyline(this PolyCurve curve, double relativeAngleTolerance = 0.04, double toleranceScaleFactor = 0.001, int maxDivisionsPerSegment = 200, double scale = 0)
        {
            if (relativeAngleTolerance <= 0)
                return null;
            if (toleranceScaleFactor < 0)
                toleranceScaleFactor = 0;
            else if (toleranceScaleFactor > 0.01)
                Base.Compute.RecordWarning("High values for toleranceGrowth can be unstable and are not recommended");
            if (scale <= 0)
            {
                BoundingBox box = curve.IBounds();

                Point min = box.Min;
                Point max = box.Max;
                double totalWidth = max.X - min.X;
                double totalHeight = max.Y - min.Y;

                scale = Math.Max(totalHeight, totalWidth) / 2;
            }

            double scaledTolerance = scale * toleranceScaleFactor;

            relativeAngleTolerance -= scaledTolerance;

            List<Polyline> list = new List<Polyline>();
            foreach (ICurve c in curve.Curves)
            {
                if (c is PolyCurve)
                    list.Add(CollapseToScaledPolyline(c as PolyCurve, relativeAngleTolerance, toleranceScaleFactor, maxDivisionsPerSegment, scale));

                list.Add(Divide(c as dynamic, scaledTolerance, relativeAngleTolerance, maxDivisionsPerSegment));
            }
            return (Geometry.Compute.Join(list))[0];
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static Polyline Divide(this Line line, double angleTolerance, double baseTolerance, int maxDivisionsPerSegment = 200)
        {
            return new Polyline() { ControlPoints = new List<Point>() { line.Start, line.End } };
        }

        /***************************************************/

        private static Polyline Divide(this Polyline line, double angleTolerance, double baseTolerance, int maxDivisionsPerSegment = 200)
        {
            return new Polyline() { ControlPoints = line.ControlPoints };
        }

        /***************************************************/

        private static Polyline Divide(this Arc arc, double angleTolerance, double baseTolerance, int maxDivisionsPerSegment = 200)
        {
            return arc.CollapseToPolyline(Soften(arc.Radius, angleTolerance, baseTolerance), maxDivisionsPerSegment);
        }

        /***************************************************/

        private static Polyline Divide(this Circle circle, double angleTolerance, double baseTolerance, int maxDivisionsPerSegment = 200)
        {
            double factor = Math.Min(Math.PI * 0.25, Math.Max(Math.PI / maxDivisionsPerSegment, Soften(circle.Radius, angleTolerance, baseTolerance)));
            int result = System.Convert.ToInt32(Math.Ceiling(Math.PI / factor));
            result += result < 4 ? 4 : 0;
            return circle.CollapseToPolyline(0, result - (result % 4));
        }

        /***************************************************/

        private static double Soften(double r, double angleTolerance, double baseTolerance)
        {
            return (angleTolerance / r) + baseTolerance;     
        }

        /***************************************************/

    }
}



