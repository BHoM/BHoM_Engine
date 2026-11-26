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

using BH.oM.Base.Attributes;
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

        [Description("Returns the constraint type from a generic type.")]
        [Input("genericType", "The generic type. Type is generic if Type.IsGenericParameter or Type.ContrainsGenericparameters is true.")]
        [Output("type", "The specific type constructed from the generic one.")]
        public static Type MakeFromGeneric(this Type genericType)
        {
            if(genericType == null)
            {
                Base.Compute.RecordError("Cannot make generic from null type.");
                return null;
            }

            return genericType.GenericTypeConstraint();
        }

        /***************************************************/

        [Description("Replaces the generic parameters with parameters whose type is their constraint, and returns a new MethodInfo from those.")]
        [Input("genericMethod", "The generic method. Method is generic if MethodInfo.ContainsGenericParameter is true.")]
        [Output("method", "The specific method constructed from the generic one.")]
        public static MethodInfo MakeFromGeneric(this MethodInfo genericMethod)
        {
            if(genericMethod == null)
            {
                Base.Compute.RecordError("Cannot make generic from a null method.");
                return null;
            }

            if (genericMethod.ContainsGenericParameters)
            {
                Type[] types = genericMethod.GetGenericArguments().Select(x => x.MakeFromGeneric()).ToArray();
                try
                {
                    genericMethod = genericMethod.MakeGenericMethod(types);
                }
                catch (ArgumentException e)
                {
                    // The constraints cannot be satisfied by the types found: the generic method is returned unchanged
                    Base.Compute.RecordWarning($"Could not apply the generic constraints of method {genericMethod.Name} from {genericMethod.DeclaringType?.FullName}. The generic method is returned instead. Exception message: {e.Message}");
                }
            }
            return genericMethod;
        }
    }
}






