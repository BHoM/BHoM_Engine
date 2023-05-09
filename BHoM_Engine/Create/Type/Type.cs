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

using BH.oM.Base.Attributes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace BH.Engine.Base
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a BHoM type that matches the given name.")]
        [Input("name", "Name to be searched for among all BHoM types.")]
        [Input("silent", "If true, the error about no type found will be suppressed, otherwise it will be raised.")]
        [Output("type", "BHoM type that matches the given name.")]
        public static Type Type(string name, bool silent = false)
        {
            if (name == null)
            {
                Compute.RecordError("Cannot create a type from a null string.");
                return null;
            }

            if (name.Contains('<'))
                return GenericTypeAngleBrackets(name, silent);
            else if (name.Contains('`') && name.Contains("[["))
                return GenericTypeSquareBrackets(name, silent);

            string unQualifiedName = name.Contains(",") ? name.Split(',').First() : name;
            List<Type> types = null;
            if (!Global.BHoMTypeDictionary.TryGetValue(unQualifiedName, out types))
            {
                Type type;
                if (m_ExplicitSystemTypes.TryGetValue(unQualifiedName, out type))   //Handling of some specific System Types that do not work without assembly qualified name
                    return type;

                type = System.Type.GetType(name);
                if (type == null && name.EndsWith("&"))
                {
                    type = Type(name.TrimEnd(new char[] { '&' }), true);
                    if (type != null)
                        type = type.MakeByRefType();
                }
                    
                if (type == null && !silent)
                    Compute.RecordError($"A type corresponding to {name} cannot be found.");

                return type;
            }
            else if (types.Count == 1)
                return types[0];
            else if (!silent)
            {
                string message = "Ambiguous match: Multiple types correspond the the name provided: \n";
                foreach (Type type in types)
                    message += "- " + type.FullName + "\n";

                Compute.RecordError(message);
            }

            return null;
        }

        /***************************************************/

        private static Dictionary<string, Type> m_ExplicitSystemTypes = new Dictionary<string, Type>
        {
            ["System.Drawing.Color"] = typeof(System.Drawing.Color),
            ["System.Text.RegularExpressions.Regex"] = typeof(System.Text.RegularExpressions.Regex),
            ["System.Drawing.Bitmap"] = typeof(System.Drawing.Bitmap)
        };

        /*******************************************/
    }
}


