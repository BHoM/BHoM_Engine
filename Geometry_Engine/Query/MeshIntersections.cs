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

using BH.oM.Geometry;
using BH.oM.Base.Attributes;
using BH.oM.Quantities.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Finds the intersection line between a Plane and a Mesh. Currently not implemented.")]
        [Input("p", "The Plane to intersect with the Mesh.")]
        [Input("mesh", "The Mesh to intersect with the Plane.")]
        [Input("tolerance", "The tolerance for the intersection calculation.", typeof(Length))]
        [Output("line", "The intersection Line between the Plane and Mesh.")]
        [NotImplemented]
        public static Line MeshIntersections(this Plane p, Mesh mesh, double tolerance = Tolerance.Distance)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        [Description("Finds the intersection Points between a Line and a Mesh using ray-triangle intersection.")]
        [Input("curve", "The Line to intersect with the Mesh.")]
        [Input("mesh", "The Mesh to intersect with the Line.")]
        [Input("tolerance", "The tolerance for the intersection calculation.", typeof(Length))]
        [Output("points", "The intersection Points between the Line and Mesh faces.")]
        public static List<Point> MeshIntersections(this Line curve, Mesh mesh, double tolerance = Tolerance.Distance)
        {
            // Outputs
            List<Point> points = new List<Point>();

            // Preprocessing Mesh
            Mesh tMesh = mesh.Triangulate();
            List<Face> faces = tMesh.Faces;
            List<Point> meshPts = tMesh.Vertices;

            for (int i = 0; i < faces.Count; i++)
            {
                // Mesh Points
                Point p1 = meshPts[faces[i].A];
                Point p2 = meshPts[faces[i].B];
                Point p3 = meshPts[faces[i].C];

                // Ray direction
                Vector d = curve.PointAtParameter(1) - curve.PointAtParameter(0);

                // Vectors from p1 to p2/p3 (edges)
                Vector e1, e2;

                Vector p, q, t;
                double det, invDet, u, v;

                //Find vectors for two edges sharing vertex/point p1
                e1 = p2 - p1;
                e2 = p3 - p1;

                // calculating determinant 
                p = Query.CrossProduct(d, e2);

                //Calculate determinat
                det = e1 * p;

                //if determinant is near zero, ray lies in plane of triangle otherwise not
                if (det > -tolerance && det < tolerance)
                    continue;

                invDet = 1.0f / det;

                //calculate distance from p1 to ray origin
                t = curve.PointAtParameter(0) - p1;

                //Calculate u parameter
                u = t * p * invDet;

                //Check for ray hit
                if (u < 0 || u > 1)
                    continue;

                //Prepare to test v parameter
                q = Query.CrossProduct(t, e1);

                //Calculate v parameter
                v = d * q * invDet;

                //Check for ray hit
                if (v < 0 || u + v > 1)
                    continue;

                if ((e2 * q * invDet) > Double.Epsilon)
                    points.Add((1 - u - v) * p1 + u * p2 + v * p3); //ray does intersect
            }
            return points;
        }

        /***************************************************/

        [Description("Finds the intersection Points between a Polyline and a Mesh by checking each segment.")]
        [Input("curve", "The Polyline to intersect with the Mesh.")]
        [Input("mesh", "The Mesh to intersect with the Polyline.")]
        [Input("tolerance", "The tolerance for the intersection calculation.", typeof(Length))]
        [Output("points", "The intersection Points between the Polyline segments and Mesh faces.")]
        public static List<Point> MeshIntersections(this Polyline curve, Mesh mesh, double tolerance = Tolerance.Distance)
        {
            List<Point> points = new List<Point>();
            foreach (Line line in curve.SubParts())
            {
                points.AddRange(MeshIntersections(line, mesh, tolerance));
            }
            return points;
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        [Description("Finds the intersection Points between any ICurve and a Mesh.")]
        [Input("curve", "The ICurve to intersect with the Mesh.")]
        [Input("mesh", "The Mesh to intersect with the curve.")]
        [Input("tolerance", "The tolerance for the intersection calculation.", typeof(Length))]
        [Output("points", "The intersection Points between the curve and Mesh faces.")]
        public static List<Point> IMeshIntersections(this ICurve curve, Mesh mesh, double tolerance = Tolerance.Distance)
        {
            return MeshIntersections(curve as dynamic, mesh);
        }


        /***************************************************/
        /**** Private Fallback Methods                  ****/
        /***************************************************/

        private static List<Point> MeshIntersections(this ICurve curve, Mesh mesh, double tolerance = Tolerance.Distance)
        {
            Base.Compute.RecordError($"MeshIntersections is not implemented for ICurves of type: {curve.GetType().Name}.");
            return null;
        }

        /***************************************************/
    }
}

