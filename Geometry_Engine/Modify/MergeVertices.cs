/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2020, the respective contributors. All rights reserved.
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
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Mesh MergedVertices(this Mesh mesh, double tolerance = Tolerance.Distance) //TODO: use the point matrix 
        {
            List<Face> faces = mesh.Faces.Select(x => x.Clone() as Face).ToList();
            List<VertexIndex> vertices = mesh.Vertices.Select((x, i) => new VertexIndex(x.Clone() as Point, i)).ToList();
            
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
                return v1.Location.SquareDistance(Point.Origin).CompareTo(v2.Location.SquareDistance(Point.Origin));
            });


            List<int> culledIndices = new List<int>();
            double sqTol = tolerance * tolerance;

            for (int i = 0; i < vertices.Count; i++)
            {
                double distance = vertices[i].Location.Distance(Point.Origin);
                int j = i + 1;
                while (j < vertices.Count && Math.Abs(vertices[j].Location.Distance(Point.Origin) - distance) < tolerance)
                {
                    VertexIndex v2 = vertices[j];
                    if (vertices[i].Location.SquareDistance(vertices[j].Location) < sqTol)
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

            return new Mesh { Vertices = vertices.Select(x => x.Location).ToList(), Faces = faces };
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static void SetFaceIndex(List<Face> faces, int from, int to)
        {
            foreach (Face f in faces)
            {
                if (f.A == from)
                    f.A = to;
                else if (f.B == from)
                    f.B = to;
                else if (f.C == from)
                    f.C = to;
                else if (f.D == from)
                    f.D = to;
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

        /***************************************************/
    }
}

