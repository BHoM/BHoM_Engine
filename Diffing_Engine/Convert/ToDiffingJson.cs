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


namespace BH.Engine.Diffing
{
    public static partial class Convert
    {
        ///***************************************************/
        ///**** Public Methods                            ****/
        ///***************************************************/

        public static string ToDiffingJson(this object obj, PropertyInfo[] fieldsToNullify = null)
        {
            List<PropertyInfo> propList = fieldsToNullify.ToList();
            if (propList == null && propList.Count == 0)
                return BH.Engine.Serialiser.Convert.ToJson(obj);

            List<string> propNames = new List<string>();
            propList.ForEach(prop => propNames.Add(prop.Name));
            return ToDiffingJson(obj, propNames);
        }
        
        ///***************************************************/

        public static string ToDiffingJson(this object obj, List<string> fieldsToNullify)
        {
            if (fieldsToNullify == null && fieldsToNullify.Count == 0)
                return BH.Engine.Serialiser.Convert.ToJson(obj);

            var jObject = JsonConvert.DeserializeObject<JObject>(BH.Engine.Serialiser.Convert.ToJson(obj));

            // Sets fields to be ignored as null, without altering the tree.
            fieldsToNullify.ForEach(propName =>
            jObject.Properties()
                .Where(attr => attr.Name.StartsWith(propName))
                .ToList()
                .ForEach(attr => attr.Value = null)
            );
            return jObject.ToString();
        }

        ///***************************************************/

    }
}

