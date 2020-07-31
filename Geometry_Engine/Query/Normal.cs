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
using System.ComponentModel;

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
                    normals.Add(normal.Normalise());
                }
            }

            return normals.ToList();
        }

        /***************************************************/

        public static List<Vector> Normals(this Mesh3D mesh)
        {
            return mesh.ToMesh().Normals();
        }


        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        public static Vector Normal(this Polyline curve, double tolerance = Tolerance.Distance)
        {

            if (!curve.IsPlanar(tolerance))
            {
                Reflection.Compute.RecordError("Input must be planar.");
                return null;
            }
            else if (!curve.IsClosed(tolerance))
            {
                Reflection.Compute.RecordError("Curve is not closed. Input must be a polygon");
                return null;
            }
            else if (curve.IsSelfIntersecting(tolerance))
            {
                Reflection.Compute.RecordWarning("Curve is self intersecting");
                return null;
            }

            Point avg = curve.ControlPoints.Average();
            Vector normal = new Vector();

            //Get out normal, from cross products between vectors from the average point to adjecent controlpoints on the curve
            for (int i = 0; i < curve.ControlPoints.Count - 1; i++)
                normal += (curve.ControlPoints[i] - avg).CrossProduct(curve.ControlPoints[i + 1] - avg);

            normal = normal.Normalise();

            //Check if normal needs to be flipped from the right hand rule
            if (!curve.IsClockwise(normal, tolerance))
                normal = -normal;

            return normal;

        }

        /***************************************************/

        public static Vector Normal(this PolyCurve curve, double tolerance = Tolerance.Distance)
        {

            if (!curve.IsPlanar(tolerance))
            {
                Reflection.Compute.RecordError("Input must be planar.");
                return null;
            }
            else if (!curve.IsClosed(tolerance))
            {
                Reflection.Compute.RecordError("Curve is not closed. Input must be a polygon");
                return null;
            }
            else if (curve.IsSelfIntersecting(tolerance))
            {
                Reflection.Compute.RecordWarning("Curve is self intersecting");
                return null;
            }

            List<ICurve> crvs = new List<ICurve>(curve.ISubParts());


            if (crvs.Count() == 0)
                return null;
            else if (crvs.Count() == 1)
                return crvs[0].INormal();
            else
            {
                List<Point> points = new List<Point>();

                foreach (ICurve crv in crvs)
                {
                    if (crv is Line)
                        points.Add((crv as Line).End);
                    else if (crv is Arc)
                    {
                        for (int j = 1; j < 5; j++)
                        {
                            points.Add((crv as Arc).PointAtParameter(j * 0.25));
                        }
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }


                Point avg = points.Average();
                Vector normal = new Vector();

                //Get out normal, from cross products between vectors from the average point to adjecent controlpoints on the curve
                for (int i = 0; i < points.Count - 1; i++)
                    normal += (points[i] - avg).CrossProduct(points[i + 1] - avg);

                normal = normal.Normalise();

                //Check if normal needs to be flipped from the right hand rule
                if (!curve.IsClockwise(normal, tolerance))
                    normal = -normal;

                return normal;
            }
        }

        /***************************************************/

        public static Vector Normal(this Circle circle, double tolerance = Tolerance.Distance)
        {
            return circle.Normal;
        }

        /***************************************************/

        public static Vector Normal(this Ellipse ellipse, double tolerance = Tolerance.Distance)
        {
            Vector normal = (ellipse.Axis1).CrossProduct(ellipse.Axis2);
            return normal;
        }

        /***************************************************/

        public static Vector Normal(this Arc arc, double tolerance = Tolerance.Distance)
        {
            if (arc.Angle() > 0)
                return arc.CoordinateSystem.Z;
            else
                return arc.CoordinateSystem.Z.Reverse();
        }


        /***************************************************/
        /**** Public Methods - Surfaces                 ****/
        /***************************************************/

        public static Vector Normal(this PlanarSurface surface)
        {
            return surface.ExternalBoundary.INormal();
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static Vector INormal(this ICurve curve, double tolerance = Tolerance.Distance)
        {
            return Normal(curve as dynamic, tolerance);
        }

        /***************************************************/

        public static List<Vector> INormals(this IGeometry geometry)
        {
            return Normals(geometry as dynamic);
        }


        /***************************************************/
        /**** Private Fallback Methods                  ****/
        /***************************************************/

        private static Vector Normal(this ICurve curve, double tolerance = Tolerance.Distance)
        {
            throw new NotImplementedException("ICurve of type: " + curve.GetType().Name + " is not implemented for Normal.");
        }

        /***************************************************/

        private static List<Vector> Normals(this IGeometry geometry)
        {
            throw new NotImplementedException("IGeometry of type: " + geometry.GetType().Name + " is not implemented for Normals.");
        }

        /***************************************************/
    }
}