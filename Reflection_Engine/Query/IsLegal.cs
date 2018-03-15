using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace BH.Engine.Reflection
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static bool IsLegal(this MethodInfo method)
        {
            try
            {
                method.GetParameters();
                return method.ReturnType != null;   //void is not null
            }
            catch
            {
                return false;
            }
        }


        /***************************************************/

        public static bool IsLegal(this Type type) //TODO: Check if there is a better way to do this, instead of using a try-catch
        {
            try
            {
                //Checking that all the constructors have loaded parameter types
                type.GetConstructors().SelectMany(x => x.GetParameters()).ToList(); //ToList() there to execute the linq query
            }
            catch
            {
                return false;
            }
            return true;
        }


        /***************************************************/



    }
}
