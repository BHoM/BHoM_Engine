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
using BH.oM.Reflection.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Analytical
{
    public static partial class Modify
    {
        /***************************************************/
        /****           Public Methods                  ****/
        /***************************************************/

        [Description("Enforce unique entities on a Graph. Source and Target properties of relations are updated to match the unique entities.")]
        [Input("graph", "The Graph from which duplicate entities should be removed.")]
        [Input("replaceMap", "A Dictionary providing the replacement mapping, where Key is replaced with Value.")]
        [Output("graph", "The Graph with unique entities.")]
        [PreviousVersion("4.2", "BH.Engine.Analytical.Modify.UniqueEntities(BH.oM.Analytical.Elements.Graph, System.Collections.Generic.Dictionary<System.Guid,BH.oM.Base.IBHoMObject>")]
        public static Graph<T> UniqueEntities<T>(this Graph<T> graph, Dictionary<Guid, T> replaceMap)
            where T : IBHoMObject
        {
            Dictionary<Guid, T> uniqueEntities = new Dictionary<Guid, T>();

            foreach (KeyValuePair<Guid, T> kvp in graph.Entities)
            {
                T unique = replaceMap[kvp.Key];
                if (!uniqueEntities.ContainsKey(unique.BHoM_Guid))
                    uniqueEntities.Add(unique.BHoM_Guid, unique);
            }

            graph.Entities = uniqueEntities;

            List<IRelation<T>> uniqueRelations = new List<IRelation<T>>();
            foreach (IRelation<T> relation in graph.Relations)
            {
                IRelation<T> relation1 = relation.UniqueEntities(replaceMap);

                //keep if it does not already exist
                if(!uniqueRelations.Any(r => r.Source.Equals(relation1.Source) && r.Target.Equals(relation1.Target)))
                    uniqueRelations.Add(relation1);

            }
            graph.Relations = uniqueRelations;
            return graph;
        }

        /***************************************************/
        /****           Private Methods                 ****/
        /***************************************************/

        private static IRelation<T> UniqueEntities<T>(this IRelation<T> relation, Dictionary<Guid, T> replaceMap)
            where T : IBHoMObject
        {
            if(replaceMap.ContainsKey(relation.Source))
                relation.Source = replaceMap[relation.Source].BHoM_Guid;
            else
                Reflection.Compute.RecordError($"The Source reference on IRelation of type {relation.GetType().ToString()} cannot be found in the entities provided. Check all required entities have been included.");
                
            if(replaceMap.ContainsKey(relation.Target))
                relation.Target = replaceMap[relation.Target].BHoM_Guid;
            else
                Reflection.Compute.RecordError($"The Target reference on IRelation of type {relation.GetType().ToString()} cannot be found in the entities provided. Check all required entities have been included.");
            
            //go deeper into making the subgraph unique is an option for future use
            //relation.Subgraph.UniqueEntities(replaceMap);
            return relation;
        }
    }
}

