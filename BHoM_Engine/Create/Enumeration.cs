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

using BH.oM.Base;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;
using System.Reflection;
using System.ComponentModel;
using BH.oM.Base.Attributes;

namespace BH.Engine.Base
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates an Enumeration of type T with a value matching the input value")]
        [Input("value", "String representation of the Enumeration to be created")]
        [Output("Enumeration of type T with a value matching the input string")]
        public static T Enumeration<T>(string value) where T : Enumeration
        {
            return Enumeration(typeof(T), value) as T;
        }

        /***************************************************/

        [Description("Creates an Enumeration of type 'type' with a value matching the input value")]
        [Input("type", "Type of Enumeration to be created")]
        [Input("value", "String representation of the Enumeration to be created")]
        [Output("Enumeration of type 'type' with a value matching the input string")]
        public static Enumeration Enumeration(Type type, string value)
        {
            if (type == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot create an enumeration from a null type");
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
                BH.Engine.Base.Compute.RecordError($"Cannot create an enumeration from type {type.ToString()} and value {value}");

            return result;
        }

        /***************************************************/
    }
}






