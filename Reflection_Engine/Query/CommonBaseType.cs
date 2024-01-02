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

using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Reflection
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns the base type that is common to all types in the input list.")]
        [Input("types", "List of types to search for a common base type.")]
        [Output("commonBaseType", "Base type that is common to all types in the input list. If none is found, there is always System.Object.")]
        public static Type CommonBaseType(this List<Type> types)
        {
            if (types == null || !types.Any())
                return default(Type);

            types = types.Where(t => t != null).ToList();

            if (types.Count <= 1)
                return types.FirstOrDefault();

            bool foundCommonBaseType = false;

            Type commonBaseType = null;

            while (!foundCommonBaseType)
            {
                commonBaseType = types[0];

                foundCommonBaseType = true;

                for (int i = 1; i < types.Count; i++)
                {
                    if (commonBaseType.Equals(types[i]))
                        continue;
                    else
                    {
                        // If the current commonBaseType is the indexed type's base type
                        // then we can continue with the test by making the indexed type to be its base type
                        if (commonBaseType.Equals(types[i].BaseType))
                        {
                            types[i] = types[i].BaseType;
                            continue;
                        }
                        // If the current commonBaseType is the indexed type's base type, then we need to change all indexed types
                        // before the current type (which are all identical) to be that base type and restart this loop
                        else if (commonBaseType.BaseType.Equals(types[i]))
                        {
                            for (int j = 0; j <= i - 1; j++)
                            {
                                types[j] = types[j].BaseType;
                            }

                            foundCommonBaseType = false;
                            break;
                        }
                        // The indexed type and the current commonBaseType are not related
                        // So make everything from index 0 up to and including the current indexed type to be their base type
                        // because the common base type must be further back
                        else
                        {
                            for (int j = 0; j <= i; j++)
                            {
                                types[j] = types[j].BaseType;
                            }

                            foundCommonBaseType = false;
                            break;
                        }
                    }
                }

                // If execution has reached here and foundCommonBaseType is true, we have found our common base type, 
                // if foundCommonBaseType is false, the process starts over with the modified types
            }

            return commonBaseType; // There's always at least System.Object.
        }

        /***************************************************/
    }
}





