/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using BH.oM.Base.Attributes;
using System.ComponentModel;

namespace BH.Engine.Reflection
{
    public static partial class Query
    {
        /***************************************************/
        /**** Interface Methods                         ****/
        /***************************************************/

        [Description("Checks whether an object has been marked as deprecated, indicating it is no longer valid to be used.")]
        [Input("obj", "The object to check the deprecated status of.")]
        [Output("isDeprecated", "True if the object is set to be replaced or removed, false otherwise.")]
        public static bool IIsDeprecated(this object obj)
        {
            if (obj == null)
                return false;

            return IsDeprecated(obj as dynamic);
        }


        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Checks whether a method has been marked as deprecated, indicating it is no longer valid to be used.")]
        [Input("method", "The method to check the deprecated status of.")]
        [Output("isDeprecated", "True if the method is set to be replaced or removed, false otherwise.")]
        public static bool IsDeprecated(this MethodBase method)
        {
            if (method == null)
                return false;

            ToBeRemovedAttribute deletedAttribute = method.GetCustomAttribute<ToBeRemovedAttribute>();
            ReplacedAttribute replacedAttribute = method.GetCustomAttribute<ReplacedAttribute>();

            if (deletedAttribute != null || replacedAttribute != null)
                return true;

            if (method is ConstructorInfo)
            {
                return method.DeclaringType.IsDeprecated();
            }

            return false;
        }

        /***************************************************/

        [Description("Checks whether a type has been marked as deprecated, indicating it is no longer valid to be used.")]
        [Input("type", "The type to check the deprecated status of.")]
        [Output("isDeprecated", "True if the type is set to be replaced or removed, false otherwise.")]
        public static bool IsDeprecated(this Type type)
        {
            if (type == null)
                return false;

            ToBeRemovedAttribute deletedAttribute = type.GetCustomAttribute<ToBeRemovedAttribute>();
            ReplacedAttribute replacedAttribute = type.GetCustomAttribute<ReplacedAttribute>();

            if (deletedAttribute != null || replacedAttribute != null)
                return true;
            else
                return false;
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static bool IsDeprecated(object obj)
        {
            // Fallback case, if the Interface method is called and no overload is found at runtime, it will end up here
            return false;
        }

        /***************************************************/

    }
}






