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
using BH.oM.Geometry.CoordinateSystem;
using System.Collections.Generic;
using System.ComponentModel;

namespace BH.Engine.Geometry
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a rectangle centred on the origin of a coordinate system, with its sides parallel to that system's X and Y axes. If no coordinate system is given, the global XY plane is used.")]
        [Input("width", "Total width of the rectangle, measured along the coordinate system's X axis.")]
        [Input("height", "Total height of the rectangle, measured along the coordinate system's Y axis.")]
        [Input("coordinateSystem", "Optional coordinate system that sets the rectangle's position and orientation. Its origin is the centre of the rectangle. Defaults to the global coordinate system.")]
        [Output("rectangle", "The rectangle as a closed Polygon, starting at its bottom-left corner and running anticlockwise when viewed from the coordinate system's Z axis.")]
        public static Polygon Rectangle(double width, double height, Cartesian coordinateSystem = null)
        {
            if (coordinateSystem == null)
                coordinateSystem = new Cartesian();

            Vector yAxis = coordinateSystem.Y;
            Vector xAxis = coordinateSystem.X;
            Point centre = coordinateSystem.Origin;

            Vector halfWidth = xAxis * width / 2;
            Vector halfHeight = yAxis * height / 2;

            List<Point> rectanglePoints = new List<Point>
            {
            centre - halfWidth - halfHeight,
            centre + halfWidth - halfHeight,
            centre + halfWidth + halfHeight,
            centre - halfWidth + halfHeight,
            centre - halfWidth - halfHeight,
            };

            return BH.Engine.Geometry.Create.Polygon(rectanglePoints);
        }

        /***************************************************/

        [Description("Creates a rectangle from one edge and a third point that sets where the opposite edge lies.")]
        [Input("point1", "One end of the first edge.")]
        [Input("point2", "The other end of the first edge.")]
        [Input("point3", "Any point on the opposite edge. Its straight-across distance from the first edge sets the length of the other two sides.")]
        [Output("rectangle", "The rectangle as a closed outline, starting and ending at the first point.")]
        public static Polygon Rectangle(Point point1, Point point2, Point point3)
        {
            Point projected = point3.Project(Create.Line(point1, point2));
            Vector height = point3 - projected;

            List<Point> rectanglePoints = new List<Point>
                {
                point1,
                point2,
                point2 + height,
                point1 + height,
                point1
                };

            return BH.Engine.Geometry.Create.Polygon(rectanglePoints);
        }

        /***************************************************/
    }
}