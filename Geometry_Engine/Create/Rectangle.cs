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

using System;
using BH.oM.Base.Attributes;
using BH.oM.Geometry;
using System.Collections.Generic;
using System.ComponentModel;

namespace BH.Engine.Geometry
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a rectangle lying flat in the XY plane, centred on the origin, with its sides parallel to the X and Y axes.")]
        [Input("width", "Total width of the rectangle, measured along the X axis.")]
        [Input("height", "Total height of the rectangle, measured along the Y axis.")]
        [Output("rectangle", "The rectangle as a closed outline, starting at its bottom-left corner and running anticlockwise.")]
        public static Polygon Rectangle(double width, double height)
        {
            Vector yAxis = oM.Geometry.Vector.YAxis;
            Vector xAxis = oM.Geometry.Vector.XAxis;
            Point centre = Point(0, 0, 0);

            Vector halfWidth = xAxis * width/2;
            Vector halfHeight = yAxis * height/2;

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

        [Description("Creates a rectangle lying flat in the XY plane, centred on a point, with its sides parallel to the X and Y axes.")]
        [Input("width", "Total width of the rectangle, measured along the X axis.")]
        [Input("height", "Total height of the rectangle, measured along the Y axis.")]
        [Output("rectangle", "The rectangle as a closed outline, starting at its bottom-left corner and running anticlockwise.")]
        public static Polygon Rectangle(double width, double height, Point centre)
        {
            Vector yAxis = oM.Geometry.Vector.YAxis;
            Vector xAxis = oM.Geometry.Vector.XAxis;
            
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

        [Description("Creates a rectangle around a centre point, with its size and direction set by two arrows pointing from the centre to the middle of two sides.")]
        [Input("centre", "The point at the middle of the rectangle.")]
        [Input("HeightVector", "Arrow from the centre to the middle of the top edge. Its direction sets which way the rectangle faces, and its length is half the rectangle's height.")]
        [Input("WidthVector", "Arrow from the centre to the middle of the right-hand edge. Its length is half the rectangle's width. It must be at right angles to the height arrow.")]
        [Output("rectangle", "The rectangle as a closed outline. If the two arrows are not at right angles, nothing is created and an error is shown.")]
        public static Polygon Rectangle(Point centre, Vector halfHeightVector, Vector halfWidthVector)
        {
            if (centre == null || halfHeightVector == null || halfWidthVector == null)
            {
                Engine.Base.Compute.RecordError("Cannot create rectangle: centre and both vectors must be provided.");
                return null;
            }

            if (Math.Abs(halfHeightVector.DotProduct(halfWidthVector)) > 1e-6)
            {
                Engine.Base.Compute.RecordError("Cannot create rectangle: the halfHeightVector and halfWidthVector vectors are not perpendicular.");
                return null;
            }

            List<Point> rectanglePoints = new List<Point>
            {
            centre - halfWidthVector - halfHeightVector,
            centre + halfWidthVector - halfHeightVector,
            centre + halfWidthVector + halfHeightVector,
            centre - halfWidthVector + halfHeightVector,
            centre - halfWidthVector - halfHeightVector,
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

        [Description("Creates a rectangle lying on a plane, centred on the plane's origin and lined up with a given direction.")]
        [Input("width", "Length of the two sides that run at right angles to the given direction.")]
        [Input("height", "Length of the two sides that run along the given direction.")]
        [Input("plane", "The plane the rectangle lies on. Its origin is the centre of the rectangle.")]
        [Input("heightDirection", "The direction the rectangle is lined up with. It must lie flat on the plane.")]
        [Output("rectangle", "The rectangle as a closed outline. If the direction does not lie on the plane, nothing is created and an error is shown.")]
        public static Polygon Rectangle(double width, double height, Plane plane, Vector heightDirection)
        {
            if (Math.Abs(heightDirection.DotProduct(plane.Normal)) > 1e-6)
            {
                Engine.Base.Compute.RecordError("Cannot create rectangle: heightDirection is not on the plane.");
                return null;
            }

            Point centre = plane.Origin;
            Vector yAxis = heightDirection.Normalise();
            Vector xAxis = heightDirection.CrossProduct(plane.Normal).Normalise();

            Vector halfWidth = xAxis * width/2;
            Vector halfHeight = yAxis * height/2;

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
    }
}