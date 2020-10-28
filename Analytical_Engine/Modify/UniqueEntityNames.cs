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

using BH.oM.Base;
using BH.oM.Reflection.Attributes;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Analytical
{
    public static partial class Modify
    {
        /***************************************************/
        /****           Public Methods                  ****/
        /***************************************************/

        [Description("Enforce unique entity names on a collection of entities.")]
        [Input("entities", "A collection of IBHoMObject entities to enforce unique names.")]

        public static void UniqueEntityNames(this List<IBHoMObject> entities)
        {
            
            List<string> distinctNames = entities.Select(x => x.Name).Distinct().ToList();

            foreach (string name in distinctNames)
            {
                List<IBHoMObject> matchnodes = entities.FindAll(x => x.Name == name);
                if (matchnodes.Count > 1)
                {
                    for (int i = 0; i < matchnodes.Count; i++)
                        matchnodes[i].Name += "_" + i;
                }
            }    
        }
        /***************************************************/
    }
}
