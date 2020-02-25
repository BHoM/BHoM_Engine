/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2020, the respective contributors. All rights reserved.
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
using BH.oM.Quantities.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace BH.Engine.Reflection
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Return the custom description of a C# class member (e.g. property, method, field)")]
        public static string Description(this MemberInfo member)
        {
            DescriptionAttribute descriptionAttribute = member.GetCustomAttribute<DescriptionAttribute>();
            QuantityAttribute quantityAttribute = member.GetCustomAttribute<QuantityAttribute>();

            string desc = "";
            if (descriptionAttribute != null && !string.IsNullOrWhiteSpace(descriptionAttribute.Description))
                desc = descriptionAttribute.Description + Environment.NewLine;

            if (member is PropertyInfo)
            {
                desc += ((PropertyInfo)member).PropertyType.Description(quantityAttribute) + Environment.NewLine;
            }

            if (member.ReflectedType != null)
                desc += member.Name + " is a " + member.MemberType.ToString() + " of " + member.ReflectedType.ToText(true);

            return desc;
        }

        /***************************************************/

        [Description("Return the custom description of a C# method argument")]
        public static string Description(this ParameterInfo parameter)
        {
            IEnumerable<InputAttribute> inputDesc = parameter.Member.GetCustomAttributes<InputAttribute>().Where(x => x.Name == parameter.Name);
            QuantityAttribute quantityAttribute = null;
            string desc = "";
            if (inputDesc.Count() > 0)
            {
                desc = inputDesc.First().Description + Environment.NewLine;
                quantityAttribute = inputDesc.First().Quantity;
            }
            else
            {
                //If no input descs are found, check if inputFromProperty descs can be found
                IEnumerable<InputFromProperty> inputFromPropDesc = parameter.Member.GetCustomAttributes<InputFromProperty>().Where(x => x.InputName == parameter.Name);

                //Only valid for engine type methods
                MethodInfo methodInfo = parameter.Member as MethodInfo;
                if (inputFromPropDesc.Count() > 0 && methodInfo != null && methodInfo.DeclaringType != null)
                {
                    Type returnType = methodInfo.ReturnType.UnderlyingType().Type;
                    if (returnType != null)
                    {
                        //Try to find matching proeprty type, matching both name and type
                        PropertyInfo prop = returnType.GetProperty(inputFromPropDesc.First().PropertyName, parameter.ParameterType);

                        //If found return description of property
                        if (prop != null)
                            return prop.Description();
                    }
                }
            }
            if (parameter.ParameterType != null)
            {
                desc += parameter.ParameterType.Description(quantityAttribute);
            }
            return desc;
        }

        /***************************************************/

        [Description("Return the custom description of a C# class")]
        public static string Description(this Type type)
        {
            return Description(type, null);
        }

        /***************************************************/
        [Description("Return the custom description of a C# class")]
        public static string Description(this Type type, QuantityAttribute quantityAttribute)
        {
            if (type == null)
            {
                return "";
            }

            DescriptionAttribute attribute = type.GetCustomAttribute<DescriptionAttribute>();

            string desc = "";


            //If a quantity attribute is present, this is used to generate the default description
            if (quantityAttribute != null)
            {
                desc += "This is a " + quantityAttribute.GetType().Name + " [" + quantityAttribute.SIUnit + "]";
                desc += " (as a " + type.ToText(type.Namespace.StartsWith("BH.")) + ")";
                return desc;
            }

            //Add the default description
            desc += "This is a " + type.ToText(type.Namespace.StartsWith("BH."));

            if (attribute != null)
            {
                desc += ":" + Environment.NewLine;
                desc += attribute.Description;
            }
            Type innerType = type;

            while (typeof(IEnumerable).IsAssignableFrom(innerType) && innerType.IsGenericType)
                innerType = innerType.GenericTypeArguments.First();

            if (innerType.IsInterface)
            {
                desc += Environment.NewLine;
                desc += "This can be of the following types: ";
                List<Type> t = innerType.ImplementingTypes();
                int m = Math.Min(15, t.Count);

                for (int i = 0; i < m; i++)
                    desc += $"{t[i].ToText()}, ";

                if (t.Count > m)
                    desc += "and more...";
                else
                    desc = desc.Remove(desc.Length - 2, 2);

                return desc;
            }

            return desc;

        }

        /***************************************************/

        [Description("Return the custom description of a C# element such as Type, MemberInfo, and ParamaterInfo")]
        [Input("item", "This item can either be a Type, a MemberInfo, or a ParamaterInfo")]
        public static string IDescription(this object item)
        {
            if (item is ParameterInfo)
                return Description(item as ParameterInfo);
            else if (item is Type)
                return Description(item as Type);
            else if (item is MemberInfo)
                return Description(item as MemberInfo);
            else
                return "";
        }

        /***************************************************/
    }
}

