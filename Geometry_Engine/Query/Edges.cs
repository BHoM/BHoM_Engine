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
        private static readonly object indices;

        /***************************************************/
        /**** Public Methods - Meshes                   ****/
        /***************************************************/

        //public static List<Line> GetEdges(this Mesh mesh)
        //{
        //    List<Line> edges = new List<Line>();
        //    List<int> hashcodes = new List<int>();
        //    List<Face> faces = mesh.Faces;
        //    for (int i = 0; i < mesh.Faces.Count; i++)
        //    {

        //        List<Line> faceEdges = (GetEdges(mesh, faces[i]));
        //        for (int j = 0; j < faceEdges.Count; j++)
        //        {
        //            int faceHash = faceEdges[j].GetPointAtParameter(0.5).GetHashCode();
        //            if (!hashcodes.Contains(faceHash))
        //            {
        //                edges.Add(faceEdges[j]);                       
        //                hashcodes.Add(faceHash);
        //            }
        //        }
        //    }
        //    return edges;
        //}

        public static List<Line> GetEdges(this Mesh mesh)
        {
            List<Face> faces = mesh.Faces;
            List<Tuple<int, int>> indices = new List<Tuple<int, int>>();
            
            for (int i = 0; i < faces.Count; i++)
            {
                Face face = faces[i];
                indices.Add(new Tuple<int, int>(face.A, face.B));
                indices.Add(new Tuple<int, int>(face.B, face.C));

                if (face.IsQuad())
                {
                    indices.Add(new Tuple<int, int>(face.C, face.D));
                    indices.Add(new Tuple<int, int>(face.D, face.A));
                }
                else
                {
                    indices.Add(new Tuple<int, int>(face.C, face.A));
                }
                
            }

            List<Tuple<int, int>> distinctIndices = indices.Select(x => (x.Item1 < x.Item2) ? x : new Tuple<int, int>(x.Item2, x.Item1)).Distinct().ToList();
            List<Line> edges = new List<Line>();
            for (int i = 0; i < distinctIndices.Count; i++)
            {
                edges.Add(new Line(mesh.Vertices[distinctIndices[i].Item1], mesh.Vertices[distinctIndices[i].Item2]));
            }
            return edges;

        }

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
