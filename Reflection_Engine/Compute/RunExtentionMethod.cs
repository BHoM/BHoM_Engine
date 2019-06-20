/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2018, the respective contributors. All rights reserved.
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
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static object RunExtentionMethod(object target, string methodName)
        {
            object result = null;
            Type type = target.GetType();

            // If the method has been called before, just use that
            Tuple < Type, string> key = new Tuple<Type, string>(type, methodName);
            if (m_PreviousInvokedMethods.ContainsKey(key))
                return m_PreviousInvokedMethods[key].Invoke(null, new object[] { target });

            // Otherwise, search for the method and call it if found
            MethodInfo method = type.ExtentionMethods(methodName).Where(x => x.GetParameters().Length == 1).SortExtensionMethods(type).FirstOrDefault();

            if (method != null)
            {
                m_PreviousInvokedMethods[key] = method;
                return method.Invoke(null, new object[] { target });
            }

            // Return null if nothing found
            return result;
        }

        /***************************************************/

        public static object RunExtentionMethod(object target, string methodName, object[] parameters)
        {
            object result = null;
            Type type = target.GetType();

            // If the method has been called before, just use that
            string name = methodName + parameters.Select(x => x.GetType().ToString()).Aggregate((a,b) => a+b);
            Tuple<Type, string> key = new Tuple<Type, string>(type, name);
            if (m_PreviousInvokedMethods.ContainsKey(key))
                return m_PreviousInvokedMethods[key].Invoke(null, new object[] { target }.Concat(parameters).ToArray());


            foreach (MethodInfo method in target.GetType().ExtentionMethods(methodName).Where(x => x.GetParameters().Length == parameters.Length +1).SortExtensionMethods(type))
            {

                ParameterInfo[] paramInfo = method.GetParameters();

                // Make sure the type of parameters is matching
                bool matchingTypes = true;
                for (int i = 0; i < parameters.Length; i++)
                {
                    if (!paramInfo[i + 1].ParameterType.IsAssignableFrom(parameters[i].GetType()))
                    {
                        matchingTypes = false;
                        break;
                    }
                }
                if (!matchingTypes)
                    continue;

                m_PreviousInvokedMethods[key] = method;
                return method.Invoke(null, new object[] { target }.Concat(parameters).ToArray());
            }

            return result;
        }


        /***************************************************/
        /**** Private fields                            ****/
        /***************************************************/

        private static Dictionary<Tuple<Type, string>, MethodInfo> m_PreviousInvokedMethods = new Dictionary<Tuple<Type, string>, MethodInfo>();


        /***************************************************/
    }
}
