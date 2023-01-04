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

using BH.oM.Geometry;
using BH.oM.Base.Attributes;
using System.Collections.Generic;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Curves                                    ****/
        /***************************************************/

        public static Point Centre(this Arc arc, double tolerance = Tolerance.Distance)
        {
            return arc.CoordinateSystem.Origin;
        }

        /***************************************************/

        [ToBeRemoved("4.1", "To be removed as it is generally incorrect. Advising using Centroid instead.")]
        public static Point Centre(this Polyline polyline, double tolerance = Tolerance.Distance)
        {
            //TODO: this is an average point, not centroid - should be distinguished

            if (!polyline.IsClosed(tolerance))
                return polyline.ControlPoints.Average(); // TODO: not true for a self-intersecting polyline?
            else
                return polyline.ControlPoints.GetRange(0, polyline.ControlPoints.Count - 1).Average();
        }

        /***************************************************/
        /**** Surfaces                                    ****/
        /***************************************************/

        public static Point Centre(this BoundingBox box)
        {
            return new Point { X = (box.Max.X + box.Min.X) / 2, Y = (box.Max.Y + box.Min.Y) / 2, Z = (box.Max.Z + box.Min.Z) / 2 };
        }

        /***************************************************/
        /**** Mesh                                      ****/
        /***************************************************/

        public static List<Point> Centres(this Mesh mesh)
        {
            List<Face> faces = mesh.Faces;
            List<Point> vertices = mesh.Vertices;
            List<Point> centres = new List<Point>(faces.Count);

            for (int i = 0; i < faces.Count; i++)
            {
                Point pA = vertices[(faces[i].A)];
                Point pB = vertices[(faces[i].B)];
                Point pC = vertices[(faces[i].C)];

                if (!faces[i].IsQuad())
                    centres.Add(new Point { X = (pA.X + pB.X + pC.X) / 3, Y = (pA.Y + pB.Y + pC.Y) / 3, Z = (pA.Z + pB.Z + pC.Z) / 3 });
                else
                {
                    Point pD = vertices[(faces[i].D)];
                    centres.Add(new Point { X = (pA.X + pB.X + pC.X + pD.X) / 4, Y = (pA.Y + pB.Y + pC.Y + pD.Y) / 4, Z = (pA.Z + pB.Z + pC.Z + pD.Z) / 4 });  // Assumption that if the face is quad, it is a flat quad.
                }
            }

            return centres;
        }

        /***************************************************/
    }
}




