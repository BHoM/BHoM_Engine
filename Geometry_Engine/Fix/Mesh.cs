using BH.Engine.Geometry;
using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Geometry
{
    public static partial class Fix
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Mesh GetWithMergedVertices(this Mesh mesh, double tolerance = 0.001) //TODO: use the point matrix 
        {
            List<Face> faces = mesh.Faces.Select(x => x.GetClone() as Face).ToList();
            List<VertexIndex> vertices = mesh.Vertices.Select((x, i) => new VertexIndex(x.GetClone() as Point, i)).ToList();
            
            foreach( Face face in faces)
            {
                vertices[face.A].Faces.Add(face);
                vertices[face.B].Faces.Add(face);
                vertices[face.C].Faces.Add(face);

                if (face.IsQuad())
                    vertices[face.A].Faces.Add(face);
            }

            vertices.Sort(delegate (VertexIndex v1, VertexIndex v2)
            {
                return v1.Location.GetDistance(Point.Origin).CompareTo(v2.Location.GetDistance(Point.Origin));
            });


            List<int> culledIndices = new List<int>();

            for (int i = 0; i < vertices.Count; i++)
            {
                double distance = vertices[i].Location.GetDistance(Point.Origin);
                int j = i + 1;
                while (j < vertices.Count && Math.Abs(vertices[j].Location.GetDistance(Point.Origin) - distance) < tolerance)
                {
                    VertexIndex v2 = vertices[j];
                    if (vertices[i].Location.GetDistance(vertices[j].Location) < tolerance)
                    {
                        SetFaceIndex(v2.Faces, vertices[j].Index, vertices[i].Index);
                        culledIndices.Add(vertices[j].Index);
                        v2.Index = vertices[i].Index;
                        break;
                    }
                    j++;
                }
            }

            for (int i = 0; i < faces.Count; i++)
            {
                for (int k = 0; k < culledIndices.Count; k++)
                {
                    if (faces[i].A > culledIndices[k])
                        faces[i].A--;
                    if (faces[i].B > culledIndices[k])
                        faces[i].B--;
                    if (faces[i].C > culledIndices[k])
                        faces[i].C--;
                    if (faces[i].D > culledIndices[k])
                        faces[i].D--;
                }
            }

            return new Mesh(vertices.Select(x => x.Location).ToList(), faces);
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static void SetFaceIndex(List<Face> faces, int from, int to)
        {
            foreach (Face f in faces)
            {
                if (f.A == from) f.A = to;
                else if (f.B == from) f.B = to;
                else if (f.C == from) f.C = to;
                else if (f.D == from) f.D = to;
            }
        }


        /***************************************************/
        /**** Private Definitions                       ****/
        /***************************************************/

        private struct VertexIndex
        {
            public VertexIndex(Point point, int index)
            {
                Location = point;
                Index = index;
                Faces = new List<Face>();
            }
            public List<Face> Faces;
            public Point Location;
            public int Index;
        }
    }
}
