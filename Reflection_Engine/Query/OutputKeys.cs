/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2026, the respective contributors. All rights reserved.
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

using BH.Engine.Base;
using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Reflection
{
    public static partial class Query
    {
        /*************************************/
        /**** Public Methods              ****/
        /*************************************/

        [Description("Gets all the text representations of types that accept the provided type as input.")]
        [Input("type", "The type to get the output key from.")]
        [Output("Keys", "Text representations of types that accept the provided type as input.")]
        public static List<string> OutputKeys(this Type type)
        {
            if (m_OutputTypeKeys.ContainsKey(type))
                return m_OutputTypeKeys[type];
            else
            {
                List<string> keys = new List<Type> { type }
                    .Concat(type.BaseTypes().Where(x => x.Namespace?.StartsWith("BH.") == true))
                    .Select(x => x.ToText(true))
                    .ToList();

                m_OutputTypeKeys[type] = keys;
                return keys;
            }
        }

        /*************************************/
        /**** Private Fields              ****/
        /*************************************/

        private static Dictionary<Type, List<string>> m_OutputTypeKeys = new Dictionary<Type, List<string>>();

        /*************************************/
    }
}
