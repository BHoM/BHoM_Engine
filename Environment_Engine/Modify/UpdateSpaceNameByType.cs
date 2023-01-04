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
using System.ComponentModel;

using BH.oM.Environment.Elements;
using BH.oM.Base.Attributes;

namespace BH.Engine.Environment
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns a list of Environment Spaces with the provided name being existing space name + '_spaceType', for example 1A_Bedroom.\nSets a unique name by numbers if multiple spaces are of the same type, i.e. 1A_Bedroom1.")]
        [Input("spaces", "A collection of Environment Spaces to set the name for.")]
        [Output("spaces", "A collection of modified Environment Spaces with assigned name.")]
        public static void UpdateSpaceNameByType(this List<Space> spaces)
        {
            if (spaces == null)
                return;

            List<Space> spacesWithNames = new List<Space>();
            foreach (Space s in spaces)
            {
                string name = s.Name + "_" + s.SpaceType.ToString();
                if (spaces.Where(x => x.SpaceType == s.SpaceType).Count() > 1)
                {
                    int current = spacesWithNames.Where(x => x.Name.StartsWith(name)).Count();
                    name += (current + 1).ToString();
                }

                s.Name = name;
                spacesWithNames.Add(s);
            }
        }
    }
}


