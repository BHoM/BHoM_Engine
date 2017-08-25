using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Reflection
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Type CreateType(string name)
        {
            Dictionary<string, Type> typeDictionary = CreateTypeDictionary();

            Type type = null;
            typeDictionary.TryGetValue(name, out type);
            return type;
        }
    }
}
