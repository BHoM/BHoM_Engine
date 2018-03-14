using BH.Engine.Reflection.Convert;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace BH.Engine.Reflection
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static string Path(this Type type)
        {
            return type.Namespace;
        }

        /***************************************************/

        public static string Path(this MethodBase method, bool beClever = true) // TODO: beClever probably needs a better name
        {
            Type type = method.DeclaringType;

            if (beClever)
            {
                if (type.Name == "Create" && method is MethodInfo)
                    type = ((MethodInfo)method).ReturnType;
                else if (method.IsDefined(typeof(ExtensionAttribute), false))
                {
                    ParameterInfo[] parameters = method.GetParameters();
                    if (parameters.Length > 0)
                        type = parameters[0].ParameterType;
                }
            }

            return type.ToText(true, true);
        }

        /***************************************************/
    }
}
