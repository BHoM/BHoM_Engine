/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
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
using BH.oM.Structure.SurfaceProperties;
using System;
using System.Collections.Generic;
using System.Linq;
using BH.oM.Geometry;
using BH.Engine.Geometry;
using BH.oM.Base.Attributes;
using BH.oM.Quantities.Attributes;
using System.ComponentModel;


namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a FEMesh based on a geometrical Mesh.")]
        [Input("mesh", "The geometrical Mesh to extract geometrical and topological information from.")]
        [InputFromProperty("property")]
        [Input("name", "The name of the created FEMesh.")]
        [Input("localX", "Vector to set as local x of the FEMeshFaces of the FEMesh. Default value of null gives default orientation. If this vector is not in the plane of the FEMeshFace it will get projected. If the vector is parallel to the normal of the FEMeshFace the operation will fail and the FEMeshFace will be assigned default orientation.")]
        [Output("feMesh", "The created FEMesh with same geometry and topology as the geometrical Mesh.")]
        public static FEMesh FEMesh(Mesh mesh, ISurfaceProperty property = null, Vector localX = null, string name = null)
        {
            if (mesh.IsNull())
                return null;

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

                feMesh.Faces.Add(feFace);

            }

            if (property != null)
                feMesh.Property = property;

            if (name != null)
                feMesh.Name = name;

            if (localX != null)
                return feMesh.SetLocalOrientations(localX);
            else
                return feMesh;
        }

        /***************************************************/
    }
}




