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
using BH.oM.Reflection.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static double IArea(this IGeometry geometry)
        {
            return Area(geometry as dynamic);
        }


        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        public static double Area(this Arc curve)
        {
            return curve.IsClosed() ? curve.Angle() * Math.Pow(curve.Radius(), 2) : 0;
        }

        /***************************************************/

        public static double Area(this Circle curve)
        {
            return Math.PI * Math.Pow(curve.Radius, 2);
        }

        /***************************************************/

        public static double Area(this Line curve)
        {
            return 0;
        }

        /***************************************************/

        [NotImplemented]
        public static double Area(this NurbsCurve curve)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        public static double Area(this PolyCurve curve)
        {
            if (curve.Curves.Count == 1 && curve.Curves[0] is Circle)
                return (curve.Curves[0] as Circle).Area();

            if (!curve.IsClosed())
                return 0;

            Plane p = curve.FitPlane();
            if (p == null)
                return 0.0;              // points are collinear
            
            Point sPt = curve.StartPoint();
            double area = 0;

            foreach (ICurve c in curve.SubParts())
            {
                if (c is NurbsCurve)
                    throw new NotImplementedException("Area of NurbsCurve is not imlemented yet so the area of this PolyCurve cannot be calculated");

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

                sPt = ePt.Clone();
            }

            return Math.Abs(area);
        }

        /***************************************************/

        public static double Area(this Polyline curve)
        {
            if (!curve.IsClosed())
                return 0;

            List<Point> pts = curve.ControlPoints;
            int ptsCount = pts.Count;
            if (ptsCount < 4)
                return 0.0;

            Plane p = pts.FitPlane();
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

        public static double Area(this Mesh mesh)
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

        public static double Area(this PolySurface pSurf)
        {
            return pSurf.Surfaces.Sum(x => x.IArea());
        }

        /***************************************************/

        public static double Area(this PlanarSurface pSurf)
        {
            double area = pSurf.ExternalBoundary.IArea();
            area -= pSurf.InternalBoundaries.Sum(x => x.IArea());
            return area;
        }

        /***************************************************/

        [NotImplemented]
        public static double Area(this NurbsSurface nurbs)
        {
            throw new NotImplementedException();
        }


        /***************************************************/
        /**** Public Methods - Vectors                  ****/
        /***************************************************/

        public static double Area(this Vector v1, Vector v2)
        {
            double area = 0;
            area = Length(CrossProduct(v1, v2)) / 2;

            return area;
        }
        

        /***************************************************/
        /**** Private Methods - Fallbacks               ****/
        /***************************************************/

        private static double Area(this IGeometry geometry)
        {
            Reflection.Compute.RecordError("Area for " + geometry.GetType().Name + " is not implemented.");
            return double.NaN;
        }

        /***************************************************/
    }
}

