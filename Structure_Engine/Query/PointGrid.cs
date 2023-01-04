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
using BH.oM.Structure.Elements;
using BH.oM.Structure.Loads;
using BH.Engine.Geometry;
using BH.Engine.Spatial;

using System.Collections.Generic;
using System.Linq;
using System;

using BH.oM.Base.Attributes;
using BH.oM.Quantities.Attributes;
using System.ComponentModel;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Generates a rectangular grid of points on the Panel, scaled depending on Panel size. Used for load visualisation.")]
        [Input("panel", "The Panel to generate a grid on.")]
        [Output("grid", "Rectangular grid of points on the Panel.")]
        public static List<Point> PointGrid(this Panel panel)
        {
            if (panel.IsNull())
                return null;

            List<ICurve> curves = panel.ExternalElementCurves();

            List<PolyCurve> joined = BH.Engine.Geometry.Compute.IJoin(curves);
            List<PolyCurve> joinedOpeningCurves = BH.Engine.Geometry.Compute.IJoin(panel.InternalElementCurves());

            Plane plane = joined.First().FitPlane();

            Vector z = Vector.ZAxis;

            double angle = plane.Normal.Angle(z);

            Vector axis = plane.Normal.CrossProduct(z);

            TransformMatrix matrix = Engine.Geometry.Create.RotationMatrix(Point.Origin, axis, angle);

            List<PolyCurve> rotated = BH.Engine.Geometry.Compute.IJoin(curves.Select(x => x.IRotate(Point.Origin, axis, angle)).ToList());

            BoundingBox bounds = rotated.First().Bounds();

            for (int i = 1; i < rotated.Count; i++)
            {
                bounds += rotated[i].Bounds();
            }

            double xMin = bounds.Min.X;
            double yMin = bounds.Min.Y;
            double zVal = bounds.Min.Z;

            int steps = 9;

            double xStep = (bounds.Max.X - xMin) / steps;
            double yStep = (bounds.Max.Y - yMin) / steps;

            List<Point> pts = new List<Point>();
            TransformMatrix transpose = matrix.Transpose();

            for (int i = 0; i < steps; i++)
            {
                double x = xMin + xStep * i;
                for (int j = 0; j < steps; j++)
                {
                    Point pt = new Point { X = x, Y = yMin + yStep * j, Z = zVal };
                    bool isInside = false;

                    pt = pt.Transform(transpose);

                    foreach (PolyCurve crv in joined)
                    {
                        List<Point> list = new List<Point> { pt };
                        if (crv.IsContaining(list, true, 1E-3))
                        {
                            if (!joinedOpeningCurves.Any(c => c.IsContaining(list, false)))
                            {
                                isInside = true;
                                break;
                            }
                        }

                    }
                    if (isInside)
                        pts.Add(pt);
                }
            }

            return pts;
        }

        /***************************************************/

        [Description("Generates a rectangular grid of points on the each face of the FEMesh. Used for load visualisation.")]
        [Input("mesh", "The FEMesh to generate a grid on.")]
        [Output("grid", "Rectangular grid of points on the FEMesh.")]
        public static List<List<Point>> PointGrid(this FEMesh mesh)
        {
            return mesh.IsNull() ? null : mesh.Faces.Select(x => x.PointGrid(mesh)).ToList();
        }

        [Description("Generates a rectangular grid of points on the FEMeshFace of the FEMesh. Used for load visualisation.")]
        [Input("face", "The FEMeshFace to generate a grid on.")]
        [Input("mesh", "The FEMesh to which the face belongs.")]
        [Output("grid", "Rectangular grid of points on the FEMeshFace.")]
        public static List<Point> PointGrid(this FEMeshFace face, FEMesh mesh)
        {
            if (face.IsNull() || mesh.IsNull())
                return null;

            List<Point> pts = face.NodeListIndices.Select(i => mesh.Nodes[i].Position).ToList();

            List<Point> temp = new List<Point>();

            for (int i = 0; i < pts.Count; i++)
            {
                int next = (i + 1) % pts.Count;
                temp.Add((pts[i] + pts[next]) / 2);
            }
            pts.AddRange(temp);
            pts.Add(pts.Average());
            return pts;
        }

        /***************************************************/
        /**** Public Methods Interface                  ****/
        /***************************************************/

        [Description("Generates a rectangular grid of points on an IAreaElement. Used for load visualisation.")]
        [Input("element", "The element to generate a grid on.")]
        [Output("grid", "The generated rectangular grid of points on the element.")]
        public static List<Point> IPointGrid(this IAreaElement element)
        {
            return element.IIsNull() ? null : PointGrid(element as dynamic);
        }

        /***************************************************/
        /**** Private Methods - fall back               ****/
        /***************************************************/

        private static List<Point> PointGrid(this IAreaElement element)
        {
            Base.Compute.RecordWarning("Point grid for element of type " + element.GetType().Name + " not implemented.");
            return new List<Point>();
        }

        /***************************************************/
    }

}




