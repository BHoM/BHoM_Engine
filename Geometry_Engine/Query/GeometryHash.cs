/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2022, the respective contributors. All rights reserved.
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
using BH.oM.Base;
using BH.oM.Base.Attributes;
using BH.oM.Geometry;
using BH.oM.Geometry.CoordinateSystem;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Security.Cryptography.Xml;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /****               Public Methods              ****/
        /***************************************************/

        [Description("Returns a signature of the input geometry, useful for distance-based comparisons and diffing." +
            "\nThe hash is computed as an array representing the coordinate of significant points taken on the geometry." +
            "\nThe number of points is reduced to the minimum essential to determine uniquely any geometry." +
            "\nAdditionally, the resulting points are transformed based on the source geometry type, to remove or minimize collisions." +
            "\n(Any transformation so performed is translational only, in order to support geometrical tolerance, i.e. numerical distance, when comparing GeometryHashes downstream).")]
        [Output("geomHash", "Array of numbers representing a unique signature of the input geometry.")]
        public static double[] IGeometryHash(this IGeometry igeometry)
        {
            return GeometryHash(igeometry as dynamic, 0);
        }

        /***************************************************/
        /****              Private Methods              ****/
        /***************************************************/


        /***************************************************/
        /****  Curves                                   ****/
        /***************************************************/

        [Description("The geometry hash of a Curve is obtained by first retrieving any Sub-part of the curve, if present." +
            "The ISubParts() methods is able to return the 'primitive' curves that a curve is composed of. " +
            "The GeometryHashes are then calculated for the individual parts and concatenated.")]
        private static double[] GeometryHash(this ICurve curve, double translationFactor)
        {
            List<ICurve> subParts = curve.ISubParts().ToList();

            List<double> hashes = new List<double>();

            //Add hash ignoring endpoint for all but last curve
            for (int i = 0; i < subParts.Count - 1; i++)
            {
                hashes.AddRange(GeometryHash(subParts[i] as dynamic, translationFactor, true));
            }
            //Include endpoint for hasing for last curve
            hashes.AddRange(GeometryHash(subParts.Last() as dynamic, translationFactor, false));

            return hashes.ToArray();
        }

        /***************************************************/

        [Description("The GeometryHash for an Arc is calculated as the GeometryHash of the start, end and middle point of the Arc.")]
        private static double[] GeometryHash(this Arc curve, double translationFactor, bool skipEndPoint= false)
        {
            translationFactor += (int)TypeTranslationFactor.Arc;

            IEnumerable<double> hash = curve.StartPoint().ToDoubleArray(translationFactor)
               .Concat(curve.PointAtParameter(0.5).ToDoubleArray(translationFactor));

            if (!skipEndPoint)
                hash = hash.Concat(curve.EndPoint().ToDoubleArray(translationFactor));

            return hash.ToArray();
        }

        /***************************************************/

        [Description("The GeometryHash for an Circle is calculated as the GeometryHash of the start, 1/3rd and 2/3rd points of the Circle.")]
        private static double[] GeometryHash(this Circle curve, double translationFactor, bool skipEndPoint = false)
        {
            // The input `skipEndPoint` is not used here because Ellipses cannot be part of Polycurves.

            translationFactor += (int)TypeTranslationFactor.Circle;

            IEnumerable<double> hash = curve.StartPoint().ToDoubleArray(translationFactor)
                   .Concat(curve.PointAtParameter(0.33).ToDoubleArray(translationFactor))
                   .Concat(curve.PointAtParameter(0.66).ToDoubleArray(translationFactor));

            return hash.ToArray();
        }

        /***************************************************/

        [Description("The GeometryHash for an Ellipse is calculated as the GeometryHash of the start, 1/3rd and 2/3rd points of the Ellipse.")]
        private static double[] GeometryHash(this Ellipse curve, double translationFactor, bool skipEndPoint = false)
        {
            // The input `skipEndPoint` is not used here because Ellipses cannot be part of Polycurves.

            translationFactor += (int)TypeTranslationFactor.Ellipse;

            IEnumerable<double> hash = curve.StartPoint().ToDoubleArray(translationFactor)
               .Concat(curve.PointAtParameter(0.33).ToDoubleArray(translationFactor))
               .Concat(curve.PointAtParameter(0.66).ToDoubleArray(translationFactor));

            return hash.ToArray();
        }

        /***************************************************/

        [Description("The GeometryHash for a Line is calculated as the GeometryHash of the start and end point of the Line.")]
        private static double[] GeometryHash(this Line curve, double translationFactor, bool skipEndPoint = false)
        {
            translationFactor += (int)TypeTranslationFactor.Line;

            if (skipEndPoint)
                return curve.StartPoint().ToDoubleArray(translationFactor);

            return curve.StartPoint().ToDoubleArray(translationFactor)
               .Concat(curve.EndPoint().ToDoubleArray(translationFactor))
               .ToArray();
        }

        /***************************************************/

        [Description("The GeometryHash for a NurbsCurve is obtained by getting moving the control points " +
            "by a translation factor composed by the weights and a subarray of the knot vector. " +
            "The subarray is made by picking as many elements from the knot vector as the curve degree value.")]
        private static double[] GeometryHash(this NurbsCurve curve, double translationFactor, bool skipEndPoint = false)
        {
            // The input `skipEndPoint` is not used here because Nurbs may well extend or end before the last ControlPoint.
            // Also consider complex situations like Periodic curves.

            int curveDegree = curve.Degree();

            if (curveDegree == 1)
                return BH.Engine.Geometry.Create.Polyline(curve.ControlPoints).GeometryHash(translationFactor);

            translationFactor += (int)TypeTranslationFactor.NurbsCurve;

            List<double> concatenated = new List<double>();

            int controlPointsCount = curve.ControlPoints.Count();

            for (int i = 0; i < controlPointsCount; i++)
            {
                double sum = curve.Knots.GetRange(i, curveDegree).Sum();
                double[] doubles = curve.ControlPoints[i].GeometryHash(sum + curve.Weights[i] + translationFactor);
                concatenated.AddRange(doubles);
            }

            return concatenated.ToArray();

            // Simpler & faster but potentially less reliable:
            // return curve.Knots.Take(pointWeights.Count())
            //    .Zip(curve.ControlPoints, (k, p) => new double[]{ p.X + k, p.Y + k, p.Z + k })
            //    .SelectMany(arr => arr).ToArray();
        }


        /***************************************************/
        /****  Surfaces                                 ****/
        /***************************************************/

        private static double[] GeometryHash(this ISurface obj, double translationFactor)
        {
            return GeometryHash(obj as dynamic, translationFactor);
        }

        /***************************************************/

        [Description("The GeometryHash for a PlanarSurface is calculated as the GeometryHash of the External and Internal boundary curves, then concatenated.")]
        private static double[] GeometryHash(this PlanarSurface obj, double translationFactor)
        {
            translationFactor += (int)TypeTranslationFactor.PlanarSurface;

            return obj.ExternalBoundary.GeometryHash(translationFactor)
                .Concat(obj.InternalBoundaries.SelectMany(ib => ib.GeometryHash(translationFactor))).ToArray();
        }

        /***************************************************/

        [Description("The GeometryHash for an Extrusion is calculated by translating the extrusion curve with the extrusion direction vector." +
            "A first GeometryHash is calculated for this translated curve. " +
            "Then, the GeometryHash of the (non-translated) extrusion curve is concatenated to the first hash to make it more reliable.")]
        private static double[] GeometryHash(this Extrusion obj, double translationFactor)
        {
            translationFactor += (int)TypeTranslationFactor.Extrusion;

            return obj.Curve.ITranslate(obj.Direction).GeometryHash(translationFactor)
                .Concat(obj.Curve.GeometryHash(translationFactor)).ToArray();
        }

        /***************************************************/

        [Description("The GeometryHash for a Loft is calculated as the GeometryHash of its curves.")]
        private static double[] GeometryHash(this Loft obj, double translationFactor)
        {
            translationFactor += (int)TypeTranslationFactor.Loft;

            return obj.Curves.GeometryHash(translationFactor);
        }

        /***************************************************/

        [Description("Moving control points by a translation factor composed by the weights " +
          "and a subarray of the knot vector. " +
          "The subarray is made by picking as many elements from the knot vector as the curve degree value.")]
        private static double[] GeometryHash(this NurbsSurface obj, double translationFactor)
        {
            translationFactor += (int)TypeTranslationFactor.NurbsSurface;

            List<int> uv = obj.UVCount();

            List<double> uKnots = obj.UKnots.ToList();
            List<double> vKnots = obj.VKnots.ToList();

            List<double> concatenated = new List<double>();
            for (int i = 0; i < uv[0]; i++)
            {
                double uSum = uKnots.GetRange(i, obj.UDegree).Sum();
                for (int j = 0; j < uv[1]; j++)
                {
                    int ptIndex = i * uv[1] + j;
                    double vSum = vKnots.GetRange(j, obj.VDegree).Sum();
                    double[] doubles = obj.ControlPoints[ptIndex].GeometryHash(uSum + vSum + obj.Weights[ptIndex] + translationFactor);
                    concatenated.AddRange(doubles);
                }
            }

            return concatenated
                .Concat(obj.InnerTrims.SelectMany(it => it.GeometryHash(translationFactor)))
                .Concat(obj.OuterTrims.SelectMany(it => it.GeometryHash(translationFactor))).ToArray();
        }

        /***************************************************/

        [Description("The GeometryHash for a Pipe is calculated as the GeometryHash of its centreline translated by its radius," +
            "then concatenated with the GeometryHash of its centreline's StartPoint for extra reliability.")]
        private static double[] GeometryHash(this Pipe obj, double translationFactor)
        {
            translationFactor += (int)TypeTranslationFactor.Pipe;

            double[] result = obj.Centreline.GeometryHash(translationFactor + obj.Radius);

            if (obj.Capped)
                result.Concat(obj.Centreline.StartPoint().GeometryHash(translationFactor + obj.Radius));

            return result;
        }

        /***************************************************/

        [Description("The GeometryHash for a PolySurface is calculated as the GeometryHash of the individual surfaces.")]
        private static double[] GeometryHash(this PolySurface obj, double translationFactor)
        {
            return obj.Surfaces.SelectMany(s => s.GeometryHash(translationFactor)).ToArray();
        }

        /***************************************************/

        [Description("The GeometryHash for a SurfaceTrim is calculated as the GeometryHash of its Curve3d and Curve2d, concatenated.")]
        private static double[] GeometryHash(this SurfaceTrim obj, double translationFactor)
        {
            translationFactor += (int)TypeTranslationFactor.SurfaceTrim;

            // We only consider the Curve3D in order to avoid being redundant with the Curve2D,
            // and allow distancing comparisons.

            return obj.Curve3d.GeometryHash(translationFactor).ToArray();
        }


        /***************************************************/
        /****  Mesh                                     ****/
        /***************************************************/

        [Description("The GeometryHash for a Mesh is obtained by getting the number of faces that are attached to each control point, " +
            "and use that count as a translation factor for control points.")]
        private static double[] GeometryHash(this Mesh obj, double translationFactor)
        {
            translationFactor += (int)TypeTranslationFactor.Mesh;

            var dic = new Dictionary<int, int>();

            for (int i = 0; i < obj.Faces.Count; i++)
            {
                foreach (var faceIndex in obj.Faces[i].FaceIndices())
                {
                    if (dic.ContainsKey(faceIndex))
                        dic[faceIndex] += i;
                    else
                        dic[faceIndex] = i;
                }
            }

            List<double> result = new List<double>();

            for (int i = 0; i < obj.Vertices.Count; i++)
            {
                int pointTranslationFactor;
                if (!dic.TryGetValue(i, out pointTranslationFactor))
                    pointTranslationFactor = 0;

                result.AddRange(obj.Vertices[i].ToDoubleArray(pointTranslationFactor + translationFactor));
            }

            return result.ToArray();
        }

        /***************************************************/

        [Description("The GeometryHash for a Mesh3D is obtained by getting the number of faces that are attached to each control point, " +
            "and using that count as a translation factor for control points.")]
        private static double[] GeometryHash(this Mesh3D obj, double translationFactor)
        {
            translationFactor += (int)TypeTranslationFactor.Mesh3D;

            var dic = new Dictionary<int, int>();

            for (int i = 0; i < obj.Faces.Count; i++)
            {
                foreach (var faceIndex in obj.Faces[i].FaceIndices())
                {
                    if (dic.ContainsKey(faceIndex))
                        dic[faceIndex] += i;
                    else
                        dic[faceIndex] = i;
                }
            }

            List<double> result = new List<double>();

            for (int i = 0; i < obj.Vertices.Count; i++)
            {
                int pointTranslationFactor;
                if (!dic.TryGetValue(i, out pointTranslationFactor))
                    pointTranslationFactor = 0;

                result.AddRange(obj.Vertices[i].ToDoubleArray(pointTranslationFactor + translationFactor));
            }

            return result.ToArray();
        }


        /***************************************************/
        /****  Vector                                   ****/
        /***************************************************/

        [Description("The GeometryHash for a Point is simply an array of 3 numbers composed by the Point X, Y and Z coordinates.")]
        private static double[] GeometryHash(this Point obj, double translationFactor)
        {
            return obj.ToDoubleArray(translationFactor);
        }


        /***************************************************/
        /****  Other methods                            ****/
        /***************************************************/

        // Fallback
        private static double[] GeometryHash(this object obj, double translationFactor)
        {
            object extensionMethodResult = null;
            if (BH.Engine.Base.Compute.TryRunExtensionMethod(obj, "GeometryHash", out extensionMethodResult))
                return (double[])extensionMethodResult;

            BH.Engine.Base.Compute.RecordError($"Could not find a {nameof(GeometryHash)} method for type {obj.GetType().FullName}.");

            return new double[] { };
        }

        /***************************************************/

        private static double[] ToDoubleArray(this Point p, double typeTranslationFactor)
        {
            return new double[]
            {
                p.X + typeTranslationFactor,
                p.Y + typeTranslationFactor,
                p.Z + typeTranslationFactor
            };
        }

        /***************************************************/

        private static double[] ToDoubleArray(this IEnumerable<Point> points, double typeTranslationFactor)
        {
            return points.SelectMany(p => p.ToDoubleArray(typeTranslationFactor)).ToArray();
        }


        /***************************************************/
        /****  Private enum                             ****/
        /***************************************************/

        [Description("Multiplier used to define the TypeTranslationFactors.")]
        private const int m_ToleranceMultiplier = (int)(1e9 * Tolerance.Distance);

        [Description("Translation factors per each type of geometry." +
            "The translation is proportional ")]
        private enum TypeTranslationFactor
        {
            Point = 0,
            Plane = 1 * m_ToleranceMultiplier,
            Line = 2 * m_ToleranceMultiplier,
            Arc = 3 * m_ToleranceMultiplier,
            Circle = 4 * m_ToleranceMultiplier,
            Ellipse = 5 * m_ToleranceMultiplier,
            NurbsCurve = 6 * m_ToleranceMultiplier,
            PlanarSurface = 7 * m_ToleranceMultiplier,
            Pipe = 8 * m_ToleranceMultiplier,
            Extrusion = 9 * m_ToleranceMultiplier,
            Loft = 10 * m_ToleranceMultiplier,
            NurbsSurface = 11 * m_ToleranceMultiplier,
            SurfaceTrim = 12 * m_ToleranceMultiplier,
            Mesh = 13 * m_ToleranceMultiplier,
            Mesh3D = 14 * m_ToleranceMultiplier
        }
    }
}



