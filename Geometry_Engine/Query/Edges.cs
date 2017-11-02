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
        /***************************************************/
        /**** Public Methods - Meshes                   ****/
        /***************************************************/

        public static List<Polyline> GetEdges(this Mesh mesh)
        {
            List<Face> faces = mesh.Faces;
            List<Point> vertices = mesh.Vertices;
            List<Polyline> edges = new List<Polyline>(faces.Count);
            for (int i = 0; i < faces.Count; i++)
            {
                List<Point> faceVertices = new List<Point>();
                Point p1 = vertices[faces[i].A];
                Point p2 = vertices[faces[i].B];
                Point p3 = vertices[faces[i].C];
                faceVertices.Add(p1);
                faceVertices.Add(p2);
                faceVertices.Add(p3);
                if (faces[i].IsQuad()) { faceVertices.Add(vertices[faces[i].D]); }
                faceVertices.Add(p1);                               // Closed Polyline
                Polyline edge = new Polyline(faceVertices);
                edges[i] = edge;
            }
            return edges;
        }

        /***************************************************/
        /**** Public Methods - Faces                    ****/
        /***************************************************/


        public static Polyline GetEdges(this Face face, Mesh mesh)
        {

            List<Point> ptList = new List<Point>();
            ptList.Add(mesh.Vertices[face.A]);
            ptList.Add(mesh.Vertices[face.B]);
            ptList.Add(mesh.Vertices[face.C]);

            if (face.IsQuad())
            {
                ptList.Add(mesh.Vertices[face.C]);
            }

            Polyline edge = new Polyline(ptList);

            return edge;
        }



        /***************************************************/

        public static List<ICurve> GetEdges(this ISurface surface)
        {
            List<ICurve> edges = surface.IGetExternalEdges();
            edges.AddRange(surface.IGetInternalEdges());
            return edges;
        }
    }
}
