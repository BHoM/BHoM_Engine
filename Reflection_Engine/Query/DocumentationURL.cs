using System;
using System.Collections.Generic;
using System.Text;

using BH.oM.Base.Attributes;
using BH.oM.Base.Attributes.Enums;
using System.Reflection;
using System.Linq;
using BH.oM.Base;
using System.ComponentModel;

namespace BH.Engine.Reflection
{
    public static partial class Query
    {
        [Description("Query the documentation URLs available for a given C# member.")]
        [Input("member", "The C# member to query the documentation URLs for.")]
        [Output("urls", "The available documentation URLs for the given C# member. The list will be empty if no URLs are available.")]
        public static List<DocumentationURLAttribute> DocumentationURL(this MemberInfo member)
        {
            if(member == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the documentation URL of a null member info object.");
                return new List<DocumentationURLAttribute>();
            }

            return member.GetCustomAttributes<DocumentationURLAttribute>().ToList();
        }
    }
}
