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

        [Description("Returns the bars local Z-axis")]
        [Input("bar", "The Bar to evaluate the normal of")]
        [Output("normal", "Vector representing the bars local Z-axis")]
        public static Vector Normal(this Bar bar)
        {
            return bar.Centreline().ElementNormal(bar.OrientationAngle);
        }

        /***************************************************/

        public static Vector Normal(this Panel panel)
        {
            return panel.AllEdgeCurves().SelectMany(x => x.IControlPoints()).ToList().FitPlane().Normal;
        }

        /***************************************************/
        /**** Public Methods - Interface methods        ****/
        /***************************************************/

        public static Vector INormal(this IAreaElement areaElement)
        {
            return Normal(areaElement as dynamic);
        }

        /***************************************************/

    }
}
