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

using BH.oM.Data.Collections;

namespace BH.Engine.Data
{
    public static partial class Modify
    {

        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static void AddLink<T>(this Graph<T> graph, GraphNode<T> from, GraphNode<T> to, double weight = 1, bool bothDirection = false)
        {
            graph.Links.Add(new GraphLink<T> { StartNode = from, EndNode = to, Weight = weight });
            if (bothDirection)
                graph.Links.Add(new GraphLink<T> { StartNode = to, EndNode = from, Weight = weight });
        }

        /***************************************************/

        public static void AddLink<T>(this Graph<T> graph, GraphLink<T> link)
        {
            graph.Links.Add(link);
        }

        /***************************************************/
    }
}





