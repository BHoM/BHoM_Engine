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
using BH.oM.Base.Attributes;
using System.Reflection;
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

        [Description("Returns all properties that have the custom attribute assigned.")]
        [Input("type", "The type to get properties from.")]
        [Input("attributeType", "The attribute type. Needs to be a Type that is inheriting from Attribute.")]
        [Input("checkBaseClass", "If true, the base class(es) properties are also checked in case the attribute has been applied on the property of the base class.")]
        [Input("checkInterfaces", "If true, the interfaces of the types properties are also checked in case the attribute has been applied on the property of the interface.")]
        [Output("props", "All properties that have the attribute assigned.")]
        public static List<PropertyInfo> PropertiesWithAttribute(this Type type, Type attributeType, bool checkBaseClass = true, bool checkInterfaces = true)
        {
            if (type == null || attributeType == null)
            {
                Engine.Base.Compute.RecordError("Cannot extract properties using a null type or null attributeType.");
                return new List<PropertyInfo>();
            }
            
            if (!typeof(Attribute).IsAssignableFrom(attributeType))
            {
                Engine.Base.Compute.RecordError($"{nameof(attributeType)} needs to be of type {typeof(Attribute).FullName}.");
                return new List<PropertyInfo>();
            }

            List<PropertyInfo> attributeProperties = type.GetProperties().Where(x => x.GetCustomAttribute(attributeType) != null).ToList();

            if (checkBaseClass && type.BaseType != null)
            {
                attributeProperties.AddRange(PropertiesWithAttribute(type.BaseType, attributeType, true, false));   //Only require interfaces to be checked on top level object
            }
            
            if (checkInterfaces)
            {
                foreach (Type interfaceType in type.GetInterfaces())
                    attributeProperties.AddRange(PropertiesWithAttribute(interfaceType, attributeType, false, false));  //All interfaces already captured, hence do not need to check interfaces of interfaces
            }

            attributeProperties = attributeProperties.GroupBy(x => x.Name).Select(x => x.First()).ToList();
            return attributeProperties;
        }

        /***************************************************/
    }
}
