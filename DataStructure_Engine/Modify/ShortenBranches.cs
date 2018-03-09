using BH.oM.DataStructure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.DataStructure
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Tree<T> ShortenBranches<T>(this Tree<T> tree)
        {
            foreach (Tree<T> child in tree.Children.Values)
                child.ShortenBranches();

            if (tree.Children.Count == 1 && tree.Children.Values.First().Children.Count > 0)
                tree.Children = tree.Children.Values.First().Children;

            return tree;
        }

        /***************************************************/
    }
}
