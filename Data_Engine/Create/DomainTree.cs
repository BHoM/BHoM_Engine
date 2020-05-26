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

        public static DomainTree<T> DomainTree<T>(IEnumerable<T> dataItems, Func<T,DomainBox> dataEvaluator, int treeDegree = 16, int leafSize = 16, int sampleSize = 60)
        {
            if (dataItems is DomainTree<T>)
                return dataItems as DomainTree<T>;

            List<DomainTree<T>> list = dataItems.Select(x => new DomainTree<T>()
            {
                Values = new List<T>() { x },
                Relation = dataEvaluator(x)
            }).ToList();

            return DomainTree<T>(list, treeDegree, leafSize, sampleSize);
        }

        /***************************************************/

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
                    Children = children,
                    Relation = children.Select(x => x.Relation).Aggregate((x, y) => x + y)
                };

                return tree;
            };

            return Node<DomainTree<T>, T>(dataItems, splitMethod, setChildrenToNode, treeDegree, leafSize);
        }
        

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static Output<List<DomainTree<T>>, List<DomainTree<T>>> SplitList<T>(List<DomainTree<T>> list, int sampleSize = 60)
        {
            sampleSize = Math.Min(list.Count, sampleSize);

            int step = (int)Math.Floor((decimal)(list.Count / sampleSize));
            // find centre
            DomainBox box = list[0].Relation;
            for (int i = 1; i < sampleSize; i++)
                box += list[i * step].Relation;

            int index = -1;
            double max = 0;

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
                if ((data.Relation.Domains[index].Min + data.Relation.Domains[index].Min) / 2 < centre)
                    one.Add(data);
                else
                    two.Add(data);
            }

            return new Output<List<DomainTree<T>>, List<DomainTree<T>>>() { Item1 = one, Item2 = two };
        }

        /***************************************************/

        private static DomainBox Bounds<T>(this DomainTree<T> domainTree)
        {
            return domainTree.Children.Select(x => x.Relation).Aggregate((x,y) => x + y);
        }

        /***************************************************/

    }
}
