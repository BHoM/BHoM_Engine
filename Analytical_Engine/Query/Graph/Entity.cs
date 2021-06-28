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

        [Description("Returns an entity from a Graph, or null if it does not exist.")]
        [Input("graph", "The Graph to extract the entity from.")]
        [Input("entityName", "The name of the entity.")]
        [Output("entity", "The entity as an IBHoMObject.")]
        [PreviousVersion("4.3", "BH.Engine.Analytical.Query.Entities(BH.oM.Analytical.Elements.Graph, System.String>")]
        public static T Entity<T>(this Graph<T> graph, string entityName)
            where T : IBHoMObject
        {
            if (graph == null)
            {
                BH.Engine.Reflection.Compute.RecordError("Cannot query the entities of a null graph.");
                return default(T);
            }

            return graph.Entities.Values.ToList().Find(x => x.Name == entityName);
        }

        /***************************************************/

        [Description("Returns an entity from a Graph, or null if it does not exist.")]
        [Input("graph", "The Graph to extract the entity from.")]
        [Input("entityGuid", "The Guid of the entity.")]
        [Output("entity", "The Guid of the entity.")]
        [PreviousVersion("4.3", "BH.Engine.Analytical.Query.Entities(BH.oM.Analytical.Elements.Graph, System.Guid>")]
        public static T Entity<T>(this Graph<T> graph, Guid entityGuid)
            where T : IBHoMObject
        {
            if (graph == null)
            {
                BH.Engine.Reflection.Compute.RecordError("Cannot query the entities of a null graph.");
                return default(T);
            }

            return graph.Entities[entityGuid]; 
        }
    }
}

