using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;


namespace BH.Engine.Reflection
{
    static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<MethodInfo> ExtentionMethods(this Type type, string methodName)
        {
            List<MethodInfo> methods = new List<MethodInfo>();

            foreach (MethodInfo method in BHoMMethodList().Where(x => x.Name == methodName))
            {
                ParameterInfo[] param = method.GetParameters();

                if (param.Length > 0 && param[0].ParameterType == type)
                    methods.Add(method);
            }

            return methods;
        }

        /***************************************************/
    }
}
