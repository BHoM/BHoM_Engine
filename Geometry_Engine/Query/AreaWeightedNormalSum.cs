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
using System.Collections.Generic;
using System.ComponentModel;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Calculates the area-weighted normal sum of a mesh. This method computes the sum of face normals weighted by their respective areas, then returns the ratio of the total normal magnitude to the total area.")]
        [Input("mesh", "The mesh to calculate the area-weighted normal sum for. Must have valid vertices and faces.")]
        [Output("ratio", "The ratio of the total normal magnitude to the total area of the mesh.")]
        public static double AreaWeightedNormalSum(this Mesh mesh)
        {
            if (mesh == null)
            {
                Engine.Base.Compute.RecordError("Cannot calculate area weighted normal sum of a null mesh.");
                return double.NaN;
            }

            if (mesh.Vertices == null || mesh.Faces == null)
            {
                Engine.Base.Compute.RecordError("Mesh has to have valid vertices and faces to calculate its area weighted normal sum.");
                return double.NaN;
            }

            Vector totalNormal = new Vector();
            double totalArea = 0.0;

            List<Point> vertices = mesh.Vertices;
            List<Face> faces = mesh.Faces;
            for (int i = 0; i < mesh.Faces.Count; i++)
            {
                Face face = faces[i];
                Point pA = vertices[face.A];
                Point pB = vertices[face.B];
                Point pC = vertices[face.C];

                if (face.D == -1) // Triangular face
                {
                    Vector normal = (pB - pA).CrossProduct(pC - pA).Normalise();

                    Vector AB = new Vector { X = pB.X - pA.X, Y = pB.Y - pA.Y, Z = pB.Z - pA.Z };
                    Vector AC = new Vector { X = pC.X - pA.X, Y = pC.Y - pA.Y, Z = pC.Z - pA.Z };
                    double area = 0.5 * AB.CrossProduct(AC).Length();

                    totalNormal += normal * area;
                    totalArea += area;
                }
                else // Quadrilateral face - split into two triangles
                {
                    Point pD = vertices[face.D];

                    // First triangle: A, B, C
                    Vector normal1 = (pB - pA).CrossProduct(pC - pA).Normalise();
                    Vector AB = new Vector { X = pB.X - pA.X, Y = pB.Y - pA.Y, Z = pB.Z - pA.Z };
                    Vector AC = new Vector { X = pC.X - pA.X, Y = pC.Y - pA.Y, Z = pC.Z - pA.Z };
                    double area1 = 0.5 * AB.CrossProduct(AC).Length();

                    // Second triangle: A, C, D
                    Vector normal2 = (pC - pA).CrossProduct(pD - pA).Normalise();
                    Vector AD = new Vector { X = pD.X - pA.X, Y = pD.Y - pA.Y, Z = pD.Z - pA.Z };
                    double area2 = 0.5 * AC.CrossProduct(AD).Length();

                    totalNormal += normal1 * area1 + normal2 * area2;
                    totalArea += area1 + area2;
                }
            }

            return totalNormal.Length() / totalArea;
        }

        /***************************************************/
    }
}

