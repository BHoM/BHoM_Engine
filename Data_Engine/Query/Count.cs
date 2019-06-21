/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2018, the respective contributors. All rights reserved.
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
using System.Linq;

namespace BH.Engine.Data
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static int Count<T>(this PriorityQueue<T> queue) where T : IComparable<T>
        {
            return queue.Data.Count();
        }

        /***************************************************/

        public static int Count<T>(this Graph<T> graph) 
        {
            return graph.Nodes.Count();
        }

        /***************************************************/

        public static int Count<T>(this Tree<T> tree)
        {
            if (tree.Children.Count == 0)
                return 1;
            else
                return tree.Children.Sum(x => x.Value.Count());
        }

        /***************************************************/

        public static int Count<T>(this VennDiagram<T> diagram)
        {
            return diagram.Intersection.Count + diagram.OnlySet1.Count + diagram.OnlySet2.Count;
        }

        /***************************************************/
    }
}
