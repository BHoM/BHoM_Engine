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

using BH.oM.Data.Collections;
using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Data
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Clusters data in different collections using a DomainTree.")]
        [Input("data", "Data to cluster.")]
        [Input("toDomainBox", "Method which takes a item in the data and produces a DomainBox for the tree.")]
        [Input("treeEvaluator", "Method which evaluates if the items within the second DomainBox could be in the same cluster as the first DomainBox.")]
        [Input("itemEvaluator", "Method which evaluates if the two items should be in the same cluster.")]
        [Input("minItemCount", "Lowest number of item in a cluster to return.")]
        [Output("clusters", "Clusters where itemEvaluator is never true between items from different clusters.")]
        public static List<List<T>> DomainTreeClusters<T>(this List<T> data, Func<T, DomainBox> toDomainBox, Func<DomainBox, DomainBox, bool> treeEvaluator, Func<T, T, bool> itemEvaluator, int minItemCount = 1)
        {
            if (data.Count == 0)
                return new List<List<T>>();

            List<T> items = new List<T>(data);
            List<bool> check = items.Select(x => false).ToList();
            List<DomainBox> domainBoxes = data.Select(x => toDomainBox(x)).ToList();
            DomainTree<int> indexTree = Data.Create.DomainTree(domainBoxes.Select((x, i) => Data.Create.DomainTreeLeaf(i, x)));

            List<List<T>> result = new List<List<T>>();
            List<int> toEvaluate = new List<int>();

            int count = -1;
            for (int i = 0; i < items.Count; i++)
            {
                if (check[i])
                    continue;

                result.Add(new List<T>());
                count++;
                toEvaluate.Add(i);
                check[i] = true;

                while (toEvaluate.Count > 0)
                {
                    // Find all the neighbours for each item in toEvaluate, and add them in toEvaluate
                    foreach (int index in Data.Query.ItemsInRange<DomainTree<int>, int>(indexTree, x => treeEvaluator(x.DomainBox, domainBoxes[toEvaluate[0]])))
                    {
                        if (!check[index])
                        {
                            if (itemEvaluator(items[toEvaluate[0]], items[index]))
                            {
                                toEvaluate.Add(index);
                                check[index] = true;
                            }
                        }
                    }

                    // move the checked items from toEvaluate to result
                    result[count].Add(items[toEvaluate[0]]);
                    toEvaluate.RemoveAt(0);
                }
            }

            return result.Where(list => list.Count >= minItemCount).ToList();
        }

        /***************************************************/
    }
}






