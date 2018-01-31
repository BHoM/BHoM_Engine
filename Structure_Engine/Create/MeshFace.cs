using BH.oM.Structural.Elements;
using BH.oM.Structural.Properties;
using System;
using System.Collections.Generic;
using System.Linq;


namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static MeshFace MeshFace(Property2D property, Node n0, Node n1, Node n2, Node n3 = null)
        {
            MeshFace mf = new MeshFace { Nodes = new List<Node> { n0, n1, n2 }, Property = property };

            if (n3 != null)
                mf.Nodes.Add(n3);

            return mf;
        }

        /***************************************************/

        public static MeshFace MeshFace(Property2D property, IEnumerable<Node> nodes)
        {
            int nodeCount = nodes.Count();
            if (nodeCount != 3 && nodeCount != 4)
                throw new ArgumentException("Mesh faces only support 3 or 4 nodes");

            return new MeshFace { Nodes = nodes.ToList(), Property = property };
        }

        /***************************************************/
    }
}
