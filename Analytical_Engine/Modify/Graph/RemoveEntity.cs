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

using BH.oM.Analytical.Graph;
using BH.oM.Base;
using BH.oM.Base.Attributes;
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
        /****           Public Constructors             ****/
        /***************************************************/

        [Description("Modifies a Graph by removing the specified entity and any dependent relations.")]
        [Input("graph", "The Graph to modify.")]
        [Input("entityToRemove", "The IBHoMObject entity to remove.")]
        [Output("graph", "The modified Graph with the specified entity and any dependent relations removed.")]
        public static Graph RemoveEntity(this Graph graph, IBHoMObject entityToRemove)
        {
            if(graph == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot remove an entity from a null graph.");
                return null;
            }

            if(entityToRemove == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot remove a null entity from a graph.");
                return null;
            }

            graph.RemoveEntity(entityToRemove.BHoM_Guid);
            return graph;
        }
        /***************************************************/

        [Description("Modifies a Graph by removing the specified entity and any dependent relations.")]
        [Input("graph", "The Graph to modify.")]
        [Input("entityToRemove", "The Guid of the entity to remove.")]
        [Output("graph", "The modified Graph with the specified entity and any dependent relations removed.")]
        public static Graph RemoveEntity(this Graph graph, Guid entityToRemove)
        {
            if (graph == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot remove an entity from a null graph.");
                return null;
            }

            if (graph.Entities.ContainsKey(entityToRemove))
            {
                List<IRelation> relations = graph.Relations.FindAll(rel => rel.Source.Equals(entityToRemove) || rel.Target.Equals(entityToRemove)).ToList();
                graph.Relations = graph.Relations.Except(relations).ToList();
                graph.Entities.Remove(entityToRemove);
            }
            else
                Base.Compute.RecordWarning("Entity was not found in the graph.");

            return graph;
        }
    }
}





