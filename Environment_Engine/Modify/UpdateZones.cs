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

using BH.oM.Environment.Elements;
using BH.oM.Base.Attributes;
using System.ComponentModel;

namespace BH.Engine.Environment
{
    public static partial class Modify
    {
        [Description("Updates the zones in a space by replacing or adding to the old zones with a list of new zones.")]
        [Input("space", "The space to update with the zones of.")]
        [Input("zones", "A list of zones to add to the space.")]
        [Input("replace", "If set to true then any already existing zones will be replaced. If false the new zones will be added to the existing zones. Default false")]
        [Output("space", "Space with the updated zones.")]
        public static Space UpdateZones(this Space space, List<string> zones, bool replace = false)
        {
            if(space == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot update the zones of a null space.");
                return space;
            }

            if (replace)
                space.Zones = zones;
            else
                space.Zones.AddRange(zones);

            space.Zones = space.Zones.Distinct().ToList();

            return space;
        }
    }
}


