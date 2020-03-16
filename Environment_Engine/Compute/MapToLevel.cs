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
        [Description("Organize the BHoM Room object into a data structure based on the level of each room.")]
        [Input("rooms", "A collection of BHoM Rooms to be organized.")]
        [Input("levels", "A collection of BHoM Levels.")]
        [Input("decimals", "Provide decimal location to define the degree of tolerance for data matching.")]
        [MultiOutput(0, "roomsByLevel", "A collection of BHoM Rooms group by levels.")]
        [MultiOutput(1, "levelsInUse", "A sublist of the BHoM Levels that have Room.")]
        public static Output<List<List<Room>>, List<oM.Geometry.SettingOut.Level>> MapToLevel(List<Room> rooms, List<oM.Geometry.SettingOut.Level> levels, int decimals = 6)
        {
            List<List<Room>> roomsByLevel = new List<List<Room>>();
            List<oM.Geometry.SettingOut.Level> levelsInUse = new List<oM.Geometry.SettingOut.Level>();

            List<oM.Geometry.SettingOut.Level> roundedLevels = new List<oM.Geometry.SettingOut.Level>();

            for (int x = 0; x < levels.Count; x++)
            {
                oM.Geometry.SettingOut.Level lvl = new oM.Geometry.SettingOut.Level { Elevation = Math.Round(levels[x].Elevation, decimals), Name = levels[x].Name, Fragments = levels[x].Fragments, CustomData = levels[x].CustomData };
                roundedLevels.Add(lvl); //Round the levels
            }

            List<oM.Geometry.SettingOut.Level> searchLevels = new List<oM.Geometry.SettingOut.Level>();

            //Loop through each surface
            foreach (Room room in rooms)
            {
                BoundingBox bbox = room.Perimeter.IBounds();
                double zLevel = Math.Round(bbox.Min.Z, decimals);

                if (roundedLevels.Where(x => x.Elevation == zLevel).FirstOrDefault() != null && searchLevels.Where(x => x.Elevation == zLevel).FirstOrDefault() == null)
                    searchLevels.Add(roundedLevels.Where(x => x.Elevation == zLevel).FirstOrDefault());                    
            }

            Dictionary<double, List<Room>> mappedRooms = new Dictionary<double, List<Room>>();

            //Map everything
            foreach (Room room in rooms)
            {
                BoundingBox bbox = room.Perimeter.IBounds();
                double zLevel = Math.Round(bbox.Min.Z, decimals);
                int levelIndex = roundedLevels.IndexOf(roundedLevels.Where(x => x.Elevation == zLevel).First());

                if (levelIndex == -1) //zLevel does not exist in the search levels
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Room with ID " + room.BHoM_Guid + " does not sit on any provided level");
                    continue;
                }

                if (!mappedRooms.ContainsKey(levelIndex))
                    mappedRooms.Add(levelIndex, new List<Room>());

                levelsInUse.Add(levels[levelIndex]);

                mappedRooms[levelIndex].Add(room);
            }

            foreach (KeyValuePair<double, List<Room>> kvp in mappedRooms)
                roomsByLevel.Add(kvp.Value);

            Output<List<List<Room>>, List<oM.Geometry.SettingOut.Level>> output = new Output<List<List<Room>>, List<oM.Geometry.SettingOut.Level>>
            {
                Item1 = roomsByLevel,
                Item2 = levelsInUse.Distinct().ToList(),
            };

            return output;
        }
    }
}
