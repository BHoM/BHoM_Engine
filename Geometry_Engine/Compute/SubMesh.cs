/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
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
using BH.oM.Base;
using BH.oM.Base.Attributes;
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
            return SubMesh<int>(mesh, faces, null).Item1;
        }

        /***************************************************/

        [Description("Create a mesh from selected faces of an existing mesh.")]
        [Input("mesh", "Mesh to get a sub-section from.")]
        [Input("faces", "The faces of the old mesh to carry over to the new mesh.")]
        [Input("vertexRelatedData", "A list where each item is related to the vertex at the same index in the mesh.")]
        [MultiOutput(0, "mesh", "Mesh composed of the faces and the vertices in those faces.")]
        [MultiOutput(1, "vertexRelatedData", "The data relating to each corresponding vertex in the new mesh.")]
        public static Output<Mesh, List<T>> SubMesh<T>(this Mesh mesh, List<Face> faces, List<T> vertexRelatedData)
        {
            List<Point> vertices = new List<Point>();
            List<Face> resultFaces = new List<Face>();

            IEnumerable<int> indecies = faces.SelectMany(x => x.Vertices()).Distinct();
            Dictionary<int, int> map = indecies.Select((x, i) => new Tuple<int, int>(x, i))
                                               .ToDictionary(x => x.Item1, x => x.Item2);

            foreach (int i in indecies)
            {
                vertices.Add(mesh.Vertices[i]);
            }

            if (vertexRelatedData != null && 
                vertexRelatedData.Count == mesh.Vertices.Count)
            {
                vertexRelatedData = indecies.Select(i => vertexRelatedData[i]).ToList();
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

            return new Output<Mesh, List<T>>()
            {
                Item1 = new Mesh()
                {
                    Vertices = vertices,
                    Faces = resultFaces,
                },
                Item2 = vertexRelatedData,
            };
        }

        /***************************************************/

    }
}






