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

        public static Tree<T> Tree<T>(List<T> items, List<List<string>> paths, string name = "")
        {
            Tree<T> tree = new Tree<T> { Name = name };

            if (items.Count != paths.Count)
                return tree;

            for (int i = 0; i < items.Count; i++)
            {
                Tree<T> subTree = tree;
                List<string> path = paths[i];

                foreach (string part in path)
                {
                    if (!subTree.Children.ContainsKey(part))
                        subTree.Children.Add(part, new Tree<T> { Name = part });
                    subTree = subTree.Children[part];
                }
                subTree.Value = items[i];
            }

            return tree;
        }



        /***************************************************/
    }
}
