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
    public static partial class Modify
    {
        /*************************************/
        /**** Public Methods              ****/
        /*************************************/

        public static Tree<T> GroupByName<T>(this Tree<T> tree)
        {
            if (tree.Children.Count > 0)
            {
                if (tree.Children.Values.Any(x => x.Value != null))
                {
                    var groups = tree.Children.GroupBy(x =>
                    {
                        int index = x.Key.IndexOf('(');
                        if (index > 0)
                            return x.Key.Substring(0, index);
                        else
                            return x.Key;
                    });

                    if (groups.Count() > 1)
                    {
                        Dictionary<string, Tree<T>> children = new Dictionary<string, Tree<T>>();
                        foreach (var group in groups)
                        {
                            if (group.Count() == 1)
                            {
                                if (group.First().Value.Value == null)
                                    children.Add(group.Key, group.First().Value);
                                else
                                    children.Add(group.Key, new Tree<T> { Name = group.Key, Value = group.First().Value.Value });
                            }
                            else
                                children.Add(group.Key, new Tree<T> { Name = group.Key, Children = group.ToDictionary(x => x.Key, x => x.Value) });
                        }
                        tree.Children = children;
                    }
                }
                else
                {
                    foreach (var child in tree.Children.Values)
                        GroupByName(child);
                }
            }

            return tree;
        }

        /*************************************/
    }
}
