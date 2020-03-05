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

using BH.oM.Data.Collections;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Data
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/
        public static Graph<T> GetSubGraph<T>(this Graph<T> graph, GraphNode<T> rootnode)
        {
            Graph<T> subgraph = new Graph<T>();
            subgraph.Nodes.Add(rootnode);
            List<GraphLink<T>> links = graph.Links.FindAll(x => x.EndNode == rootnode);
            GetChildren<T>(links, graph, ref subgraph);
            return subgraph;
        }
        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/
        private static void GetChildren<T>(List<GraphLink<T>> links, Graph<T> graph, ref Graph<T> subgraph)
        {
            foreach (GraphLink<T> link in links)
            {
                links = graph.Links.FindAll(x => x.EndNode == link.StartNode);
                GetChildren<T>(links, graph, ref subgraph);
                subgraph.Nodes.Add(link.StartNode);
                subgraph.Links.Add(link);
            }
        }
    }
}