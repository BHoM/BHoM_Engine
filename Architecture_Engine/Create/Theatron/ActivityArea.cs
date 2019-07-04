/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2019, the respective contributors. All rights reserved.
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
using BH.oM.Architecture.Theatron;
using System.Collections.Generic;

namespace BH.Engine.Architecture.Theatron
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/
        public static ActivityArea ActivityArea(double scale = 1.0)
        {
            var p1 = Geometry.Create.Point(30*scale, 45 * scale, 0);
            var p2 = Geometry.Create.Point(30 * scale, -45 * scale, 0);
            var p3 = Geometry.Create.Point(-30 * scale, -45 * scale, 0);
            var p4 = Geometry.Create.Point(-30 * scale, 45 * scale, 0);
            return new ActivityArea
            {
                PlayingArea = Geometry.Create.Polyline(new List<Point> { p1, p2, p3, p4, p1 }),
                Width = 60 * scale,
                Length = 90 * scale,

            };
        }
        /***************************************************/
        public static ActivityArea ActivityArea(double width = 60,double length = 90)
        {
            var p1 = Geometry.Create.Point(width/2, length/2, 0);
            var p2 = Geometry.Create.Point(width / 2, -length / 2, 0);
            var p3 = Geometry.Create.Point(-width / 2, -length / 2, 0);
            var p4 = Geometry.Create.Point(-width / 2, length / 2, 0);
            return new ActivityArea
            {
                PlayingArea = Geometry.Create.Polyline(new List<Point> { p1, p2, p3, p4, p1 }),
                Width = width,
                Length = length,
            };
        }
        /***************************************************/
        public static ActivityArea ActivityArea(Polyline activityArea, Point aValueFocalPoint)
        {
            
            return new ActivityArea
            {
                PlayingArea = activityArea,
                AValueFocalPoint = aValueFocalPoint,
            };
        }
    }
}
