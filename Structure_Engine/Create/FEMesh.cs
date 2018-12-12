using BH.oM.Structure.Elements;
using BH.oM.Structure.Properties.Surface;
using System;
using System.Collections.Generic;
using System.Linq;
using BH.oM.Geometry;
using BH.Engine.Geometry;


namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static FEMesh FEMesh(BH.oM.Geometry.Mesh mesh, ISurfaceProperty property = null, string name = null)
        {
            FEMesh feMesh = new FEMesh();
            feMesh.Nodes = mesh.Vertices.Select(x => Node(x)).ToList();

            foreach (Face face in mesh.Faces)
            {
                FEMeshFace feFace = new FEMeshFace();
                feFace.NodeListIndices.Add(face.A);
                feFace.NodeListIndices.Add(face.B);
                feFace.NodeListIndices.Add(face.C);

                if (face.IsQuad())
                    feFace.NodeListIndices.Add(face.D);

                feMesh.MeshFaces.Add(feFace);

            }

            if (property != null)
                feMesh.Property = property;

            if (name != null)
                feMesh.Name = name;

            return feMesh;
        }

        /***************************************************/
    }
}
