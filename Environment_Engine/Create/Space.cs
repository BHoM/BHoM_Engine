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

using BH.oM.Environment.Elements;
using BH.oM.Environment.SpaceCriteria;

using BH.oM.Reflection.Attributes;
using System.ComponentModel;

using BH.oM.Geometry;

namespace BH.Engine.Environment
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns an Environment Space object")]
        [Input("name", "The name of the space, default empty string")]
        [Input("zones", "A collection of zone names the space is to be included in, default null")]
        [Input("gains", "A collection of gains to be applied to the space, default null")]
        [Input("type", "The type of space from the Space Type enum, default undefined")]
        [Input("location", "A point in 3D space providing a basic location point of the space, default null")]
        [Output("space", "An Environment Space object")]
        [Deprecated("3.0", "Deprecated in favour of default create components produced by BHoM")]
        public static Space Space(string name = "", List<string> zones = null, List<IGain> gains = null, SpaceType type = SpaceType.Undefined, Point location = null)
        {
            zones = zones ?? new List<string>();
            gains = gains ?? new List<IGain>();

            return new Space
            {
                Name = name,
                Zones = zones,
                SpaceType = type,
                Location = location,
            };
        }
    }
}

