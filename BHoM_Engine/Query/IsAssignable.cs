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

using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.oM.Base;

namespace BH.Engine.Base
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Checks if a type is assignable from another type by first checking the system IsAssignableFrom and, if this is false, checks if the assignable is generic and tests if it can be assigned as a generics version.")]
        [Input("assignableTo", "The type to check if it can be assigned to.")]
        [Input("assignableFrom", "The type to check if it can be assigned from.")]
        [Output("result", "Returns true if 'assignableTo' is assignable from 'assignableFrom'.")]
        public static bool IsAssignableFromIncludeGenerics(this Type assignableTo, Type assignableFrom)
        {
            if(assignableTo == null || assignableFrom == null)
            {
                Compute.RecordError("Cannot assign to or from null types.");
                return false;
            }

            //Check if standard IsAssignableFrom works.
            if (assignableTo.IsAssignableFrom(assignableFrom))
                return true;
            //If not, check if the argument is generic, and if so, use the IsAssignableToGenericType method to check if it can be assigned.
            else
                return assignableTo.IsGenericType && assignableFrom.IsAssignableToGenericType(assignableTo.GetGenericTypeDefinition());
        }

        /***************************************************/

        [Description("Checks if the given method can be assigned to the generic type. Returns false if the second argument is a non generic.")]
        [Input("givenType", "The type to check if it can be assigned to the generic type.")]
        [Input("genericType", "The generic type to check assignability to.")]
        [Output("result", "Returns true if the given type can be assigned to the generic type.")]
        public static bool IsAssignableToGenericType(this Type givenType, Type genericType)
        {
            //Check nulls
            if (givenType == null || genericType == null)
                return false;

            //Check that the generic type is actually a generic
            if (!genericType.IsGenericType)
                return false;

            //Check if the given type is generic, and if so, if its generic type definition matches the generic type
            if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
                return true;

            //Get out the interfaces of the generic type
            var interfaceTypes = givenType.GetInterfaces();

            foreach (var inter in interfaceTypes)
            {
                //Check if the generic type definition for any of the interfaces matches the generic type
                if (inter.IsGenericType && inter.GetGenericTypeDefinition() == genericType)
                    return true;
            }

            //Check if given type contains a base type
            Type baseType = givenType.BaseType;
            if (baseType == null)
                return false;

            //Reqursively check the base type
            return IsAssignableToGenericType(baseType, genericType);
        }

        /***************************************************/

        [Description("Checks if a type is assignable from another type by first checking the system IsAssignableFrom;" +
            "\nif the previous is false, checks if the assignable is generic and tests if it can be assigned as a generics version;" +
            "\nif the previous is false, lastly this checks if the types are a byRef version (&typeName) of the same type.")]
        [Input("assignableTo", "The type to check if it can be assigned to.")]
        [Input("assignableFrom", "The type to check if it can be assigned from.")]
        [Output("result", "Returns true if 'assignableTo' is assignable from 'assignableFrom'.")]
        public static bool IsAssignableFromIncludeGenericsAndRefTypes(this Type assignableTo, Type assignableFrom)
        {
            if (assignableTo == null || assignableFrom == null)
            {
                Compute.RecordError("Cannot assign to or from null types.");
                return false;
            }

            if (IsAssignableFromIncludeGenerics(assignableTo, assignableFrom))
                return true;

            if (assignableTo.FullName.EndsWith("&") || assignableFrom.FullName.EndsWith("&"))
            {
                // Check for reference types
                return assignableTo.Assembly == assignableFrom.Assembly && assignableTo.FullName.Except(assignableFrom.FullName).Count() <= 1;
            }

            return false;
        }
    }
}



