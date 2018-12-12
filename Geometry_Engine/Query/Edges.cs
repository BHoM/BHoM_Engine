using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods - Meshes                   ****/
        /***************************************************/

        public static List<Line> Edges(this Mesh mesh)
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
                    indices.Add(new Tuple<int, int>(face.C, face.A));               
                                   
            }

            List<Tuple<int, int>> distinctIndices = indices.Select(x => (x.Item1 < x.Item2) ? x : new Tuple<int, int>(x.Item2, x.Item1)).Distinct().ToList();
            List<Line> edges = new List<Line>();

            for (int i = 0; i < distinctIndices.Count; i++)
            {
                edges.Add(new Line { Start = mesh.Vertices[distinctIndices[i].Item1], End = mesh.Vertices[distinctIndices[i].Item2] });
            }

            return edges;
        }


        /***************************************************/
        /**** Public Methods - Faces                    ****/
        /***************************************************/
        
        public static List<Line> Edges(this Face face, Mesh mesh)
        {
            List<Line> edges = new List<Line>();
            edges.Add(new Line { Start = mesh.Vertices[face.A], End = mesh.Vertices[face.B] });
            edges.Add(new Line { Start = mesh.Vertices[face.B], End = mesh.Vertices[face.C] });

            if (face.IsQuad())
            {
                edges.Add(new Line { Start = mesh.Vertices[face.C], End = mesh.Vertices[face.D] });
                edges.Add(new Line { Start = mesh.Vertices[face.D], End = mesh.Vertices[face.A] });
            }
            else
                edges.Add(new Line { Start = mesh.Vertices[face.C], End = mesh.Vertices[face.A] });

            return edges;
        }

        /***************************************************/

        public static List<ICurve> Edges(this ISurface surface)
        {
            List<ICurve> edges = surface.IExternalEdges();
            edges.AddRange(surface.IInternalEdges());
            return edges;
        }

        /***************************************************/
    }
}
