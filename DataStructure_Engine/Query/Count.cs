using BH.oM.DataStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.DataStructure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static int Count<T>(this PriorityQueue<T> queue) where T : IComparable<T>
        {
            return queue.Data.Count();
        }

        /***************************************************/

        public static int Count<T>(this Graph<T> graph) 
        {
            return graph.Nodes.Count();
        }

        /***************************************************/

        public static int Count<T>(this Tree<T> tree)
        {
            if (tree.Children.Count == 0)
                return 1;
            else
                return tree.Children.Sum(x => x.Value.Count());
        }

        /***************************************************/

        public static int Count<T>(this VennDiagram<T> diagram)
        {
            return diagram.Intersection.Count + diagram.OnlySet1.Count + diagram.OnlySet2.Count;
        }

        /***************************************************/
    }
}
