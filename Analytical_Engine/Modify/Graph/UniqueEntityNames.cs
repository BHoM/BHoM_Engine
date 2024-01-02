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

using BH.oM.Analytical.Elements;
using BH.oM.Base;
using BH.oM.Base.Attributes;
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
        [Input("graph", "The Graph to modify.")]
        [Output("graph", "The modified graph.")]
        public static Graph UniqueEntityNames(this Graph graph)
        {
            if(graph == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot modify a null graph.");
                return null;
            }

            List<IBHoMObject> entities = graph.Entities.Values.ToList();
            List<string> distinctNames = entities.Select(x => x.Name).Distinct().ToList();

            foreach (string name in distinctNames)
            {
                List<IBHoMObject> matchentities = entities.FindAll(x => x.Name == name);
                if (matchentities.Count > 1)
                {
                    for (int i = 0; i < matchentities.Count; i++)
                        matchentities[i].Name += "_" + i;
                }
            }
            return graph;
        }
        /***************************************************/
    }
}




