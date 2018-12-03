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

        [Description("Return names and descriptions of the multiple outputs of a C# method")]
        public static List<OutputAttribute> OutputAttributes(this MethodBase method)
        {
            if (method.IsMultipleOutputs())
            {
                Dictionary<int, MultiOutputAttribute> outputDefs = method.GetCustomAttributes<MultiOutputAttribute>().ToDictionary(x => x.Index);
                Type[] types = method.OutputType().GetGenericArguments();

                List<OutputAttribute> outputs = new List<OutputAttribute>();
                for (int i = 0; i < types.Length; i++)
                {
                    if (outputDefs.ContainsKey(i))
                        outputs.Add(new OutputAttribute(outputDefs[i].Name, outputDefs[i].Description));
                    else
                        outputs.Add(new OutputAttribute(types[i].UnderlyingType().Type.Name.Substring(0, 1), ""));
                }
                return outputs;
            }
            else
            {
                OutputAttribute attribute = method.GetCustomAttribute<OutputAttribute>();
                if (attribute != null)
                    return new List<OutputAttribute> { attribute };
                else
                    return new List<OutputAttribute>();
            }
        }

        /***************************************************/
    }
}
