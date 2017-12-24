using System;
using System.Collections.Generic;
using System.Reflection;

namespace BH.Engine.Reflection
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Type Type(string name)
        {
            Dictionary<string, List<Type>> typeDictionary = Query.GetBHoMTypeDictionary();

            List<Type> types = null;
            if (!typeDictionary.TryGetValue(name, out types))
                throw new KeyNotFoundException("A type corresponding to " + name + " cannot be found.");
            else if (types.Count == 1)
                return types[0];
            else
            {
                string message = "Multiple types correspond the the name provided: \n";
                foreach (Type type in types)
                    message += "- " + type.FullName + "\n";

                throw new AmbiguousMatchException(message);
            }
                
        }
    }
}
