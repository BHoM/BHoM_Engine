using BH.oM.DataStructure;
using System.Collections.Generic;

namespace BH.Engine.DataStructure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Tree<T> Tree<T>(T value, string name = "")
        {
            return new Tree<T>
            {
                Value = value,
                Name = name
            };
        }

        /***************************************************/

        public static Tree<T> Tree<T>(Dictionary<string, Tree<T>> children, string name = "")
        {
            return new Tree<T>
            {
                Children = children,
                Name = name
            };
        }

        /***************************************************/
    }
}
