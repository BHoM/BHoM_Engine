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

using BH.oM.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BH.Engine.Reflection
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Dictionary<string, Dictionary<string, object>> PropertyFullNameValueGroups(this object obj, Type typeFilter = null, Type declaringTypeFilter = null, int maxNesting = -1)
        {
            if (typeFilter != null)
            {
                var currentMethod = MethodBase.GetCurrentMethod();

                var result = currentMethod.DeclaringType.GetMethods()
                    .First(mi => mi.Name == currentMethod.Name && mi.GetGenericArguments().Length == 1)
                    .MakeGenericMethod(typeFilter).Invoke(null, new object[] { obj, declaringTypeFilter, maxNesting }) as IDictionary;

                Dictionary<string, Dictionary<string, object>> output = new Dictionary<string, Dictionary<string, object>>();

                return result as Dictionary<string, Dictionary<string, object>>;
            }

            var propertyFullNameValueDictionary = PropertyFullNameValueDictionary<object>(obj, declaringTypeFilter, maxNesting, true);

            return propertyFullNameValueDictionary as Dictionary<string, Dictionary<string, object>>;
        }

        /***************************************************/

        public static Dictionary<string, Dictionary<string, T>> PropertyFullNameValueGroups<T>(this object obj, Type declaringTypeFilter = null, int maxNesting = -1)
        {
            var propertyFullNameValueDictionary = PropertyFullNameValueDictionary<T>(obj, declaringTypeFilter, maxNesting, true);

            return propertyFullNameValueDictionary as Dictionary<string, Dictionary<string, T>>;
        }
    }
}



