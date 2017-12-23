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

        public static GraphNode<T> OppositeNode<T>(this GraphLink<T> link, GraphNode<T> node)
        {
            if (node == link.StartNode)
                return link.EndNode;
            else if (node == link.EndNode)
                return link.StartNode;
            else
                return null;
        }

        /***************************************************/
    }
}
