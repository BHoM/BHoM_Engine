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
using BH.oM.Environment;

using BH.Engine.Geometry;
using BH.oM.Geometry;

using BH.oM.Reflection.Attributes;
using System.ComponentModel;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns the orientation of a given environmental object")]
        [Input("environmentObject", "Any object implementing the IEnvironmentObject interface that can have its orientation queried")]
        [Output("orientation", "The orientation of the Environment Object")]
        public static double Orientation(this IEnvironmentObject environmentObject)
        {
            Polyline pLine = environmentObject.Polyline();

            List<Point> pts = pLine.DiscontinuityPoints();
            Plane plane = BH.Engine.Geometry.Create.Plane(pts[0], pts[1], pts[2]); //Some protection on this needed maybe?

            Vector xyNormal = BH.Engine.Geometry.Create.Vector(0, 1, 0);

            return BH.Engine.Geometry.Query.Angle(plane.Normal, xyNormal) * (180 / Math.PI);
        }
    }
}