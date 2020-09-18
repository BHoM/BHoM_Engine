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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Reflection;
using BH.oM.Geometry;
using BH.oM.Environment;
using BH.oM.Architecture.Elements;
using BH.oM.Geometry.SettingOut;

using BH.Engine.Geometry;

using System.ComponentModel;
using BH.oM.Reflection.Attributes;

namespace BH.Engine.Environment
{
    public static partial class Compute
    {
        [Description("Organise the BHoM Room object into a data structure based on the level of each room.")]
        [Input("rooms", "A collection of BHoM Rooms to be organized.")]
        [Input("levels", "A collection of BHoM Levels.")]
        [Input("decimals", "Provide decimal location to define the degree of tolerance for data matching.")]
        [MultiOutput(0, "roomsByLevel", "A collection of BHoM Rooms group by levels.")]
        [MultiOutput(1, "levelsInUse", "A sublist of the BHoM Levels that have Room.")]
        [MultiOutput(2, "roomsNotMapped", "A collection of BHoM Rooms which did not sit neatly on any of the provided levels")]
        public static Output<List<List<Room>>, List<oM.Geometry.SettingOut.Level>, List<Room>> MapToLevel(List<Room> rooms, List<oM.Geometry.SettingOut.Level> levels, int decimals = 6)
        {
            List<List<Room>> roomsByLevel = new List<List<Room>>();
            List<oM.Geometry.SettingOut.Level> levelsInUse = new List<oM.Geometry.SettingOut.Level>();
            List<Room> roomsNotByLevel = new List<Room>();
            List<oM.Geometry.SettingOut.Level> roundedLevels = new List<oM.Geometry.SettingOut.Level>();

            for (int x = 0; x < levels.Count; x++)
            {
                oM.Geometry.SettingOut.Level lvl = new oM.Geometry.SettingOut.Level { Elevation = Math.Round(levels[x].Elevation, decimals), Name = levels[x].Name, Fragments = levels[x].Fragments, CustomData = levels[x].CustomData };
                roundedLevels.Add(lvl); //Round the levels
            }

            Dictionary<double, List<Room>> mappedRooms = new Dictionary<double, List<Room>>();

            //Map everything
            foreach (Room room in rooms)
            {
                BoundingBox bbox = room.Perimeter.IBounds();
                double zLevel = Math.Round(bbox.Min.Z, decimals);

                oM.Geometry.SettingOut.Level roundedLevel = roundedLevels.Where(x => x.Elevation == zLevel).FirstOrDefault();
                if(roundedLevel == null)
                {
                    roomsNotByLevel.Add(room);
                    continue; //zLevel does not exist in the search levels
                }

                int levelIndex = roundedLevels.IndexOf(roundedLevel);

                if (levelIndex == -1)
                {
                    roomsNotByLevel.Add(room);
                    continue; //zLevel does not exist in the search levels
                }

                if (!mappedRooms.ContainsKey(levels[levelIndex].Elevation))
                    mappedRooms.Add(levels[levelIndex].Elevation, new List<Room>());

                levelsInUse.Add(levels[levelIndex]);

                mappedRooms[levels[levelIndex].Elevation].Add(room);
            }

            foreach (KeyValuePair<double, List<Room>> kvp in mappedRooms.OrderBy(x => x.Key))
                roomsByLevel.Add(kvp.Value);

            Output<List<List<Room>>, List<oM.Geometry.SettingOut.Level>, List<Room>> output = new Output<List<List<Room>>, List<oM.Geometry.SettingOut.Level>, List<Room>>
            {
                Item1 = roomsByLevel,
                Item2 = levelsInUse.OrderBy(x => x.Elevation).Distinct().ToList(),
                Item3 = roomsNotByLevel,
            };

            if(roomsNotByLevel.Count > 0)
                BH.Engine.Reflection.Compute.RecordWarning("Some rooms were not able to be mapped to a level. See the roomsNotMapped output to examine which rooms and resolve any issues");

            return output;
        }
    }
}
