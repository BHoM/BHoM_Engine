/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
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
using BH.oM.Base.Attributes;
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

        [Description("Computes the contour meshes for the iso-values in the scalar field defined by the mesh and the values. \n" +
                     "The values are assumed to change linearly between vertices.")]
        [Input("mesh3d", "Mesh which defines the positions of the values in the scalar field.")]
        [Input("vertexValues", "Value for of scalar field for each vertex in the mesh.")]
        [Input("isoValues", "Values in the scalar field to produce the iso-meshes for.")]
        [Output("isoMeshes", "Meshes on the iso-values of the scalar field defined by the mesh and values.")]
        public static List<Mesh> MarchingCubes(this Mesh3D mesh3d, List<double> vertexValues, List<double> isoValues)
        {
            if (mesh3d?.Vertices == null || vertexValues == null || mesh3d.Vertices.Count != vertexValues.Count)
            {
                Base.Compute.RecordError("Number of vertexValues must match the number of vertices in the mesh.");
                return new List<Mesh>();
            }

            if (isoValues == null)
                return new List<Mesh>();

            // add empty lists
            List<List<Point>> vertices = new List<List<Point>>();
            List<List<Face>> faces = new List<List<Face>>();
            for (int i = 0; i < isoValues.Count; i++)
            {
                vertices.Add(new List<Point>());
                faces.Add(new List<Face>());
            }

            // SubMesh works on regular meshes, and since we're looking at it one cell at a time, the extra information in the mesh3d is not needed
            Mesh meshVersion = mesh3d.ToMesh();
            // Look at every cell
            foreach (List<Face> cell in mesh3d.Cells())
            {
                // Create a mesh from the cell
                var meshAndValues = SubMesh(meshVersion, cell, vertexValues);

                // Find the iso lines for the boundary of the cell
                List<List<Polyline>> pLines = MarchingSquares(meshAndValues.Item1, meshAndValues.Item2, isoValues).Select(x => x.Join()).ToList();

                // Create faces from the polyline
                // Note that each vertex may be added twice here from neighboring cells
                for (int j = 0; j < isoValues.Count; j++)
                {
                    foreach (Polyline polyline in pLines[j])
                    {
                        vertices[j].AddRange(polyline.ControlPoints.Skip(1));
                        int nPts = vertices[j].Count;

                        int first = polyline.ControlPoints.Count - 1;
                        // A rough triangulation of the imagined face covering the polyline, (assumes convex polyline)
                        for (int i = 1; i < first - 1; i++)
                        {
                            faces[j].Add(new Face()
                            {
                                A = nPts - first,
                                B = nPts - (i + 1),
                                C = nPts - i,
                            });
                        }

                    }
                }
            }

            return vertices.Zip(faces, (v, f) => new Mesh()
            {
                Vertices = v,
                Faces = f,
            }).ToList();
        }

        /***************************************************/

    }
}





