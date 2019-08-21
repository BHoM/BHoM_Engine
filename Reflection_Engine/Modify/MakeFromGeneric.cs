/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2019, the respective contributors. All rights reserved.
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

using BH.oM.Reflection.Attributes;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace BH.Engine.Reflection
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns the constraint type from a generic type")]
        [Input("genericType", "The generic type. Type is generic if Type.IsGenericParameter or Type.ContrainsGenericparameters is true")]
        [Output("type", "The specific type constructed from the generic one")]
        public static Type MakeFromGeneric(this Type genericType)
        {
            if (genericType.IsGenericParameter)
            {
                Type[] constrains = genericType.GetGenericParameterConstraints();
                if (constrains.Length == 0)
                    return typeof(object);
                else
                    return constrains[0];
            }
            else if (genericType.ContainsGenericParameters)
            {
                Type[] constrains = genericType.GetGenericArguments().Select(x => MakeFromGeneric(x)).ToArray();
                return genericType.GetGenericTypeDefinition().MakeGenericType(constrains);
            }
            else
                return genericType;
        }

        /***************************************************/

        [Description("Replaces the generic parameters with parameters whose type is their constraint, and returns a new MethodInfo from those")]
        [Input("genericMethod", "The generic method. Method is generic if MethodInfo.ContainsGenericParameter is true")]
        [Output("method", "The specific method constructed from the generic one")]
        public static MethodInfo MakeFromGeneric(this MethodInfo genericMethod)
        {
            if (genericMethod.ContainsGenericParameters)
            {
                Type[] types = genericMethod.GetGenericArguments().Select(x => x.MakeFromGeneric()).ToArray();
                genericMethod = genericMethod.MakeGenericMethod(types);

            }
            return genericMethod;
        }
    }
}