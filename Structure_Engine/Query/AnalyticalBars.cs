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

using BH.oM.Structure.Elements;
using System.Linq;
using System.Collections.Generic;
using BH.oM.Structure.FramingProperties;
using BH.oM.Geometry;
using BH.oM.Reflection.Attributes;
using BH.Engine.Geometry;
using System.ComponentModel;
using System;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Deprecated("2.3", "Deprecated to be replaced with method with same name with more settings", null, "AnalyticalBars")]
        public static List<Bar> AnalyticalBars(this FramingElement element)
        {
            return element.AnalyticalBars(0.05 * Math.PI);
        }

        /***************************************************/

        [Description("Create a seres of analytical Bar elements from the framing element. Could return any number of bars for each framing element depending on the settings")]
        [Input("element", "The framing element to generate bars from")]
        [Input("angleTolerance", "Angle tolerance to control the splitting up of non-linear curves. Unused for line based FramingElements")]
        [Input("maxNbBarsPerArc", "The maximum number of bars that each arc segement of the element will be split up into. Unused for line based FramingElements")]
        public static List<Bar> AnalyticalBars(this FramingElement element, double angleTolerance = 0.05 * Math.PI, int maxNbBarsPerArc = 10)
        {
            return AnalyticalBars(element.Property as dynamic, element.LocationCurve, element.Name, angleTolerance, maxNbBarsPerArc);
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/


        private static List<Bar> AnalyticalBars(ConstantFramingElementProperty property, ICurve centreLine, string name, double angleTolerance, int maxNbBars)
        {
            if (centreLine is NurbsCurve)
            {
                Engine.Reflection.Compute.RecordError("The analytical bars method is currently not supported for NurbsCurves. Please use another method to split up the nurbs to polylines that can be used to construct the bars.");
                return new List<Bar>();
            }
            return centreLine.ISubParts().SelectMany(x => x.ICollapseToPolyline(angleTolerance, maxNbBars).SubParts()).Select(x => Create.Bar(x, property.SectionProperty, property.OrientationAngle, Create.BarReleaseFixFix(), BarFEAType.Flexural, name)).ToList();
        }

        /***************************************************/
    }
}
