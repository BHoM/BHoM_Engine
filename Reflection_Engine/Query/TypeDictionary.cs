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

using System;
using System.Collections.Generic;

namespace BH.Engine.Reflection
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Dictionary<string, List<Type>> BHoMTypeDictionary()
        {
            lock (m_GetTypesLock)
            {
                // If the dictionary exists already return it
                if (m_BHoMTypeDictionary != null && m_BHoMTypeDictionary.Count > 0)
                    return m_BHoMTypeDictionary;

                // Otherwise, create it
                m_BHoMTypeDictionary = new Dictionary<string, List<Type>>();
                ExtractTypesFromNewAssemblies();

                return m_BHoMTypeDictionary;
            }
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static void AddBHoMTypeToDictionary(string name, Type type)
        {
            if (m_BHoMTypeDictionary.ContainsKey(name))
                m_BHoMTypeDictionary[name].Add(type);
            else
            {
                List<Type> list = new List<Type>();
                list.Add(type);
                m_BHoMTypeDictionary[name] = list;
            }

            int firstDot = name.IndexOf('.');
            if (firstDot >= 0)
                AddBHoMTypeToDictionary(name.Substring(firstDot + 1), type);
        }
        

        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        private static Dictionary<string, List<Type>> m_BHoMTypeDictionary = new Dictionary<string, List<Type>>();


        /***************************************************/
    }
}


