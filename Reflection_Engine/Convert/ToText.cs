using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Reflection.Convert
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static string ToText(this MethodBase method, bool includePath = false, string paramStart = "(", string paramSeparator = ", ", string paramEnd = ")", bool removeIForInterface = true)
        {
            string name = (method is ConstructorInfo) ? method.DeclaringType.ToText(false, true) : method.Name;
            if (removeIForInterface && Query.IsInterfaceMethod(method))
                name = name.Substring(1);

            string text = name + paramStart;
            try
            {
                ParameterInfo[] parameters = method.GetParameters();
                if (parameters.Length > 0)
                    text += parameters.Select(x => x.ParameterType.ToText()).Aggregate((x, y) => x + paramSeparator + y);
            }
            catch (Exception e)
            {
                Compute.RecordWarning("Method " + name + " failed to load its paramters.\nError: " + e.ToString());
                text += "?";
            }
            text += paramEnd;

            if (includePath)
            {
                string path = method.Path();
                text = path + '.' + text;
            }   

            return text;
        }

        /***************************************************/

        public static string ToText(this Type type, bool includePath = false, bool replaceGeneric = false, string genericStart = "<", string genericSeparator = ", ", string genericEnd = ">")
        {
            IEnumerable<string> interfaces = type.GetInterfaces().Select(x => x.ToString());

            if (!type.IsGenericType)
            {
                if (includePath)
                    return type.Path() + "." + type.Name;
                else
                    return type.Name;
            }
            else
            {
                Type[] types = type.GetGenericArguments();

                if (replaceGeneric && types.Count() == 1 && !type.Namespace.StartsWith("BH"))
                    return types[0].ToText(includePath, replaceGeneric);
                else
                {
                    string text = type.Name.Substring(0, type.Name.IndexOf('`'))
                        + genericStart
                        + types.Select(x => x.ToText()).Aggregate((x, y) => x + genericSeparator + y)
                        + genericEnd;

                    if (includePath)
                        text = type.Path() + '.' + text;

                    return text;
                }
            }
        }


        /***************************************************/
    }
}
