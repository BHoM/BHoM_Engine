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
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods - Vectors                  ****/
        /***************************************************/

        [Description("Reverses a Vector by negating all its components.")]
        [Input("vector", "The Vector to reverse.")]
        [Output("vector", "The reversed Vector with negated X, Y, and Z components.")]
        public static Vector Reverse(this Vector vector)
        {
            return new Vector { X = -vector.X, Y = -vector.Y, Z = -vector.Z };
        }


        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        [Description("Reverses a Line by swapping its start and end points.")]
        [Input("line", "The Line to reverse.")]
        [Output("line", "The reversed Line with swapped start and end points.")]
        public static Line Reverse(this Line line)
        {
            return new Line { Start = line.End, End = line.Start, Infinite = line.Infinite };
        }

        /***************************************************/
        /**** Public Methods - Arcs                     ****/
        /***************************************************/

        [Description("Reverses an Arc by swapping its start and end points while maintaining the arc geometry.")]
        [Input("arc", "The Arc to reverse.")]
        [Output("arc", "The reversed Arc with swapped start and end points.")]
        public static Arc Reverse(this Arc arc)
        {
            // Compute the midpoint parameter (halfway between start and end angles)
            double midAngle = arc.StartAngle + (arc.EndAngle - arc.StartAngle) / 2.0;
            // Calculate the midpoint in the arc's coordinate system
            Point midPoint = arc.CoordinateSystem.Origin + arc.CoordinateSystem.X * (arc.Radius * Math.Cos(midAngle)) + arc.CoordinateSystem.Y * (arc.Radius * Math.Sin(midAngle));

            // Create a new arc with swapped start and end, and the computed midpoint
            return Geometry.Create.Arc(arc.EndPoint(), midPoint, arc.StartPoint());
        }

        /***************************************************/
    }
}
