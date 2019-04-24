/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2019, the respective contributors. All rights reserved.
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

using BH.oM.Environment.Properties;

using BH.oM.Reflection.Attributes;
using System.ComponentModel;

namespace BH.Engine.Environment
{
    public static partial class Create
    {
        [Description("Returns an Space Context Fragment object")]
        [Input("name", "The name of the fragment property, default empty string")]
        [Input("isExternal", "Defines whether the space is externally facing, default false")]
        [Input("colour", "Defines the colour of the space, default empty string")]
        [Input("connectedElements", "A collection of the elements which enclose the space described by their name or unique identifer as a string, default null")]
        [Output("spaceContextFragment", "A Space Context Fragment object - this can be added to an Environment Space")]
        public static SpaceContextFragment SpaceContextFragment(string name = "", bool isExternal = false, string colour = "", List<string> connectedElements = null)
        {
            connectedElements = connectedElements ?? new List<string>();

            return new SpaceContextFragment
            {
                Name = name,
                IsExternal = isExternal,
                Colour = colour,
                ConnectedElements = connectedElements,
            };
        }
    }
}
