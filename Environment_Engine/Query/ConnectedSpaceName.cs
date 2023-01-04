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

using System.Linq;
using System.Collections.Generic;
using BH.oM.Environment.Elements;
using BH.oM.Geometry;
using BH.oM.Base;
using BH.oM.Environment.Fragments;

using BH.oM.Base.Attributes;
using System.ComponentModel;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        [Description("Returns the name of the space the panels are enclosing")]
        [Input("panels", "A collection of Environment Panels")]
        [Output("spaceName", "The space name the panels are jointly connected to")]
        public static string ConnectedSpaceName(this List<Panel> panels)
        {
            //Gets the single space name which most commonly unites these panels
            List<string> uniqueNames = panels.UniqueSpaceNames();

            Dictionary<string, int> nameCount = new Dictionary<string, int>();

            foreach (string s in uniqueNames)
                nameCount.Add(s, 0);

            foreach (Panel be in panels)
            {
                foreach (string name in be.ConnectedSpaces)
                {
                    if (name != "-1")
                        nameCount[name]++;
                }
            }

            return nameCount.Where(x => x.Value == nameCount.Max(y => y.Value)).FirstOrDefault().Key;
        }
    }
}




