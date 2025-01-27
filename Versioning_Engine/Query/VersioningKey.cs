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

using BH.Engine.Reflection;
using BH.oM.Base;
using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BH.Engine.Base;

namespace BH.Engine.Versioning
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Provide a string representation of a method as it used for versioning by the PreviousVersion attribute.")]
        [Input("method", "Method to generate the key for")]
        [Output("key", "String representation of the method as it will be used by the PreviousVersion attribute.")]
        public static string VersioningKey(this MethodBase method)
        {
            if (method == null)
                return "";

            string name = method.Name;
            if (name == ".ctor")
                name = "";
            else
                name = "." + name;

            string declaringType = method.DeclaringType.FullName;

            string parametersString = "";
            List<string> parameterTypes = method.GetParameters().Select(x => x.ParameterType.MakeFromGeneric().ToText(true)).ToList();
            if (parameterTypes.Count > 0)
                parametersString = parameterTypes.Aggregate((a, b) => a + ", " + b);

            return declaringType + name + "(" + parametersString + ")";
        }

        /***************************************************/
    }
}





