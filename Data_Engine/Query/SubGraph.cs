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
using System.ComponentModel;
using BH.oM.Reflection.Attributes;
using System;

namespace BH.Engine.Data
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/
        [Description("Extracts a sub graph from a graph")]
        [Input("graph", "The graph to extract the sub graph from")]
        [Input("rootNode", "The graph node that is the root of the sub graph")]
        public static Graph<T> SubGraph<T>(this Graph<T> graph, GraphNode<T> rootNode)
        {
            Graph<T> subgraph = new Graph<T>();
            subgraph.Nodes.Add(rootNode);
            if (!graph.Nodes.Contains(rootNode))
                throw new ArgumentException("rootnode provided cannot be found in the original graph. Ensure the node is from the original graph");
            List<GraphLink<T>> links = graph.Links.FindAll(x => x.EndNode.Value.Equals(rootNode.Value));
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
                GetChildren(links, graph, ref subgraph);
                subgraph.Nodes.Add(link.StartNode);
                subgraph.Links.Add(link);
            }
        }
    }
}