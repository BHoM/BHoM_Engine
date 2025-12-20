/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2026, the respective contributors. All rights reserved.
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

using BH.oM.Base.Attributes;
using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Checks if a mesh is manifold. A manifold mesh is one where each edge is shared by exactly two faces, and the mesh forms a closed surface without holes or self-intersections.")]
        [Input("mesh", "The mesh to check for manifold properties.")]
        [Output("isManifold", "True if the mesh is manifold, false otherwise.")]
        public static bool IsManifold(this Mesh mesh)
        {
            if (mesh == null)
            {
                Engine.Base.Compute.RecordError("Cannot check manifold property of a null mesh.");
                return false;
            }

            if (mesh.Vertices == null || mesh.Faces == null)
            {
                Engine.Base.Compute.RecordError("Mesh must have valid vertices and faces to check manifold property.");
                return false;
            }

            if (mesh.Faces.Count == 0)
            {
                Engine.Base.Compute.RecordWarning("Empty mesh is considered non-manifold.");
                return false;
            }

            // Create a dictionary to count how many faces share each edge
            Dictionary<(int, int), int> edgeCount = new Dictionary<(int, int), int>();

            // Iterate through all faces and count edge occurrences
            foreach (Face face in mesh.Faces)
            {
                // Get all edges of the face
                List<(int, int)> faceEdges = EdgeVertices(face);

                foreach ((int, int) edge in faceEdges)
                {
                    // Normalize edge (smaller index first) to ensure consistent counting
                    (int, int) normalizedEdge = (
                        Math.Min(edge.Item1, edge.Item2),
                        Math.Max(edge.Item1, edge.Item2)
                    );

                    if (edgeCount.ContainsKey(normalizedEdge))
                        edgeCount[normalizedEdge]++;
                    else
                        edgeCount[normalizedEdge] = 1;
                }
            }

            // Check if all edges are shared by exactly 2 faces
            foreach (KeyValuePair<(int, int), int> kvp in edgeCount)
            {
                if (kvp.Value != 2)
                {
                    // Edge is shared by more or less than 2 faces - not manifold
                    return false;
                }
            }

            // Additional check: verify vertex indices are valid
            int maxVertexIndex = mesh.Vertices.Count - 1;
            foreach (Face face in mesh.Faces)
            {
                if (face.A < 0 || face.A > maxVertexIndex ||
                    face.B < 0 || face.B > maxVertexIndex ||
                    face.C < 0 || face.C > maxVertexIndex ||
                    (face.D != -1 && (face.D < 0 || face.D > maxVertexIndex)))
                {
                    Engine.Base.Compute.RecordError("Mesh contains invalid vertex indices.");
                    return false;
                }
            }

            return true;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static List<(int, int)> EdgeVertices(Face face)
        {
            List<(int, int)> edges = new List<(int, int)>();

            // Add edges for triangular or quad faces
            edges.Add((face.A, face.B));
            edges.Add((face.B, face.C));

            if (face.D == -1) // Triangular face
            {
                edges.Add((face.C, face.A));
            }
            else // Quad face
            {
                edges.Add((face.C, face.D));
                edges.Add((face.D, face.A));
            }

            return edges;
        }

        /***************************************************/
    }
}

