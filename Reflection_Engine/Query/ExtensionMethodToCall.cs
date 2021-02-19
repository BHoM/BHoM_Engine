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

using BH.oM.Reflection.Attributes;
using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Linq;
using System.Reflection;


namespace BH.Engine.Reflection
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/
        
        [Description("Finds an extension method accepting a multiple argument based on a provided main object and method name.\n" +
                     "The method is found via reflection at the first time it is queried, then it gets compiled into a function and stored for subsequent calls.")]
        [Input("target", "First argument of the method to find.")]
        [Input("methodName", "The name of the method to be sought.")]
        [Output("method", "Most suitable extension method with requested target, name and parameters. If no method was found, null is returned.")]
        public static Func<object[], object> ExtensionMethodToCall(object target, string methodName)
        {
            return ExtensionMethodToCall(methodName, new object[] { target });
        }

        /***************************************************/

        [Description("Finds an extension method accepting a multiple argument based on a provided main object and method name and additional arguments.\n" +
                     "The method is found via reflection at the first time it is queried, then it gets compiled into a function and stored for subsequent calls.")]
        [Input("target", "First argument of the method to find.")]
        [Input("methodName", "The name of the method to be sought.")]
        [Input("parameters", "The additional arguments of the call to the method, skipping the first argument provided by 'target'.")]
        [Output("method", "Most suitable extension method with requested target, name and parameters. If no method was found, null is returned.")]
        public static Func<object[], object> ExtensionMethodToCall(object target, string methodName, object[] parameters)
        {
            return ExtensionMethodToCall(methodName, new object[] { target }.Concat(parameters).ToArray());
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        [Description("Helper method doing the heavy lifting of ExtensionMethodToCall. Finds the matching method via reflection, compiles it to a function, caches if for subsequent calls.")]
        private static Func<object[],object> ExtensionMethodToCall(string methodName, object[] parameters)
        {
            if (parameters == null || parameters.Length == 0 || parameters.Any(x => x == null) || string.IsNullOrWhiteSpace(methodName))
                return null;
            
            //Get type of first argument, to be used for first method extraction filtering
            Type type = parameters[0].GetType();

            //Construct key used to store/extract method
            string name = methodName + parameters.Select(x => x.GetType().ToString()).Aggregate((a, b) => a + b);
            Tuple<Type, string> key = new Tuple<Type, string>(type, name);

            // If the method has been called before, just use that
            if (MethodPreviouslyExtracted(key))
                return GetStoredExtensionMethod(key);

            //Loop through all methods with matching name, first argument and number of parameters, sorted by best match to the first argument
            foreach (MethodInfo method in type.ExtensionMethods(methodName).Where(x => x.GetParameters().Length == parameters.Length).SortExtensionMethods(type))
            {
                ParameterInfo[] paramInfo = method.GetParameters();

                // Make sure the type of parameters is matching, skipping first as already used to extract parameters
                bool matchingTypes = true;
                for (int i = 1; i < parameters.Length; i++)
                {
                    if (!paramInfo[i].ParameterType.IsAssignableFromIncludeGenerics(parameters[i].GetType()))
                    {
                        //Parameter does not match, abort for this method
                        matchingTypes = false;
                        break;
                    }
                }

                if (!matchingTypes)
                    continue;

                // If method is generic, make sure the appropriate generic arguments are set
                MethodInfo finalMethod = method.MakeGenericFromInputs(parameters.Select(x => x.GetType()).ToList());
                Func<object[], object> func = finalMethod.ToFunc();
                StoreExtensionMethod(key, func);
                return func;
            }

            // If nothing found, store null, to avoid having to search again in vain
            StoreExtensionMethod(key, null);

            // Return null if nothing found
            return null;
        }

        /***************************************************/

        [Description("Checks if an entry with the provided key has already been extracted. Put in its own method to simplify the use of locks to provide thread safety.")]
        private static bool MethodPreviouslyExtracted(Tuple<Type, string> key)
        {
            lock (m_RunExtensionMethodLock)
            {
                return m_PreviousInvokedMethods.ContainsKey(key);
            }
        }

        /***************************************************/

        [Description("Gets a previously extracted method from the stored methods. Put in its own method to simplify the use of locks to provide thread safety.")]
        private static Func<object[], object> GetStoredExtensionMethod(Tuple<Type, string> key)
        {
            lock (m_RunExtensionMethodLock)
            {
                return m_PreviousInvokedMethods[key];
            }
        }

        /***************************************************/

        [Description("Stores an extracted method. Put in its own method to simplify the use of locks to provide thread safety.")]
        private static void StoreExtensionMethod(Tuple<Type, string> key, Func<object[], object> method)
        {
            lock (m_RunExtensionMethodLock)
            {
                m_PreviousInvokedMethods[key] = method;
            }
        }


        /***************************************************/
        /**** Private fields                            ****/
        /***************************************************/

        private static ConcurrentDictionary<Tuple<Type, string>, Func<object[], object>> m_PreviousInvokedMethods = new ConcurrentDictionary<Tuple<Type, string>, Func<object[], object>>();
        private static readonly object m_RunExtensionMethodLock = new object();

        /***************************************************/
    }
}


