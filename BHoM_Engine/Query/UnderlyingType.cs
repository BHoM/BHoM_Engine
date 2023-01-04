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

using BH.oM.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Base.Attributes;
using BH.oM.Base.Reflection;

namespace BH.Engine.Base
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static UnderlyingType UnderlyingType(this Type type)
        {
            if (type == null)
                return null;

            int depth = 0;
            while (typeof(IEnumerable).IsAssignableFrom(type) && type != typeof(string))
            {
                Type subType = type.GetElementType();
                
                if (subType == null)
                {
                    Type[] generics = type.GetGenericArguments();
                    if (generics.Count() == 1)
                        subType = generics.First();
                    else if (generics.Count() == 0)
                    {
                        foreach (ConstructorInfo constructor in type.GetConstructors())
                        {
                            ParameterInfo[] parameters = constructor.GetParameters();
                            if (parameters.Count() == 1)
                            {
                                string paramType = parameters[0].ParameterType.Name;
                                if (paramType == "List`1" || paramType == "IEnumerable`1")
                                {
                                    subType = parameters[0].ParameterType.GetGenericArguments()[0];
                                    break;
                                }   
                            }
                        }
                    }
                }

                if (subType != null)
                {
                    type = subType;
                    depth++;
                }
                else
                    break;
            }

            return new UnderlyingType { Type = type, Depth = depth };
        }

        /***************************************************/
    }
}




