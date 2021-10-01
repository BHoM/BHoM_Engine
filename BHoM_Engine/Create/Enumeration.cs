/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2021, the respective contributors. All rights reserved.
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

using BH.oM.Base;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;
using System.Reflection;

namespace BH.Engine.Base
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static T Enumeration<T>(string value) where T : Enumeration
        {
            return Enumeration(typeof(T), value) as T;
        }

        /***************************************************/

        public static Enumeration Enumeration(Type type, string value)
        {
            if (type == null)
            {
                BH.Engine.Reflection.Compute.RecordError("Cannot create an enumeration from a null type");
                return null;
            }

            List<Enumeration> test = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
                     .Select(f => f.GetValue(null))
                     .OfType<Enumeration>().ToList();

            Enumeration result = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
                     .Select(f => f.GetValue(null))
                     .OfType<Enumeration>()
                     .Where(x => x.Value == value || x.Description == value)
                     .FirstOrDefault();

            if (result == null)
                BH.Engine.Reflection.Compute.RecordError($"Cannot create an enumeration from type {type.ToString()} and value {value}");

            return result;
        }

        /***************************************************/
    }
}


