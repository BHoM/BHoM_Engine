/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
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

using BH.Engine.Geometry;
using BH.oM.Analytical.Graph;
using BH.oM.Base;
using BH.oM.Geometry;
using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Analytical
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns the collection of entity Guids that are never used as Relation targets or sources.")]
        [Input("graph", "The Graph to search.")]
        [Output("isolated entities", "The collection of entity Guids that are isolated.")]
        public static List<Guid> IsolatedEntities(this Graph graph)
        {
            if(graph == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the isolated entities of a null graph.");
                return new List<Guid>();
            }

            HashSet<Guid> relationGuids = new HashSet<Guid>();

            foreach (Relation relation in graph.Relations)
            {
                relationGuids.Add(relation.Source);
                relationGuids.Add(relation.Target);
            }

            return graph.Entities.Keys.Where(x => !relationGuids.Contains(x)).ToList();

        }

        /***************************************************/
    }

}




