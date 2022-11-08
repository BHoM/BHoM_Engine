/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2022, the respective contributors. All rights reserved.
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
using System.Linq;
using System.Reflection;

namespace BH.Engine.Reflection
{
    public static partial class Query
    {
        /***************************************************/
        /**** Interface Methods                         ****/
        /***************************************************/

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

        public static string Url(this Type type)
        {
            if (type == null)
                return null;

            Assembly ass = type.Assembly;
            if (ass == null)
                return null;

            AssemblyUrlAttribute att = ass.GetCustomAttribute<AssemblyUrlAttribute>();
            if (att == null)
                return null;

            string url = att.Url;
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

        public static string Url(this MethodBase method)
        {
            if (method == null)
                return null;

            Assembly ass = method.DeclaringType.Assembly;
            if (ass == null)
                return null;

            AssemblyUrlAttribute att = ass.GetCustomAttribute<AssemblyUrlAttribute>();
            if (att == null)
                return null;

            string url = att.Url;

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



