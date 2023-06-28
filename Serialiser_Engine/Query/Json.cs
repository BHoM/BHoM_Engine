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

using BH.oM.Base.Attributes;
using MongoDB.Bson;
using System;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace BH.Engine.Serialiser
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        
        [Description("Checks if a string is a valid JSON format.")]
        [Input("str", "String to check validity of.")]
        [Output("validity", "True if string is a valid JSON format, false otherwise.")]
        public static bool IsValidJson(this string str)
        {

            if (str == "" || str.HasTrailingCommas())
            {
                return false;
            }
            else if (str.StartsWith("["))
            {

                return str.IsJsonArray();
            }
            else if (str.StartsWith("{"))
            {
                BsonDocument document;
                return (BsonDocument.TryParse(str, out document));
            }
            else
            {
                bool matched = Regex.IsMatch(str, "[[]{}]");
                if (!Regex.IsMatch(str, "[[]{}]")) return IsValidJson("[" + str + "]");
            }

            return false;

        }

        public static bool IsJsonArray(this string jsonArray)
        {
            if (!jsonArray.StartsWith("[") || !jsonArray.EndsWith("]"))
            {
                return false;
            }

            BsonArray array = new BsonArray();
            try
            {
                array = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonArray>(jsonArray);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private static bool HasTrailingCommas(this string str)
        {
            //JSON checking trailing commas regex based on: https://stackoverflow.com/questions/34344328/json-remove-trailing-comma-from-last-object
            return Regex.IsMatch(str, @"\,(?!\s*?[\{\[\""\w])");
        }
    }
}

