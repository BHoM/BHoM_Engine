using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace BH.Engine.Reflection
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Return the custom description of a C# class member")]
        public static string Description(this MemberInfo member)
        {
            DescriptionAttribute attribute = member.GetCustomAttribute<DescriptionAttribute>();
            if (attribute != null)
                return attribute.Description;
            else
                return "";
        }

        /***************************************************/

        [Description("Return the custom description of a C# method argument")]
        public static string Description(this ParameterInfo parameter)
        {
            DescriptionAttribute attribute = parameter.GetCustomAttribute<DescriptionAttribute>();
            if (attribute != null)
                return attribute.Description;
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
                return "";
        }

        /***************************************************/

        [Description("Return the custom description of a C# element such as Type, MethodInfo, and ParamaterInfo")]
        public static string IDescription(
            [Description("This item can either be a Type, a MethodInfo, or a ParamaterInfo")] this object item
        )
        {
            if (item is MemberInfo)
                return Description(item as MemberInfo);
            else if (item is ParameterInfo)
                return Description(item as ParameterInfo);
            else if (item is Type)
                return Description(item as Type);
            else
                return "";
        }

        /***************************************************/
    }
}
