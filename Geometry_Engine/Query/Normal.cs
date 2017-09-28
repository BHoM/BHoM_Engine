using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        public static Vector GetNormal(this Face face, Mesh mesh)
        {
            List<Point> vertices = mesh.Vertices;
            Face meshFace = mesh.Faces[mesh.Faces.IndexOf(face)];
            Point pA = vertices[meshFace.A];
            Point pB = vertices[meshFace.B];
            Point pC = vertices[meshFace.C];
            if (!meshFace.IsQuad())
            {
                Vector normal = GetCrossProduct(pB - pA, pC - pB);
                return normal.GetNormalised();
            }
            else
            {
                Point pD = vertices[(meshFace.D)];
                Vector normal = (GetCrossProduct(pA - pD, pB - pA)) + (GetCrossProduct(pC - pB, pD - pC));
                return normal.GetNormalised();
            }
        }
        public static List<Vector> GetNormals(this Mesh mesh)
        {
            List<Vector> normals = new List<Vector>(mesh.Faces.Count);
            List<Point> vertices = mesh.Vertices;
            List<Face> faces = mesh.Faces;
            for (int i = 0; i < mesh.Faces.Count; i++)
            {
                Point pA = vertices[(faces[i].A)];
                Point pB = vertices[(faces[i].B)];
                Point pC = vertices[(faces[i].C)];
                if (!faces[i].IsQuad())
                {
                    Vector normal = GetCrossProduct(pB - pA, pC - pB);
                    normals.Add(normal.GetNormalised());
                }
                else
                {
                    Point pD = vertices[(faces[i].D)];
                    Vector normal = (GetCrossProduct(pA - pD, pB - pA)) + (GetCrossProduct(pC - pB, pD - pC));
                    normal.GetNormalised();
                    normals[i] = normal;
                }
            }
            return normals.ToList();
        }
        public static List<Vector> GetNormals(this ISurface surface)
        {
            throw new NotImplementedException();
        }
    }
}
