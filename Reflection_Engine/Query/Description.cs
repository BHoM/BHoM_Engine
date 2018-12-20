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

using BH.oM.Reflection.Attributes;
using System;
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
            DescriptionAttribute attribute = member.GetCustomAttribute<DescriptionAttribute>();
            if (attribute != null)
                return attribute.Description;
            else if (member.ReflectedType != null)
                return member.Name + " is a " + member.MemberType.ToString() + " of " + member.ReflectedType.ToText(true);
            else
                return "";
        }

        /***************************************************/

        [Description("Return the custom description of a C# method argument")]
        public static string Description(this ParameterInfo parameter)
        {
            IEnumerable<Input> inputDesc = parameter.Member.GetCustomAttributes<Input>().Where(x => x.Name == parameter.Name);
            if (inputDesc.Count() > 0)
                return inputDesc.First().Description;
            else if (parameter.ParameterType != null)
                return parameter.Name + " is a " + parameter.ParameterType.ToText();
            else
                return "";
        }

        /***************************************************/

        [Description("Return the custom description of a C# class")]
        public static string Description(this Type type)
        {
            DescriptionAttribute attribute = type.GetCustomAttribute<DescriptionAttribute>();
            if (attribute != null)
                return attribute.Description;
            else
                return "This is a " + type.ToText();
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
