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
using BH.oM.Quantities.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a Rectangle aligned with the XY plane based on its core properties.")]
        [InputFromProperty("height")]
        [InputFromProperty("width")]
        [Output("rectangle", "The created Rectangle.")]
        public static Polygon Rectangle(Point centre, double width, double height, Plane plane = null)
        {
            if (plane == null)
                plane = BH.oM.Geometry.Plane.XY;

            Vector xAxis = oM.Geometry.Vector.XAxis;
            Vector yAxis = oM.Geometry.Vector.YAxis;
            Point origin = oM.Geometry.Point.Origin;
            Vector halfWidth = xAxis * width;
            Vector halfheight = yAxis * height;

            {
                List<Point> rectanglePoints = new List<Point>
            {
            centre - halfWidth - halfheight,
            centre + halfWidth - halfheight,
            centre + halfWidth + halfheight,
            centre - halfWidth + halfheight,
            centre - halfWidth - halfheight,
            };

                return BH.Engine.Geometry.Create.Polygon(rectanglePoints);
            }



            /***************************************************/

            [Description("Creates a Rectangle aligned with the XY plane based on its core properties.")]
            [InputFromProperty("height")]
            [InputFromProperty("width")]
            [Output("rectangle", "The created Rectangle.")]
            public static Polygon Rectangle(Point point1, Point point2, Plane plane = Plane.xy)
            {
                Vector xAxis = oM.Geometry.Vector.XAxis;
                Vector yAxis = oM.Geometry.Vector.YAxis;
                Point origin = oM.Geometry.Point.Origin;
                Point centre = (point1 + point2) / 2;
                plane.Normal;
                RotationMatrix(centre, plane.Normal,)
                {

                    List<Point> rectanglePoints = new List<Point>
                {
                centre - halfWidth - halfheight,
                centre + halfWidth - halfheight,
                centre + halfWidth + halfheight,
                centre - halfWidth + halfheight,
                centre - halfWidth - halfheight,
                };

                    return BH.Engine.Geometry.Create.Polygon(rectanglePoints);
                }

                /***************************************************/

            }
        }
    }