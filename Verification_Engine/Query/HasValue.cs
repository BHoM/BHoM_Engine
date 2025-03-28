﻿/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
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

namespace BH.Engine.Verification
{
    public static partial class Query
    {
        /***************************************************/
        /****             Interface Methods             ****/
        /***************************************************/

        [Description("Checks if a given object has value (e.g. if a string is not empty etc.).")]
        [Input("obj", "Object to check for valid value.")]
        [Output("hasValue", "True if the input object has value, otherwise false.")]
        public static bool? IHasValue(this object obj)
        {
            if (obj == null)
                return false;

            object result;
            if (!BH.Engine.Base.Compute.TryRunExtensionMethod(obj, nameof(HasValue), out result))
            {
                BH.Engine.Base.Compute.RecordError($"Can't check if object has value because type {obj.GetType().Name} is currently not supported.");
                return null;
            }

            return (bool?)result;
        }


        /***************************************************/
        /****              Public Methods               ****/
        /***************************************************/

        [Description("Checks if a given number has value (is not NaN).")]
        [Input("obj", "Number to check for valid value.")]
        [Output("hasValue", "True if the number has value, otherwise false.")]
        public static bool? HasValue(this double obj)
        {
            return !double.IsNaN(obj);
        }

        /***************************************************/

        [Description("Checks if a given string is not null or empty.")]
        [Input("obj", "String to check for valid value.")]
        [Output("hasValue", "True if the string is not null or empty, otherwise false.")]
        public static bool? HasValue(this string obj)
        {
            return !string.IsNullOrEmpty(obj);
        }

        /***************************************************/

        [Description("Checks if a given object has value (e.g. if a string is not empty etc.).")]
        [Input("obj", "Object to check for valid value.")]
        [Output("hasValue", "True if the input object has value, otherwise false.")]
        public static bool? HasValue(this object obj)
        {
            return obj != null;
        }

        /***************************************************/
    }
}
