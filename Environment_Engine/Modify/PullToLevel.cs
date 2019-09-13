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
using BH.oM.Architecture.Elements;

namespace BH.Engine.Environment
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Pull an Environment Object to a given Architectural Level")]
        [Input("environmentObject", "An Environment Object implementing the IEnvironmentObject interface")]
        [Input("level", "The Architectural Level to pull to")]
        [Input("tolerance", "The tolerance distance in which to catch points for adjustment, default is set to BH.oM.Geometry.Tolerance.Distance")]
        [Output("environmentObject", "Environment Object pulled to the given level")]
        public static IEnvironmentObject PullToLevel(this IEnvironmentObject environmentObject, Level level, double tolerance = BH.oM.Geometry.Tolerance.Distance)
        {
            return environmentObject.PullToLevel(level.Elevation, tolerance);
        }

        [Description("Pull a BHoM Geometry Polyline to a given Architectural Level")]
        [Input("polyline", "A BHoM Geometry Polyline to pull to the given level")]
        [Input("level", "The Architectural Level to pull to")]
        [Input("tolerance", "The tolerance distance in which to catch points for adjustment, default is set to BH.oM.Geometry.Tolerance.Distance")]
        [Output("polyline", "Polylinue pulled to the given level")]
        public static Polyline PullToLevel(this Polyline polyline, Level level, double tolerance = BH.oM.Geometry.Tolerance.Distance)
        {
            return polyline.PullToLevel(level.Elevation, tolerance);
        }

        [Description("Pull an Environment Object to a planar level")]
        [Input("environmentObject", "An Environment Object implementing the IEnvironmentObject interface")]
        [Input("level", "The level value (Z-value) to pull to")]
        [Input("tolerance", "The tolerance distance in which to catch points for adjustment, default is set to BH.oM.Geometry.Tolerance.Distance")]
        [Output("environmentObject", "Environment Object pulled to the given level")]
        public static IEnvironmentObject PullToLevel(this IEnvironmentObject environmentObject, double level, double tolerance = BH.oM.Geometry.Tolerance.Distance)
        {
            return SetGeometry(environmentObject as dynamic, PullToLevel(environmentObject.Polyline(), level, tolerance) as dynamic);
        }

        [Description("Pull a BHoM Geometry Polyline to a planar level")]
        [Input("polyline", "A BHoM Geometry Polyline to pull to the given level")]
        [Input("level", "The level value (Z-value) to pull to")]
        [Input("tolerance", "The tolerance distance in which to catch points for adjustment, default is set to BH.oM.Geometry.Tolerance.Distance")]
        [Output("polyline", "Polylinue pulled to the given level")]
        public static Polyline PullToLevel(this Polyline polyline, double level, double tolerance = BH.oM.Geometry.Tolerance.Distance)
        {

            List<Point> aPointList = new List<Point>();
            foreach (Point aPoint in polyline.IControlPoints())
            {
                Point aPoint_Temp = Geometry.Create.Point(aPoint.X, aPoint.Y, level);
                if (Geometry.Query.Distance(aPoint_Temp, aPoint) <= tolerance)
                    aPointList.Add(aPoint_Temp);
                else
                    aPointList.Add(aPoint.Clone());
            }

            return  Geometry.Create.Polyline(aPointList);
        }

    }
}
