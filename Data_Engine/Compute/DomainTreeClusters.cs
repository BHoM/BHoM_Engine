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
using System;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Data
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<List<T>> DomainTreeClusters<T>(this List<T> data, Func<T, DomainBox> toDomainBox, Func<T, T, double> distFunc, double maxDist, int minPointCount = 1)
        {
            if (data.Count == 0)
                return new List<List<T>>();

            List<T> items = new List<T>(data);
            List<bool> check = items.Select(x => false).ToList();
            DomainTree<int> indexTree = Data.Create.DomainTree(items.Select((x, i) => Data.Create.DomainTreeLeaf(i, toDomainBox(x))));

            double sqDist = maxDist * maxDist;

            Func<DomainTree<int>, DomainBox, bool> evaluator = (a, b) => a.DomainBox.SquareDistance(b) < sqDist;

            List<List<T>> result = new List<List<T>>();
            List<T> toEvaluate = new List<T>();

            int count = -1;
            for (int i = 0; i < items.Count; i++)
            {
                T pivot = items[i];

                if (check[i])
                    continue;

                result.Add(new List<T>());
                count++;
                toEvaluate.Add(pivot);
                check[i] = true;

                while (toEvaluate.Count > 0)
                {
                    // Find all the neighbours for each item in toEvaluate, and add them in toEvaluate
                    foreach (int index in Data.Query.ItemsInRange<DomainTree<int>, int>(indexTree, x => evaluator(x, toDomainBox(toEvaluate[0]))))
                    {
                        if (!check[index])
                        {
                            T item = items[index];
                            if (distFunc(toEvaluate[0], item) < maxDist)
                            {
                                toEvaluate.Add(item);
                                check[index] = true;
                            }
                        }
                    }

                    // move the checked items from toEvaluate to result
                    result[count].Add(toEvaluate[0]);
                    toEvaluate.RemoveAt(0);
                }
            }

            return result.Where(list => list.Count >= minPointCount).ToList();
        }

        /***************************************************/
    }
}

