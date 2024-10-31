/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
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
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Finds an extension method accepting a multiple argument based on a provided main object and method name.\n" +
                     "The method is found via reflection at the first time it is queried, then it is stored for subsequent calls.")]
        [Input("target", "First argument of the method to find.")]
        [Input("methodName", "The name of the method to be sought.")]
        [Output("method", "Most suitable extension method with requested target, name and parameters. If no method was found, null is returned.")]
        public static MethodInfo ExtensionMethodToCall(this object target, string methodName)
        {
            return ExtensionMethodToCall(methodName, new object[] { target });
        }

        /***************************************************/

        [Description("Finds an extension method accepting a multiple argument based on a provided main object and method name and additional arguments.\n" +
                     "The method is found via reflection at the first time it is queried, then it is stored for subsequent calls.")]
        [Input("target", "First argument of the method to find.")]
        [Input("methodName", "The name of the method to be sought.")]
        [Input("parameters", "The additional arguments of the call to the method, skipping the first argument provided by 'target'.")]
        [Output("method", "Most suitable extension method with requested target, name and parameters. If no method was found, null is returned.")]
        public static MethodInfo ExtensionMethodToCall(this object target, string methodName, object[] parameters)
        {
            return ExtensionMethodToCall(methodName, new object[] { target }.Concat(parameters).ToArray());
        }


        /***************************************************/

        [Description("Method doing the heavy lifting of ExtensionMethodToCall. Finds the matching method via reflection and caches if for subsequent calls.\n" +
                     "Finds an extension method accepting multiple arguments with extra emphasis on the first argument in terms of type matching.\n" +
                     "Method found can have more arguments than the provided parameters, if all of those additional arguments have default values.")]
        [Input("methodName", "The name of the method to be sought.")]
        [Input("parameters", "The arguments of the call to the method.")]
        [Output("method", "Most suitable extension method with requested name and parameters. If no method was found, null is returned.")]
        public static MethodInfo ExtensionMethodToCall(string methodName, object[] parameters)
        {
            if (parameters == null || parameters.Length == 0 || parameters[0] == null)
            {
                BH.Engine.Base.Compute.RecordError("Can't find extension method to call because provided method arguments are not valid.");
                return null;
            }

            if (string.IsNullOrWhiteSpace(methodName))
            {
                BH.Engine.Base.Compute.RecordError("Can't find extension method to call because provided method name is not valid.");
                return null;
            }

            // Get types of input arguments, to be used for first method extraction filtering
            Type[] types = parameters.Select(x => x?.GetType()).ToArray();

            // Construct key used to store/extract method
            string name = methodName + parameters.Select(x => x?.GetType()?.ToString() ?? "null").Aggregate((a, b) => a + b);
            Tuple<Type, string> key = new Tuple<Type, string>(types[0], name);

            // If the method has been called before, just use that
            if (MethodPreviouslyExtracted(key))
                return GetStoredExtensionMethod(key);

            // Loop through all methods with matching name, pick applicable ones, then sort by best match to the provided inputs
            var applicableMethods = types[0].ExtensionMethods(methodName).Where(x => x.IsApplicable(parameters)).ExtensionMethodHierarchy(types);
            
            // Return null if no applicable methods
            if (applicableMethods.Count == 0 || applicableMethods[0].Count == 0)
            {
                StoreExtensionMethod(key, null);
                return null;
            }

            // If more than one method at the top level of applicable method hierarchy, there is a risk of ambiguous call
            // In such case iterate over other method arguments to find a uniquely best matching method
            HashSet<MethodInfo> candidates = new HashSet<MethodInfo>(applicableMethods[0][0]);
            for (int i = 0; i < applicableMethods.Count; i++)
            {
                // If empty collection, it means the input is null so neutral to binding, fine to skip
                if (applicableMethods[i].Count != 0)
                {
                    // Only consider the candidates that were among the most applicable ones so far
                    candidates.IntersectWith(applicableMethods[i][0]);
                    
                    // If no candidates left, there is ambiguity that cannot be solved
                    if (candidates.Count == 0)
                        break;
                }
            }

            // If only one candidate is left, ambiguity is solved and method to call identified
            // If method is generic, make sure the appropriate generic arguments are set
            MethodInfo methodToCall = null;
            if (candidates.Count == 1)
                methodToCall = candidates.First()?.MakeGenericFromInputs(parameters.Select(x => x?.GetType()).ToList());

            // Cache and return the method
            StoreExtensionMethod(key, methodToCall);
            return methodToCall;
        }

        /***************************************************/

        private static bool IsApplicable(this MethodInfo method, object[] parameters)
        {
            ParameterInfo[] paramInfo = method.GetParameters();
            if (paramInfo.Length < parameters.Length)
                return false;

            // Make sure the type of parameters is matching, skipping first as already used to extract parameters
            for (int i = 1; i < paramInfo.Length; i++)
            {
                //If more parameters to method then provided, check if parameter has default value (is optional)
                if (i >= parameters.Length)
                {
                    if (!paramInfo[i].Attributes.HasFlag(ParameterAttributes.HasDefault))
                    {
                        //No default value -> no match -> not applicable
                        return false;
                    }
                }
                else if ((parameters[i] != null && !paramInfo[i].ParameterType.IsAssignableFromIncludeGenerics(parameters[i].GetType()))
                    || (parameters[i] == null && !paramInfo[i].ParameterType.IsNullable()))
                {
                    //Parameter does not match, not applicable
                    return false;
                }
            }

            return true;
        }

        /***************************************************/

        [Description("Checks if an entry with the provided key has already been extracted. Put in its own method to simplify the use of locks to provide thread safety.")]
        private static bool MethodPreviouslyExtracted(Tuple<Type, string> key)
        {
            lock (m_ExtensionMethodToCallLock)
            {
                return m_PreviousFoundMethods.ContainsKey(key);
            }
        }

        /***************************************************/

        [Description("Gets a previously extracted method from the stored methods. Put in its own method to simplify the use of locks to provide thread safety.")]
        private static MethodInfo GetStoredExtensionMethod(Tuple<Type, string> key)
        {
            lock (m_ExtensionMethodToCallLock)
            {
                return m_PreviousFoundMethods[key];
            }
        }

        /***************************************************/

        [Description("Stores an extracted method. Put in its own method to simplify the use of locks to provide thread safety.")]
        private static void StoreExtensionMethod(Tuple<Type, string> key, MethodInfo method)
        {
            lock (m_ExtensionMethodToCallLock)
            {
                m_PreviousFoundMethods[key] = method;
            }
        }


        /***************************************************/
        /**** Private fields                            ****/
        /***************************************************/

        private static ConcurrentDictionary<Tuple<Type, string>, MethodInfo> m_PreviousFoundMethods = new ConcurrentDictionary<Tuple<Type, string>, MethodInfo>();
        private static readonly object m_ExtensionMethodToCallLock = new object();

        /***************************************************/
    }
}




