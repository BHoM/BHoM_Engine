/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
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
using BH.Engine.Base;
using System.Collections.Generic;
using System.Linq;
using System;
using System.ComponentModel;
using BH.oM.Base.Attributes;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates an differential temprature load to be applied to Bars.")]
        [InputFromProperty("loadcase")]
        [Input("positions", "The parametric distance distance from the top (local z) or left (local y) of the profile.")]
        [Input("temperatures", "The temperature at the corresponding position on the profile.")]
        [Input("localLoadDirection", "The local direction of the temperature variation relative to the profile. Typically limited to local y or z.")]
        [Input("objects", "The collection of Bars the load should be applied to.")]
        [Input("name", "The name of the created load.")]
        [Output("barDiffTempLoad", "The created BarDifferentialTemperatureLoad.")]
        public static BarDifferentialTemperatureLoad BarDifferentialTemperatureLoad(Loadcase loadcase, List<double> positions, List<double> temperatures, DifferentialTemperatureLoadDirection localLoadDirection, IEnumerable<Bar> objects, string name = "")
        {
            if (loadcase.IsNull() || positions.IsNullOrEmpty() || temperatures.IsNullOrEmpty())
                return null;

            //Checks for positions and profiles
            if (positions.Count != temperatures.Count)
            {
                Base.Compute.RecordError("Number of positions and temperatures provided are not equal");
                return null;
            }
            else if (positions.Exists((double d) => { return d > 1; }) || positions.Exists((double d) => { return d < 0; }))
            {
                Base.Compute.RecordError("Positions must exist between 0 and 1 (inclusive)");
                return null;
            }

            // Check that top and bottom positions are included
            if (!(positions.Contains(0) && positions.Contains(1)))
            {
                Base.Compute.RecordError("Positions must inlude the bottom (0) and top (1) position.");
                return null;
            }

            if (positions.Zip(positions.Skip(1), (a, b) => new { a, b }).Any(p => p.a > p.b))
            {
                Base.Compute.RecordError("Positions must be sorted in ascending order.");
                return null;
            }

            //Create ditionary for TaperedProfile
            Dictionary<Double, Double> temperatureProfile = positions.Zip(temperatures, (z, t) => new { z, t })
                .ToDictionary(x => x.z, x => x.t);

            BHoMGroup<Bar> group = new BHoMGroup<Bar>();
            if (objects == null)
                group = null;
            else
                group.Elements = objects.ToList();

            return new BarDifferentialTemperatureLoad
            {
                Loadcase = loadcase,
                TemperatureProfile = temperatureProfile,
                LoadDirection = localLoadDirection,
                Objects = group,
                Name = name
            };
        }

        /***************************************************/

        [Description("Creates an differential temprature load to be applied to Bars.")]
        [InputFromProperty("loadcase")]
        [Input("topTemperature", "The temperature at the top of the specified local axis e.g. the top of the surface property in the local z or the left of it in the local y.")]
        [Input("bottomTemperature", "The temperature at the bottom of the specific local axis e.g. the bottom of the surface property in the local z or the right of it in the local y.")]
        [Input("localLoadDirection", "The local direction of the temperature variation relative to the profile. Typically limited to local y or z.")]
        [Input("objects", "The collection of elements the load should be applied to.")]
        [Input("name", "The name of the created load.")]
        [Output("barDiffTempLoad", "The created BarDifferentialTemperatureLoad.")]
        public static BarDifferentialTemperatureLoad BarDifferentialTemperatureLoad(Loadcase loadcase, double topTemperature, double bottomTemperature, DifferentialTemperatureLoadDirection localLoadDirection, IEnumerable<Bar> objects, string name = "")
        {
            return BarDifferentialTemperatureLoad(loadcase, new List<double>() { 0, 1 }, new List<double>() { topTemperature, bottomTemperature }, localLoadDirection, objects, name);

        }

        /***************************************************/

    }
}






