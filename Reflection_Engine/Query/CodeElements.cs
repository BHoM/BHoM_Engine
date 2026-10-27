/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2026, the respective contributors. All rights reserved.
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

using BH.Engine.Base;
using BH.oM.Base.Attributes;
using BH.oM.Base.Reflection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;

namespace BH.Engine.Reflection
{
    public static partial class Query
    {
        /*************************************/
        /**** Public Methods              ****/
        /*************************************/

        [Description("Collect all the code elements that can be used to create UI components from the loaded assemblies.")]
        [Output("codeElements", "All code elements already loaded that can be used in the UI to create components.")]
        public static List<CodeElementRecord> CodeElements()
        {
            List<CodeElementRecord> items = new List<CodeElementRecord>();

            //TODO: add filter by assembly names to avoid creation of all first to then treeshake (as the current flow)
            //TODO: add filter by code element type maybe? not sure though... could try to optimise Revit_Tk by only loading types and extension methods?

            /// Types

            // All constructable BHoM objects
            items.AddRange(Query.ConstructableTypeItems()
                .Select(x => WrapInTryCatch(() => CodeElement(x, CodeElementType.Constructor, x.ConstructorText()))));

            // All adapter constructors
            items.AddRange(Query.AdapterConstructorItems()
                .Select(x => WrapInTryCatch(() => CodeElement(x, CodeElementType.Constructor, x.ToText(true)))));

            // All Enums
            items.AddRange(Query.EnumItems()
                .Select(x => WrapInTryCatch(() => CodeElement(x, CodeElementType.Enum, x.ToText(true)))));

            // All Types
            items.AddRange(Query.TypeItems()
                .Select(x => WrapInTryCatch(() => CodeElement(x, CodeElementType.Type, x.ToText(true)))));

            /// Methods

            // All methods for the BHoM Engine
            items.AddRange(BH.Engine.Base.Query.BHoMMethodList()
                        .Where(x => x.IsExposed())
                        //TODO: remove I at postfilter
                        //.Select(x => CodeElement(x, GetMethodType(x), x.ToText(includePath: true, removeIForInterface: false))));
                        .Select(x => WrapInTryCatch(() => CodeElement(x, GetMethodType(x), x.ToText(true)))));

            // All methods from external class
            items.AddRange(Query.ExternalItems()
                .Select(x => WrapInTryCatch(() => CodeElement(x, CodeElementType.Method_External, x.ToText(true)))));

            // Return the list
            return items;
        }

        /*************************************/

        [Description("Collect code elements from the loaded assemblies that match the provided assembly names.")]
        [Input("assemblyNames", "Assembly names to filter the code elements by.")]
        [Output("codeElements", "Code elements from the specified assemblies.")]
        public static List<CodeElementRecord> CodeElements(IEnumerable<string> assemblyNames)
        {
            HashSet<string> names = new HashSet<string>(assemblyNames, StringComparer.OrdinalIgnoreCase);
            return CodeElements().Where(x => names.Contains(x.AssemblyName)).ToList();
        }


        /*************************************/
        /**** Public Methods              ****/
        /*************************************/

        private static CodeElementRecord CodeElement(Type type, CodeElementType elementType, string displayText)
        {
            List<Type> inputTypes = type.GetProperties()
                .Select(x => x.PropertyType?.UnderlyingType()?.Type)
                .Where(x => x != null)
                .Distinct()
                .ToList();

            return new CodeElementRecord
            {
                AssemblyName = AssemblyName(type),
                AssemblyModifiedTime = AssemblyModifiedTime(type),
                Type = elementType,
                DisplayText = displayText,
                //Json = type.ToJson(),
                InputKeys = inputTypes.Select(x => x.ToText(true)).ToList(),
                OutputKeys = type.UnderlyingType()?.Type.OutputKeys()
            };
        }

        /*************************************/

        //TODO: made temp public, to rethink how to do it right
        public static CodeElementRecord CodeElement(MethodBase method, CodeElementType elementType, string displayText)
        {
            Type outputType = (method is MethodInfo) ? ((MethodInfo)method).ReturnType : method.DeclaringType;
            List<Type> inputTypes = method.GetParameters()
                .Select(x => x.ParameterType?.UnderlyingType()?.Type)
                .Where(x => x != null)
                .Distinct()
                .ToList();

            return new CodeElementRecord
            {
                AssemblyName = AssemblyName(method),
                AssemblyModifiedTime = AssemblyModifiedTime(method),
                Type = elementType,
                DisplayText = displayText,
                //Json = method.ToJson(),
                InputKeys = inputTypes.Select(x => x.ToText(true)).ToList(),
                OutputKeys = outputType.UnderlyingType()?.Type.OutputKeys()
            };
        }

        /*************************************/

        private static string AssemblyName(MethodBase method)
        {
            return AssemblyName(method.DeclaringType);
        }

        /*************************************/

        private static string AssemblyName(Type type)
        {
            return type.Assembly.GetName().Name;
        }

        /*************************************/

        private static DateTime AssemblyModifiedTime(MethodBase method)
        {
            return AssemblyModifiedTime(method.DeclaringType);
        }

        /*************************************/

        private static DateTime AssemblyModifiedTime(Type type)
        {
            if (string.IsNullOrEmpty(type?.Assembly?.Location))
                return DateTime.MinValue;
            else
                return File.GetLastWriteTimeUtc(type.Assembly.Location);
        }

        /*************************************/

        //TODO: made temp public, to rethink how to do it right
        public static CodeElementType GetMethodType(MethodInfo method)
        {
            switch (method.DeclaringType.Name)
            {
                case "Create":
                    return CodeElementType.Method_Create;
                case "Compute":
                    return CodeElementType.Method_Compute;
                case "Convert":
                    return CodeElementType.Method_Convert;
                case "Modify":
                    return CodeElementType.Method_Modify;
                case "Query":
                    return CodeElementType.Method_Query;
                default:
                    return CodeElementType.Undefined;
            }
        }

        /*************************************/

        private static T WrapInTryCatch<T>(Func<T> func)
        {
            try
            {
                return func();
            }
            catch
            {
                return default;
            }
        }

        /*************************************/
    }
}
