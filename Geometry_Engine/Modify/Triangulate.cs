/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
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
using System.Collections.Generic;

namespace BH.Engine.Geometry
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Mesh Triangulate(this Mesh mesh)
        {
            Mesh tMesh = new Mesh();
            List<Point> vertices = mesh.Vertices;
            List<Face> faces = mesh.Faces;

            tMesh.Vertices.AddRange(vertices);
            for (int i = 0; i < faces.Count; i++)
            {
                if (!faces[i].IsQuad())
                {
                    tMesh.Faces.Add(faces[i]);
                }
                else
                {
                    int i1 = faces[i].A;
                    int i2 = faces[i].B;
                    int i3 = faces[i].C;
                    int i4 = faces[i].D;
                    Point p1 = vertices[i1];
                    Point p2 = vertices[i2];
                    Point p3 = vertices[i3];
                    Point p4 = vertices[i4];
                    double d1 = (p1 - p3).Length();
                    double d2 = (p2 - p4).Length();

                    if (d1 > d2)    //Bracing based on shortest diagonal criteria
                    {
                        Face fA = new Face { A = i1, B = i2, C = i4 };
                        Face fB = new Face { A = i2, B = i3, C = i4 };
                        tMesh.Faces.Add(fA);
                        tMesh.Faces.Add(fB);
                    }
                    else
                    {
                        Face fA = new Face { A = i1, B = i2, C = i3 };
                        Face fB = new Face { A = i1, B = i3, C = i4 };
                        tMesh.Faces.Add(fA);
                        tMesh.Faces.Add(fB);
                    }
                }
            }

            return tMesh;
        }

        /***************************************************/
    }
}





