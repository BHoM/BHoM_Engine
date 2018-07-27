using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using BH.oM.Reflection.Attributes;

namespace BH.Engine.Reflection
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static bool IsDeprecated(this MethodBase method)
        {
            Deprecated attribute = method.GetCustomAttribute<Deprecated>();
            if (attribute != null)
            {
                try
                {
                    Version version = new Version(attribute.FromVersion);
                    return (version.CompareTo(method.DeclaringType.Assembly.GetName().Version) <= 0);
                }
                catch
                {
                    return true;
                }
            }
            else
                return false;
        }


        /***************************************************/

        public static bool IsDeprecated(this Type type) 
        {
            Deprecated attribute = type.GetCustomAttribute<Deprecated>();
            if (attribute != null)
            {
                try
                {
                    Version version = new Version(attribute.FromVersion);
                    return (version.CompareTo(type.Assembly.GetName().Version) > 0);
                }
                catch
                {
                    return true;
                }
            }
            else
                return false;
        }


        /***************************************************/



    }
}
