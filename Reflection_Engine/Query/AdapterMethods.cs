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

using BH.oM.Base;
using BH.oM.Reflection.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;


namespace BH.Engine.Reflection
{
    static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns a list of MethodInfo for all methods contained in classes whose name ends with `_Adapter`.")]
        public static List<MethodInfo> AdapterMethods()
        {
            // If the list exists already, return it
            if (m_AdapterMethodsList != null && m_AdapterMethodsList.Count > 0)
                return m_AdapterMethodsList;

            // Check if the allMethodList has already been populated; otherwise populate it
            if (m_AllMethodList == null || m_AllMethodList.Count == 0)
                ExtractAllMethods();

            return m_AdapterMethodsList;
        }

        /***************************************************/

        [Description("Returns a collection of all CRUD methods found in all Adapters loaded at runtime, grouped per CRUD type.")]
        [Output("Dictionary with string key, one of: `Create`, `Read`, `Update`, `Delete`; value is the List of CRUD methods.")]
        public static Dictionary<string, List<MethodInfo>> AdapterCRUDMethods()
        {
            List<MethodInfo> adapterMethods = AdapterMethods();

            // Select methods whose name includes any of keywords and group them based on that
            List<string> CreateKeywords = new List<string>() { "Create", "CreateCollection", "CreateObject" };
            List<string> ReadKeywords = new List<string>() { "Read" };
            List<string> UpdateKeywords = new List<string>() { "Update" };
            List<string> DeleteKeywords = new List<string>() { "Delete" };

            Dictionary<string, List<MethodInfo>> GroupedCRUDmethods = new Dictionary<string, List<MethodInfo>>();

            GroupedCRUDmethods.Add("Create", adapterMethods.Where(x => CreateKeywords.Any(s => x.Name.Contains(s))).ToList());
            GroupedCRUDmethods.Add("Read", adapterMethods.Where(x => ReadKeywords.Any(s => x.Name.Contains(s))).ToList());
            GroupedCRUDmethods.Add("Update", adapterMethods.Where(x => UpdateKeywords.Any(s => x.Name.Contains(s))).ToList());
            GroupedCRUDmethods.Add("Delete", adapterMethods.Where(x => DeleteKeywords.Any(s => x.Name.Contains(s))).ToList());

            return GroupedCRUDmethods;
        }

        /***************************************************/

        [Description("Returns a collection of all Types that are supported by some CRUD action, grouped per object type.")]
        [Output("Dictionary with string key, one of: `Create`, `Read`, `Update`, `Delete`;\n" +
            "value is a Dictionary with key: BHoM Type of with some CRUD method that supports it; value: list of CRUD methods compatible with that Type.")]
        public static Dictionary<string, Dictionary<Type, List<MethodInfo>>> CRUDcompatibleTypes()
        {
            Dictionary<string, Dictionary<Type, List<MethodInfo>>> result = new Dictionary<string, Dictionary<Type, List<MethodInfo>>>();

            Dictionary<string, List<MethodInfo>> adapterCRUDMethods = AdapterCRUDMethods();

            result["Create"] = Create_CompatibleTypes(adapterCRUDMethods["Create"]);
            result["Read"] = Read_CompatibleTypes(adapterCRUDMethods["Read"]);
            // Update and Delete to be done.

            return result;
        }

        private static Dictionary<Type, List<MethodInfo>> Create_CompatibleTypes(List<MethodInfo> createMethods)
        {
            Dictionary<Type, List<MethodInfo>> result = new Dictionary<Type, List<MethodInfo>>();

            for (int i = 0; i < createMethods.Count; i++)
            {
                // For Create methods, the compatibility is given by the type of the FIRST parameter of the method.
                ParameterInfo firstPar = createMethods[i].GetParameters().FirstOrDefault();

                if (firstPar != null && firstPar.ParameterType.IsOfAllowedTypes())
                {
                    if (!result.ContainsKey(firstPar.ParameterType))
                        result[firstPar.ParameterType] = new List<MethodInfo>() { createMethods[i] };
                    else
                        result[firstPar.ParameterType].Add(createMethods[i]);
                }
            }

            return result;
        }


        private static Dictionary<Type, List<MethodInfo>> Read_CompatibleTypes(List<MethodInfo> readMethods)
        {
            Dictionary<Type, List<MethodInfo>> result = new Dictionary<Type, List<MethodInfo>>();

            for (int i = 0; i < readMethods.Count; i++)
            {
                // For Read methods, the compatibility is given by the return type of the method.
                Type returnType = readMethods[i].ReturnType;
                if (returnType != null && returnType.IsOfAllowedTypes())
                {
                    if (!result.ContainsKey(returnType))
                        result[returnType] = new List<MethodInfo>() { readMethods[i] };
                    else
                        result[returnType].Add(readMethods[i]);
                }
            }

            return result;
        }


        // To collect CRUD methods that support certain object types, we need to exclude all types that are not one of allowed types.
        private static bool IsOfAllowedTypes(this Type type)
        {
            bool isOfAllowedTypes = false;

            // If the type is a collection with a generic argument, extract the contained type.
            // This way we can collect types like IEnumerable<Bar> --> Bar.
            Type genericType = type.GetGenericArguments().FirstOrDefault();
            if (genericType != null)
                type = genericType;

            if (type.IsSubclassOf(typeof(BHoMObject)))
                isOfAllowedTypes = true;

            if (type.IsSubclassOf(typeof(IObject)) && type != typeof(BHoMObject))
                isOfAllowedTypes = true;

            // Some types must be excluded by default.
            // E.g. BHoM Fragments cannot be a type that can be `Create`d.
            if (typeof(IBHoMFragment).IsAssignableFrom(type))
                isOfAllowedTypes = false;

            return isOfAllowedTypes;
        }

        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

    
    }
}

