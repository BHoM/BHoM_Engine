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
using BH.oM.Reflection.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Compute
    {
        /***************************************************/
        /****   Public Methods                          ****/
        /***************************************************/

        [Description("Create a mesh from selected faces of an existing mesh.")]
        [Input("mesh", "Mesh to get a sub-section from.")]
        [Input("faces", "The faces of the old mesh to carry over to the new mesh.")]
        [Output("mesh", "Mesh composed of the faces and the vertices in those faces.")]
        public static Mesh SubMesh(this Mesh mesh, List<Face> faces)
        {
            List<int> dummyList = new List<int>();
            return SubMesh<int>(mesh, faces, ref dummyList);
        }

        /***************************************************/

        [Description("Create a mesh from selected faces of an existing mesh.")]
        [Input("mesh", "Mesh to get a sub-section from.")]
        [Input("faces", "The faces of the old mesh to carry over to the new mesh.")]
        [Input("vertexRelatedData", "A list where each item is related to the vertex at the same index in the mesh. \n" +
                                    "Will be changed to relate to the new mesh's vertex list.")]
        [Output("mesh", "Mesh composed of the faces and the vertices in those faces.")]
        public static Mesh SubMesh<T>(this Mesh mesh, List<Face> faces, ref List<T> vertexRelatedData)
        {
            List<Point> vertices = new List<Point>();
            List<Face> resultFaces = new List<Face>();

            List<int> indecies = faces.SelectMany(x => x.ToArray()).Distinct().ToList();
            List<Tuple<int, int>> mapping = indecies.Select((x, i) => new Tuple<int, int>(x, i)).ToList();

            Dictionary<int, int> map = mapping.ToDictionary(x => x.Item1, x => x.Item2);

            foreach (int i in indecies)
            {
                vertices.Add(mesh.Vertices[i]);
            }

            if (vertexRelatedData != null && 
                vertexRelatedData.Count == mesh.Vertices.Count)
            {
                List<T> newList = new List<T>();
                foreach (int i in indecies)
                {
                    newList.Add(vertexRelatedData[i]);
                }
                vertexRelatedData = newList;
            }

            foreach (Face face in faces)
            {
                Face newFace = new Face() 
                {
                    A = map[face.A],
                    B = map[face.B],
                    C = map[face.C]
                };

                if (face.IsQuad())
                    newFace.D = map[face.D];

                resultFaces.Add(newFace);
            }

            return new Mesh()
            {
                Vertices = vertices,
                Faces = resultFaces,
            };
        }

        /***************************************************/

    }
}

