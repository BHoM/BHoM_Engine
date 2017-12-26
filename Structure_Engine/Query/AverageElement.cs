using BH.oM.Structural.Elements;
using System.Collections.Generic;

namespace BH.Engine.Structure
{
    public static partial class Query 
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Node AverageElement(this List<Node> nodes, List<double> weights = null)
        {
            if (weights == null)
                weights = new List<double>(new double[1]);


            Node tempNode = nodes[1];
            return tempNode;        
        }

        /***************************************************/
    }
}
