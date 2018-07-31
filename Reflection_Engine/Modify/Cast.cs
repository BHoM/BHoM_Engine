using BH.oM.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BH.Engine.Reflection
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static object CastGeneric<T>(this IEnumerable<T> source, Type genericType)
        {
            return typeof(System.Linq.Enumerable).GetMethod("Cast").MakeGenericMethod(genericType).Invoke(null, new object[] { source });
        }

        /***************************************************/
    }
}
