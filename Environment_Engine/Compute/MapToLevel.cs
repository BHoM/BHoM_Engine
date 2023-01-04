/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
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

using BH.oM.Geometry;
using BH.oM.Environment.Elements;
using BH.oM.Architecture.Elements;
using BH.oM.Spatial.SettingOut;

using BH.Engine.Geometry;
using BH.oM.Base;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.Engine.Base;
using BH.oM.Analytical.Elements;

namespace BH.Engine.Environment
{
    public static partial class Compute
    {
        [Description("Organise the BHoM Region objects into a data structure based on the level of each region.")]
        [Input("regions", "A collection of BHoM Regions to be organised.")]
        [Input("levels", "A collection of BHoM Levels.")]
        [Input("decimals", "Provide decimal location to define the degree of tolerance for data matching.")]
        [MultiOutput(0, "regionsByLevel", "A collection of BHoM Regions grouped by levels.")]
        [MultiOutput(1, "levelsInUse", "A sublist of the BHoM Levels that have Room.")]
        [MultiOutput(2, "regionsNotMapped", "A collection of BHoM Regions which did not sit neatly on any of the provided levels")]
        [PreviousInputNames("regions", "spaces, rooms")]
        public static Output<List<List<IRegion>>, List<oM.Spatial.SettingOut.Level>, List<IRegion>> MapToLevel(List<IRegion> regions, List<oM.Spatial.SettingOut.Level> levels, int decimals = 6)
        {
            List<List<IRegion>> regionsByLevel = new List<List<IRegion>>();
            List<oM.Spatial.SettingOut.Level> levelsInUse = new List<oM.Spatial.SettingOut.Level>();
            List<IRegion> regionsNotByLevel = new List<IRegion>();
            List<oM.Spatial.SettingOut.Level> roundedLevels = new List<oM.Spatial.SettingOut.Level>();            

            for (int x = 0; x < levels.Count; x++)
            {
                oM.Spatial.SettingOut.Level lvl = levels[x].DeepClone();
                lvl.Elevation = Math.Round(lvl.Elevation, decimals); 
                roundedLevels.Add(lvl); //Round the levels
            }

            Dictionary<double, List<IRegion>> mappedRooms = new Dictionary<double, List<IRegion>>();

            //Map everything
            foreach (IRegion region in regions)
            {
                BoundingBox bbox = region.Perimeter.IBounds();
                double zLevel = Math.Round(bbox.Min.Z, decimals);

                oM.Spatial.SettingOut.Level roundedLevel = roundedLevels.Where(x => x.Elevation == zLevel).FirstOrDefault();
                if (roundedLevel == null)
                {
                    regionsNotByLevel.Add(region);
                    continue; //zLevel does not exist in the search levels
                }

                int levelIndex = roundedLevels.IndexOf(roundedLevel);

                if (levelIndex == -1)
                {
                    regionsNotByLevel.Add(region);
                    continue; //zLevel does not exist in the search levels
                }

                if (!mappedRooms.ContainsKey(levels[levelIndex].Elevation))
                    mappedRooms.Add(levels[levelIndex].Elevation, new List<IRegion>());

                levelsInUse.Add(levels[levelIndex]);

                mappedRooms[levels[levelIndex].Elevation].Add(region);
            }

            foreach (KeyValuePair<double, List<IRegion>> kvp in mappedRooms.OrderBy(x => x.Key))
                regionsByLevel.Add(kvp.Value);

            Output<List<List<IRegion>>, List<oM.Spatial.SettingOut.Level>, List<IRegion>> output = new Output<List<List<IRegion>>, List<oM.Spatial.SettingOut.Level>, List<IRegion>>
            {
                Item1 = regionsByLevel,
                Item2 = levelsInUse.OrderBy(x => x.Elevation).Distinct().ToList(),
                Item3 = regionsNotByLevel,
            };

            if (regionsNotByLevel.Count > 0)
                BH.Engine.Base.Compute.RecordWarning("Some regions were not able to be mapped to a level. See the regionsNotMapped output to examine which regions and resolve any issues");

            return output;
        }

        [Description("Organise the BHoM Region objects into a data structure based on the level of each region")]
        [Input("regions", "A collection of BHoM Regions to be organised.")]
        [Input("decimals", "Provide decimal location to define the degree of tolerance for data matching.")]
        [MultiOutput(0, "regionsByLevel", "A collection of BHoM Regions grouped by levels.")]
        [MultiOutput(1, "levelsInUse", "A sublist of the BHoM Levels that have Room.")]
        [MultiOutput(2, "regionsNotMapped", "A collection of BHoM Regions which did not sit neatly on any of the provided levels")]
        public static Output<List<List<IRegion>>, List<oM.Spatial.SettingOut.Level>, List<IRegion>> MaptoLevel(List<IRegion> regions, int decimals = 6)
        {
            List<oM.Spatial.SettingOut.Level> levels = Create.Levels(regions);
            Output<List<List<IRegion>>, List<oM.Spatial.SettingOut.Level>, List<IRegion>> output = MapToLevel(regions, levels, decimals);

            return output;
        }
    }
}


