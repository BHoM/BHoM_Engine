/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2020, the respective contributors. All rights reserved.
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
using System.Reflection;

namespace BH.Engine.Reflection
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Type Type(string name, bool silent = false)
        {
            Dictionary<string, List<Type>> typeDictionary = Query.BHoMTypeDictionary();

            if (name.Contains('<'))
                return GenericType(name, silent);

            List<Type> types = null;
            if (!typeDictionary.TryGetValue(name, out types))
            {
                Type type = System.Type.GetType(name);
                if (type != null)
                    return type;
                else
                {
                    if (!silent)
                        Compute.RecordError($"A type corresponding to {name} cannot be found.");
                    return null;
                }
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

        public static Type GenericType(string name, bool silent = false)
        {
            string[] parts = name.Split('<', '>', ',').Select(x => x.Trim()).ToArray();
            string[] arguments = parts.Skip(1).Where(x => x.Length > 0).ToArray();

            Type typeDefinition = Type(parts[0] + "`" + arguments.Length);
            if (typeDefinition == null)
                return null;

            try
            {
                return typeDefinition.MakeGenericType(arguments.Select(x => Type(x)).ToArray());
            }
            catch
            {
                return null;
            }
        }

        /***************************************************/

        public static Type EngineType(string name, bool silent = false)
        {
            List<Type> methodTypeList = Query.EngineTypeList();

            List<Type> types = methodTypeList.Where(x => x.AssemblyQualifiedName.StartsWith(name)).ToList();

            if (types.Count == 1)
                return types[0];
            else
            {
                //Unique method not found in list, check if it can be extracted using the system Type
                Type type = System.Type.GetType(name, silent);
                if (type == null && !silent)
                {
                    if (types.Count == 0)
                        Compute.RecordError($"A type corresponding to {name} cannot be found.");
                    else
                    {
                        string message = "Ambiguous match: Multiple types correspond the the name provided: \n";
                        foreach (Type t in types)
                            message += "- " + t.FullName + "\n";

                        message += "To get a Engine type from a specific Assembly, try adding ', NameOfTheAssmebly' at the end of the name string, or use the AllEngineTypes method to retreive all the types.";

                        Compute.RecordError(message);
                    }
                }

                return type;
            }
        }

        /***************************************************/

        public static List<Type> AllTypes(string name, bool silent = false)
        {
            List<Type> typeList = new List<Type>();
            if (name.StartsWith("BH.Engine"))
                typeList = Query.EngineTypeList();
            else if (name.StartsWith("BH.Adapter"))
                typeList = Query.AdapterTypeList();
            else if (name.StartsWith("BH.oM"))
                typeList = Query.BHoMTypeList();
            else
                typeList = Query.AllTypeList();

            List<Type> types = typeList.Where(x => x.AssemblyQualifiedName.Contains(name)).ToList();

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

