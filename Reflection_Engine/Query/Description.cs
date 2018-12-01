using BH.Engine.Reflection.Convert;
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
            else if (member.MemberType != null && member.ReflectedType != null)
                return member.Name + " is a " + member.MemberType.ToString() + " of " + member.ReflectedType.ToText(true);
            else
                return "";
        }

        /***************************************************/

        [Description("Return the custom description of a C# method argument")]
        public static string Description(this ParameterInfo parameter)
        {
            IEnumerable<InputAttribute> inputDesc = parameter.Member.GetCustomAttributes<InputAttribute>().Where(x => x.Name == parameter.Name);
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
