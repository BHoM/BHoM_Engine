using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Mono.Cecil;
using Mono.Reflection;

namespace BH.Engine.Reflection
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<Type> InheritedTypes(this Type type, bool onlyBHoM = false)
        {
            List<Type> types = type.GetInterfaces().ToList();

            if (type.BaseType != typeof(object))
                types.Add(type.BaseType);

            if (onlyBHoM)
                return types.Where(x => x.Namespace.StartsWith("BH.")).ToList();
            else
                return types;
        }

        /***************************************************/
    }
}
