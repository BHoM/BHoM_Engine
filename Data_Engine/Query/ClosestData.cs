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
            List<ITree> list = new List<ITree>() { tree };

            ITree closest = list.Last();
            do
            {
                // if the closest item is a branch, find the sub Branches and add to the end of the list, sorted by distance
                list = OpenBranch(list, closest as NTree<T>, box);
                closest = list.Last();

                if (!double.IsNaN(maxDistance) && closest.Bounds.SquareDistance(box) > maxDistance)
                    return new List<T>();

            } while (closest is NTree<T>);
            list.RemoveAt(list.Count - 1);

            // Save the furthest possible distance from the item with the smallest furthest possible distance
            double max = closest.Bounds.FurthestSquareDistance(box) + Tolerance.Distance * Tolerance.Distance;
            List<Leaf<T>> result = new List<Leaf<T>>() { closest as Leaf<T> };

            // Get all the items with closest point closer than the max
            int i = list.Count - 1;
            while (i != -1 && list[i].Bounds.SquareDistance(box) < max)
            {
                if (list[i] is NTree<T>)
                {
                    list = OpenBranch(list, list[i] as NTree<T>, box);
                }
                else
                {
                    result.Add(list[i] as Leaf<T>);
                    list.RemoveAt(i);
                }
                i = list.Count - 1;
            }

            return result.Select(x => x.Item).ToList();
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        [Description("Opens the branch, sorts it by furthest distance to box and adds to the end of the list")]
        private static List<ITree> OpenBranch<T>(List<ITree> list, NTree<T> branch, NBound box)
        {
            list.RemoveAt(list.Count - 1);
            list.AddRange(branch.Items.ToList());
            return list.OrderBy(x => -x.Bounds.SquareDistance(box)).ToList();
        }

        /***************************************************/
    }
}

