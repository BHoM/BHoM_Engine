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

using BH.oM.Base;
using BH.oM.Data.Collections;
using BH.oM.Diffing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Reflection;
using BH.Engine.Serialiser;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using BH.oM.Reflection.Attributes;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace BH.Engine.Diffing
{
    public static partial class Convert
    {
        ///***************************************************/
        ///**** Public Methods                            ****/
        ///***************************************************/

        public static string ToDiffingJson(this object obj, PropertyInfo[] fieldsToNullify)
        {
            List<PropertyInfo> propList = fieldsToNullify.ToList();
            if (propList == null && propList.Count == 0)
                return BH.Engine.Serialiser.Convert.ToJson(obj);

            List<string> propNames = new List<string>();
            propList.ForEach(prop => propNames.Add(prop.Name));
            return ToDiffingJson(obj, propNames);
        }

        ///***************************************************/

        public static string ToDiffingJson(this object obj, List<string> fieldsToRemove)
        {
            if (fieldsToRemove == null && fieldsToRemove.Count == 0)
                return BH.Engine.Serialiser.Convert.ToJson(obj);

            Stopwatch sw = Stopwatch.StartNew();
            string jsonObj = BH.Engine.Serialiser.Convert.ToJson(obj);
            sw.Stop();

            long timespan1 = sw.ElapsedMilliseconds;

            sw = Stopwatch.StartNew();
            jsonObj = JsonConvert.SerializeObject(obj);
            sw.Stop();

            long timespan2 = sw.ElapsedMilliseconds;



            // Sets fields to be ignored as null, without altering the tree.
            List<Regex> regexes = new List<Regex>();

            foreach (string fieldName in fieldsToRemove)
                regexes.Add(new Regex(fieldName, RegexOptions.Compiled));

            return RemoveFields(jsonObj, regexes);
        }

        ///***************************************************/


        private static string RemoveFields(string json, IEnumerable<Regex> regexes)
        {
            JToken token = JToken.Parse(json);
            RemoveFields(token, regexes);
            return token.ToString();
        }

        private static void RemoveFields(JToken token, IEnumerable<Regex> regexes)
        {
            if (token.Type == JTokenType.Object)
            {
                foreach (JProperty prop in token.Children<JProperty>().ToList())
                {
                    bool removed = false;
                    foreach (Regex regex in regexes)
                    {
                        if (regex.IsMatch(prop.Name))
                        {
                            prop.Remove();
                            removed = true;
                            break;
                        }
                    }
                    if (!removed)
                    {
                        RemoveFields(prop.Value, regexes);
                    }
                }
            }
            else if (token.Type == JTokenType.Array)
            {
                foreach (JToken child in token.Children())
                {
                    RemoveFields(child, regexes);
                }
            }
        }

    }
}

