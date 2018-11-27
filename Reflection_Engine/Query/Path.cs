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

        public static string Path(this MethodBase method, bool userReturnTypeForCreate = true, bool useExtentionType = false) 
        {
            Type type = method.DeclaringType;

            if (userReturnTypeForCreate && type.Name == "Create" && method is MethodInfo)
            {
                Type returnType = ((MethodInfo)method).ReturnType.UnderlyingType().Type;
                if (returnType.Namespace.StartsWith("BH."))
                    type = returnType;
            }
            else if (useExtentionType && method.IsDefined(typeof(ExtensionAttribute), false))
            {
                ParameterInfo[] parameters = method.GetParameters();
                if (parameters.Length > 0)
                    type = parameters[0].ParameterType;
            }

            return type.ToText(true, true);
        }

        /***************************************************/
    }
}
