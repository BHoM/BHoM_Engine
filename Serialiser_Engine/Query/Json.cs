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
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace BH.Engine.Serialiser
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        //JSON validation based on code from https://github.com/prototypejs/prototype/blob/560bb59414fc9343ce85429b91b1e1b82fdc6812/src/prototype/lang/string.js#L699
        
        [Description("Checks if a string is a valid Json")]
        [Input("string", "String to check.")]
        [Output("validity", "True if string is a json, false otherwise.")]
        public static bool IsValidJson(this string str)
        {

            if (string.IsNullOrWhiteSpace(str))
                return false;
            else if (str.HasTrailingCommas()) return false;

            str = Regex.Replace(str, @"\\(?:[""\\\/bfnrt]|u[0-9a-fA-F]{4})", "@");
            str = Regex.Replace(str, @"""[^""\\\n\r]*""|true|false|null|-?\d+(?:\.\d*)?(?:[eE][+\-]?\d+)?", "]");
            str = Regex.Replace(str, @"(?:^|:|,)(?:\s*\[)+", "");

            return Regex.IsMatch(str, @"^[\],:{}\s]*$");

        }

        //JSON checking trailing coomas regex based on: https://stackoverflow.com/questions/34344328/json-remove-trailing-comma-from-last-object
        private static bool HasTrailingCommas(this string str)
        {
            return Regex.IsMatch(str, @"\,(?!\s*?[\{\[\""\w])");
        }

    }
}

