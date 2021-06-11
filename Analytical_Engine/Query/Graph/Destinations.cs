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

using BH.Engine.Geometry;
using BH.oM.Analytical.Elements;
using BH.oM.Base;
using BH.oM.Geometry;
using BH.oM.Reflection.Attributes;
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

        [Description("Returns the collection of entity guids that can be accessed from a given entity.")]
        [Input("graph", "The Graph to search.")]
        [Input("entity", "The Guid of the entity for which the destinations are required.")]
        [Output("entities", "The collection of guids of the destination entities.")]
        public static List<Guid> Destinations<T>(this Graph<T> graph, Guid entity)
            where T : IBHoMObject
        {
            if(graph == null)
            {
                BH.Engine.Reflection.Compute.RecordError("Cannot query the destinations of a null graph.");
                return new List<Guid>();
            }

            return graph.Relations.Where(r => r.Source.Equals(entity)).Select(r => r.Target).ToList();
        }

        /***************************************************/

        [Description("Returns the collection of IBHoMObject entities that can be accessed from a given entity.")]
        [Input("graph", "The Graph to search.")]
        [Input("entity", "The IBHoMObject entity for which the destinations are required.")]
        [Output("entities", "The collection of IBHoMObjects of the destination entities.")]
        public static List<T> Destinations<T>(this Graph<T> graph, IBHoMObject entity)
             where T : IBHoMObject
        {
            if (graph == null)
            {
                BH.Engine.Reflection.Compute.RecordError("Cannot query the destinations of a null graph.");
                return new List<T>();
            }

            if(entity == null)
            {
                BH.Engine.Reflection.Compute.RecordError("Cannot query the destinations of a graph when the entity is null.");
                return new List<T>();
            }

            List<T> destinations = new List<T>();
            foreach (Guid g in graph.Destinations(entity.BHoM_Guid))
                destinations.Add(graph.Entities[g]);
            return destinations;
        }
    }
    
}

