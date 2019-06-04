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
        /**** Public Methods - Curves                   ****/
        /***************************************************/
        
        public static Vector Normal(this Polyline curve)
        {

            if (!curve.IsPlanar())
            {
                Reflection.Compute.RecordError("Input must be planar.");
                return null;
            }
            else if (!curve.IsClosed())
            {
                Reflection.Compute.RecordError("Curve is not closed. Input must be a polygon");
                return null;
            }
            else if (curve.IsSelfIntersecting())
            {
                Reflection.Compute.RecordWarning("Curve is self intersecting");
                return null;
            }

            Vector normal = new Vector { X = 0, Y = 0, Z = 0 };

            for (int i = 0; i < curve.ControlPoints.Count - 1; i++)
            {
                Point pA = curve.ControlPoints[i];
                Point pB = curve.ControlPoints[i + 1];
                Point pC = curve.ControlPoints[(i + 2) % curve.ControlPoints.Count];

                normal += CrossProduct(pB - pA, pC - pB);
            }

            return normal.Normalise();
        }

        /***************************************************/

        public static Vector Normal(this PolyCurve curve)
        {

            if (!curve.IsPlanar())
            {
                Reflection.Compute.RecordError("Input must be planar.");
                return null;
            }
            else if (!curve.IsClosed())
            {
                Reflection.Compute.RecordError("Curve is not closed. Input must be a polygon");
                return null;
            }
            else if (curve.IsSelfIntersecting())
            {
                Reflection.Compute.RecordWarning("Curve is self intersecting");
                return null;
            }

            List<ICurve> crvs = new List<ICurve>(curve.ISubParts());

            Vector normal = new Vector { X = 0, Y = 0, Z = 0 };

            List<Point> pts = new List<Point> { curve.Curves[0].IStartPoint() };

            if (crvs.Count() == 0)
                return null;
            else if (crvs.Count() == 1)
                return crvs[0].INormal();
            else
            {
                foreach (ICurve crv in crvs)
                {
                    if (crv is Line)
                        pts.Add((crv as Line).End);
                    else if (crv is Arc)
                    {
                        int numb = 4;
                        List<Point> aPts = crv.SamplePoints(numb);
                        for (int i = 1; i < aPts.Count; i++)
                            pts.Add(aPts[i]);
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }

                Point pA = pts[0];

                for (int i = 1; i < pts.Count - 1; i++)
                {
                    Point pB = pts[i];
                    Point pC = pts[i + 1];

                    normal += CrossProduct((pB - pA).Normalise(), (pC - pB).Normalise());
                    normal += CrossProduct(pB - pA, pC - pA);
                }

                return normal.Normalise();
            }
        }

        /***************************************************/

        public static Vector Normal(this Circle circle)
        {
            return circle.Normal;
        }
        
        /***************************************************/

        public static Vector Normal(this Ellipse ellipse)
        {
            Vector normal = (ellipse.Axis1).CrossProduct(ellipse.Axis2);
            return normal;
        }

        /***************************************************/

        [NotImplemented]
        public static List<Vector> Normals(this ISurface surface)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        public static Vector Normal(this Arc arc)
        {
            if (arc.Angle() > 0)
                return arc.CoordinateSystem.Z;
            else
                return arc.CoordinateSystem.Z.Reverse();
        }

        /***************************************************/

        [NotImplemented]
        public static Vector Normal(this Line line)
        {
            throw new NotImplementedException();
        }
        
        /***************************************************/

        [NotImplemented]
        public static Vector Normal(this NurbsCurve nurbsCurve)
        {
            throw new NotImplementedException();
        }
        
        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static Vector INormal(this ICurve curve)
        {
            return Normal(curve as dynamic);
        }

        /***************************************************/
    }
}

