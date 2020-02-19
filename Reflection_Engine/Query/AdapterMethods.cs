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

        public static List<MethodInfo> AdapterMethods()
        {
            return BHoMMethodList().Where(x => x.DeclaringType.Name.Contains("_Adapter")).ToList();
            // If we move the IBHoMAdapter interface from the BHoM_Adapter down in the base BH.oM, we can test for inheritance instead
        }

        /***************************************************/

        [Description("Returns a collection of all CRUD methods found in all Adapters loaded at runtime, grouped per CRUD type.")]
        [Output("Dictionary with key that can be one of the following: `Create`, `Read`, `Update`, `Delete`; corresponding value is the List of CRUD methods.")]
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
        [Output("Dictionary with key that is the Type of the BHoM class that has some CRUD method that supports it; corresponding value is the list of CRUD methods compatible with that Type.")]
        public static Dictionary<Type, List<MethodInfo>> CRUDcompatibleTypes()
        {
            List<MethodInfo> adapterMethods = AdapterMethods();


            return null;
        }

        public static Dictionary<Type, List<MethodInfo>> CreateCompatibleTypes(List<MethodInfo> createMethods)
        {
            createMethods.Where(m => m.GetParameters().First().GetType().IsOfAllowedTypes())
            return null;
        }

        // To collect CRUD methods that support certain object types, we need to exclude all types that are not one of allowed types.
        internal static bool IsOfAllowedTypes(this Type type)
        {
            bool isOfAllowedTypes = false;

            // If the type is a collection with a generic argument, extract the contained type.
            // This way we can collect types like IEnumerable<Bar> --> Bar.
            Type genericType = type.GetGenericArguments().First();
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

    }
}

