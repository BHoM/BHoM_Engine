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

        public static Type Type(string name)
        {
            Dictionary<string, Type> typeDictionary = TypeDictionary();

            Type type = null;
            typeDictionary.TryGetValue(name, out type);
            return type;
        }
    }
}
