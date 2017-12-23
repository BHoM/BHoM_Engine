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

        public static GraphNode<T> ClosestNode<T>(this Graph<T> graph, T value, Func<T, T, double> nodeDist)
        {
            double minDist = 1e10;
            GraphNode<T> bestNode = null;

            foreach (GraphNode<T> node in graph.Nodes)
            {
                double dist = nodeDist(value, node.Value);
                if (dist < minDist)
                {
                    minDist = dist;
                    bestNode = node;
                }
            }

            return bestNode;
        }

        /***************************************************/
    }
}
