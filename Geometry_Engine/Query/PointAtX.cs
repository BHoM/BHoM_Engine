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
using System.Linq;
using BH.oM.Geometry;
using System.ComponentModel;
using BH.oM.Base.Attributes;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Gets the point at the global x-value for a line passing through point a and b")]
        [Input("a", "One of the points the line passes through")]
        [Input("b", "One of the points the line passes through")]
        [Input("x", "The global x coordinate to evaluate the line at")]
        [Input("tol", "The tolerance for deciding the line is parallel to the Y-axis")]
        [Output("pt", "The Point at x on the line passing through the two points")]
        public static Point PointAtX(Point a, Point b, double x, double tol = Tolerance.MicroDistance)
        {
            if (Math.Abs(b.X - a.X) < tol)
                return null;
            return new Point()  
            {
                X = x,
                Y = ((b.Y - a.Y) * (x - a.X)
                                     / (b.X - a.X)
                                     + a.Y)
            };
        }

        /***************************************************/

    }
}



