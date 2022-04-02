/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2022, the respective contributors. All rights reserved.
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

        [Description("Creates all types with full names that end with given string, with or without assembly information after comma.")]
        [Input("name", "Name to be searched for among all reflected types.")]
        [Input("silent", "If true, the error about no types found will be suppressed, otherwise it will be raised.")]
        [Output("types", "All reflected types that match the given name.")]
        public static List<Type> AllTypes(string name, bool silent = false)
        {
            if (name == null)
            {
                Compute.RecordError("Cannot create types from a null string.");
                return new List<Type>();
            }

            List<Type> typeList = new List<Type>();
            if (name.StartsWith("BH.Engine"))
                typeList = Query.EngineTypeList();
            else if (name.StartsWith("BH.Adapter"))
                typeList = Query.AdapterTypeList();
            else if (name.StartsWith("BH.oM"))
                typeList = Query.BHoMTypeList();
            else
                typeList = Query.AllTypeList();

            List<Type> types;
            if (name.Contains(','))
                types = typeList.Where(x => x.AssemblyQualifiedName.Contains(name)).ToList();
            else
                types = typeList.Where(x => x.FullName.EndsWith(name)).ToList();

            if (types.Count != 0)
                return types;
            else
            {
                //No method found in dictionary, try System.Type
                Type type = System.Type.GetType(name);
                if (type == null)
                {
                    if (!silent)
                        Compute.RecordError($"A type corresponding to {name} cannot be found.");
                    return new List<Type>();
                }
                return new List<Type> { type };
            }
        }

        /***************************************************/
    }
}

