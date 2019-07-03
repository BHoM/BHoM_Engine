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
using System;
using System.Collections.Generic;
using System.Linq;

using BH.oM.Environment.Elements;
using BH.oM.Environment.Gains;
using BH.Engine.Geometry;
using BH.oM.Reflection.Attributes;
using System.ComponentModel;

namespace BH.Engine.Environment
{
    public static partial class Compute
    {
        /***************************************************/
        /****          public Methods                   ****/
        /***************************************************/

        [Description("Compute a lighting gain from the watts per square meter and floor area of the space")]
        [Input("wattsMeterSquared", "The watts per meter squared from a standard or set of lights")]
        [Input("area", "The floor area (in square meters) of the space")]
        [Output("lightingGain", "The calculated lighting gain with the sensible watts for the space")]
        public static Lighting LightingGain(double wattsMeterSquared, double area)
        {
            return Create.Lighting(wattsMeterSquared * area);
        }
    }
}
