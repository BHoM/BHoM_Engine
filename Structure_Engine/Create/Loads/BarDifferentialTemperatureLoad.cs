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
        [Input("positions", "The parametric distance distance from the top (local z) or left (local y) of the profile.")]
        [Input("temperatures", "The temperature at the corresponding position on the profile.")]
        [Input("localDirection", "The local direction of the temperature variation relative to the profile. Typically limited to local y or z.")]
        [Input("objects", "The collection of Bars the load should be applied to.")]
        [Input("name", "The name of the created load.")]
        [Output("barDiffTempLoad", "The created BarDifferentialTemperatureLoad.")]
        public static BarDifferentialTemperatureLoad BarDifferentialTemperatureLoad(Loadcase loadcase, List<double> positions, List<double> temperatures, Vector localDirection, IEnumerable<Bar> objects, string name = "")
        {
            //Checks for positions and profiles
            if (positions.Count != temperatures.Count)
            {
                Reflection.Compute.RecordError("Number of positions and temperatures provided are not equal");
                return null;
            }
            else if (positions.Exists((double d) => { return d > 1; }) || positions.Exists((double d) => { return d < 0; }))
            {
                Reflection.Compute.RecordError("Positions must exist between 0 and 1 (inclusive)");
                return null;
            }

            List<double> sortedPositions = positions;
            sortedPositions.Sort();

            if (!positions.SequenceEqual(sortedPositions))
            {
                Reflection.Compute.RecordError("Positions must be sorted in ascending order.");
                return null;
            }

            //Create ditionary for TaperedProfile
            Dictionary<double, double> temperatureProfile = positions.Zip(temperatures, (z, T) => new { z, T })
                .ToDictionary(x => x.z, x => x.T);


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

