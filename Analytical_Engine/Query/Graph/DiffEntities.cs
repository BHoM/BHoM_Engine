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
        [Input("diffConfig", "Configuration of diffing used to find unique entities.")]
        [Output("unique entities", "A Dictionary replacement map of the entities where the keys are the Guid of the original entity and the Values the matching IBHoMObject entity.")]
        public static Dictionary<Guid, IBHoMObject> DiffEntities(this List<IBHoMObject> entities, DiffConfig diffConfig = null)
        {
            Dictionary<Guid, IBHoMObject> replaceMap = new Dictionary<Guid, IBHoMObject>();
            Dictionary<IBHoMObject, string> objectHash = new Dictionary<IBHoMObject, string>();

            //remove old hashes
            entities.ForEach(ent => ent.RemoveFragment(typeof(HashFragment)));

            //find decimal places from numeric tolerance
            int decimalPlaces = DecimalPlaces(diffConfig.NumericTolerance);

            //generate hashes
            entities.ForEach(ent => objectHash.Add(ent, ent.HashEntity(diffConfig, decimalPlaces)));

            foreach (KeyValuePair<IBHoMObject, string> entityA in objectHash)
            {
                foreach (KeyValuePair<IBHoMObject, string> entityB in objectHash)
                {
                    //only if same object type
                    if(entityA.Key.GetType() == entityB.Key.GetType())
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

        /***************************************************/
        /****           Private Methods                 ****/
        /***************************************************/

        private static string HashEntity(this IBHoMObject entity, DiffConfig diffConfig, int decimalPlaces)
        {
            if (diffConfig == null)
                diffConfig = new DiffConfig();

            List<string> propertiesToIgnore = BH.Engine.Reflection.Query.PropertyNames(entity).Except(diffConfig.PropertiesToConsider).ToList();

            return Base.Query.Hash(entity, propertyNameExceptions: propertiesToIgnore, fractionalDigits: decimalPlaces);
        }

        /***************************************************/

        private static int DecimalPlaces(double numericTolerance)
        {
            //horrible conversion from numeric tolerance to precision needed for Hashing 
            int precision = 0;

            while ((decimal)numericTolerance * (decimal)Math.Pow(10, precision) !=
                    Math.Round((decimal)numericTolerance * (decimal)Math.Pow(10, precision)))
                precision++;

            return precision;
        }
    }

}
