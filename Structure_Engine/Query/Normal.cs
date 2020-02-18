/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2020, the respective contributors. All rights reserved.
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
using BH.oM.Structure.Elements;
using BH.Engine.Geometry;
using System;
using System.Linq;
using System.ComponentModel;
using BH.oM.Reflection.Attributes;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns the bars local z-axis, generally the major axis direction of the section of the Bar. \n" +
                     "For non - vertical members the local z is aligned with the global z and rotated with the orientation angle around the local x. \n" +
                     "For vertical members the local y is aligned with the global y and rotated with the orientation angle around the local x. For this case the normal will be the vector orthogonal to the local y and local x")]
        [Input("bar", "The Bar to evaluate the normal of")]
        [Output("normal", "Vector representing the local z-axis of the Bar")]
        public static Vector Normal(this Bar bar)
        {
            return bar.Centreline().ElementNormal(bar.OrientationAngle);
        }

        /***************************************************/

        [Deprecated("3.1", "Deprecated by method targeting IElement2D")]
        [Description("Returns the Panels local z-axis, a vector orthogonal to the plane of the panel. This is found by fitting a plane through all the edge curves and taking the Normal from this plane.")]
        [Input("bar", "The Panel to evaluate the normal of")]
        [Output("normal", "Vector representing the local z-axis Panel")]
        public static Vector Normal(this Panel panel)
        {
            return panel.AllEdgeCurves().SelectMany(x => x.IControlPoints()).ToList().FitPlane().Normal;
        }

        /***************************************************/
        /**** Public Methods - Interface methods        ****/
        /***************************************************/

        [Description("Returns the local z-axis of the IAreaElement")]
        [Input("bar", "The element to evaluate the normal of")]
        [Output("normal", "Vector representing the local z-axis element")]
        public static Vector INormal(this IAreaElement areaElement)
        {
            return Normal(areaElement as dynamic);
        }

        /***************************************************/
        /**** Private Methods - fall back               ****/
        /***************************************************/

        private static Vector Normal(this IAreaElement areaElement)
        {
            Reflection.Compute.RecordWarning("Can not get normal for element of type " + areaElement.GetType().Name);
            return null;
        }

        /***************************************************/

    }
}
