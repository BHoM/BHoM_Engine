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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace BH.Engine.Data
{
    [Description("Helper class used by the Path algorithm for Graph")]
    public class PathItem<T> : IComparable<PathItem<T>>
    {
        /***************************************************/
        /**** Properties                                ****/
        /***************************************************/

        public double StartCost { get; set; }    // Cost of going from start to that node

        public double EndCost { get; set; }     // estimated cost of going from this node to the end

        public GraphNode<T> Node { get; set; }

        public PathItem<T> Previous { get; set; }

        public bool IsValid { get { return Node != null; } }

        public double Score { get { return StartCost + EndCost; } }


        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        public PathItem()
        {
            Node = null;
        }

        /***************************************************/

        public PathItem(GraphNode<T> node, double startCost = 0, double endCost = 0)
        {
            Node = node;
            StartCost = startCost;
            EndCost = endCost;
            Previous = null;
        }


        /***************************************************/
        /**** Comparer                                  ****/
        /***************************************************/

        int IComparable<PathItem<T>>.CompareTo(PathItem<T> other)
        {
            return Score.CompareTo(other.Score);
        }

        /***************************************************/
    }

}
