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

using BH.Engine.Base;
using BH.oM.Analytical.Elements;
using BH.oM.Base;
using BH.oM.Diffing;
using BH.Engine.Diffing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using BH.oM.Reflection.Attributes;

namespace BH.Engine.Analytical
{
    public static partial class Query
    {
        /***************************************************/
        /****           Public Methods                  ****/
        /***************************************************/

        [Description("Identifies unique objects from a collection IBHoMObjects using hash comparison.")]
        [Input("entities", "A collection of IBHoMObjects from which unique instances are identified.")]
        [Input("comparisonConfig", "Configuration of diffing used to find unique entities.")]
        [Output("replacementMap", "A Dictionary replacement map of the entities where the keys are the Guid of the original entity and the Values the matching IBHoMObject entity.")]
        [PreviousVersion("4.3", "BH.Engine.Analytical.Query.UniqueEntitiesReplacementMap(System.Collections.Generic.List<BH.oM.Base.IBHoMObject>, BH.oM.Base.ComparisonConfig")]
        public static Dictionary<Guid, T> UniqueEntitiesReplacementMap<T>(this List<T> entities, ComparisonConfig comparisonConfig = null)
            where T : IBHoMObject
        {
            ComparisonConfig cc = comparisonConfig ?? new ComparisonConfig();

            Dictionary<Guid, T> replaceMap = new Dictionary<Guid, T>();
            Dictionary<T, string> entitiesHash = new Dictionary<T, string>();

            // populate dictionary
            entities.ForEach(ent => entitiesHash.Add(ent, ent.Hash(cc)));

            foreach (KeyValuePair<T, string> entityA in entitiesHash)
            {
                foreach (KeyValuePair<T, string> entityB in entitiesHash)
                {
                    //only if same object type
                    if (entityA.Key.GetType() == entityB.Key.GetType())
                    {
                        //compare hashes
                        if (entityA.Value == entityB.Value)
                        {
                            //store in map dictionary where key is original Guid and Value is replacement object
                            replaceMap[entityA.Key.BHoM_Guid] = entityB.Key;

                            //first match has been found so break inner loop.
                            break;
                        }
                    }
                }
            }
            return replaceMap;
        }
    }
}

