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

using BH.oM.Geometry;
using BH.oM.Reflection.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods - Surfaces                 ****/
        /***************************************************/

        public static Vector Normal(this Face face, Mesh mesh)
        {
            List<Point> vertices = mesh.Vertices;
            Face meshFace = mesh.Faces[mesh.Faces.IndexOf(face)];
            Point pA = vertices[meshFace.A];
            Point pB = vertices[meshFace.B];
            Point pC = vertices[meshFace.C];

            Vector normal;
            if (!meshFace.IsQuad())
                normal = CrossProduct(pB - pA, pC - pB);
            else
            {
                Point pD = vertices[(meshFace.D)];
                normal = (CrossProduct(pA - pD, pB - pA)) + (CrossProduct(pC - pB, pD - pC));
            }

            return normal.Normalise();
        }

        /***************************************************/

        public static List<Vector> Normals(this Mesh mesh)
        {
            List<Vector> normals = new List<Vector>(mesh.Faces.Count);
            List<Point> vertices = mesh.Vertices;
            List<Face> faces = mesh.Faces;

            for (int i = 0; i < mesh.Faces.Count; i++)
            {
                Point pA = vertices[(faces[i].A)];
                Point pB = vertices[(faces[i].B)];
                Point pC = vertices[(faces[i].C)];

                if (!faces[i].IsQuad())
                {
                    Vector normal = CrossProduct(pB - pA, pC - pB);
                    normals.Add(normal.Normalise());
                }
                else
                {
                    Point pD = vertices[(faces[i].D)];
                    Vector normal = (CrossProduct(pA - pD, pB - pA)) + (CrossProduct(pC - pB, pD - pC));
                    normal.Normalise();
                    normals[i] = normal;
                }
            }

            return normals.ToList();
        }

        /***************************************************/

        [NotImplemented]
        public static List<Vector> Normals(this ISurface surface)
        {
            throw new NotImplementedException();
        }

        /***************************************************/
    }
}
