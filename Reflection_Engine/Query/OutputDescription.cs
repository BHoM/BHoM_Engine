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

        [Description("Return the custom description of the output of a C# method")]
        public static string OutputDescription(this MethodBase method)
        {
            Output attribute = method.GetCustomAttribute<Output>();
            if (attribute != null)
                return attribute.Description;
            else
                return "";
        }

        /***************************************************/
    }
}
