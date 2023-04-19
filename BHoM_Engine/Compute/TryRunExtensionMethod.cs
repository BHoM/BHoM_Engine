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

using BH.oM.Base.Debugging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using System.Collections.Concurrent;

namespace BH.Engine.Base
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Looks for an extension method applicable to the input object with the provided `methodName` and, if found, invokes it.\n" +
            "Extension methods are searched using Reflection through all BHoM assemblies.\n" +
            "If no method is found, this returns `false`, and the `result` is null.")]
        [Input("obj", "Object whose extension method is to be found, and to which the method will be applied in order to obtain the result.")]
        [Input("methodName", "Name of the extension method defined for the input object that is to be found in any of the BHoM assemblies.")]
        [Input("result", "Result of the method invocation, if the method had been invoked. If no method is found this is null.")]
        [Output("True if a method was found and an invocation was attempted. False otherwise.")]
        public static bool TryRunExtensionMethod(this object obj, string methodName, out object result)
        {
            return TryRunExtensionMethod(methodName, new object[] { obj }, out result);
        }

        /***************************************************/

        [Description("Looks for an extension method applicable to the input object with the provided `methodName` and  and, if found, invokes it.\n" +
            "Extension methods are searched using Reflection through all BHoM assemblies.\n" +
            "If no method is found, this returns `false`, and the `result` is null.")]
        [Input("obj", "Object whose extension method is to be found, and to which the method will be applied in order to obtain the result.")]
        [Input("methodName", "Name of the extension method defined for the input object that is to be found in any of the BHoM assemblies.")]
        [Input("parameters", "The additional arguments of the call to the method, skipping the first argument provided by 'target'.")]
        [Input("result", "Result of the method invocation, if the method had been invoked. If no method is found this is null.")]
        [Output("True if a method was found and an invocation was attempted. False otherwise.")]
        public static bool TryRunExtensionMethod(this object obj, string methodName, object[] parameters, out object result)
        {
            return TryRunExtensionMethod(methodName, new object[] { obj }.Concat(parameters).ToArray(), out result);
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        [Description("Runs the requested method and returns the result. For performance reasons compiles the method to a function the first time it is run, then stores it for subsequent calls.")]
        private static bool TryRunExtensionMethod(string methodName, object[] parameters, out object result)
        {
            if (parameters == null || parameters.Length == 0 || parameters.Any(x => x == null) || string.IsNullOrWhiteSpace(methodName))
            {
                result = null;
                return false;
            }

            //Construct key used to store/extract method
            string key = methodName + string.Join("", parameters.Select(x => x.GetType().ToString()));
            
            // If the method has been called before, use the previously compiled function
            Func<object[], object> func;
            if (FunctionPreviouslyCompiled(key))
                func = GetStoredCompiledFunction(key);
            else
            {
                MethodInfo method = Query.ExtensionMethodToCall(methodName, parameters);
                if (method == null)
                    func = null;
                else
                {
                    //COmpiles the method to a function for quicker execution
                    Func<object[], object> methodFunc = method.ToFunc();

                    //ExtensionMethodToCall allows for returning methods that har more arguments than the provided parameters, if those have default value.
                    //Check if the provided parameter count matches the parameters of the method
                    ParameterInfo[] parameterInfo = method.GetParameters();
                    if (parameterInfo.Length != parameters.Length)
                    {
                        //If count is different, loop through all additional arguments and find the default value
                        List<object> defaultArgs = new List<object>();
                        for (int i = parameters.Length; i < parameterInfo.Length; i++)
                        {
                            //Check that the parameter has a default value.
                            //This should always be true, based on the logic of the ExtensionMethodToCall but additional check here for saftey
                            if (parameterInfo[i].HasDefaultValue)   
                            {
                                defaultArgs.Add(parameterInfo[i].DefaultValue); //Add the argument to be used when calling the function
                            }
                            else
                            {
                                //Should never happen, but added as a guard against crash
                                func = null;
                                break;
                            }
                        }
                        //Make a new function to store that adds the default arguments to the end of the array of parameters and then calls the method function
                        //This function will be stored rather than the methodFunction
                        func = x => methodFunc(x.Concat(defaultArgs).ToArray());
                    }
                    else
                        func = methodFunc;  //Provided parameter count matches the parameters of the method -> set func to the compiled function of the method
                }
                    
                StoreCompiledFunction(key, func);
            }

            // Try calling the method
            try
            {
                if (func == null)
                {
                    result = null;
                    return false;
                }
                else
                {
                    result = func(parameters);
                    return true;
                }
            }
            catch (Exception e)
            {
                BH.Engine.Base.Compute.RecordError($"Failed to run {methodName} extension method.\nError: {e.Message}");
                result = null;
                return false;
            }
        }

        /***************************************************/

        [Description("Checks if an entry with the provided key has already been extracted. Put in its own method to simplify the use of locks to provide thread safety.")]
        private static bool FunctionPreviouslyCompiled(string methodKey)
        {
            lock (m_RunExtensionMethodLock)
            {
                return m_CompiledFunctions.ContainsKey(methodKey);
            }
        }

        /***************************************************/

        [Description("Gets a previously compiled function from the stored functions. Put in its own method to simplify the use of locks to provide thread safety.")]
        private static Func<object[], object> GetStoredCompiledFunction(string methodKey)
        {
            lock (m_RunExtensionMethodLock)
            {
                return m_CompiledFunctions[methodKey];
            }
        }

        /***************************************************/

        [Description("Stores a compiled function. Put in its own method to simplify the use of locks to provide thread safety.")]
        private static void StoreCompiledFunction(string methodKey, Func<object[], object> method)
        {
            lock (m_RunExtensionMethodLock)
            {
                m_CompiledFunctions[methodKey] = method;
            }
        }


        /***************************************************/
        /**** Private fields                            ****/
        /***************************************************/

        private static ConcurrentDictionary<string, Func<object[], object>> m_CompiledFunctions = new ConcurrentDictionary<string, Func<object[], object>>();
        //private static readonly object m_RunExtensionMethodLock = new object();

        /***************************************************/


    }
}



