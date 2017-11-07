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

        public static List<Line> GetEdges(this Mesh mesh)
        {
            List<Line> edges = new List<Line>();
            List<int> hashcodes = new List<int>();
            List<Face> faces = mesh.Faces;
            for (int i = 0; i < mesh.Faces.Count; i++)
            {

                List<Line> faceEdges = (GetEdges(mesh, faces[i]));
                for (int j = 0; j < faceEdges.Count; j++)
                {
                    int faceHash = faceEdges[j].GetPointAtParameter(0.5).GetHashCode();
                    if (!hashcodes.Contains(faceHash))
                    {
                        edges.Add(faceEdges[j]);
                       // hashcodes.Add(faceEdges[j].GetFlipped().GetHashCode());
                        hashcodes.Add(faceHash);
                    }
                }
            }
            return edges;
        }

        //public static List<Line> GetEdges(this Mesh mesh)
        //{
        //    List<Line> edges = new List<Line>();
        //    List<Line> distinctEdges = new List<Line>();
        //    List<Face> faces = mesh.Faces;            
        //    for (int i = 0; i < faces.Count; i++)
        //    {
        //        edges.AddRange(mesh.GetEdges(faces[i]));
        //    }
        //    IEnumerable<Line> nonDuplicates = edges.Distinct();
        //    foreach (Line edge in nonDuplicates)
        //    {
        //        distinctEdges.Add(edge);
        //    }
        //    return edges;
        //}

        /***************************************************/
        /**** Public Methods - Faces                    ****/
        /***************************************************/


        public static List<Line> GetEdges(this Mesh mesh, Face face)
        {
            List<Line> edges = new List<Line>();
            edges.Add(new Line(mesh.Vertices[face.A], mesh.Vertices[face.B]));
            edges.Add(new Line(mesh.Vertices[face.B], mesh.Vertices[face.C]));

            if (face.IsQuad())
            {
                edges.Add(new Line(mesh.Vertices[face.C], mesh.Vertices[face.D]));
                edges.Add(new Line(mesh.Vertices[face.D], mesh.Vertices[face.A]));
            }
            else
            {
                edges.Add(new Line(mesh.Vertices[face.C], mesh.Vertices[face.A]));
            }

            return edges;
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
