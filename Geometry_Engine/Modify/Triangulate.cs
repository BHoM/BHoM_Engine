using BH.oM.Geometry;
using System.Collections.Generic;

namespace BH.Engine.Geometry
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Mesh Triangulate(this Mesh mesh)
        {
            Mesh tMesh = new Mesh();
            List<Point> vertices = mesh.Vertices;
            List<Face> faces = mesh.Faces;
            tMesh.Vertices.AddRange(vertices);
            for (int i = 0; i < faces.Count; i++)
            {
                if (!faces[i].IsQuad())
                {
                    tMesh.Faces.Add(faces[i]);
                }
                else
                {
                    int i1 = faces[i].A;
                    int i2 = faces[i].B;
                    int i3 = faces[i].C;
                    int i4 = faces[i].D;
                    Point p1 = vertices[i1];
                    Point p2 = vertices[i2];
                    Point p3 = vertices[i3];
                    Point p4 = vertices[i4];
                    double d1 = (p1 - p3).Length();
                    double d2 = (p2 - p4).Length();
                    if (d1 > d2)    //Bracing based on shortest diagonal criteria
                    {
                        Face fA = new Face { A = i1, B = i2, C = i4 };
                        Face fB = new Face { A = i2, B = i3, C = i4 };
                        tMesh.Faces.Add(fA);
                        tMesh.Faces.Add(fB);
                    }
                    else
                    {
                        Face fA = new Face { A = i1, B = i2, C = i3 };
                        Face fB = new Face { A = i1, B = i3, C = i4 };
                        tMesh.Faces.Add(fA);
                        tMesh.Faces.Add(fB);
                    }
                }
            }
            return tMesh;
        }
    }
}
