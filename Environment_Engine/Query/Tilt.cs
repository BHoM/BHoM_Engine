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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Environment;
using BH.oM.Geometry;
using BH.Engine.Geometry;

using BH.oM.Reflection.Attributes;
using System.ComponentModel;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns the tilt of an Environment Object")]
        [Input("environmentObject", "Any object implementing the IEnvironmentObject interface that can have its tilt queried")]
        [Output("tilt", "The tilt of the Environment Object")]
        public static double Tilt(this IEnvironmentObject environmentObject)
        {
            return environmentObject.Polyline().Tilt();
        }

        [Description("Returns the tilt of a BHoM Geometry Polyline")]
        [Input("polyline", "The BHoM Geometry Polyline having its tilt queried")]
        [Output("tilt", "The tilt of the polyline")]
        public static double Tilt(this Polyline polyline)
        {
            double tilt;

            List<Point> pts = polyline.DiscontinuityPoints();

            if (pts.Count < 3 || !BH.Engine.Geometry.Query.IsClosed(polyline)) return -1; //Error protection on pts having less than 3 elements to create a plane or pLine not being closed

            Plane plane = BH.Engine.Geometry.Create.Plane(pts[0], pts[1], pts[2]);

            //The polyline can be locally concave. Check if the polyline is clockwise.
            if (!polyline.IsClockwise(plane.Normal))
                plane.Normal = -plane.Normal;

            tilt = BH.Engine.Geometry.Query.Angle(plane.Normal, Plane.XY.Normal) * (180 / Math.PI);

            return tilt;
        }
    }
}
