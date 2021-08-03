/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2021, the respective contributors. All rights reserved.
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

using BH.Engine.Base;
using BH.oM.Geometry;
using BH.oM.Reflection.Attributes;
using System.Linq;
using System.ComponentModel;
using System;
using System.Collections.Generic;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        [Description("Calculates the area of the provided geometry.")]
        [Input("geometry", "Geometry to get the area of.")]
        [Input("tolerance", "The tolerance to apply to the area calculation.")]
        [Output("area", "The area of the geometry.")]
        [PreviousVersion("4.3", "BH.Engine.Geometry.Query.IArea(BH.oM.Geometry.IGeometry)")]
        public static double IArea(this IGeometry geometry, double tolerance = Tolerance.Distance)
        {
            return Area(geometry as dynamic, tolerance);
        }


        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        [Description("Calculates the area of the provided geometry.")]
        [Input("curve", "The Arc to get the area of.")]
        [Input("tolerance", "The tolerance to apply to the area calculation.")]
        [Output("area", "The area of the geometry.")]
        [PreviousVersion("4.3", "BH.Engine.Geometry.Query.Area(BH.oM.Geometry.Arc)")]
        
        public static double Area(this Arc curve, double tolerance = Tolerance.Distance)
        {
            if (curve.IsClosed(tolerance))
                return Math.PI * curve.Radius * curve.Radius;
            else
            {
                Reflection.Compute.RecordWarning("Cannot calculate area for an open curve.");
                return 0;
            }
        }

        /***************************************************/

        [Description("Calculates the area of the provided geometry.")]
        [Input("curve", "The Circle to get the area of.")]
        [Input("tolerance", "The tolerance to apply to the area calculation.")]
        [Output("area", "The area of the geometry.")]
        [PreviousVersion("4.3", "BH.Engine.Geometry.Query.Area(BH.oM.Geometry.Circle)")]

        public static double Area(this Circle curve, double tolerance = Tolerance.Distance)
        {
            return Math.PI * curve.Radius * curve.Radius;
        }

        /***************************************************/

        [Description("Calculates the area of the provided geometry.")]
        [Input("curve", "The Line to get the area of.")]
        [Input("tolerance", "The tolerance to apply to the area calculation.")]
        [Output("area", "The area of the geometry.")]
        [PreviousVersion("4.3", "BH.Engine.Geometry.Query.Area(BH.oM.Geometry.Line)")]

        public static double Area(this Line curve, double tolerance = Tolerance.Distance)
        {
            Reflection.Compute.RecordWarning("Cannot calculate area for an open curve.");
            return 0;
        }

        /***************************************************/

        [Description("Calculates the area of the provided geometry.")]
        [Input("curve", "The PolyCurve to get the area of.")]
        [Input("tolerance", "The tolerance to apply to the area calculation.")]
        [Output("area", "The area of the geometry.")]
        [PreviousVersion("4.3", "BH.Engine.Geometry.Query.Area(BH.oM.Geometry.PolyCurve)")]
        
        public static double Area(this PolyCurve curve, double tolerance = Tolerance.Distance)
        {
            if (curve.Curves.Count == 1 && curve.Curves[0] is Circle)
                return (curve.Curves[0] as Circle).Area(tolerance);

            if (!curve.IsClosed(tolerance))
            {
                Reflection.Compute.RecordWarning("Cannot calculate area for an open curve.");
                return 0;
            }

            Plane p = curve.FitPlane(tolerance);
            if (p == null)
                return 0.0;              // points are collinear

            Point sPt = curve.StartPoint();
            double area = 0;

            foreach (ICurve c in curve.SubParts())
            {
                if (c is NurbsCurve)
                {
                    Reflection.Compute.RecordError("Area for NurbsuCurve is not implemented.");
                    return double.NaN;
                }
                Point ePt = c.IEndPoint();
                Vector prod = CrossProduct(sPt - p.Origin, ePt - p.Origin);
                area += prod * p.Normal * 0.5;

                if (c is Arc)
                {
                    Arc arc = c as Arc;
                    double radius = arc.Radius;
                    double angle = arc.Angle();
                    double arcArea = (angle - Math.Sin(angle)) * radius * radius * 0.5;

                    if (arc.CoordinateSystem.Z.DotProduct(p.Normal) > 0)
                        area += arcArea;
                    else
                        area -= arcArea;
                }

                sPt = ePt.DeepClone();
            }

            return Math.Abs(area);
        }

        /***************************************************/

        [Description("Calculates the area of the provided geometry.")]
        [Input("curve", "The Polyline to get the area of.")]
        [Input("tolerance", "The tolerance to apply to the area calculation.")]
        [Output("area", "The area of the geometry.")]
        [PreviousVersion("4.3", "BH.Engine.Geometry.Query.Area(BH.oM.Geometry.Polyline)")]

        public static double Area(this Polyline curve, double tolerance = Tolerance.Distance)
        {
            if (!curve.IsClosed(tolerance))
            {
                Reflection.Compute.RecordWarning("Cannot calculate area for an open curve.");
                return 0;
            }

            List<Point> pts = curve.ControlPoints;
            int ptsCount = pts.Count;
            if (ptsCount < 4)
                return 0.0;

            Plane p = pts.FitPlane(tolerance);
            if (p == null)
                return 0.0;              // points are collinear

            double x = 0, y = 0, z = 0;
            for (int i = 0; i < ptsCount; i++)
            {
                int j = (i + 1) % ptsCount;
                Vector prod = CrossProduct(pts[i] - p.Origin, pts[j] - p.Origin);
                x += prod.X;
                y += prod.Y;
                z += prod.Z;
            }

            return Math.Abs((new Vector { X = x, Y = y, Z = z } * p.Normal) * 0.5);
        }


        /***************************************************/
        /**** Public Methods - Surfaces                 ****/
        /***************************************************/

        [Description("Calculates the area of the provided geometry.")]
        [Input("mesh", "The mesh to get the area of.")]
        [Input("tolerance", "The tolerance to apply to the area calculation.")]
        [Output("area", "The area of the geometry.")]
        [PreviousVersion("4.3", "BH.Engine.Geometry.Query.Area(BH.oM.Geometry.Mesh)")]
        
        public static double Area(this Mesh mesh, double tolerance = Tolerance.Distance)
        {
            Mesh tMesh = mesh.Triangulate();
            double area = 0;
            List<Face> faces = tMesh.Faces;
            List<Point> vertices = tMesh.Vertices;

            for (int i = 0; i < faces.Count; i++)
            {
                Point pA = vertices[faces[i].A];
                Point pB = vertices[faces[i].B];
                Point pC = vertices[faces[i].C];
                Vector AB = new Vector { X = pB.X - pA.X, Y = pB.Y - pA.Y, Z = pB.Z - pA.Z };
                Vector AC = new Vector { X = pC.X - pA.X, Y = pC.Y - pA.Y, Z = pC.Z - pA.Z };
                area += AB.CrossProduct(AC).Length();
            }

            return area / 2;
        }

        /***************************************************/

        [Description("Calculates the area of the provided geometry.")]
        [Input("pSurf", "The PolySurface to get the area of.")]
        [Input("tolerance", "The tolerance to apply to the area calculation.")]
        [Output("area", "The area of the geometry.")]
        [PreviousVersion("4.3", "BH.Engine.Geometry.Query.Area(BH.oM.Geometry.PolySurface)")]
        
        public static double Area(this PolySurface pSurf, double tolerance = Tolerance.Distance)
        {
            return pSurf.Surfaces.Sum(x => x.IArea(tolerance));
        }

        /***************************************************/

        [Description("Calculates the area of the provided geometry.")]
        [Input("pSurf", "The PlanarSurface to get the area of.")]
        [Input("tolerance", "The tolerance to apply to the area calculation.")]
        [Output("area", "The area of the geometry.")]
        [PreviousVersion("4.3", "BH.Engine.Geometry.Query.Area(BH.oM.Geometry.PlanarSurface)")]
        
        public static double Area(this PlanarSurface pSurf, double tolerance = Tolerance.Distance)
        {
            double area = pSurf.ExternalBoundary.IArea(tolerance);

            if (pSurf.InternalBoundaries != null)
            {
                area -= pSurf.InternalBoundaries.Sum(x => x.IArea(tolerance));
            }

            return area;
        }


        /***************************************************/
        /**** Public Methods - Vectors                  ****/
        /***************************************************/

        [Description("Calculates the area of the provided geometry.")]
        [Input("v1", "First vector to use for vector-based area calculation.")]
        [Input("v2", "Second vector to use for vector-based area calculation.")]
        [Input("tolerance", "The tolerance to apply to the area calculation.")]
        [Output("area", "The area of the geometry.")]
        [PreviousVersion("4.3", "BH.Engine.Geometry.Query.Area(BH.oM.Geometry.Vector, BH.oM.Geometry.Vector)")]
        
        public static double Area(this Vector v1, Vector v2, double tolerance = Tolerance.Distance)
        {
            double area = 0;
            area = Length(CrossProduct(v1, v2)) / 2;

            return area;
        }


        /***************************************************/
        /**** Private Methods - Fallbacks               ****/
        /***************************************************/

        [Description("Calculates the area of the provided geometry.")]
        [Input("geometry", "Geometry to get the area of.")]
        [Input("tolerance", "The tolerance to apply to the area calculation.")]
        [Output("area", "The area of the geometry.")]
        [PreviousVersion("4.3", "BH.Engine.Geometry.Query.Area(BH.oM.Geometry.IGeometry)")]
        
        private static double Area(this IGeometry geometry, double tolerance = Tolerance.Distance)
        {
            Reflection.Compute.RecordError("Area for " + geometry.GetType().Name + " is not implemented.");
            return double.NaN;
        }

        /***************************************************/
    }
}
