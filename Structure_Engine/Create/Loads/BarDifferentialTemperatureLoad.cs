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
using BH.oM.Structure.Loads;
using BH.oM.Base;
using BH.oM.Structure.Elements;
using System.Collections.Generic;
using System.Linq;
using System;
using System.ComponentModel;
using BH.oM.Reflection.Attributes;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates an uniform temprature load to be applied to Bars.")]
        [InputFromProperty("loadcase")]
        [InputFromProperty("temperatureProfile")]
        [Input("objects", "The collection of Bars the load should be applied to.")]
        [Input("name", "The name of the created load.")]
        [Output("barTempLoad", "The created BarDifferentialTemperatureLoad.")]
        public static BarDifferentialTemperatureLoad BarDifferentialTemperatureLoad(Loadcase loadcase, Dictionary<double, double> temperatureProfile, Vector localDirection, IEnumerable<Bar> objects, string name = "")
        {
            return new BarDifferentialTemperatureLoad
            {
                Loadcase = loadcase,
                TemperatureProfile = temperatureProfile,
                LocalDirection = localDirection,
                Objects = new BHoMGroup<Bar>() { Elements = objects.ToList() },
                Name = name
            };
        }

        /***************************************************/

    }
}

