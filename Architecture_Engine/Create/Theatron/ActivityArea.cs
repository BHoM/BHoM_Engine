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
using BH.Engine.Geometry;
using System;
using System.ComponentModel;
using BH.oM.Reflection.Attributes;

namespace BH.Engine.Architecture.Theatron
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/
        [Description("Create an activityArea rectangle with width: 60 and length: 90, focal point is at 0,0,0")]
        [Input("scale", "Optional input to scale from default values")]
        
        public static ActivityArea ActivityArea(double scale = 1.0)
        {
            var p1 = Geometry.Create.Point(30*scale, 45 * scale, 0);
            var p2 = Geometry.Create.Point(30 * scale, -45 * scale, 0);
            var p3 = Geometry.Create.Point(-30 * scale, -45 * scale, 0);
            var p4 = Geometry.Create.Point(-30 * scale, 45 * scale, 0);
            return new ActivityArea
            {
                PlayingArea = Geometry.Create.Polyline(new List<Point> { p1, p2, p3, p4, p1 }),
            };
        }

        /***************************************************/
        [Description("Create an activityArea rectangle, focal point is at 0,0,0")]
        [Input("width", "Optional, width default is 60")]
        [Input("width", "Optional, length default is 90")]
        public static ActivityArea ActivityArea(double width = 60,double length = 90)
        {
            var p1 = Geometry.Create.Point(width/2, length/2, 0);
            var p2 = Geometry.Create.Point(width / 2, -length / 2, 0);
            var p3 = Geometry.Create.Point(-width / 2, -length / 2, 0);
            var p4 = Geometry.Create.Point(-width / 2, length / 2, 0);
            return new ActivityArea
            {
                PlayingArea = Geometry.Create.Polyline(new List<Point> { p1, p2, p3, p4, p1 }),
            };
        }

        /***************************************************/
        [Description("Create an ActivityArea from any closed polyline and a focal point")]
        [Input("activityArea", "Closed polyline defining the activity area")]
        [Input("activityFocalPoint", "Point defining the centre of attention for the activity area")]
        public static ActivityArea ActivityArea(Polyline activityArea, Point activityFocalPoint)
        {
            if (!activityArea.IsClosed())
            {
                throw new ArgumentException("activityArea must be closed"); ;
            }
            return new ActivityArea
            {
                PlayingArea = activityArea,
                ActivityFocalPoint = activityFocalPoint,
            };
        }
    }
}
