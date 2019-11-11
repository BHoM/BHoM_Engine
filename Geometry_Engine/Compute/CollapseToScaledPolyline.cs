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

using System;
using System.Collections.Generic;
using BH.oM.Geometry;
using System.ComponentModel;
using BH.oM.Reflection.Attributes;

namespace BH.Engine.Geometry
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Collapses a PolyCurve to a polyline where curved segments are split into Polyline segments for computations. Where their partitioning is based on both their curvature and size, ergo; shorter segment less partitions, very curved segment more partitions")]
        [Input("curve", "The curve to collapse")]
        [Input("tolerance", "tolarance is the angleTolerance for a unit circle, the tolerance value will variy by: toleranceGrowth over curvature")]
        [Input("toleranceGrowth", "toleranceGrowth is the value that decides how much the angleTolerance varies due to curvature")]
        [Input("scale", "the radie of a baseline circle for the angle tolerance, smaller circles will have less segments and bigger more. deafalult of 0 will auto-generate for the PolyCurve")]
        [Output("C", "A polyline aproximating the provided curve")]
        public static Polyline CollapseToScaledPolyline(this PolyCurve curve, double tolerance = 0.04, double toleranceGrowth = 0.001, double scale = 0)
        {
            if (tolerance <= 0)
                return null;
            if (tolerance < 0)
                toleranceGrowth = 0;
            if (scale <= 0)
            {
                BoundingBox box = curve.IBounds();

                Point min = box.Min;
                Point max = box.Max;
                double totalWidth = max.X - min.X;
                double totalHeight = max.Y - min.Y;

                scale = Math.Max(totalHeight, totalWidth) / 2;
            }

            double scaledTolerance = scale * toleranceGrowth;

            tolerance -= scaledTolerance;

            List<Polyline> list = new List<Polyline>();
            foreach (ICurve c in curve.Curves)
            {
                if (c is PolyCurve)
                    list.Add(CollapseToScaledPolyline(c as PolyCurve, tolerance, toleranceGrowth, scale));

                list.Add(Divide(c as dynamic, scaledTolerance, tolerance));
            }
            return (Geometry.Compute.Join(list))[0];
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static Polyline Divide(this Line line, double tolerance, double baseTolerance)
        {
            return new Polyline() { ControlPoints = new List<Point>() { line.Start, line.End } };
        }

        /***************************************************/

        private static Polyline Divide(this Polyline line, double tolerance, double baseTolerance)
        {
            return new Polyline() { ControlPoints = line.ControlPoints };
        }

        /***************************************************/

        private static Polyline Divide(this Arc arc, double tolerance, double baseTolerance)
        {
            return arc.CollapseToPolyline(Soften(arc.Radius, tolerance, baseTolerance),200);
        }

        /***************************************************/

        private static Polyline Divide(this Circle circle, double tolerance, double baseTolerance)
        {
            return circle.CollapseToPolyline(Soften(circle.Radius, tolerance, baseTolerance), 200);
        }

        /***************************************************/

        private static double Soften(double r, double tolerance, double baseTolerance)
        {
            return (tolerance / r) + baseTolerance;     
        }

        /***************************************************/

    }
}
