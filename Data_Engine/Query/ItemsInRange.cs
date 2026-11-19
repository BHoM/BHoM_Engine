/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2026, the respective contributors. All rights reserved.
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
using System.Threading;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.oM.Base;
using System.Threading.Tasks;
using BH.oM.Data.Collections;
using BH.oM.Geometry;

namespace BH.Engine.Data
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Gets all data in the tree which DomainBox is in range of the box.")]
        [Input("tree", "Tree to search from.")]
        [Input("box", "Search box which will intersect all the retuned data's DomainBoxes.")]
        [Output("data", "All data in the tree which DomainBox is in range of the box.")]
        public static IEnumerable<T> ItemsInRange<T>(this DomainTree<T> tree, DomainBox box, double tolerance = Tolerance.Distance)
        {
            if (box == null)
                return new List<T>();

            List<T> result = new List<T>();
            if (tree != null)
                CollectItemsInRange(tree, box, tolerance, result);

            return result;
        }

        /***************************************************/

        [Description("Gets the values and evaluates the children based on the provided function.")]
        [Input("tree", "Tree to search from.")]
        [Input("isWithinSearch", "Method to traverse the tree. " +
                                 "A false result means that no descendants of that node can return true. " +
                                 "A true result means that that nodes data is returned and its descendants might return true.")]
        [Output("data", "All data in the tree which nodes returned true for the isWithinSearch method.")]
        public static IEnumerable<T> ItemsInRange<TNode, T>(this TNode tree, Func<TNode, bool> isWithinSearch) where TNode : INode<T>
        {
            // DomainTrees are traversed directly, which avoids the dynamic dispatch needed to get the children and values of a general node
            DomainTree<T> domainTree = (object)tree as DomainTree<T>;
            Func<DomainTree<T>, bool> domainTreeSearch = (object)isWithinSearch as Func<DomainTree<T>, bool>;
            if (domainTree != null && domainTreeSearch != null)
            {
                List<T> result = new List<T>();
                CollectItemsInRange(domainTree, domainTreeSearch, result);
                return result;
            }

            if (tree != null && isWithinSearch(tree))
            {
                return tree.IChildren<TNode, T>().SelectMany(x => ItemsInRange<TNode, T>(x, isWithinSearch)).Concat(tree.IValues());
            }
            else
            {
                return new List<T>();
            }
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        // Adds the values of every node within the tolerance of the box to the result. Children first, then the node's own values.
        private static void CollectItemsInRange<T>(DomainTree<T> node, DomainBox box, double tolerance, List<T> result)
        {
            if (node == null || !node.DomainBox.IsInRange(box, tolerance))
                return;

            List<DomainTree<T>> children = node.Children;
            if (children != null)
            {
                for (int i = 0; i < children.Count; i++)
                    CollectItemsInRange(children[i], box, tolerance, result);
            }

            if (node.Values != null)
                result.AddRange(node.Values);
        }

        /***************************************************/

        private static void CollectItemsInRange<T>(DomainTree<T> node, Func<DomainTree<T>, bool> isWithinSearch, List<T> result)
        {
            if (node == null || !isWithinSearch(node))
                return;

            List<DomainTree<T>> children = node.Children;
            if (children != null)
            {
                for (int i = 0; i < children.Count; i++)
                    CollectItemsInRange(children[i], isWithinSearch, result);
            }

            if (node.Values != null)
                result.AddRange(node.Values);
        }

        /***************************************************/

    }
}






