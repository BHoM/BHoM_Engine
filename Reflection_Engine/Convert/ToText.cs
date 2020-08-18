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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Reflection
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Interface Methods                         ****/
        /***************************************************/

        public static string IToText(this object member, bool includePath = false)
        {
            return ToText(member as dynamic, includePath);
        }


        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static string ToText(this MemberInfo member, bool includePath = false)
        {
            if (member is MethodBase)
                return ToText(member as MethodBase, includePath);
            else if (member is Type)
                return ToText(member as Type, includePath);
            else
                return member.ToString();
        }


        /***************************************************/

        public static string ToText(this MethodBase method, bool includePath = false, string paramStart = "(", string paramSeparator = ", ", string paramEnd = ")", bool removeIForInterface = true, bool includeParamNames = true, int maxParams = 5, int maxChars = 40, bool includeParamPaths = false)
        {
            string name = (method is ConstructorInfo) ? method.DeclaringType.ToText(false, true) : method.Name;
            if (removeIForInterface && Query.IsInterfaceMethod(method))
                name = name.Substring(1);

            string text = name + paramStart;
            try
            {
                ParameterInfo[] parameters = method.GetParameters();

                string paramText = "";
                if (parameters.Length > 0)
                {
                    // Collect parameters text
                    for (int i = 0; i < parameters.Count(); i++)
                    {
                        string singleParamText = includeParamNames ?
                            parameters[i].ParameterType.ToText(includeParamPaths) + " " + parameters[i].Name : parameters[i].ParameterType.ToText(includeParamPaths);

                        if (i == 0)
                        {
                            paramText = singleParamText;
                            continue;
                        }

                        if (i > maxParams || string.Join(paramText, singleParamText).Length > maxChars)
                        {
                            paramText += $", and {parameters.Length - i} more inputs";
                            break;
                        }

                        paramText += ", " + singleParamText;
                    }
                }

                text += paramText;
            }
            catch (Exception e)
            {
                Compute.RecordWarning("Method " + name + " failed to load its paramters.\nError: " + e.ToString());
                text += "?";
            }
            text += paramEnd;

            if (includePath)
            {
                string path = method.Path();
                text = path + '.' + text;
            }

            return text;
        }

        /***************************************************/

        public static string ToText(this Type type, bool includePath = false, bool replaceGeneric = false, string genericStart = "<", string genericSeparator = ", ", string genericEnd = ">")
        {
            IEnumerable<string> interfaces = type.GetInterfaces().Select(x => x.ToString());

            if (!type.IsGenericType)
            {
                if (includePath)
                    return type.Path() + "." + type.Name;
                else
                    return type.Name;
            }
            else
            {
                Type[] types = type.GetGenericArguments();

                if (replaceGeneric && types.Count() == 1 && type.Namespace != null && !type.Namespace.StartsWith("BH"))
                    return types[0].ToText(includePath, replaceGeneric);
                else
                {
                    string text = type.Name.Substring(0, type.Name.IndexOf('`'))
                        + genericStart
                        + types.Select(x => x.ToText(includePath)).Aggregate((x, y) => x + genericSeparator + y)
                        + genericEnd;

                    if (includePath)
                        text = type.Path() + '.' + text;

                    return text;
                }
            }
        }



        /***************************************************/
        /**** Fallback Methods                          ****/
        /***************************************************/

        private static string ToText(object item, bool includePath = false)
        {
            return item.ToString();
        }

        /***************************************************/
    }
}

