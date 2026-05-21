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

using BH.oM.Base.Attributes;
using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Checks whether a Polyline represents a rectangle.")]
        [Input("polyline", "Polyline to check.")]
        [Input("tolerance", "Maximum allowed distance deviation for closure and diagonal equality checks.")]
        [Output("isRectangular", "True if the polyline represents a rectangle; otherwise false.")]
        public static bool IsRectangle(this Polyline polyline, double tolerance)
        {
            List<Point> pts = polyline?.ControlPoints;
            if (pts == null || pts.Count != 5)
                return false;

            if (polyline.IsClosed(tolerance) != true)
                return false;

            double diagonal1 = pts[2].Distance(pts[0]);
            double diagonal2 = pts[3].Distance(pts[1]);

            return Math.Abs(diagonal1 - diagonal2) <= tolerance;
        }
    }
}