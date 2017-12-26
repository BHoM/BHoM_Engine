using BH.oM.Structural.Elements;
using BH.oM.Geometry;
using System.Collections.Generic;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static MeshFace MeshFace(Node n0, Node n1, Node n2, Node n3 = null)
        {
            MeshFace mf = new MeshFace { Nodes = new List<Node> { n0, n1, n2 } };

            if (n3 != null)
                mf.Nodes.Add(n3);

            return mf;
        }

        /***************************************************/
    }
}
