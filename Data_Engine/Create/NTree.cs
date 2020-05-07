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
        
        public static NTree<T> NTree<T>(IEnumerable<NBound> boundingBoxes, IEnumerable<T> data, int splits = 16)
        {
            if (boundingBoxes.Count() != data.Count())
                return null;

            List<Leaf<T>> leaves = boundingBoxes.Zip(data, (x, y) => new Leaf<T>() { Bounds = x, Item = y }).ToList();

            return NTree(leaves, splits);
        }

        /***************************************************/

        public static NTree<T> NTree<T>(IEnumerable<Leaf<T>> leaves, int splits = 16)
        {
            splits = splits < 2 ? 2 : splits;

            if (leaves.Count() > splits)
            {
                int temp = (int)Math.Ceiling((decimal)(leaves.Count() / splits)) + 1;
                temp = temp < splits ? temp : splits;


                List<NTree<T>> safetyBranches = new List<NTree<T>>();

                List<List<Leaf<T>>> subLists = new List<List<Leaf<T>>>() { leaves.ToList() };
                while (subLists.Count < temp)
                {
                    // Find the one with the most items
                    int index = LargestList(subLists);

                    Output<List<Leaf<T>>, List<Leaf<T>>> split = SplitList(subLists[index]);

                    subLists.RemoveAt(index);
                    // One empty and one full list implies a singular BoundingBox for the data, which can't be stored spatially and is hence delegated to a single branch
                    if (split.Item1.Count < 1)
                    {
                        safetyBranches.Add(new NTree<T>() { Items = split.Item2.ToArray(), Bounds = Bounds(split.Item2) });
                        temp -= 2;
                        continue;
                    }
                    else if (split.Item2.Count < 1)
                    {
                        safetyBranches.Add(new NTree<T>() { Items = split.Item1.ToArray(), Bounds = Bounds(split.Item1) });
                        temp -= 2;
                        continue;
                    }

                    subLists.Add(split.Item1);
                    subLists.Add(split.Item2);

                }

                // Recursion
                NTree<T>[] branches = subLists.Select(x => NTree(x, splits)).ToArray().Concat(safetyBranches).ToArray();

                return new NTree<T>() { Items = branches, Bounds = Bounds(branches) };
            }
            else
            {
                return new NTree<T>() { Items = leaves.ToArray(), Bounds = Bounds(leaves) };
            }
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/
        
        private static Output<List<Leaf<T>>, List<Leaf<T>>> SplitList<T>(List<Leaf<T>> list, int sampleSize = 60)
        {
            //sampleSize = Math.Max(2, sampleSize);
            sampleSize = Math.Min(list.Count, sampleSize);

            int step = (int)Math.Floor((decimal)(list.Count / sampleSize));
            // find centre
            NBound box = list[0].Bounds;
            for (int i = 1; i < sampleSize; i++)
                box += list[i * step].Bounds;

            int index = -1;
            double max = 0;

            for (int i = 0; i < box.Min.Length; i++)
            {
                double temp = box.Max[i] - box.Min[i];
                if (temp > max)
                {
                    max = temp;
                    index = i;
                }
            }

            double centre = (box.Min[index] + box.Max[index]) / 2;

            // split into two
            List<Leaf<T>> one = new List<Leaf<T>>();
            List<Leaf<T>> two = new List<Leaf<T>>();
            // do the splitting

            foreach (Leaf<T> leaf in list)
            {
                if ((leaf.Bounds.Min[index] + leaf.Bounds.Max[index]) / 2 < centre)
                    one.Add(leaf);
                else
                    two.Add(leaf);
            }

            return new Output<List<Leaf<T>>, List<Leaf<T>>>() { Item1 = one, Item2 = two };
        }
        
        /***************************************************/

        private static NBound Bounds(this IEnumerable<ITree> branches)
        {
            return branches.Select(x => x.Bounds).Aggregate((x, y) => x + y);
        }

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
