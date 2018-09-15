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

        public static Type OutputType(this MethodBase method)
        {
            if (method is MethodInfo)
                return ((MethodInfo)method).ReturnType;
            else if (method is ConstructorInfo)
                return ((ConstructorInfo)method).DeclaringType;
            else
                return null;
        }

        /***************************************************/
    }
}
