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

        [Description("Returns a BHoM mesh representation for the BHoM Bar.")]
        public static BH.oM.Graphics.RenderMesh RenderMesh(this Sphere sphere, RenderMeshOptions renderMeshOptions = null)
        {
            renderMeshOptions = renderMeshOptions ?? new RenderMeshOptions();

            int nLatitude = 8;                  // Number of vertical lines.
            int nLongitude = nLatitude / 2;      // Number of horizontal lines. A good sphere mesh has about half the number of longitude lines than latitude.

            double DEGS_TO_RAD = Math.PI / 180;
            int numVertices = 0;    // Tallies the number of vertex points added.

            int p, s, i, j;
            double x, y, z, output;
            int nPitch = nLongitude + 1;

            double pitchInc = (180 / nPitch) * DEGS_TO_RAD;
            double rotInc = (360 / nLatitude) * DEGS_TO_RAD;
            double radius = sphere.Radius;
            Point centrePoint = sphere.Centre;

            // ------- Generate all points -------- //

            List<Point> allPoints = new List<Point>();

            Point top = new Point() { X = sphere.Centre.X, Y = sphere.Centre.Y + radius, Z = sphere.Centre.Z };
            Point bottom = new Point() { X = sphere.Centre.X, Y = sphere.Centre.Y - radius, Z = sphere.Centre.Z };

            allPoints.Add(top); allPoints.Add(bottom);
            numVertices = numVertices + 2;

            int fVert = numVertices;    // Record the first vertex index for intermediate vertices.
            for (p = 1; p < nPitch; p++)     // Generate all "intermediate vertices"
            {
                output = radius * Math.Sin(p * pitchInc);
                output = Math.Abs(output);

                y = radius * Math.Cos(p * pitchInc);

                for (s = 0; s < nLatitude; s++)
                {
                    x = output * Math.Cos(s * rotInc);
                    z = output * Math.Sin(s * rotInc);

                    allPoints.Add(new Point() { X = x + centrePoint.X, Y = y + centrePoint.Y, Z = z + centrePoint.Z });
                    numVertices++;
                }
            }

            // ------- Generate all faces -------- //

            List<Face> allFaces = new List<Face>();

            // Square faces between intermediate points
            for (p = 1; p < nPitch - 1; p++)
            {
                for (s = 0; s < nLatitude; s++)
                {
                    i = p * nLatitude + s;
                    j = (s == nLatitude - 1) ? i - nLatitude : i;

                    allFaces.Add(new Face() { A = (i + 1 - nLatitude) + fVert, B = (j + 2 - nLatitude) + fVert, C = (j + 2) + fVert, D = (i + 1) + fVert });
                }
            }

            // Triangle faces between top/bottom points and the intermediate points
            int offLastVerts = fVert + (nLatitude * (nLongitude - 1));
            for (s = 0; s < nLatitude; s++)
            {
                j = (s == nLatitude - 1) ? -1 : s;
                allFaces.Add(new Face() { A = fVert - 1, B = (j + 2) + fVert, C = (s + 1) + fVert });
                allFaces.Add(new Face() { A = fVert, B = (s + 1) + offLastVerts, C = (j + 2) + offLastVerts });
            }

            return new RenderMesh() { Faces = allFaces, Vertices = allPoints.Select(pt => (Vertex)pt).ToList() };
        }

    }
}