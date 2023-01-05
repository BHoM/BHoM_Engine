/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
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

using BH.oM.Base.Attributes;
using BH.oM.Data.Collections;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Data
{
    public static partial class Query
    {

        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Get the number of items in the Priority Queue Data.")]
        [Input("queue", "A priority queue with data from which to obtain the number of items from.")]
        [Output("count", "The number of items in the priority queue's data.")]
        public static int Count<T>(this PriorityQueue<T> queue) where T : IComparable<T>
        {
            return queue.Data.Count();
        }

        /***************************************************/

        [Description("Get the number of nodes in the graph.")]
        [Input("graph", "The graph from which to obtain the number of nodes it contains.")]
        [Output("count", "The number of nodes in the graph.")]
        public static int Count<T>(this Graph<T> graph) 
        {
            return graph.Nodes.Count();
        }

        /***************************************************/

        [Description("Get the number of children contained by the tree. Takes the sum of the number of values each child of the tree has.")]
        [Input("tree", "The tree to obtain the number of child values from.")]
        [Output("count", "The total number of values held by the children of the tree. Returns 1 if there are no children on the tree.")]
        public static int Count<T>(this Tree<T> tree)
        {
            if (tree.Children.Count == 0)
                return 1;
            else
                return tree.Children.Sum(x => x.Value.Count());
        }

        /***************************************************/

        [Description("Get the total number of objects contained within the Venn Diagram. The count will be the sum of the objects contained solely within each set plus those in the intersection.")]
        [Input("diagram", "The Venn Diagram to count the number of objects for.")]
        [Output("count", "The total number of items in the diagram.")]
        public static int Count<T>(this VennDiagram<T> diagram)
        {
            return diagram.Intersection.Count + diagram.OnlySet1.Count + diagram.OnlySet2.Count;
        }

        /***************************************************/

        [Description("Get the count of items in the list. Returns the total number of objects held within the list.")]
        [Input("list", "The list of objects to obtain the count from.")]
        [Output("count", "The number of items in the list.")]
        public static int Count<T>(this List<T> list)
        {
            return list.Count;
        }
    }
}




