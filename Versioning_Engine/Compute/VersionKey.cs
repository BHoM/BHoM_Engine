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

using BH.Engine.Reflection;
using BH.oM.Base;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
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
using BH.oM.Reflection.Attributes;

namespace BH.Engine.Versioning
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Provide a string representation of a method as it used for versioning by the PreviousVersion attribute.")]
        [Input("declaringType", "Type in which the method is declared. You can use just the name of the type or include a (part of the) namespace in front of it.")]
        [Input("methodName", "Name of the method. It has to be the exact string. If the method is a constructor, you can leave this input blank.")]
        [Output("keys", "String representation for each method that matches the input filters.")]
        public static List<string> VersioningKey(string declaringType, string methodName = "")
        {
            if (methodName == "")
                methodName = ".ctor";

            return Engine.Reflection.Query.AllMethodList()
                .Where(x => x.Name == methodName && x.DeclaringType.FullName.EndsWith(declaringType))
                .Select(x => x.VersioningKey())
                .ToList();
        }

        /***************************************************/
    }
}
