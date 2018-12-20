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

using Engine_Explore.BHoM.DataStructure;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Engine_Explore.Engine.Reflection
{
    public static class Types
    {
        public static Dictionary<Type, IList> GroupByType(IEnumerable<object> data)
        {
            Dictionary<Type, IList> groups = new Dictionary<Type, IList>();

            foreach (object item in data)
            {
                Type type = item.GetType();
                Type listType = typeof(List<>).MakeGenericType(type);

                if (!groups.ContainsKey(type))
                    groups.Add(type, Activator.CreateInstance(listType) as IList);
                groups[type].Add(item);
            }

            return groups;
        }

        /***************************************************/

        public static Tree<Type> GetTypeTree(Type type)
        {
            Tree<Type> tree = new Tree<Type>();

            tree.Value = type;

            foreach (PropertyInfo info in type.GetProperties())
                tree.Childrens.Add(GetTypeTree(info.PropertyType));

            return tree;
        }

        /***************************************************/

        public static List<Type> GetLinkedTypes(Type type)
        {
            HashSet<Type> set = new HashSet<Type>();

            foreach (PropertyInfo info in type.GetProperties())
            {
                foreach (Type linkedType in GetLinkedTypes(info.PropertyType))
                    set.Add(linkedType);
            }

            return set.ToList();
        }
    }
}
