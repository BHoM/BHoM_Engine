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

using BH.Engine.Geometry;
using BH.oM.Environment.Elements;
using BH.oM.Environment;
using BH.oM.Geometry;

using BH.oM.Reflection.Attributes;
using System.ComponentModel;
using System.Collections.Generic;

namespace BH.Engine.Environment
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Adjust Environmnet Panel to get planar by adjusting Z-coordiante")]
        [Input("environmentPanel", " Environment Panel")]
        [Input("Z", " Z-coordinate elevation for points to adjust")]
        [Input("tolerance", " tolerance distnace to pick Z coordinates to adjust")]
        [Output("environmentObject", "Environmnet Panel")]
        public static IEnvironmentObject PullToLevel(this IEnvironmentObject environmentObject, double Z, double tolerance = 0.05 )
        {
            Polyline aPolyline = PullToLevel(environmentObject.Polyline(), tolerance);

            return SetGeometry(environmentObject as dynamic, aPolyline as dynamic);
        }

        [Description("Adjust Polyline to get planar, Topologic")]
        [Input("Polyline", "")]
        [Input("Z", "Z-coordinate elevation for points to adjust")]
        [Input("tolerance", " tolerance distnace to pick Z coordinates to adjust")]
        [Output("Polyline", "Polyline")]
        public static Polyline PullToLevel(this Polyline polyline, double Z, double tolerance = 0.05)
        {

            List<Point> aPointList = new List<Point>();
            foreach (Point aPoint in polyline.IControlPoints())
            {
                Point aPoint_Temp = Geometry.Create.Point(aPoint.X, aPoint.Y, Z);
                if (Geometry.Query.Distance(aPoint_Temp, aPoint) <= tolerance)
                    aPointList.Add(aPoint_Temp);
                else
                    aPointList.Add(aPoint.Clone());
            }

            return  Geometry.Create.Polyline(aPointList);
        }

    }
}
