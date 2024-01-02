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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace BH.Engine.Reflection
{
    public static partial class Query
    {
        /***************************************************/
        /**** Interface Methods                         ****/
        /***************************************************/

        [Description("Obtain the URL to the code of the given object. The URL returned will link directly to the source code for that object if it exists.")]
        [Input("obj", "An object to obtain the URL for.")]
        [Output("url", "The URL of the source code for the object. Null if no source code URL could be ascertained.")]
        public static string IUrl(this object obj)
        {
            if (obj == null)
            {
                return null;
            }
            else if (obj is Type)
            {
                return Url(obj as Type);
            }
            else if (obj is MethodBase)
            {
                return Url(obj as MethodBase);
            }
            else
            {
                return null;
            }
        }


        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Obtain the URL to the code of the given type. The URL returned will link directly to the source code for that type if it exists.")]
        [Input("type", "The type to obtain the URL for.")]
        [Output("url", "The URL of the source code for the type. Null if no source code URL could be ascertained.")]
        public static string Url(this Type type)
        {
            if (type == null)
                return null;

            Assembly ass = type.Assembly;
            if (ass == null)
                return null;

            AssemblyDescriptionAttribute att = ass.GetCustomAttribute<AssemblyDescriptionAttribute>();
            if (att == null)
                return null;

            string url = att.Description;
            if (url == "")
                return null;

            List<string> path = new List<string>() { url, "blob/main/" };
            path.Add(ass.GetName().Name);
            path.AddRange(type.Namespace.Split('.').Skip(3));
            if (type.IsEnum)
                path.Add("Enums");
            else if (type.IsInterface)
                path.Add("Interface");
            path.Add(type.Name + ".cs");
            url = System.IO.Path.Combine(path.ToArray());

            return url;
        }

        /***************************************************/

        [Description("Obtain the URL to the code of the given method. The URL returned will link directly to the source code for that method if it exists.")]
        [Input("method", "The method to obtain the URL for.")]
        [Output("url", "The URL of the source code for the method. Null if no source code URL could be ascertained.")]
        public static string Url(this MethodBase method)
        {
            if (method == null)
                return null;

            Assembly ass = method.DeclaringType.Assembly;
            if (ass == null)
                return null;

            AssemblyDescriptionAttribute att = ass.GetCustomAttribute<AssemblyDescriptionAttribute>();
            if (att == null)
                return null;

            string url = att.Description;

            List<string> path = new List<string>() { url, "blob/main/" };
            path.Add(ass.GetName().Name);
            path.AddRange(method.DeclaringType.Namespace.Split('.').Skip(3));
            path.Add(method.DeclaringType.Name);
            path.Add(method.Name + ".cs");
            url = System.IO.Path.Combine(path.ToArray());

            return url;
        }

        /***************************************************/
    }
}





