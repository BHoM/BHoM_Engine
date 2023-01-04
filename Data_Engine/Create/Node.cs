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

using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.oM.Base;
using BH.oM.Geometry;
using BH.oM.Data.Collections;

namespace BH.Engine.Data
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Passes the data down through the tree to the leaves and creates the parent nodes on the way back up. " +
                     "i.e. The data will be stored in the leaves and the parent node can be defined through its children.")]
        [Input("dataItems", "The data to store in the Node tree, formatted as leaf nodes.")]
        [Input("splitDataMethod", "Method clustering a collection of leaves into two collections based on their data.")]
        [Input("setChildrenToNode", "Method which creates a parent node of the provided nodes and assignes them as children. " +
                                    "Called last, any data assigned within to the parent will be avalible for use on the children it operates on due to recursion.")]
        [Input("treeDegree", "The number of child nodes for each node.")]
        [Input("leafSize", "The number of siblings a leaf node can have.")]
        [Output("node", "Root node for a data tree with all the data in its leaves and with nodes defined by their children.")]
        public static TNode Node<TNode, T>(IEnumerable<TNode> dataItems,
                                        Func<IEnumerable<TNode>, Output<List<TNode>, List<TNode>>> splitDataMethod,
                                        Func<IEnumerable<TNode>, TNode> setChildrenToNode,
                                        int treeDegree = 16, int leafSize = 16) where TNode : INode<T>
        {
            treeDegree = Math.Max(treeDegree, 2);

            Func<IEnumerable<TNode>, IEnumerable<IEnumerable<TNode>>> partitionMethod =
                (data) =>
                {
                    List<List<TNode>> subLists = new List<List<TNode>>() { data.ToList() };
                    while (subLists.Count < treeDegree)
                    {
                        // Find the one with the most items
                        int index = LargestList(subLists);

                        Output<List<TNode>, List<TNode>> split = splitDataMethod(subLists[index]);

                        subLists.RemoveAt(index);

                        // One empty and one full list implies singular data, which can't be diffirentianted by this Node
                        if (!split.Item1.Any())
                        {
                            subLists.Add(split.Item2);
                            break;
                        }
                        else if (!split.Item2.Any())
                        {
                            subLists.Add(split.Item1);
                            break;
                        }
                        else
                        {
                            subLists.Add(split.Item1);
                            subLists.Add(split.Item2);
                        }
                    }
                    return subLists;
                };


            return Node<TNode, T>(dataItems, partitionMethod, setChildrenToNode, leafSize);
        }

        /***************************************************/

        [Description("Passes the data down through the tree to the leaves and creates the parent nodes on the way back up. " +
                     "i.e. The data will be stored in the leaves and the parent node can be defined through its children.")]
        [Input("dataItems", "The data to store in the Node tree, formatted as leaf nodes.")]
        [Input("partitionMethod", "Method to separate a flat collection of data into the data collections for the child nodes. " +
                                  "Number of collections returned will be the degree of the tree. Breaks the recursion when it only returns a single collection.")]
        [Input("setChildrenToNode", "Method which creates a parent node of the provided nodes and assignes them. " +
                                    "Called last, any data assigned within to the parent will be avalible for use on the children it operates on due to recursion.")]
        [Input("leafSize", "The number of siblings a leaf node can have.")]
        [Output("node", "Root node for a data tree with all the data in its leaves and with nodes defined by their children.")]
        public static TNode Node<TNode, T>(IEnumerable<TNode> dataItems,
                                            Func<IEnumerable<TNode>, IEnumerable<IEnumerable<TNode>>> partitionMethod,
                                            Func<IEnumerable<TNode>, TNode> setChildrenToNode,
                                            int leafSize = 16) where TNode : INode<T>
        {

            leafSize = Math.Max(leafSize, 2);

            if (dataItems != null && dataItems.Skip(leafSize).Any())
            {
                // Partition the data where each collection will form a child Node
                IEnumerable<IEnumerable<TNode>> subLists = partitionMethod(dataItems);

                // Singular solution, stop recursion
                if (!subLists.Skip(1).Any())
                {
                    // Sets the children to the Node and computes it's Relation afterwards
                    return setChildrenToNode(subLists.First());
                }

                // Recursion
                IEnumerable<TNode> branches = subLists.Select(x => Create.Node<TNode, T>(x, partitionMethod, setChildrenToNode, leafSize));

                // Sets the children to the Node and computes it's Relation afterwards
                return setChildrenToNode(branches);
            }
            else
            {
                // We're at the last parent Node, no more recursion needed
                // Sets the children to the Node and computes it's Relation afterwards
                return setChildrenToNode(dataItems);
            }
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static int LargestList<T>(List<List<T>> lists)
        {
            int index = 0;
            for (int i = 0; i < lists.Count; i++)
            {
                if (lists[i].Count > lists[index].Count)
                    index = i;
            }
            return index;
        }

        /***************************************************/

    }
}



