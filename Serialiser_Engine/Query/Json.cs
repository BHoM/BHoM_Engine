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

namespace BH.Engine.Serialiser
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Checks if a string is a valid Json")]
        [Input("string", "String to check.")]
        [Output("validity", "True if string is a json, false otherwise.")]
        public static bool IsValidJson(this string json)
        {
            if (json == "")
            {
                return false;
            }
            else if (json.StartsWith("{"))
            {
                BsonDocument document;
                if (BsonDocument.TryParse(json, out document))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (json.StartsWith("["))
            {

                return IsValidJsonArray(json);
            }
            return true;

        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        //Json Array is automatically checked during checking IsValidJson, so no need to expose this.
        private static bool IsValidJsonArray(this string jsonArray)
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



            foreach (BsonValue value in array)
            {
                if (!value.ToString().IsValidJson()) { return false; }
            }

            return true;
        }

        /***************************************************/

    }
}

