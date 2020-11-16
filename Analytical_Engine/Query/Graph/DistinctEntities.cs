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
        [Input("distinctConfig", "Configuration of diffing used to find unique entities.")]
        [Output("unique entities", "A Dictionary replacement map of the entities where the keys are the Guid of the original entity and the Values the matching IBHoMObject entity.")]
        public static Dictionary<Guid, IBHoMObject> DistinctEntities(this List<IBHoMObject> entities, ComparisonConfig distinctConfig = null) // this is not a diff, it's finding unique entities
        {
            ComparisonConfig dc = distinctConfig ?? new ComparisonConfig();

            Dictionary<Guid, IBHoMObject> replaceMap = new Dictionary<Guid, IBHoMObject>();
            Dictionary<IBHoMObject, string> objectHash = new Dictionary<IBHoMObject, string>();

            HashComparer<object> hashComparer = new HashComparer<object>(dc);

            foreach (KeyValuePair<IBHoMObject, string> entityA in objectHash)
            {
                foreach (KeyValuePair<IBHoMObject, string> entityB in objectHash)
                {
                    //only if same object type
                    if (entityA.Key.GetType() == entityB.Key.GetType())
                    {
                        //compare hashes
                        if (hashComparer.Equals(entityA.Value, entityB.Value))
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
