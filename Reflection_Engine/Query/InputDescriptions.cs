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

        [Description("Return the custom description of all inputs of a C# method")]
        [Output("Dictionary where the keys are the names of the inputs, and the values their descriptions")]
        public static Dictionary<string,string> InputDescriptions(this MethodBase method)
        {
            return method.GetCustomAttributes<Input>().ToDictionary(x => x.Name, x => x.Description);
        }

        /***************************************************/
    }
}
