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
        [Input("entities", "A collection of IBHoMObjects to try and find unique instances.")]
        [Input("propertiesToConsider", "Property names to consider when attempting to determine unique entities.")]
        [Input("decimalPlaces", "The decimal places to consider when comparing double, decimal and float types.")]
        [Output("unique entities", "A Dictionary replacement map of the entities where the keys are the Guid of the original entity and the Values the matching IBHoMObject entity.")]
        
        public static Dictionary<Guid, IBHoMObject> DiffEntities(List<IBHoMObject> entities, List<string> propertiesToConsider = null, int decimalPlaces = 12)
        {
            
            Dictionary<Guid, IBHoMObject> replaceMap = new Dictionary<Guid, IBHoMObject>();
            
            foreach (IBHoMObject entityA in entities)
            {
                foreach (IBHoMObject entityB in entities)
                {
                    if(entityA.GetType() == entityB.GetType())
                    {
                        //bool match = CompareEntities(entityA, entityB, propertiesToConsider , decimalPlaces)
                        List<string> props = new List<string>()
                        {
                            "Position.X",
                            "Position.Y",
                            "Position.Z",
                            "StartNode.Position.X",
                            "StartNode.Position.Y",
                            "StartNode.Position.Z",
                            "EndNode.Position.X",
                            "EndNode.Position.Y",
                            "EndNode.Position.Z",

                        };
                        DiffConfig diffConfig = new DiffConfig()
                        {
                            PropertiesToConsider = props,
                            NumericTolerance = 0.001,
                        };
                        Dictionary<string, Tuple<object, object>> modifiedProps = Diffing.Query.DifferentProperties(entityA, entityB, diffConfig);
                        if (modifiedProps == null)
                        {
                            //matched entities
                            replaceMap[entityA.BHoM_Guid] = entityB;
                            break;
                        }
                    }
                }
                
            }
            return replaceMap;
        }

        /***************************************************/
        private static bool CompareEntities(IBHoMObject entityA, IBHoMObject entityB, List<string> propertiesToConsider = null, int decimalPlaces = 12)
        {
            return HashEntity(entityA, propertiesToConsider , decimalPlaces) == HashEntity(entityB, propertiesToConsider, decimalPlaces);
        }

        /***************************************************/

        private static string HashEntity(IBHoMObject entity, List<string> propertiesToConsider = null, int decimalPlaces = 12)
        {
            List<string> propertiesToIgnore = BH.Engine.Reflection.Query.PropertyNames(entity).Except(propertiesToConsider).ToList();

            // The current Hash must not be considered when computing the hash. Remove HashFragment if present. 
            IBHoMObject bhomobj = BH.Engine.Base.Query.DeepClone(entity);
            bhomobj.Fragments.Remove(typeof(HashFragment));
            
            Dictionary<string, int> fractionalDigitsPerProperty = new Dictionary<string, int>();
            //fractionalDigitsPerProperty: fractionalDigitsPerProperty
            //assign decimal places to hash input

            propertiesToConsider.ForEach(p => fractionalDigitsPerProperty.Add(p, decimalPlaces));

            return "";// Base.Compute.Hash(bhomobj, propertiesToIgnore );
        }
        /***************************************************/
    }

}
