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
using BH.oM.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using BH.Engine.Geometry;
using System.ComponentModel;

namespace BH.Engine.Graphics
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods - Graphics                 ****/
        /***************************************************/

        [Description("Joins a multiple RenderMesh into a single one. Currently this does not optimise for duplicate vertices.")]
        public static RenderMesh JoinRenderMeshes(this List<RenderMesh> renderMeshes)
        {
            List<Vertex> vertices = new List<Vertex>();
            List<Face> faces = new List<Face>();

            vertices.AddRange(renderMeshes[0].Vertices);
            faces.AddRange(renderMeshes[0].Faces);

            for (int i = 1; i < renderMeshes.Count; i++)
            {
                int lastVerticesCount = vertices.Count;
                vertices.AddRange(renderMeshes[i].Vertices);
                faces.AddRange(
                    renderMeshes[i].Faces.Select(f =>
                        new Face() { A = f.A + lastVerticesCount, B = f.B + lastVerticesCount, C = f.C + lastVerticesCount, D = f.D == -1 ? f.D : f.D + lastVerticesCount }));
            }

            return new RenderMesh() { Vertices = vertices, Faces = faces };
        }

        /***************************************************/
    }
}

