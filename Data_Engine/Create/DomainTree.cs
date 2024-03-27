/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
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

using BH.oM.Base;
using BH.oM.Base.Attributes;
using BH.oM.Data.Collections;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Data
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Create a data tree with its data in its leaves which can be traversed by DomainBoxes.")]
        [Input("dataItems", "The data to store in the DomainTree.")]
        [Input("dataEvaluator", "A function to query each data item's DomainBox. Used to build the tree.")]
        [Input("treeDegree", "The number of child nodes for each node.")]
        [Input("leafSize", "The number of siblings a leaf node can have.")]
        [Input("sampleSize", "The number of items used to determine how to split the collection.")]
        [Output("domainTree", "A data tree with its data in its leaves which can be traversed by DomainBoxes.")]
        public static DomainTree<T> DomainTree<T>(IEnumerable<T> dataItems, Func<T, DomainBox> dataEvaluator, int treeDegree = 16, int leafSize = 16, int sampleSize = 60)
        {
            if (dataItems is DomainTree<T>)
                return dataItems as DomainTree<T>;

            List<DomainTree<T>> list = dataItems.Select(x => new DomainTree<T>()
            {
                Values = new List<T>() { x },
                DomainBox = dataEvaluator(x)
            }).ToList();

            return DomainTree<T>(list, treeDegree, leafSize, sampleSize);
        }

        /***************************************************/

        [Description("Create a data tree with its data in its leaves which can be traversed by DomainBoxes.")]
        [Input("dataItems", "The data to store in the DomainTree. Provided as DomainTrees, needs to have the DomainBox property set to function.")]
        [Input("treeDegree", "The number of child nodes for each node.")]
        [Input("leafSize", "The number of siblings a leaf node can have.")]
        [Input("sampleSize", "The number of items used to determine how to split the collection.")]
        [Output("domainTree", "A data tree with its data in its leaves which can be traversed by DomainBoxes.")]
        public static DomainTree<T> DomainTree<T>(IEnumerable<DomainTree<T>> dataItems, int treeDegree = 16, int leafSize = 16, int sampleSize = 60)
        {
            sampleSize = Math.Max(2, sampleSize);

            Func<IEnumerable<DomainTree<T>>, Output<List<DomainTree<T>>, List<DomainTree<T>>>> splitMethod = (list) =>
            {
                return SplitList((list as List<DomainTree<T>>), sampleSize);
            };

            Func<IEnumerable<DomainTree<T>>, DomainTree<T>> setChildrenToNode = (children) =>
            {
                DomainTree<T> tree = new DomainTree<T>()
                {
                    Children = children?.ToList() ?? new List<DomainTree<T>>(),
                    DomainBox = children?.Select(x => x?.DomainBox).Aggregate((x, y) => x + y)
                };

                return tree;
            };

            return Node<DomainTree<T>, T>(dataItems, splitMethod, setChildrenToNode, treeDegree, leafSize);
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        [Description("Separates a collection of DomainTree leaves into two collections based on their DomainBox in an attempt to get the smallest Union DomainBox around the two collections.")]
        private static Output<List<DomainTree<T>>, List<DomainTree<T>>> SplitList<T>(List<DomainTree<T>> list, int sampleSize = 60)
        {
            sampleSize = Math.Min(list.Count, sampleSize);

            int step = (int)Math.Floor((decimal)(list.Count / sampleSize));
            // find centre
            DomainBox box = list[0].DomainBox;
            for (int i = 1; i < sampleSize; i++)
                box += list[i * step].DomainBox;

            int index = -1;
            double max = double.MinValue;

            for (int i = 0; i < box.Domains.Length; i++)
            {
                double temp = box.Domains[i].Max - box.Domains[i].Min;
                if (temp > max)
                {
                    max = temp;
                    index = i;
                }
            }

            double centre = (box.Domains[index].Min + box.Domains[index].Max) / 2;

            // split into two
            List<DomainTree<T>> one = new List<DomainTree<T>>();
            List<DomainTree<T>> two = new List<DomainTree<T>>();
            // do the splitting

            foreach (DomainTree<T> data in list)
            {
                if ((data.DomainBox.Domains[index].Min + data.DomainBox.Domains[index].Max) / 2 < centre)
                    one.Add(data);
                else
                    two.Add(data);
            }

            return new Output<List<DomainTree<T>>, List<DomainTree<T>>>() { Item1 = one, Item2 = two };
        }

        /***************************************************/

        [Description("Creates a union DomainBox around the childrens DomainBoxes.")]
        private static DomainBox Bounds<T>(this DomainTree<T> domainTree)
        {
            return domainTree.Children.Select(x => x.DomainBox).Aggregate((x, y) => x + y);
        }

        /***************************************************/

    }
}




