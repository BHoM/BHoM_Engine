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
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Data
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Tree<T> Tree<T>(T value, string name = "")
        {
            return new Tree<T>
            {
                Value = value,
                Name = name
            };
        }

        /***************************************************/

        public static Tree<T> Tree<T>(Dictionary<string, Tree<T>> children, string name = "")
        {
            return new Tree<T>
            {
                Children = children,
                Name = name
            };
        }

        /***************************************************/

        public static Tree<T> Tree<T>(List<T> items, List<List<string>> paths, string name = "")
        {
            Tree<T> tree = new Tree<T> { Name = name };

            if (items.Count != paths.Count)
                return tree;

            for (int i = 0; i < items.Count; i++)
            {
                Tree<T> subTree = tree;
                List<string> path = paths[i];

                foreach (string part in path)
                {
                    if (!subTree.Children.ContainsKey(part))
                        subTree.Children.Add(part, new Tree<T> { Name = part });
                    subTree = subTree.Children[part];
                }
                subTree.Value = items[i];
            }

            return tree;
        }

        /***************************************************/
    }
}
