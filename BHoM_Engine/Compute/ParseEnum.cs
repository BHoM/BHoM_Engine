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
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Base
{
    public static partial class Compute
    {
        /*******************************************/
        /**** Public Methods                    ****/
        /*******************************************/

        [Description("Converts a string into its corresponding enum of type T")]
        [Input("value", "String representation of the enum to be created")]
        [Output("Enum of type T with a value matching the input string")]
        public static T ParseEnum<T>(string value)
        {
            object result = ParseEnum(typeof(T), value);
            if (result == null)
                return default(T);
            else
                return (T)result;
        }

        /*******************************************/

        [Description("Converts a string into its corresponding enum of type enumType")]
        [Input("enumType", "Type of enum to be created")]
        [Input("value", "String representation of the enum to be created")]
        [Output("Enum of type enumType with a value matching the input string")]
        public static object ParseEnum(Type enumType, string value)
        {
            if (Enum.IsDefined(enumType, value))
                return Enum.Parse(enumType, value);
            else
            {
                return Enum.GetValues(enumType).OfType<Enum>()
                    .FirstOrDefault(x => {
                        FieldInfo fi = enumType.GetField(x.ToString());
                        DisplayTextAttribute[] displayTexts = fi.GetCustomAttributes(typeof(DisplayTextAttribute), false) as DisplayTextAttribute[];
                        DescriptionAttribute[] descriptions = fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

                        return (displayTexts != null && displayTexts.Count() > 0 && displayTexts.First().Text == value)
                            || (descriptions != null && descriptions.Count() > 0 && descriptions.First().Description == value);
                    });
            }
        }

        /*******************************************/
    }
}

