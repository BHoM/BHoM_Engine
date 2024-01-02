/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
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

using BH.oM.Environment;
using BH.oM.Environment.Elements;
using BH.oM.Environment.Fragments;
using BH.oM.Base;

using BH.oM.Base.Attributes;
using System.ComponentModel;

using BH.Engine.Base;

using BH.oM.Spatial.SettingOut;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns the floor heights for a collection of levels, calculated as the difference between elevations in levels. Levels are sorted by their elevation prior to calculating the floor heights, and the output order will be from lowest elevation floor to highest")]
        [Input("levels", "A collection of levels with elevations. The floor height will be calculated as the difference between two consecutive level elevations")]
        [Input("roofHeight", "The height/elevation of the roof the building to provide the floor height for the top-most floor level provided")]
        [Output("floorHeights", "A collection of floor heights based on the elevation differences")]
        public static List<double> FloorHeights(this List<Level> levels, double roofHeight)
        {
            List<double> elevations = levels.Select(x => x.Elevation).ToList();
            elevations.Sort();

            List<double> heights = new List<double>();

            for (int x = 0; x < elevations.Count - 1; x++)
                heights.Add(elevations[x + 1] - elevations[x]); //Last level will be handled separately so for loop is valid to go to (count - 1)

            heights.Add(roofHeight - elevations.Last()); //Add last level based on roof height

            return heights;
        }
    }
}




