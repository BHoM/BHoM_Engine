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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using BH.Engine.Geometry;
using BH.oM.Base;
using System.ComponentModel;

namespace BH.Engine.Graphics
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods - Graphics                 ****/
        /***************************************************/

        public static BH.oM.Graphics.RenderMesh RenderMesh(this Cuboid cuboid, RenderMeshOptions renderMeshOptions = null)
        {
            renderMeshOptions = renderMeshOptions ?? new RenderMeshOptions();

            return BoxRenderMesh(cuboid.CoordinateSystem.Origin, cuboid.Length, cuboid.Depth, cuboid.Height, renderMeshOptions);
        }

        private static BH.oM.Graphics.RenderMesh BoxRenderMesh(Point centrePoint, double length, double depth, double height, RenderMeshOptions renderMeshOptions = null)
        {
            renderMeshOptions = renderMeshOptions ?? new RenderMeshOptions();

            List<Vertex> vertices = new List<Vertex>();

            // Top face (normal to global z, +)
            //0
            vertices.Add((Vertex)new Point()
            {
                X = centrePoint.X + (length / 2.0),
                Y = centrePoint.Y + (depth / 2.0),
                Z = centrePoint.Z + (height / 2.0)
            });

            //1
            vertices.Add((Vertex)new Point()
            {
                X = centrePoint.X + (length / -2.0),
                Y = centrePoint.Y + (depth / 2.0),
                Z = centrePoint.Z + (height / 2.0)
            });

            //2
            vertices.Add((Vertex)new Point()
            {
                X = centrePoint.X + (length / -2.0),
                Y = centrePoint.Y + (depth / -2.0),
                Z = centrePoint.Z + (height / 2.0)
            });

            //3
            vertices.Add((Vertex)new Point()
            {
                X = centrePoint.X + (length / 2.0),
                Y = centrePoint.Y + (depth / -2.0),
                Z = centrePoint.Z + (height / 2.0)
            });

            // Bottom face (normal to global z, -)
            //4
            vertices.Add((Vertex)new Point()
            {
                X = centrePoint.X + (length / 2.0),
                Y = centrePoint.Y + (depth / 2.0),
                Z = centrePoint.Z + (height / -2.0)
            });

            //5
            vertices.Add((Vertex)new Point()
            {
                X = centrePoint.X + (length / 2.0),
                Y = centrePoint.Y + (depth / -2.0),
                Z = centrePoint.Z + (height / -2.0)
            });

            //6
            vertices.Add((Vertex)new Point()
            {
                X = centrePoint.X + (length / -2.0),
                Y = centrePoint.Y + (depth / -2.0),
                Z = centrePoint.Z + (height / -2.0)
            });

            //7
            vertices.Add((Vertex)new Point()
            {
                X = centrePoint.X + (length / -2.0),
                Y = centrePoint.Y + (depth / 2.0),
                Z = centrePoint.Z + (height / -2.0)
            });

            List<Face> faces = new List<Face>();

            // Top face (normal to global z, +): 0, 1, 2, 3
            faces.Add(new Face() { A = 0, B = 1, C = 2, D = 3 });

            // Bottom face (normal to global z, -): 4, 5, 6, 7
            faces.Add(new Face() { A = 4, B = 5, C = 6, D = 7 });

            // Right face (normal to global x, +): 0, 3, 5, 4
            faces.Add(new Face() { A = 0, B = 3, C = 5, D = 4 });

            // Left face (normal to global x, -): 2, 1, 7, 6
            faces.Add(new Face() { A = 2, B = 1, C = 7, D = 6 });

            // Front face (normal to global y, +): 1, 0, 4, 7
            faces.Add(new Face() { A = 1, B = 0, C = 4, D = 7 });

            // Rear face (normal to global y, -): 3, 2, 6, 5
            faces.Add(new Face() { A = 3, B = 2, C = 6, D = 5 });

            return new RenderMesh() { Faces = faces, Vertices = vertices };
        }


    }
}