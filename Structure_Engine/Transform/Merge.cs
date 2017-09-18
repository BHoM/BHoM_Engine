using BH.oM.Geometry;
using BH.oM.Structural.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Structure
{
    public static partial class Transform 
    {

        public static Node Merge(this List<Node> nodes, List<double> weights = null)
        {
            if (weights == null)
                weights = new List<double>(new double[1]);


            Node tempNode = nodes[1];
            return tempNode;        
            
        }

        
    }

}
