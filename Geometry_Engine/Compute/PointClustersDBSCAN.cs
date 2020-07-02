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

using BH.Engine.Data;
using BH.oM.Geometry;
using BH.oM.Data.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Compute
    {
        /***************************************************/
        /**** public Methods - Vectors                  ****/
        /***************************************************/
        
        public static List<List<Point>> PointClustersDBSCAN(this List<Point> points, double maxDist, int minPointCount = 1)
        {
            List<Point> items = new List<Point>(points);
            DomainTree<int> indexTree = Data.Create.DomainTree(items.Select((x,i) => Data.Create.DomainTreeLeaf(i, x.IBounds().DomainBox())));

            double sqDist = maxDist * maxDist;

            // Distance between boxes is the distance between Points for Points
            Func<DomainTree<int>, DomainBox, bool> evaluator = (a, b) => a.DomainBox.SquareDistance(b) < sqDist;

            List<List<Point>> result = new List<List<Point>>();
            List<Point> toEvaluate = new List<Point>();

            int count = -1;
            for (int i = 0; i < items.Count; i++)
            {
                Point pivot = items[i];

                if (pivot == null)
                    continue;

                result.Add(new List<Point>());
                count++;
                toEvaluate.Add(pivot);
                items[i] = null;    // Set to null as it is not there anymore, but this also retains the indecies for the tree.

                while (toEvaluate.Count > 0)
                {
                    // Find all the neighbours for each point in toEvaluate, and add them in toEvaluate
                    foreach (int index in Data.Query.ItemsInRange<DomainTree<int>, int>(indexTree, x => evaluator(x, toEvaluate[0].IBounds().DomainBox())))
                    {
                        Point pt = items[index];
                        if (pt != null)
                        {
                            toEvaluate.Add(pt);
                            items[index] = null;
                        }
                    }

                    // move the checked points from toEvaluate to result
                    result[count].Add(toEvaluate[0]);
                    toEvaluate.RemoveAt(0);
                }
            }

            return result.Where(list => list.Count >= minPointCount).ToList();
        }

        /***************************************************/
    }
}

