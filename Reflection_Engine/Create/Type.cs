/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2018, the respective contributors. All rights reserved.
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

using System;
using System.Collections.Generic;
using System.Reflection;

namespace BH.Engine.Reflection
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Type Type(string name)
        {
            Dictionary<string, List<Type>> typeDictionary = Query.BHoMTypeDictionary();

            List<Type> types = null;
            if (!typeDictionary.TryGetValue(name, out types))
                throw new KeyNotFoundException("A type corresponding to " + name + " cannot be found.");
            else if (types.Count == 1)
                return types[0];
            else
            {
                string message = "Multiple types correspond the the name provided: \n";
                foreach (Type type in types)
                    message += "- " + type.FullName + "\n";

                throw new AmbiguousMatchException(message);
            }  
        }

        /***************************************************/
    }
}
