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

using BH.oM.Structure.Elements;
using BH.oM.Structure.Properties.Surface;
using System;
using System.Collections.Generic;
using System.Linq;
using BH.oM.Geometry;


namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static MeshFace MeshFace(Node n0, Node n1, Node n2, Node n3 = null, ISurfaceProperty property = null, string name = null)
        {
            MeshFace mf = new MeshFace { Nodes = new List<Node> { n0, n1, n2 }, Property = property };

            if (n3 != null)
                mf.Nodes.Add(n3);

            if (name != null)
                mf.Name = name;

            return mf;
        }

        /***************************************************/

        public static MeshFace MeshFace(IEnumerable<Node> nodes, ISurfaceProperty property = null, string name = null)
        {
            int nodeCount = nodes.Count();
            if (nodeCount != 3 && nodeCount != 4)
                throw new ArgumentException("Mesh faces only support 3 or 4 nodes");

            MeshFace mf = new MeshFace { Nodes = nodes.ToList(), Property = property };

            if (name != null)
                mf.Name = name;
            return mf;
        }

        /***************************************************/

        public static List<MeshFace> MeshFaces(BH.oM.Geometry.Mesh mesh, ISurfaceProperty property = null, string name = null)
        {
            List<MeshFace> meshFaces = new List<MeshFace>();

            foreach (Face face in mesh.Faces)
            {
                List<Node> nodes = new List<Node>();
                nodes.Add(new Node() { Position = mesh.Vertices[face.A] });
                nodes.Add(new Node() { Position = mesh.Vertices[face.B] });
                nodes.Add(new Node() { Position = mesh.Vertices[face.C] });

                if (BH.Engine.Geometry.Query.IsQuad(face))
                    nodes.Add(new Node() { Position = mesh.Vertices[face.D] });
                MeshFace mf = new MeshFace() { Property = property, Nodes = nodes };

                if (name != null)
                    mf.Name = name;

                meshFaces.Add(mf);
            }

            return meshFaces;
        }

        /***************************************************/
    }
}
