using BH.oM.Structure.Elements;
using BH.oM.Structure.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using BH.oM.Geometry;


namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static MeshFace MeshFace(Node n0, Node n1, Node n2, Node n3 = null, IProperty2D property = null, string name = null)
        {
            MeshFace mf = new MeshFace { Nodes = new List<Node> { n0, n1, n2 }, Property = property };

            if (n3 != null)
                mf.Nodes.Add(n3);

            if (name != null)
                mf.Name = name;

            return mf;
        }

        /***************************************************/

        public static MeshFace MeshFace(IEnumerable<Node> nodes, IProperty2D property = null, string name = null)
        {
            int nodeCount = nodes.Count();
            if (nodeCount != 3 && nodeCount != 4)
                throw new ArgumentException("Mesh faces only support 3 or 4 nodes");

            MeshFace mf = new MeshFace { Nodes = nodes.ToList(), Property = property };

            if (name != null)
                mf.Name = name;
            return mf;
        }

        /***************************************************/

        public static List<MeshFace> MeshFaces(BH.oM.Geometry.Mesh mesh, IProperty2D property = null, string name = null)
        {
            List<MeshFace> meshFaces = new List<MeshFace>();

            foreach (Face face in mesh.Faces)
            {
                List<Node> nodes = new List<Node>();
                nodes.Add(new Node() { Position = mesh.Vertices[face.A] });
                nodes.Add(new Node() { Position = mesh.Vertices[face.B] });
                nodes.Add(new Node() { Position = mesh.Vertices[face.C] });

                if (BH.Engine.Geometry.Query.IsQuad(face))
                    nodes.Add(new Node() { Position = mesh.Vertices[face.D] });
                MeshFace mf = new MeshFace() { Property = property, Nodes = nodes };

                if (name != null)
                    mf.Name = name;

                meshFaces.Add(mf);
            }

            return meshFaces;
        }

        /***************************************************/
    }
}
