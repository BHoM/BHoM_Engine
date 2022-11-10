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
            List<Tuple<Point,int>> vertices = mesh.Vertices.Select((x, i) => new Tuple<Point, int>(x, i)).ToList();

            //Find duplicate vertex indecies using the same methodology as utilised by cull duplicates
            double sqDist = tolerance * tolerance;
            Func<Tuple<Point, int>, DomainBox> toDomainBox = a => new DomainBox()
            {
                Domains = new Domain[] {
                    new Domain(a.Item1.X, a.Item1.X),
                    new Domain(a.Item1.Y, a.Item1.Y),
                    new Domain(a.Item1.Z, a.Item1.Z),
                }
            };
            Func<DomainBox, DomainBox, bool> treeFunction = (a, b) => a.SquareDistance(b) < sqDist;
            Func<Tuple<Point, int>, Tuple<Point, int>, bool> itemFunction = (a, b) => true;  // The distance between the boxes is enough to determine if a Point is in range
            //Clusters the points within distance tolerance of each other. This is utilising a DB scan methodology to find duplicate vertices
            List<List<Tuple<Point, int>>> clusteredVertices = Data.Compute.DomainTreeClusters(vertices, toDomainBox, treeFunction, itemFunction, 1);

            //Map of oldIndex -> newindex
            Dictionary<int, int> indexMap = new Dictionary<int, int>();

            //Mesh to be returned
            Mesh returnMesh = new Mesh();

            for (int i = 0; i < clusteredVertices.Count; i++)
            {
                List<Tuple<Point, int>> current = clusteredVertices[i];   //Current list of duplicate vertices
                returnMesh.Vertices.Add(current.Select(x => x.Item1).Average());     //Add average point of duplicates

                foreach (Tuple<Point, int> vertex in current) //Loop through all the vertices, setting from previous (vertix.Index) to current (current list index)
                {
                    indexMap[vertex.Item2] = i;
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
    }
}



