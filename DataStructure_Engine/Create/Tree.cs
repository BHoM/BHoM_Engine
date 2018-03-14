using BH.oM.DataStructure;
using System.Collections.Generic;
using System.Linq;

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

        public static Tree<T> Tree<T>(IEnumerable<T> items, IEnumerable<IEnumerable<string>> paths, string name = "")
        {
            Tree<T> tree = new Tree<T> { Name = name };

            if (items.Count() != paths.Count())
                return tree;

            for (int i = 0; i < items.Count(); i++)
            {
                Tree<T> subTree = tree;

                foreach (string part in paths.ElementAt(i))
                {
                    if (!subTree.Children.ContainsKey(part))
                        subTree.Children.Add(part, new Tree<T> { Name = part });
                    subTree = subTree.Children[part];
                }
                subTree.Value = items.ElementAt(i);
            }

            return tree;
        }



        /***************************************************/
    }
}
