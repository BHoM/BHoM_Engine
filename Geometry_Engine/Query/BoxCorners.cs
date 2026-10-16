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

        [Description("Returns corners of a box that has the input center point, width, height and directions.")]
        [Input("center", "Center of the new box.")]
        [Input("width", "Width of the new box.")]
        [Input("height", "Height of the new box.")]
        [Input("xVector", "Direction of the new box's horizontal edges.")]
        [Input("yVector", "Direction of the new box's vertical edges.")]
        [Output("corners", "Corners of a box that has the input center point, width, height and directions.")]
        public static List<Point> BoxCorners(this Point center, double width, double height, Vector xVector, Vector yVector)
        {
            return center.BoxCorners(width, height, xVector, yVector, Plane.XY.Origin);
        }

        /***************************************************/

        [Description("Returns corners of a box defined by center point, width, height, directions, and origin.")]
        [Input("center", "Center of the new box.")]
        [Input("width", "Width of the new box.")]
        [Input("height", "Height of the new box.")]
        [Input("xVector", "Direction of the new box's horizontal edges.")]
        [Input("yVector", "Direction of the new box's vertical edges.")]
        [Input("origin", "The origin point for the plane on which the box lies.")]
        [Output("corners", "Corners of a box that has the input center point, width, height and directions.")]
        public static List<Point> BoxCorners(this Point center, double width, double height, Vector xVector, Vector yVector, Point origin)
        {
            Plane xyPlane = Create.Plane(origin, xVector.CrossProduct(yVector));
            Point p0 = center.Project(xyPlane) - xVector * width / 2 + yVector * height / 2;
            Point p1 = p0 + xVector * width;
            Point p2 = p1 - yVector * height;
            Point p3 = p2 - xVector * width;

            return new List<Point> { p0, p1, p2, p3 };
        }

        /***************************************************/
    }
}
