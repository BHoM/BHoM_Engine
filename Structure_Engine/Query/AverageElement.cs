using BH.oM.Structure.Elements;
using System.Collections.Generic;
using BH.oM.Reflection.Attributes;
using System;

namespace BH.Engine.Structure
{
    public static partial class Query 
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [NotImplemented]
        public static Node AverageElement(this List<Node> nodes, List<double> weights = null)
        {
            throw new NotImplementedException();

            //Rather throw a not implemented exception then just returning the first node as was done before. Method need complete rewriting

            //if (weights == null)
            //    weights = new List<double>(new double[1]);


            //Node tempNode = nodes[1];
            //return tempNode;        
        }

        /***************************************************/
    }
}
