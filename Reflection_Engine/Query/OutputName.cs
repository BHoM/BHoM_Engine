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

        [Description("Return the custom name of the output of a C# method")]
        public static string OutputName(this MethodBase method)
        {
            OutputAttribute attribute = method.GetCustomAttribute<OutputAttribute>();
            if (attribute != null)
                return attribute.Name;
            else
                return "";
        }

        /***************************************************/
    }
}
