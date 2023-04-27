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

using BH.oM.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Test.Versioning
{
    public static partial class Helpers
    {
        /*************************************/
        /**** Public Methods              ****/
        /*************************************/

        public static bool CanReplaceMethodWithType(string json)
        {
            string customJson = json.Replace(" \"_t\" : \"System.Reflection.MethodBase\", ", "");
            CustomObject customObj = Engine.Serialiser.Convert.FromJson(customJson) as CustomObject;
            if (customObj == null || !customObj.CustomData.ContainsKey("TypeName") || !customObj.CustomData.ContainsKey("MethodName"))
                return false;

            string typeJson = customObj.CustomData["TypeName"] as string;
            string methodName = customObj.CustomData["MethodName"] as string;
            if (typeJson == null || methodName == null)
                return false;

            List<string> split = typeJson.Split(new char[] { '"', ',' }).Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
            if (split.Count < 7)
                return false;
            string typeName = split[6];

            Type matchingType = MatchingType(typeName, methodName);
            return matchingType != null;
        }


        /*************************************/
        /**** Private Methods             ****/
        /*************************************/

        private static Type MatchingType(string typeName, string methodName)
        {
            if (m_TypeDic.Count == 0)
            {
                m_TypeDic = Engine.Base.Query.BHoMTypeList()
                    .GroupBy(x => x.Namespace.NameSpaceKey())
                    .ToDictionary(x => x.Key, x => x.ToList());
            }

            if (!typeName.EndsWith("Create"))
                return null;

            string key = typeName.NameSpaceKey();
            List<Type> types = new List<Type>();
            if (m_TypeDic.ContainsKey(key))
                types = m_TypeDic[key];

            Type match = null;
            //Splitting by ` to acount for generics
            List<Type> matches = types.Where(x => x.Name.Split('`')[0] == methodName).ToList();
            if (matches.Count == 1)
                match = matches.First();

            return match;
        }

        /*************************************/

        private static string NameSpaceKey(this string typeName)
        {
            string[] splitNamespace = typeName.Split(new char[] { '.' });
            string key = splitNamespace[2];
            if (key == "Adapters" && splitNamespace.Length > 3)
                key += "." + splitNamespace[3];
            return key;
        }

        /*************************************/
        /**** Private Methods             ****/
        /*************************************/

        private static Dictionary<string, List<Type>> m_TypeDic = new Dictionary<string, List<Type>>();

        /*************************************/
    }
}



