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
using System.Collections.Generic;
using System.ComponentModel;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns the four corners of a bounding rectangle on the XY plane defined by two opposite points.")]
        [Input("point1", "The first point defining the bounding rectangle.")]
        [Input("point2", "The second point defining the bounding rectangle.")]
        [Output("corners", "The four corner points of the bounding rectangle on the XY plane.")]
        public static List<Point> BoxCornersOnXY(this Point point1, Point point2)
        {
            if (point1 == null || point2 == null)
                return null;

            BoundingBox box = new List<Point> { point1, point2 }.Bounds();
            Point pMin = box.Min.Project(Plane.XY);
            Point pMax = box.Max.Project(Plane.XY);

            Point p0 = new Line { Start = pMin, End = pMin + Vector.YAxis, Infinite = true }
                .LineIntersection(new Line { Start = pMax, End = pMax - Vector.XAxis, Infinite = true }, true);
            Point p1 = new Line { Start = pMin, End = pMin + Vector.XAxis, Infinite = true }
                .LineIntersection(new Line { Start = pMax, End = pMax - Vector.YAxis, Infinite = true }, true);

            if (p0 == null || p1 == null)
                return null;

            return new List<Point> { pMin, p0, pMax, p1 };
        }

        /***************************************************/
    }
}
