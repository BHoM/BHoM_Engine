/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
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

using BH.Engine.Base;
using BH.oM.Data.Collections;
using BH.oM.Geometry;
using BH.oM.Quantities.Attributes;
using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Data
{
    public static partial class Query
    {
        /***********************************************/
        /**** Point Matrix                          ****/
        /***********************************************/

        public static LocalData<T> ClosestData<T>(this PointMatrix<T> matrix, Point refPt, double maxDist)
        {
            List<LocalData<T>> closePts = matrix.CloseToPoint(refPt, maxDist);

            return closePts.OrderBy(x => x.Position.PMSquareDistance(refPt)).FirstOrDefault();
        }


        /***********************************************/
        /**** DomainTree<T>                         ****/
        /***********************************************/

        [Description("Finds all data which could be the closest item based on a best and worst case senario.")]
        [Input("tree", "DomainTree to query for all data items which may be the closest data.")]
        [Input("searchBox", "Search box containing data to find the closest data from.")]
        [Input("tightBox", "Assume that the DomainBox around the data is tight. i.e. A data point can be found on each boundary of the DomainBox.")]
        [Input("maxDistance", "Data whose distance is further than this distance will not be returned.", typeof(Length))]
        [Input("tolerance", "Distance tolerance, used as a margin of error for the closest data.", typeof(Length))]
        [Output("data", "All data which could be the closest item based on a best and worst case senario.")]
        public static IEnumerable<T> ClosestData<T>(this DomainTree<T> tree,
                                        DomainBox searchBox,
                                        bool tightBox = false,
                                        double maxDistance = double.PositiveInfinity,
                                        double tolerance = Tolerance.Distance)
        {
            if (searchBox == null)
                return new List<T>();

            Func<DomainTree<T>, double> evaluationMethod = (x) => x.DomainBox?.SquareDistance(searchBox) ?? double.PositiveInfinity;

            Func<DomainTree<T>, double> worstCaseMethod;
            if (tightBox)
                worstCaseMethod = (x) => x.DomainBox?.FurthestTightSquareDistance(searchBox) ?? double.PositiveInfinity;
            else
                worstCaseMethod = (x) => x.DomainBox?.FurthestSquareDistance(searchBox) ?? double.PositiveInfinity;

            return ClosestData<DomainTree<T>, T>(tree, evaluationMethod, worstCaseMethod, maxDistance * maxDistance, tolerance * tolerance);
        }

        /***********************************************/
        /**** Node<T>                               ****/
        /***********************************************/

        [Description("Assumes all data to be queried is stored in leaves. i.e. Nodes without children." +
                     "Gets the data in all nodes which evaluationMethod evaluates to less than the smallest evaluation of the WorstCaseMethod.")]
        [Input("tree", "Node tree to query for all data items which may be the closest data.")]
        [Input("evaluationMethod", "Method returning a value evaluating its closeness based on a best case scenario.")]
        [Input("worstCaseMethod", "Method returning a value evaluating its closeness based on a worst case scenario.")]
        [Input("maxEvaluation", "Data which evaluates to more than this value will not be returned.")]
        [Input("tolerance", "Margin of error for the evaluation method.")]
        [Output("data", "All data which could be the closest item based on a best and worst case scenario.")]
        public static IEnumerable<T> ClosestData<TNode, T>(this TNode tree,
                                            Func<TNode, double> evaluationMethod,
                                            Func<TNode, double> worstCaseMethod,
                                            double maxEvaluation = double.PositiveInfinity,
                                            double tolerance = Tolerance.Distance) where TNode : INode<T>
        {
            if (tree == null)
                return new List<T>();

            List<Tuple<double, TNode>> list = new List<Tuple<double, TNode>>()
            {
                new Tuple<double, TNode>(evaluationMethod(tree), tree)
            };

            int closestIndex = 0;
            do
            {
                // Add the sub items of the closest item to the list
                list.AddRange((list[closestIndex].Item2).IChildren<TNode, T>().Select(x =>
                     new Tuple<double, TNode>(evaluationMethod(x), x)).ToList());

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
                if (list[closestIndex].Item1 > maxEvaluation)
                    return new List<T>();

            } while (list[closestIndex].Item2.IChildren<TNode, T>().Any());


            // Save the furthest possible distance from the closest item
            double max = worstCaseMethod(list[closestIndex].Item2) + tolerance;
            maxEvaluation = Math.Min(max, maxEvaluation);

            // gets every item with closest distance less than max, and evaluate new maxes based on the new closest data.
            List<Tuple<double, TNode>> closest = LessThan<TNode, T>(
                        list,
                        evaluationMethod,
                        worstCaseMethod,
                        ref maxEvaluation,
                        tolerance);

            // Gets everything closer than that and returns the data
            return closest.Where(x => x.Item1 < maxEvaluation).SelectMany(x => x.Item2.IValues()).ToList();
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        [Description("Gets all data which evaluates to less than the maxEvaluation. " +
                     "Evaluates the leaves first and updates the maxEvaluation to the lowest worstcase for a leaf. " +
                     "Then calls itself for all non-leaf nodes children within the maxEvaluation.")]
        private static List<Tuple<double, TNode>> LessThan<TNode, T>(List<Tuple<double, TNode>> list,
                                            Func<TNode, double> evaluationMethod,
                                            Func<TNode, double> worstCaseMethod,
                                            ref double maxEvaluation,
                                            double tolerance) where TNode : INode<T>
        {
            List<Tuple<double, TNode>> result = new List<Tuple<double, TNode>>();
            // find those which one can get a worst case from to attempt to lower that value asap
            for (int i = 0; i < list.Count; i++)
            {
                Tuple<double, TNode> o = list[i];
                if (o.Item1 < maxEvaluation && !o.Item2.IChildren<TNode, T>().Any())
                {
                    double temp = worstCaseMethod(o.Item2) + tolerance;
                    if (temp < maxEvaluation)
                        maxEvaluation = temp;

                    result.Add(o);
                }
            }
            // Afterwards go deeper into the children of those who pass the check
            for (int i = 0; i < list.Count; i++)
            {
                Tuple<double, TNode> o = list[i];
                if (o.Item1 < maxEvaluation)
                {
                    result.AddRange(LessThan<TNode, T>(
                        o.Item2.IChildren<TNode, T>().Select(x => new Tuple<double, TNode>(evaluationMethod(x), x)).ToList(),
                        evaluationMethod,
                        worstCaseMethod,
                        ref maxEvaluation,
                        tolerance));
                }
            }
            return result;
        }

        /***************************************************/

    }
}




