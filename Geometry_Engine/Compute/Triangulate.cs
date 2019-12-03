/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2019, the respective contributors. All rights reserved.
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

using System.ComponentModel;

namespace BH.Engine.Geometry
{
    public static partial class Compute
    {
        /***************************************************/
        /****      public Methods                       ****/
        /***************************************************/

        [Description("Create a Delaunay mesh from an outline and holes")]
        [Input("outerCurve", "A BHoM Polyline representing the mesh boundary")]
        [Input("innerCurves", "A list of holes to \"punch\" through the mesh generated mesh")]
        [Output("curve", "A list of BHoM Polylines")]
        public static List<Polyline> Triangulate(this Polyline outerCurve, List<Polyline> innerCurves = null)
        {
            // Create a zero length list if no holes input
            if (innerCurves == null) innerCurves = new List<Polyline>();

            // Get the transformation matrix
            Plane plane = outerCurve.IFitPlane();
            Vector normal = plane.Normal;
            List<Point> vertices = outerCurve.IDiscontinuityPoints();
            Point refPoint = vertices.Min();
            Point refPointP = BH.Engine.Geometry.Create.Point(refPoint.X, refPoint.Y, 0);
            Vector zVector = BH.Engine.Geometry.Create.Vector(0, 0, 1);
            Vector rotationVector = normal.CrossProduct(zVector).Normalise();
            double rotationAngle = normal.Angle(zVector);
            TransformMatrix transformMatrix = BH.Engine.Geometry.Create.RotationMatrix(vertices.Min(), rotationVector, rotationAngle);

            // Get the translation vector
            Vector translateVector = refPointP - refPoint;

            // Transform the original input curve/s
            Polyline transformedCurve = Modify.Translate(outerCurve.Transform(transformMatrix), translateVector);
            List<Polyline> transformedHole = new List<Polyline>();
            foreach (Polyline h in innerCurves)
            {
                if (h.IsCoplanar(outerCurve))
                {
                    transformedHole.Add(Modify.Translate(h.Transform(transformMatrix), translateVector));
                }
            }

            // Convert geometry to Triangle inputs
            TriangleNet.Geometry.Polygon parentPolygon = new TriangleNet.Geometry.Polygon();
            List<TriangleNet.Geometry.Vertex> parentVertices = new List<TriangleNet.Geometry.Vertex>();
            foreach (Point point in transformedCurve.IDiscontinuityPoints())
            {
                parentPolygon.Add(new TriangleNet.Geometry.Vertex(point.X, point.Y));
                parentVertices.Add(new TriangleNet.Geometry.Vertex(point.X, point.Y));
            }
            TriangleNet.Geometry.Contour parentContour = new TriangleNet.Geometry.Contour(parentVertices);
            parentPolygon.Add(parentContour);

            foreach (Polyline h in transformedHole)
            {
                List<TriangleNet.Geometry.Vertex> childVertices = new List<TriangleNet.Geometry.Vertex>();
                foreach (Point point in h.IDiscontinuityPoints())
                {
                    childVertices.Add(new TriangleNet.Geometry.Vertex(point.X, point.Y));
                }
                TriangleNet.Geometry.Contour childContour = new TriangleNet.Geometry.Contour(childVertices);
                Point childCentroid = h.PointInRegion();
                parentPolygon.Add(childContour, new TriangleNet.Geometry.Point(childCentroid.X, childCentroid.Y));
            }

            // Triangulate
            TriangleNet.Meshing.ConstraintOptions options = new TriangleNet.Meshing.ConstraintOptions() { ConformingDelaunay = true, };
            TriangleNet.Meshing.QualityOptions quality = new TriangleNet.Meshing.QualityOptions() { };
            TriangleNet.Mesh mesh = (TriangleNet.Mesh)TriangleNet.Geometry.ExtensionMethods.Triangulate(parentPolygon, options, quality);

            // Convert triangulations back to BHoM geometry
            List<Polyline> translatedPolylines = new List<Polyline>();
            foreach (var face in mesh.Triangles)
            {
                // List points defining the triangle
                List<Point> pts = new List<Point>();
                pts.Add(BH.Engine.Geometry.Create.Point(face.GetVertex(0).X, face.GetVertex(0).Y));
                pts.Add(BH.Engine.Geometry.Create.Point(face.GetVertex(1).X, face.GetVertex(1).Y));
                pts.Add(BH.Engine.Geometry.Create.Point(face.GetVertex(2).X, face.GetVertex(2).Y));
                pts.Add(pts.First());
                translatedPolylines.Add(BH.Engine.Geometry.Create.Polyline(pts));
            }

            // Translate back to original plane
            List<Polyline> meshPolylines = new List<Polyline>();
            foreach (Polyline pl in translatedPolylines)
            {
                TransformMatrix matrixTransposed = transformMatrix.Invert();
                Polyline meshPolyline = pl.Translate(-translateVector).Transform(transformMatrix.Invert());
                meshPolylines.Add(meshPolyline);
            }

            return meshPolylines;
        }
    }
}
