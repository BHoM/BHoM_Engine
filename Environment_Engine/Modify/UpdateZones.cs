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

using BH.oM.Environment.Elements;
using BH.oM.Reflection.Attributes;
using System.ComponentModel;

namespace BH.Engine.Environment
{
    public static partial class Modify
    {
        [Description("Updates the zones in a space by replacing or adding to the old zones with a new list of zones.")]
        [Input("space", "The space to update with the new zones.")]
        [Input("zones", "A list of zones to add to the space.")]
        [Input("replace", "If set to true the already existing zones will be replaced, if false the new zones will be added to the old.")]
        [Output("spaceWithZones", "Space with the new zones.")]
        public static Space UpdateZones(this Space space, List<string> zones, bool replace = false)
        {
            if (replace)
                space.Zones = zones;
            else
                space.Zones.AddRange(zones);

            return space;
        }
    }
}
