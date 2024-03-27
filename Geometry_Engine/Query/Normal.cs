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
using BH.oM.Base.Attributes;
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

        [Description("Returns a vector normal to a given face of the mesh.")]
        [Input("face", "The Face to get the normal to.")]
        [Input("mesh", "The Mesh containing the face to get the normal to.")]
        [Output("normal", "Vector normal to the given mesh face.")]
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

        [Description("Returns a list of vectors normal to each face of the mesh.")]
        [Input("mesh", "The Mesh to get the normals to.")]
        [Output("normals", "List of vectors normal to the faces of a given mesh.")]
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

        [Description("Returns a list of vectors normal to each face of the mesh.")]
        [Input("mesh", "The Mesh3D to get the normals to.")]
        [Output("normals", "List of vectors normal to the faces of a given mesh.")]
        public static List<Vector> Normals(this Mesh3D mesh)
        {
            return mesh.ToMesh().Normals();
        }


        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        [Description("Returns a vector normal to the plane of a given curve, oriented according to the right hand rule. Works only for closed, planar curves.")]
        [Input("curve", "The Polyline to get the normal to.")]
        [Input("tolerance", "Distance tolerance used in geometry processing, default set to BH.oM.Geometry.Tolerance.Distance.")]
        [Output("normal", "Vector normal to the plane of a curve.")]
        public static Vector Normal(this IPolyline curve, double tolerance = Tolerance.Distance)
        {
            if (curve == null)
            {
                Engine.Base.Compute.RecordError("Cannot compute the normal of a null curve.");
                return null;
            }
            if (!curve.IIsPlanar(tolerance))
            {
                Base.Compute.RecordError("A single normal vector is not unambiguously definable for non-planar curves.");
                return null;
            }
            else if (!curve.IIsClosed(tolerance))
            {
                Base.Compute.RecordError("A single normal vector is not unambiguously definable for open curves.");
                return null;
            }
            //Turning of check as shown to be an extreme performance burden. To be handled more long term in https://github.com/BHoM/BHoM_Engine/issues/2803
            //else if (curve.IsSelfIntersecting(tolerance))
            //    Base.Compute.RecordWarning("Input curve is self-intersecting. Resulting normal vector might be flipped.");

            List<Point> ctrlPts = curve.IControlPoints();
            Point avg = ctrlPts.Average();
            Vector normal = new Vector();

            //Get out normal, from cross products between vectors from the average point to adjacent control points on the curve
            for (int i = 0; i < ctrlPts.Count - 1; i++)
                normal += (ctrlPts[i] - avg).CrossProduct(ctrlPts[i + 1] - avg);

            if (normal.Length() < tolerance)
            {
                Base.Compute.RecordError("Couldn't calculate a normal vector of the given curve.");
                return null;
            }

            normal = normal.Normalise();

            //Check if normal needs to be flipped from the right hand rule
            if (!curve.IsClockwise(normal, tolerance))
                normal = -normal;

            return normal;

        }

        /***************************************************/

        [Description("Returns a vector normal to the plane of a given curve, oriented according to the right hand rule. Works only for closed and planar curves.")]
        [Input("curve", "The PolyCurve to get the normal to.")]
        [Input("tolerance", "Distance tolerance used in geometry processing, default set to BH.oM.Geometry.Tolerance.Distance.")]
        [Output("normal", "Vector normal to the plane of a curve.")]
        public static Vector Normal(this IPolyCurve curve, double tolerance = Tolerance.Distance)
        {
            if (curve == null)
            {
                Engine.Base.Compute.RecordError("Cannot compute the normal of a null curve.");
                return null;
            }
            List<ICurve> crvs = new List<ICurve>(curve.ISubParts());
            if (crvs.Any(x => x is NurbsCurve))
            {
                Base.Compute.RecordError("Querying normal from PolyCurves with segments of type NurbsCurve is not supported.");
                return null;
            }

            if (!curve.IIsPlanar(tolerance))
            {
                Base.Compute.RecordError("A single normal vector is not unambiguously definable for non-planar curves.");
                return null;
            }
            else if (!curve.IIsClosed(tolerance))
            {
                Base.Compute.RecordError("A single normal vector is not unambiguously definable for open curves.");
                return null;
            }
            //Turning of check as shown to be an extreme performance burden. To be handled more long term in https://github.com/BHoM/BHoM_Engine/issues/2803
            //else if (curve.IsSelfIntersecting(tolerance))     
            //    Base.Compute.RecordWarning("Input curve is self-intersecting. Resulting normal vector might be flipped.");

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
                }

                Point avg = points.Average();
                Vector normal = new Vector();

                //Get out normal, from cross products between vectors from the average point to adjecent controlpoints on the curve
                for (int i = 0; i < points.Count - 1; i++)
                    normal += (points[i] - avg).CrossProduct(points[i + 1] - avg);

                if (normal.Length() < tolerance)
                {
                    Base.Compute.RecordError("Couldn't calculate a normal vector of the given curve.");
                    return null;
                }

                normal = normal.Normalise();

                //Check if normal needs to be flipped from the right hand rule
                if (!curve.IsClockwise(normal, tolerance))
                    normal = -normal;

                return normal;
            }
        }

        /***************************************************/

        [Description("Returns a vector normal to the plane of a circle.")]
        [Input("curve", "The Circle to get the normal to.")]
        [Input("tolerance", "Distance tolerance used in geometry processing, default set to BH.oM.Geometry.Tolerance.Distance.")]
        [Output("normal", "Vector normal to the plane of a curve.")]
        public static Vector Normal(this Circle curve, double tolerance = Tolerance.Distance)
        {
            if (curve == null)
            {
                Engine.Base.Compute.RecordError("Cannot compute the normal of a null curve.");
                return null;
            }
            return curve.Normal;
        }

        /***************************************************/

        [Description("Returns a vector normal to the plane of an ellipse.")]
        [Input("curve", "The Ellipse to get the normal to.")]
        [Input("tolerance", "Distance tolerance used in geometry processing, default set to BH.oM.Geometry.Tolerance.Distance.")]
        [Output("normal", "Vector normal to the plane of a curve.")]
        public static Vector Normal(this Ellipse curve, double tolerance = Tolerance.Distance)
        {
            if (curve == null)
            {
                Engine.Base.Compute.RecordError("Cannot compute the normal of a null curve.");
                return null;
            }
            Vector normal = (curve.Axis1).CrossProduct(curve.Axis2);
            return normal;
        }

        /***************************************************/

        [Description("Returns a vector normal to the plane of an arc.")]
        [Input("curve", "The Arc to get the normal to.")]
        [Input("tolerance", "Distance tolerance used in geometry processing, default set to BH.oM.Geometry.Tolerance.Distance.")]
        [Output("normal", "Vector normal to the plane of a curve.")]
        public static Vector Normal(this Arc curve, double tolerance = Tolerance.Distance)
        {
            if (curve == null)
            {
                Engine.Base.Compute.RecordError("Cannot compute the normal of a null curve.");
                return null;
            }
            if (curve.Angle() > 0)
                return curve.CoordinateSystem.Z;
            else
                return curve.CoordinateSystem.Z.Reverse();
        }


        /***************************************************/
        /**** Public Methods - Surfaces                 ****/
        /***************************************************/

        [Description("Returns a vector normal to the planar surface.")]
        [Input("surface", "The PlanarSurface to get the normal to.")]
        [Output("normal", "Vector normal to the surface.")]
        public static Vector Normal(this PlanarSurface surface)
        {
            if (surface == null)
            {
                Engine.Base.Compute.RecordError("Cannot compute the normal of a null surface.");
                return null;
            }
            return surface.ExternalBoundary.INormal();
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        [Description("Interface method that returns a vector normal to the plane of any ICurve and oriented according to the right hand rule. Works only for closed and planar curves with an exception for single Arcs.")]
        [Input("curve", "The ICurve to get the normal to.")]
        [Input("tolerance", "Distance tolerance used in geometry processing, default set to BH.oM.Geometry.Tolerance.Distance.")]
        [Output("normal", "Vector normal to the plane of a curve.")]
        public static Vector INormal(this ICurve curve, double tolerance = Tolerance.Distance)
        {
            if (curve == null)
            {
                Engine.Base.Compute.RecordError("Cannot compute the normal of a null curve.");
                return null;
            }
            return Normal(curve as dynamic, tolerance);
        }

        /***************************************************/

        [Description("Interface method that returns the list of vectors normal to any IGeometry.")]
        [Input("geometry", "The IGeometry to get the normal to.")]
        [Output("normals", "List of vectors normal to the given geometry.")]
        public static List<Vector> INormals(this IGeometry geometry)
        {
            return Normals(geometry as dynamic);
        }


        /***************************************************/
        /**** Private Fallback Methods                  ****/
        /***************************************************/

        private static Vector Normal(this ICurve curve, double tolerance = Tolerance.Distance)
        {
            Base.Compute.RecordError($"Normal is not implemented for ICurves of type: {curve.GetType().Name}.");
            return null;
        }

        /***************************************************/

        private static List<Vector> Normals(this IGeometry geometry)
        {
            Base.Compute.RecordError($"Normals is not implemented for IGeometry of type: {geometry.GetType().Name}.");
            return null;
        }

        /***************************************************/
    }
}



