/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
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
using BH.oM.Base.Attributes;
using System.ComponentModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace BH.Engine.Base
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Checks if a collection is null or empty and outputs relevant error message.")]
        [Input("collection", "The collection to test for null value or emptiness.")]
        [Input("methodName", "The name of the method to reference in the error message.")]
        [Input("msg", "Optional message to be returned in addition to the generated error message.")]
        [Output("isNull", "True if the collection is null or empty.")]
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> collection, string msg = "", [CallerMemberName] string methodName = "")
        {
            return collection.NullCheckCollection(true, msg, methodName);
        }

        /***************************************************/

        [Description("Checks if a collection is null and outputs relevant error message.")]
        [Input("collection", "The collection to test for null value.")]
        [Input("methodName", "The name of the method to reference in the error message.")]
        [Input("msg", "Optional message to be returned in addition to the generated error message.")]
        [Output("isNull", "True if the collection is null.")]
        public static bool IsNull<T>(this IEnumerable<T> collection, string msg = "", [CallerMemberName] string methodName = "")
        {
            return collection.NullCheckCollection(false, msg, methodName);
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static bool NullCheckCollection<T>(this IEnumerable<T> collection, bool checkForEmpty, string msg, string methodName)
        {
            if (collection == null)
            {
                if (string.IsNullOrEmpty(methodName))
                {
                    methodName = "Method";
                }
                Base.Compute.RecordError($"Cannot evaluate {methodName} because the input collection failed a null check. {msg}");

                return true;
            }
            else if (checkForEmpty && !collection.Any())
            {
                if (string.IsNullOrEmpty(methodName))
                {
                    methodName = "Method";
                }
                Base.Compute.RecordError($"Cannot evaluate {methodName} because the input collection is empty. {msg}");

                return true;
            }

            return false;
        }

        /***************************************************/
    }
}



