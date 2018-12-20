/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2018, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or          
 * (at your option) any later version.                                          
 *                                                                              
 * The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License     
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */

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
