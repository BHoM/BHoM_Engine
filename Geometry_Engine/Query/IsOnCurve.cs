/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2026, the respective contributors. All rights reserved.
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
using BH.oM.Quantities.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Checks if a Point lies on a Line within the given tolerance.")]
        [Input("line", "The Line to check.")]
        [Input("pt", "The Point to check.")]
        [Input("tolerance", "The distance tolerance for the check.", typeof(Length))]
        [Output("isOnCurve", "True if the Point is on the Line, false otherwise.")]
        public static bool IsOnCurve(this Line line, Point pt, double tolerance = Tolerance.Distance)
        {
            double distToStart = Query.Distance(pt, line.Start);
            double distToEnd = Distance(line.End, pt);

            double maxTol = (distToStart + distToEnd) + tolerance;
            double minTol = (distToStart + distToEnd) - tolerance;

            return line.Length() >= minTol && line.Length() <= maxTol;
        }

        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        [Description("Checks if a Point lies on an Arc within the given tolerance.")]
        [Input("point", "The Point to check.")]
        [Input("curve", "The Arc to check against.")]
        [Input("tolerance", "The distance tolerance for the check.", typeof(Length))]
        [Output("isOnCurve", "True if the Point is on the Arc, false otherwise.")]
        public static bool IsOnCurve(this Point point, Arc curve, double tolerance = Tolerance.Distance)
        {
            return point.Distance(curve) < tolerance;
        }

        /***************************************************/

        [Description("Checks if a Point lies on a Circle within the given tolerance.")]
        [Input("point", "The Point to check.")]
        [Input("curve", "The Circle to check against.")]
        [Input("tolerance", "The distance tolerance for the check.", typeof(Length))]
        [Output("isOnCurve", "True if the Point is on the Circle, false otherwise.")]
        public static bool IsOnCurve(this Point point, Circle curve, double tolerance = Tolerance.Distance)
        {
            return point.Distance(curve) < tolerance;
        }

        /***************************************************/

        [Description("Checks if a Point lies on a Line within the given tolerance.")]
        [Input("point", "The Point to check.")]
        [Input("curve", "The Line to check against.")]
        [Input("tolerance", "The distance tolerance for the check.", typeof(Length))]
        [Output("isOnCurve", "True if the Point is on the Line, false otherwise.")]
        public static bool IsOnCurve(this Point point, Line curve, double tolerance = Tolerance.Distance)
        {
            return point.Distance(curve) < tolerance;
        }

        /***************************************************/

        [Description("Checks if a Point lies on a PolyCurve within the given tolerance.")]
        [Input("point", "The Point to check.")]
        [Input("curve", "The PolyCurve to check against.")]
        [Input("tolerance", "The distance tolerance for the check.", typeof(Length))]
        [Output("isOnCurve", "True if the Point is on the PolyCurve, false otherwise.")]
        public static bool IsOnCurve(this Point point, PolyCurve curve, double tolerance = Tolerance.Distance)
        {
            return point.Distance(curve) < tolerance;
        }

        /***************************************************/

        [Description("Checks if a Point lies on a Polyline within the given tolerance.")]
        [Input("point", "The Point to check.")]
        [Input("curve", "The Polyline to check against.")]
        [Input("tolerance", "The distance tolerance for the check.", typeof(Length))]
        [Output("isOnCurve", "True if the Point is on the Polyline, false otherwise.")]
        public static bool IsOnCurve(this Point point, Polyline curve, double tolerance = Tolerance.Distance)
        {
            return point.Distance(curve) < tolerance;
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        [Description("Checks if a Point lies on any ICurve within the given tolerance.")]
        [Input("point", "The Point to check.")]
        [Input("curve", "The ICurve to check against.")]
        [Input("tolerance", "The distance tolerance for the check.", typeof(Length))]
        [Output("isOnCurve", "True if the Point is on the curve, false otherwise.")]
        public static bool IIsOnCurve(this Point point, ICurve curve, double tolerance = Tolerance.Distance)
        {
            return IsOnCurve(point, curve as dynamic, tolerance);
        }


        /***************************************************/
        /**** Private Fallback Methods                  ****/
        /***************************************************/

        private static bool IsOnCurve(this Point point, ICurve curve, double tolerance = Tolerance.Distance)
        {
            throw new NotImplementedException($"IsOnCurve is not implemented for ICurves of type: {curve.GetType().Name}.");
        }

        /***************************************************/
    }
}
