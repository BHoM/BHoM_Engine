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

using System;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Reflection
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Type CommonBaseType(this List<Type> types)
        {
            types = types.Where(t => t != null).ToList();
            List<List<Type>> inheritanceChain = new List<List<Type>>();

            foreach (Type type in types.Distinct())
                inheritanceChain.Add(type.ClassHierarchy());

            HashSet<Type> table = new HashSet<Type>(inheritanceChain.FirstOrDefault());
            for (int i = 1; i < inheritanceChain.Count; i++)
                table.IntersectWith(inheritanceChain[i]);

            return table.FirstOrDefault() ?? typeof(object);
        }

        /***************************************************/
    }
}
