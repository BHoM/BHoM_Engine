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

using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Reflection.Attributes;
using BH.oM.Base;
using BH.oM.Geometry;
using BH.oM.Reflection;
using BH.oM.Data.Collections;

namespace BH.Engine.Data
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Passes the data down trough the tree to the leaves and computes the relations on the way back")]
        private static TNode Node<TNode, T>(IEnumerable<TNode> dataItems,
                                        Func<IEnumerable<TNode>, Output<List<TNode>, List<TNode>>> splitDataMethod,
                                        Func<IEnumerable<TNode>, TNode> setChildrenToNode,
                                        int treeDegree = 16, int leafSize = 16) where TNode : Node<T>
        {

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


            return Node<TNode,T>(dataItems, partitionMethod, setChildrenToNode, leafSize);
        }

        /***************************************************/

        [Description("Passes the data down trough the tree to the leaves and computes the relations on the way back")]
        private static INode Node<INode, T>(IEnumerable<INode> dataItems,
                                            Func<IEnumerable<INode>, IEnumerable<IEnumerable<INode>>> partitionMethod,
                                            Func<IEnumerable<INode>, INode> setChildrenToNode,
                                            int leafSize = 16) where INode : Node<T>
        {

            leafSize = Math.Min(leafSize, 2);

            if (dataItems.Count() > leafSize)
            {
                // Partition the data where each collection will form a child Node
                IEnumerable<IEnumerable<INode>> subLists = partitionMethod(dataItems);

                // Singular solution, stop recursion
                if (subLists.Count() == 1)
                {
                    // Sets the children to the Node and computes it's Relation afterwards
                    return setChildrenToNode(subLists.First());
                }

                // Recursion
                IEnumerable<INode> branches = subLists.Select(x => Create.Node<INode, T>(x, partitionMethod, setChildrenToNode, leafSize));

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
