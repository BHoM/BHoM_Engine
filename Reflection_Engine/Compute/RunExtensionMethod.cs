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
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Reflection
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Runs an extension method accepting a single argument based on a provided object and method name.\n" +
                     "Finds the method via reflection the first time it is run, thean compiles it to a function and stores it for subsequent calls.")]
        [Input("target", "The object to find and run the extension method for.")]
        [Input("methodName", "The name of the method to be run.")]
        [Output("result", "The result of the method execution. If no method was found, null is returned.")]
        public static object RunExtensionMethod(object target, string methodName)
        {
            Func<object[], object> method = Query.ExtensionMethodToCall(target, methodName);
            return RunExtensionMethod(target, method);
        }

        /***************************************************/

        [Description("Runs an extension method accepting a multiple argument based on a provided main object and method name and additional arguments.\n" +
                     "Finds the method via reflection the first time it is run, thean compiles it to a function and stores it for subsequent calls.")]
        [Input("target", "The first of the argument of the method to find and run the extention method for.")]
        [Input("methodName", "The name of the method to be run.")]
        [Input("parameters", "The additional arguments of the call to the method, skipping the first argument provided by 'target'.")]
        [Output("result", "The result of the method execution. If no method was found, null is returned.")]
        public static object RunExtensionMethod(object target, string methodName, object[] parameters)
        {
            Func<object[], object> method = Query.ExtensionMethodToCall(target, methodName, parameters);
            return RunExtensionMethod(target, method, parameters);
        }

        /***************************************************/

        [Description("Runs given extension method (in a form of compiled function) accepting a single argument.")]
        [Input("target", "The object to run the extension method on.")]
        [Input("method", "The method to be run, provided in a form of compiled function.")]
        [Output("result", "The result of the method execution. If no or invalid method was provided, null is returned.")]
        public static object RunExtensionMethod(object target, Func<object[], object> method)
        {
            return RunExtensionMethod(method, new object[] { target });
        }

        /***************************************************/
        
        [Description("Runs given extension method (in a form of compiled function) accepting multiple arguments.")]
        [Input("target", "The object to run the extension method on.")]
        [Input("method", "The method to be run, provided in a form of compiled function.")]
        [Input("parameters", "The additional arguments of the call to the method, skipping the first argument provided by 'target'.")]
        [Output("result", "The result of the method execution. If no or invalid method was provided, null is returned.")]
        public static object RunExtensionMethod(object target, Func<object[], object> method, object[] parameters)
        {
            return RunExtensionMethod(method, new object[] { target }.Concat(parameters).ToArray());
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        [Description("Runs the requested function and returns the result.")]
        private static object RunExtensionMethod(Func<object[], object> method, object[] parameters)
        {
            // Throw an error and return null if method is null
            if (method == null)
            {
                BH.Engine.Reflection.Compute.RecordError($"Failed to run the extension method because it was null.");
                return null;
            }

            // Try calling the method
            try
            {
                return method(parameters);
            }
            catch (Exception e)
            {
                BH.Engine.Reflection.Compute.RecordError($"Failed to run the extension method.\nError: {e.Message}");
                return null;
            }
        }

        /***************************************************/
    }
}


