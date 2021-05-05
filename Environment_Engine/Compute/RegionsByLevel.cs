/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2021, the respective contributors. All rights reserved.
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
using System.Text;
using System.Threading.Tasks;

using BH.oM.Reflection;
using BH.oM.Geometry;
using BH.oM.Environment.Elements;
using BH.oM.Architecture.Elements;
using BH.oM.Geometry.SettingOut;

using BH.Engine.Geometry;

using System.ComponentModel;
using BH.oM.Reflection.Attributes;
using BH.Engine.Base;
using BH.oM.Analytical.Elements;

namespace BH.Engine.Environment
{
    public static partial class Compute
    {
        [Description("Organise the BHoM Region objects into a data structure based on the level of each region")]
        [Input("regions", "A collection of BHoM Regions to be organised.")]
        [Input("decimals", "Provide decimal location to define the degree of tolerance for data matching.")]
        [MultiOutput(0, "regionsByLevel", "A collection of BHoM Regions grouped by levels.")]
        [MultiOutput(1, "levelsInUse", "A sublist of the BHoM Levels that have Room.")]
        [MultiOutput(2, "regionsNotMapped", "A collection of BHoM Regions which did not sit neatly on any of the provided levels")]
        public static Output<List<List<IRegion>>, List<oM.Geometry.SettingOut.Level>, List<IRegion>> RegionsByLevel(List<IRegion> regions, int decimals = 6)
        {
            List<oM.Geometry.SettingOut.Level> levels = Create.Levels(regions);
            Output<List<List<IRegion>>, List<oM.Geometry.SettingOut.Level>, List<IRegion>> output = MapToLevel(regions, levels, decimals); 

            return output;
        }
    }
}
