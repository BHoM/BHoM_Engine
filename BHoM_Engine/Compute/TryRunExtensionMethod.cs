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
            result = null;

            System.Reflection.MethodInfo mi = Query.ExtensionMethodToCall(obj, methodName);

            if (mi == null) return false;

            result = Compute.RunExtensionMethod(obj, mi);
            return true;
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
            result = null;

            System.Reflection.MethodInfo mi = Query.ExtensionMethodToCall(obj, methodName, parameters);

            if (mi == null) return false;

            result = Compute.RunExtensionMethod(obj, mi, parameters);
            return true;
        }
    }
}



