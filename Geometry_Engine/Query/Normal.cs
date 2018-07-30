using BH.oM.Geometry;
using BH.oM.Reflection.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods - Surfaces                 ****/
        /***************************************************/

        public static Vector Normal(this Face face, Mesh mesh)
        {
            List<Point> vertices = mesh.Vertices;
            Face meshFace = mesh.Faces[mesh.Faces.IndexOf(face)];
            Point pA = vertices[meshFace.A];
            Point pB = vertices[meshFace.B];
            Point pC = vertices[meshFace.C];
            if (!meshFace.IsQuad())
            {
                Vector normal = CrossProduct(pB - pA, pC - pB);
                return normal.Normalise();
            }
            else
            {
                Point pD = vertices[(meshFace.D)];
                Vector normal = (CrossProduct(pA - pD, pB - pA)) + (CrossProduct(pC - pB, pD - pC));
                return normal.Normalise();
            }
        }

        /***************************************************/

        public static List<Vector> Normals(this Mesh mesh)
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
                    Vector normal = CrossProduct(pB - pA, pC - pB);
                    normals.Add(normal.Normalise());
                }
                else
                {
                    Point pD = vertices[(faces[i].D)];
                    Vector normal = (CrossProduct(pA - pD, pB - pA)) + (CrossProduct(pC - pB, pD - pC));
                    normal.Normalise();
                    normals[i] = normal;
                }
            }
            return normals.ToList();
        }

        /***************************************************/

        [NotImplemented]
        public static List<Vector> Normals(this ISurface surface)
        {
            throw new NotImplementedException();
        }

        /***************************************************/
    }
}
