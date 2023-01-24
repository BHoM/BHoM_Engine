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

using BH.oM.Base.Attributes;
using System;
using System.Linq;

namespace BH.Engine.Reflection
{
    public static partial class Query
    {
        /*******************************************/
        /**** Public Methods                    ****/
        /*******************************************/

        [PreviousVersion("6.1", "BH.Engine.Serialiser.Query.GenericTypeConstraint(System.Type)")]
        public static Type GenericTypeConstraint(this Type type)
        {
            if(type == null)
            {
                Base.Compute.RecordError("Cannot query the generic type constraints when the type is null.");
                return null;
            }

            Type constraint = type.GetGenericParameterConstraints().FirstOrDefault();

            if (constraint == null)
                return typeof(object);
            else if (constraint.ContainsGenericParameters)
            {
                if (constraint.GetGenericArguments().Any(x => x == type))
                    return constraint.GetGenericTypeDefinition().MakeGenericType(new Type[] { typeof(object) });

                Type[] generics = constraint.GetGenericArguments().Select(x => GenericTypeConstraint(x)).ToArray();
                if (generics.Length == 0)
                    generics = new Type[] { typeof(object) };
                return constraint.GetGenericTypeDefinition().MakeGenericType(generics);
            }
            else
                return constraint;
        }

        /*******************************************/
    }
}




