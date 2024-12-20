/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
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
using BH.oM.Base.Attributes;
using System.ComponentModel;
using System.Threading.Tasks;

namespace BH.Engine.Base
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Runs an extension method accepting a single argument based on a provided object and method name.\n" +
                     "Finds the method via reflection the first time it is run, then compiles it to a function and stores it for subsequent calls.")]
        [Input("target", "The object to find and run the extension method for.")]
        [Input("methodName", "The name of the method to be run.")]
        [Output("result", "The result of the method execution. If no method was found, null is returned.")]
        public static object RunExtensionMethod(object target, string methodName)
        {
            if (TryRunExtensionMethod(target, methodName, out object result))
                return result;
            else
                return null;
        }

        /***************************************************/

        [Description("Runs an extension method accepting a multiple argument based on a provided main object and method name and additional arguments.\n" +
                     "Finds the method via reflection the first time it is run, then compiles it to a function and stores it for subsequent calls.")]
        [Input("target", "The first of the argument of the method to find and run the extention method for.")]
        [Input("methodName", "The name of the method to be run.")]
        [Input("parameters", "The additional arguments of the call to the method, skipping the first argument provided by 'target'.")]
        [Output("result", "The result of the method execution. If no method was found, null is returned.")]
        public static object RunExtensionMethod(object target, string methodName, object[] parameters)
        {
            if(TryRunExtensionMethod(target, methodName, parameters, out object result))
                return result;
            else
                return null;
        }

        /***************************************************/

        [Description("Asynchronously runs an extension method accepting a single argument based on a provided object and method name.\n" +
                     "Finds the method via reflection the first time it is run, then compiles it to a function and stores it for subsequent calls.")]
        [Input("target", "The object to find and run the extension method for.")]
        [Input("methodName", "The name of the method to be run.")]
        [Output("result", "The result of the method execution. If no method was found, null is returned.")]
        public static async Task<object> RunExtensionMethodAsync(object target, string methodName)
        {
            Output<bool, object> result = await TryRunExtensionMethodAsync(target, methodName);
            if (result.Item1)
                return result.Item2;
            else
                return null;
        }

        /***************************************************/

        [Description("Asynchronously runs an extension method accepting multiple arguments based on a provided main object and method name and additional arguments.\n" +
                     "Finds the method via reflection the first time it is run, then compiles it to a function and stores it for subsequent calls.")]
        [Input("target", "The first of the argument of the method to find and run the extention method for.")]
        [Input("methodName", "The name of the method to be run.")]
        [Input("parameters", "The additional arguments of the call to the method, skipping the first argument provided by 'target'.")]
        [Output("result", "The result of the method execution. If no method was found, null is returned.")]
        public static async Task<object> RunExtensionMethodAsync(object target, string methodName, object[] parameters)
        {
            Output<bool, object> result = await TryRunExtensionMethodAsync(target, methodName, parameters);
            if (result.Item1)
                return result.Item2;
            else
                return null;
        }

        /***************************************************/
    }
}






