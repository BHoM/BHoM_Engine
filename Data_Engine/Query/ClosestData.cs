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
using BH.oM.Geometry;
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

        /***********************************************/
        /**** Point Matrix                          ****/
        /***********************************************/

        public static LocalData<T> ClosestData<T>(this PointMatrix<T> matrix, Point refPt, double maxDist)
        {
            List<LocalData<T>> closePts = matrix.CloseToPoint(refPt, maxDist);

            return closePts.OrderBy(x => x.Position.PMSquareDistance(refPt)).FirstOrDefault();
        }


        /***********************************************/
        /**** NTree                                 ****/
        /***********************************************/

        [Description("Returns all possible closest data, assuming that they can be anywhere in their respective bounding box.")]
        public static IEnumerable<T> ClosestData<T>(this NTree<T> tree, NBound box, double maxDistance = double.NaN)
        {
            List<Tuple<double, ITree>> list = new List<Tuple<double, ITree>>()
            {
                new Tuple<double, ITree>(tree.Bounds.SquareDistance(box), tree)
            };

            int closestIndex = 0;
            do
            {
                // Add the sub items of the closest item to the list
                list.AddRange((list[closestIndex].Item2 as NTree<T>).Items.Select(x =>
                    new Tuple<double, ITree>(x.Bounds.SquareDistance(box), x)).ToList());

                // remove the parent item
                list.RemoveAt(closestIndex);

                // Find the index of the closest item
                double min = double.PositiveInfinity;
                for (int j = 0; j < list.Count; j++)
                {
                    if (list[j].Item1 < min)
                    {
                        min = list[j].Item1;
                        closestIndex = j;
                    }
                }

                // Break if the closest item is further away than the maxDistance
                if (!double.IsNaN(maxDistance) && list[closestIndex].Item1 > maxDistance)
                    return new List<T>();

            } while (list[closestIndex].Item2 is NTree<T>);

            if (double.IsNaN(maxDistance))
                maxDistance = double.PositiveInfinity;

            // Save the furthest possible distance from the closest item
            double max = list[closestIndex].Item2.Bounds.FurthestSquareDistance(box) + Tolerance.Distance * Tolerance.Distance;
            max = Math.Min(max, maxDistance);

            // gets every item with closest distance less than max
            List<Tuple<double, Leaf<T>>> closest = LessThan<T>(list, max, box);

            // Finds the smallest furtest distance among the closest items, (Due to how the tree structure encapsulates the smaller items it can't be done before this)
            max = closest.Min(x => x.Item2.Bounds.FurthestSquareDistance(box)) + Tolerance.Distance * Tolerance.Distance;
            max = Math.Min(max, maxDistance);

            // Gets everything closer than that and returns the data
            return closest.Where(x => x.Item1 < max).Select(x => x.Item2.Item).ToList();
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static List<Tuple<double, Leaf<T>>> LessThan<T>(List<Tuple<double, ITree>> list, double val, NBound box)
        {
            List<Tuple<double, Leaf<T>>> result = new List<Tuple<double, Leaf<T>>>();
            for (int i = 0; i < list.Count; i++)
            {
                Tuple<double, ITree> o = list[i];
                if (o.Item1 < val)
                {
                    if (o.Item2 is Leaf<T>)
                    {
                        result.Add(new Tuple<double, Leaf<T>>(o.Item1, o.Item2 as Leaf<T>));
                    }
                    else
                    {
                        result.AddRange(LessThan<T>((o.Item2 as NTree<T>).Items.Select(x => new Tuple<double, ITree>(x.Bounds.SquareDistance(box), x)).ToList(), val, box));
                    }
                }
            }
            return result;
        }

        /***************************************************/

    }
}

