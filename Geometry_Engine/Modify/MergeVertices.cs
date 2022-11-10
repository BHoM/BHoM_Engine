/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2022, the respective contributors. All rights reserved.
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

using BH.Engine.Base;
using BH.oM.Data.Collections;
using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using BH.Engine.Data;
using BH.oM.Base.Attributes;
using System.ComponentModel;
using BH.oM.Quantities.Attributes;

namespace BH.Engine.Geometry
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [PreviousVersion("6.0", "BH.Engine.Geometry.Modify.MergedVertices(BH.oM.Geometry.Mesh, System.Double)")]
        [Description("Merges duplicate vertices of the mesh and ensures faces are updated to have their indecies pointig at the index of the new merged vertex.")]
        [Input("mesh", "The mesh to merge duplicate vertices of.")]
        [Input("tolerance", "The maximum allowable distance between two vertices for them to be deemed the same vertex.", typeof(Length))]
        [Output("mesh", "Mesh with merged vertices.")]
        public static Mesh MergeVertices(this Mesh mesh, double tolerance = Tolerance.Distance)
        {
            if (mesh == null)
                return null;

            //Set up list on structs containing location and index
            List<VertexIndex> vertices = mesh.Vertices.Select((x, i) => new VertexIndex(x, i)).ToList();

            //Find duplicate vertex indecies using the same methodology as utilised by cull duplicates
            double sqDist = tolerance * tolerance;
            Func<VertexIndex, DomainBox> toDomainBox = a => new DomainBox()
            {
                Domains = new Domain[] {
                    new Domain(a.Location.X, a.Location.X),
                    new Domain(a.Location.Y, a.Location.Y),
                    new Domain(a.Location.Z, a.Location.Z),
                }
            };
            Func<DomainBox, DomainBox, bool> treeFunction = (a, b) => a.SquareDistance(b) < sqDist;
            Func<VertexIndex, VertexIndex, bool> itemFunction = (a, b) => true;  // The distance between the boxes is enough to determine if a Point is in range
            //Clusters the points within distance tolerance of each other. This is utilising a DB scan methodology to find duplicate vertices
            List<List<VertexIndex>> clusteredVertices = Data.Compute.DomainTreeClusters(vertices, toDomainBox, treeFunction, itemFunction, 1);

            //Map of oldIndex -> newindex
            Dictionary<int, int> indexMap = new Dictionary<int, int>();

            //Mesh to be returned
            Mesh returnMesh = new Mesh();

            for (int i = 0; i < clusteredVertices.Count; i++)
            {
                List<VertexIndex> current = clusteredVertices[i];   //Current list of duplicate vertices
                returnMesh.Vertices.Add(current.Select(x => x.Location).Average());     //Add average point of duplicates

                foreach (VertexIndex vertex in current) //Loop through all the vertices, setting from previous (vertix.Index) to current (current list index)
                {
                    indexMap[vertex.Index] = i;
                }
            }

            foreach (Face face in mesh.Faces)   //Loop through all faces, make use of index map to find new index value
            {
                Face newFace = new Face();
                int a, b, c, d;
                if (indexMap.TryGetValue(face.A, out a))
                    newFace.A = a;
                if (indexMap.TryGetValue(face.B, out b))
                    newFace.B = b;
                if (indexMap.TryGetValue(face.C, out c))
                    newFace.C = c;

                if (face.D != -1)   //if -1, keep as -1, as this indicates a triangular face
                {
                    if (indexMap.TryGetValue(face.D, out d))
                        newFace.D = d;
                }
                else
                    newFace.D = -1;

                returnMesh.Faces.Add(newFace);
            }

            return returnMesh;

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
            }
            public Point Location;
            public int Index;
        }

        /***************************************************/
    }
}



