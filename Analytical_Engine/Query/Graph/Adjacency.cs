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

using BH.Engine.Base;
using BH.oM.Analytical.Graph;
using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Analytical
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns the adjacency dictionary for a Graph.")]
        [Input("graph", "The Graph to represent as an adjacency dictionary.")]
        [Input("relationDirection", "Optional RelationDirection used to determine the direction that relations can be traversed. Defaults to Forwards indicating traversal is from source to target.")]
        [Output("adjacency", "The Dictionary where the keys are entities and the values are the collection of adjacent entities.")]
        public static Dictionary<Guid, List<Guid>> Adjacency(this Graph graph, RelationDirection relationDirection = RelationDirection.Forwards)
        {
            if (graph == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the adjacency of a null graph.");
                return new Dictionary<Guid, List<Guid>>();
            }

            var sourceLookup = graph.Relations.ToLookup(x => x.Source, x => x.Target);
            var targetLookup = graph.Relations.ToLookup(x => x.Target, x => x.Source);

            Dictionary<Guid, List<Guid>> adjacency = new Dictionary<Guid, List<Guid>>();

            foreach (Guid entity in graph.Entities.Keys.ToList())
            {
                List<Guid> connected = new List<Guid>();
                switch (relationDirection)
                {
                    case RelationDirection.Forwards:
                        connected.AddRange(sourceLookup[entity]);
                        break;
                    case RelationDirection.Backwards:
                        connected.AddRange(targetLookup[entity]);
                        break;
                    case RelationDirection.Both:
                        connected.AddRange(sourceLookup[entity]);
                        connected.AddRange(targetLookup[entity]);
                        break;
                }

                //keep unique only
                adjacency[entity] = connected.Distinct().ToList();

            }
            return adjacency;
        }
    }
}



