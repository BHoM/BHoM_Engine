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
using BH.Engine.Geometry;


namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static FEMesh FEMesh(BH.oM.Geometry.Mesh mesh, ISurfaceProperty property = null, string name = null)
        {
            FEMesh feMesh = new FEMesh();
            feMesh.Nodes = mesh.Vertices.Select(x => Node(x)).ToList();

            foreach (Face face in mesh.Faces)
            {
                FEMeshFace feFace = new FEMeshFace();
                feFace.NodeListIndices.Add(face.A);
                feFace.NodeListIndices.Add(face.B);
                feFace.NodeListIndices.Add(face.C);

                if (face.IsQuad())
                    feFace.NodeListIndices.Add(face.D);

                feMesh.MeshFaces.Add(feFace);

            }

            if (property != null)
                feMesh.Property = property;

            if (name != null)
                feMesh.Name = name;

            return feMesh;
        }

        /***************************************************/
    }
}
