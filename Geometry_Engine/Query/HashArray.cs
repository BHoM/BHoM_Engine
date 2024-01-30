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

using BH.Engine.Base;
using BH.oM.Base;
using BH.oM.Base.Attributes;
using BH.oM.Geometry;
using BH.oM.Geometry.CoordinateSystem;
using System;
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
        [Input("igeometry", "Geometry you want to compute the hash array for.")]
        [Input("comparisonConfig", "Configurations on how the hash array is computed, with options for numerical approximation, type exceptions and many others.")]
        [Output("hashArray", "Array of numbers representing a unique signature of the input geometry.")]
        public static double[] IHashArray(this IGeometry igeometry, BaseComparisonConfig comparisonConfig = null)
        {
            return HashArray(igeometry as dynamic, 0, comparisonConfig, null);
        }

        /***************************************************/

        [Description("Returns a signature of the input geometry, useful for distance-based comparisons and diffing." +
            "\nThe hash is computed as an array representing the coordinate of significant points taken on the geometry." +
            "\nThe number of points is reduced to the minimum essential to determine uniquely any geometry." +
            "\nAdditionally, the resulting points are transformed based on the source geometry type, to remove or minimize collisions." +
            "\n(Any transformation so performed is translational only, in order to support geometrical tolerance, i.e. numerical distance, when comparing GeometryHashes downstream).")]
        [Input("igeometry", "Geometry you want to compute the hash array for.")]
        [Input("comparisonConfig", "Configurations on how the hash array is computed, with options for numerical approximation, type exceptions and many others.")]
        [Input("fullName", "Name of the property that holds the target object to calculate the hash array for. This name will be used to seek any matching custom configuration to apply against the `comparisonConfig` input.")]
        [Output("hashArray", "Array of numbers representing a unique signature of the input geometry.")]
        public static double[] IHashArray(this IGeometry igeometry, BaseComparisonConfig comparisonConfig, string fullName = null)
        {
            if (igeometry == null)
                return new double[] { };

            fullName = fullName ?? igeometry.GetType().FullName;

            return HashArray(igeometry as dynamic, 0, comparisonConfig, fullName);
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
        private static double[] HashArray(this ICurve curve, double translationFactor, BaseComparisonConfig comparisonConfig = null, string fullName = null)
        {
            if (comparisonConfig?.TypeExceptions?.Any(t => typeof(ICurve).IsAssignableFrom(t)) ?? false)
                return default;

            if (curve == null)
                return default;

            List<ICurve> subParts = curve.ISubParts().ToList();

            if (!subParts.Any())
                if (curve is Polyline)
                    return HashArray(curve.IControlPoints(),
                        translationFactor,
                        comparisonConfig: comparisonConfig,
                        fullName: fullName.AppendPropertyName($"{nameof(ControlPoints)}"));
                else
                    return HashArray(curve as dynamic,
                        translationFactor,
                        skipEndPoint: false,
                        comparisonConfig: comparisonConfig,
                        fullName: fullName);

            List<double> hashes = new List<double>();

            //Add hash ignoring endpoint for all but last curve
            for (int i = 0; i < subParts.Count - 1; i++)
            {
                hashes.AddRange(HashArray(subParts[i] as dynamic,
                    translationFactor,
                    skipEndPoint: true,
                    comparisonConfig: comparisonConfig,
                    fullName: fullName.AppendPropertyName($"[{i}]"))
                );
            }

            //Include endpoint for hashing for last curve
            hashes.AddRange(HashArray(subParts.Last() as dynamic,
                translationFactor,
                skipEndPoint: false,
                comparisonConfig: comparisonConfig,
                fullName: fullName.AppendPropertyName($"[{subParts.Count - 1}]")
                )
            );

            return hashes.ToArray();
        }

        /***************************************************/

        [Description("The GeometryHash for an Arc is calculated as the GeometryHash of the start, end and middle point of the Arc.")]
        private static double[] HashArray(this Arc curve, double translationFactor, bool skipEndPoint = false, BaseComparisonConfig comparisonConfig = null, string fullName = null)
        {
            if (comparisonConfig?.TypeExceptions?.Any(t => typeof(ICurve).IsAssignableFrom(t)) ?? false)
                return default;

            translationFactor += (int)TypeTranslationFactor.Arc;

            IEnumerable<double> hash = curve.StartPoint().HashArray(translationFactor,
                    comparisonConfig,
                    fullName.AppendPropertyName($"{fullName}.{nameof(StartPoint)}"))
               .Concat(curve.PointAtParameter(0.5).HashArray(translationFactor,
                    comparisonConfig,
                    fullName.AppendPropertyName($"{nameof(PointAtParameter)}(5e-1)"))
               );

            if (!skipEndPoint)
                hash = hash.Concat(curve.EndPoint().HashArray(translationFactor, comparisonConfig, fullName.AppendPropertyName($"{nameof(EndPoint)}")));

            return hash.ToArray();
        }

        /***************************************************/

        [Description("The GeometryHash for an Circle is calculated as the GeometryHash of the start, 1/3rd and 2/3rd points of the Circle.")]
        private static double[] HashArray(this Circle curve, double translationFactor, BaseComparisonConfig comparisonConfig, string fullName = null)
        {
            if (comparisonConfig?.TypeExceptions?.Any(t => typeof(Circle).IsAssignableFrom(t)) ?? false)
                return default;

            // The input `skipEndPoint` is not used here because Circles do not have a clearly defined endpoint to be used in a chain of segment curves.

            translationFactor += (int)TypeTranslationFactor.Circle;

            IEnumerable<double> hash = curve.StartPoint().HashArray(translationFactor,
                        comparisonConfig,
                        fullName.AppendPropertyName($"{nameof(StartPoint)}"))
                   .Concat(curve.PointAtParameter(0.33).HashArray(translationFactor,
                        comparisonConfig,
                        fullName.AppendPropertyName($"{nameof(PointAtParameter)}(33e-2)"))
                   ).Concat(curve.PointAtParameter(0.66).HashArray(translationFactor,
                        comparisonConfig,
                        fullName.AppendPropertyName($"{nameof(PointAtParameter)}(66e-2)"))
                   );

            return hash.ToArray();
        }

        /***************************************************/

        [Description("The GeometryHash for an Ellipse is calculated as the GeometryHash of the start, 1/3rd and 2/3rd points of the Ellipse.")]
        private static double[] HashArray(this Ellipse curve, double translationFactor, BaseComparisonConfig comparisonConfig, string fullName = null)
        {
            if (comparisonConfig?.TypeExceptions?.Any(t => typeof(Ellipse).IsAssignableFrom(t)) ?? false)
                return default;

            // The input `skipEndPoint` is not used here because Ellipses do not have a clearly defined endpoint to be used in a chain of segment curves.

            translationFactor += (int)TypeTranslationFactor.Ellipse;

            IEnumerable<double> hash = curve.StartPoint().HashArray(translationFactor,
                    comparisonConfig,
                    fullName.AppendPropertyName($"{nameof(StartPoint)}"))
               .Concat(curve.PointAtParameter(0.33).HashArray(translationFactor,
                    comparisonConfig,
                    fullName.AppendPropertyName($"{nameof(PointAtParameter)}(33e-2)"))
               ).Concat(curve.PointAtParameter(0.66).HashArray(translationFactor,
                    comparisonConfig,
                    fullName.AppendPropertyName($"{nameof(PointAtParameter)}(66e-2)"))
               );

            return hash.ToArray();
        }

        /***************************************************/

        [Description("The GeometryHash for a Line is calculated as the GeometryHash of the start and end point of the Line.")]
        private static double[] HashArray(this Line curve, double translationFactor, BaseComparisonConfig comparisonConfig, bool skipEndPoint = false, string fullName = null)
        {
            if (comparisonConfig?.TypeExceptions?.Any(t => typeof(Line).IsAssignableFrom(t)) ?? false)
                return default;

            translationFactor += (int)TypeTranslationFactor.Line;

            if (skipEndPoint)
                return curve.StartPoint().HashArray(translationFactor,
                    comparisonConfig,
                    fullName.AppendPropertyName($"{nameof(StartPoint)}"));

            return curve.StartPoint().HashArray(translationFactor,
                    comparisonConfig,
                    fullName.AppendPropertyName($"{nameof(StartPoint)}"))
               .Concat(curve.EndPoint().HashArray(translationFactor,
                    comparisonConfig,
                    fullName.AppendPropertyName($"{nameof(EndPoint)}"))
               ).ToArray();
        }

        /***************************************************/

        [Description("The GeometryHash for a NurbsCurve is obtained by getting moving the control points " +
            "by a translation factor composed by the weights and a subarray of the knot vector. " +
            "The subarray is made by picking as many elements from the knot vector as the curve degree value.")]
        private static double[] HashArray(this NurbsCurve curve, double translationFactor, BaseComparisonConfig comparisonConfig, string fullName = null)
        {
            if (comparisonConfig?.TypeExceptions?.Any(t => typeof(NurbsCurve).IsAssignableFrom(t)) ?? false)
                return default;

            // The input `skipEndPoint` is not used here because Nurbs may well extend or end before the last ControlPoint.
            // Also consider complex situations like Periodic curves.

            int curveDegree = Math.Abs(curve.Degree());

            if (curveDegree == 1)
                return BH.Engine.Geometry.Create.Polyline(curve.ControlPoints).HashArray(translationFactor,
                    comparisonConfig,
                    fullName.AppendPropertyName($"{nameof(curve.ControlPoints)}"));

            translationFactor += (int)TypeTranslationFactor.NurbsCurve;

            List<double> concatenated = new List<double>();

            int controlPointsCount = curve.ControlPoints.Count();

            for (int i = 0; i < controlPointsCount; i++)
            {
                // Use the sum of the knots plus the i-Weight to obtain an unique traslation factor.
                double knotsSum = 0;
                if (i < curve.Knots.Count - 1 && curveDegree < curve.Knots.Count - 1)
                    knotsSum = curve.Knots.GetRange(Math.Min(i, curve.Knots.Count), Math.Min(curve.Knots.Count - 1 - i, curveDegree)).Sum();

                double[] doubles = curve.ControlPoints[i].HashArray(knotsSum + curve.Weights.ElementAtOrDefault(i) + translationFactor,
                    comparisonConfig,
                    fullName.AppendPropertyName($"{nameof(curve.ControlPoints)}[{i}]"));
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

        private static double[] HashArray(this ISurface obj, double translationFactor, BaseComparisonConfig comparisonConfig, string fullName = null)
        {
            if (comparisonConfig?.TypeExceptions?.Any(t => typeof(ISurface).IsAssignableFrom(t)) ?? false)
                return default;

            return HashArray(obj as dynamic, translationFactor, comparisonConfig, fullName);
        }

        /***************************************************/

        [Description("The GeometryHash for a PlanarSurface is calculated as the GeometryHash of the External and Internal boundary curves, then concatenated.")]
        private static double[] HashArray(this PlanarSurface obj, double translationFactor, BaseComparisonConfig comparisonConfig, string fullName = null)
        {
            if (comparisonConfig?.TypeExceptions?.Any(t => typeof(PlanarSurface).IsAssignableFrom(t)) ?? false)
                return default;

            translationFactor += (int)TypeTranslationFactor.PlanarSurface;

            return obj.ExternalBoundary.HashArray(translationFactor,
                    comparisonConfig,
                    fullName.AppendPropertyName($"{nameof(obj.ExternalBoundary)}"))
                .Concat(obj.InternalBoundaries.SelectMany((ib, i) => ib.HashArray(translationFactor,
                    comparisonConfig,
                    fullName.AppendPropertyName($"{nameof(obj.InternalBoundaries)}[{i}]"))
                )).ToArray();
        }

        /***************************************************/

        [Description("The GeometryHash for an Extrusion is calculated by translating the extrusion curve with the extrusion direction vector." +
            "A first GeometryHash is calculated for this translated curve. " +
            "Then, the GeometryHash of the (non-translated) extrusion curve is concatenated to the first hash to make it more reliable.")]
        private static double[] HashArray(this Extrusion obj, double translationFactor, BaseComparisonConfig comparisonConfig, string fullName = null)
        {
            if (comparisonConfig?.TypeExceptions?.Any(t => typeof(Extrusion).IsAssignableFrom(t)) ?? false)
                return default;

            translationFactor += (int)TypeTranslationFactor.Extrusion;

            return obj.Curve.ITranslate(obj.Direction).HashArray(translationFactor,
                    comparisonConfig,
                    fullName.AppendPropertyName($"{nameof(obj.Curve)}"))
                .Concat(obj.Curve.HashArray(translationFactor,
                    comparisonConfig,
                    fullName.AppendPropertyName($"{nameof(obj.Curve)}"))
                ).ToArray();
        }

        /***************************************************/

        [Description("The GeometryHash for a Loft is calculated as the GeometryHash of its curves.")]
        private static double[] HashArray(this Loft obj, double translationFactor, BaseComparisonConfig comparisonConfig, string fullName = null)
        {
            if (comparisonConfig?.TypeExceptions?.Any(t => typeof(Loft).IsAssignableFrom(t)) ?? false)
                return default;

            translationFactor += (int)TypeTranslationFactor.Loft;

            return obj.Curves.SelectMany((c, i) =>
                c.HashArray(translationFactor,
                    comparisonConfig,
                    fullName.AppendPropertyName($"{nameof(obj.Curves)}[{i}]")
                    )
                ).ToArray();
        }

        /***************************************************/

        [Description("Moving control points by a translation factor composed by the weights " +
          "and a subarray of the knot vector. " +
          "The subarray is made by picking as many elements from the knot vector as the curve degree value.")]
        private static double[] HashArray(this NurbsSurface obj, double translationFactor, BaseComparisonConfig comparisonConfig, string fullName = null)
        {
            if (comparisonConfig?.TypeExceptions?.Any(t => typeof(NurbsSurface).IsAssignableFrom(t)) ?? false)
                return default;

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
                    double[] doubles = obj.ControlPoints[ptIndex].HashArray(uSum + vSum + obj.Weights[ptIndex] + translationFactor,
                        comparisonConfig,
                        fullName.AppendPropertyName($"{nameof(obj.ControlPoints)}[{ptIndex}]"));
                    concatenated.AddRange(doubles);
                }
            }

            return concatenated
                .Concat(obj.InnerTrims.SelectMany((it, i) =>
                    it.HashArray(translationFactor,
                        comparisonConfig,
                        fullName.AppendPropertyName($"{nameof(obj.InnerTrims)}[{i}]"))
                    )
                )
                .Concat(obj.OuterTrims.SelectMany((it, i) =>
                    it.HashArray(translationFactor,
                    comparisonConfig,
                    fullName.AppendPropertyName($"{nameof(obj.OuterTrims)}[{i}]"))
                    )
                ).ToArray();
        }

        /***************************************************/

        [Description("The GeometryHash for a Pipe is calculated as the GeometryHash of its centreline translated by its radius," +
            "then concatenated with the GeometryHash of its centreline's StartPoint for extra reliability.")]
        private static double[] HashArray(this Pipe obj, double translationFactor, BaseComparisonConfig comparisonConfig, string fullName = null)
        {
            if (comparisonConfig?.TypeExceptions?.Any(t => typeof(Pipe).IsAssignableFrom(t)) ?? false)
                return default;

            translationFactor += (int)TypeTranslationFactor.Pipe;

            double[] result = obj.Centreline.HashArray(translationFactor + obj.Radius, comparisonConfig, fullName.AppendPropertyName($"{nameof(obj.Centreline)}"));

            if (obj.Capped)
                result.Concat(
                    obj.Centreline.IStartPoint().HashArray(translationFactor + obj.Radius,
                        comparisonConfig,
                        fullName.AppendPropertyName($"{nameof(obj.Centreline)}.{nameof(StartPoint)}")) // StartPoint's method name must not be passed as IStartPoint here.
                    );

            return result;
        }

        /***************************************************/

        [Description("The GeometryHash for a PolySurface is calculated as the GeometryHash of the individual surfaces.")]
        private static double[] HashArray(this PolySurface obj, double translationFactor, BaseComparisonConfig comparisonConfig, string fullName = null)
        {
            if (comparisonConfig?.TypeExceptions?.Any(t => typeof(PolySurface).IsAssignableFrom(t)) ?? false)
                return default;

            return obj.Surfaces.SelectMany((s, i) =>
                s.HashArray(translationFactor,
                    comparisonConfig,
                    fullName.AppendPropertyName($"{nameof(obj.Surfaces)}[{i}]"))
                ).ToArray();
        }

        /***************************************************/

        [Description("The GeometryHash for a SurfaceTrim is calculated as the GeometryHash of its Curve3d.")]
        private static double[] HashArray(this SurfaceTrim obj, double translationFactor, BaseComparisonConfig comparisonConfig, string fullName = null)
        {
            if (comparisonConfig?.TypeExceptions?.Any(t => typeof(SurfaceTrim).IsAssignableFrom(t)) ?? false)
                return default;

            translationFactor += (int)TypeTranslationFactor.SurfaceTrim;

            // We only consider the Curve3D in order to avoid being redundant with the Curve2D,
            // and allow distancing comparisons.

            return obj.Curve3d.HashArray(translationFactor,
                comparisonConfig,
                fullName.AppendPropertyName($"{nameof(obj.Curve3d)}")).ToArray();
        }


        /***************************************************/
        /****  Mesh                                     ****/
        /***************************************************/

        [Description("The GeometryHash for a Mesh is obtained by getting the number of faces that are attached to each control point, " +
            "and use that count as a translation factor for control points.")]
        private static double[] HashArray(this Mesh obj, double translationFactor, BaseComparisonConfig comparisonConfig, string fullName = null)
        {
            if (comparisonConfig?.TypeExceptions?.Any(t => typeof(Mesh).IsAssignableFrom(t)) ?? false)
                return default;

            translationFactor += (int)TypeTranslationFactor.Mesh;

            var dic = new Dictionary<int, int>();
            List<double> result = new List<double>();

            if (!comparisonConfig?.TypeExceptions?.Any(t => typeof(Face).IsAssignableFrom(t)) ?? true)
                for (int i = 0; i < obj.Faces.Count; i++)
                {
                    // If Points are excluded from the HashArray, include at least "topological" information i.e. the faces
                    if (comparisonConfig?.TypeExceptions?.Any(t => typeof(Point).IsAssignableFrom(t)) ?? false)
                        result.AddRange(obj.Faces[i].FaceIndices().Select<int, double>(n => n));

                    foreach (var faceIndex in obj.Faces[i].FaceIndices())
                    {
                        if (dic.ContainsKey(faceIndex))
                            dic[faceIndex] += i;
                        else
                            dic[faceIndex] = i;
                    }
                }

            for (int i = 0; i < obj.Vertices.Count; i++)
            {
                int pointTranslationFactor;
                if (!dic.TryGetValue(i, out pointTranslationFactor))
                    pointTranslationFactor = 0;

                result.AddRange(
                    obj.Vertices[i].HashArray(
                        pointTranslationFactor + translationFactor,
                        comparisonConfig,
                        fullName.AppendPropertyName($"{nameof(obj.Vertices)}[{i}]")
                        ) ?? new double[0]
                    );
            }

            return result.ToArray();
        }

        /***************************************************/

        [Description("The GeometryHash for a Mesh3D is obtained by getting the number of faces that are attached to each control point, " +
            "and using that count as a translation factor for control points.")]
        private static double[] HashArray(this Mesh3D obj, double translationFactor, BaseComparisonConfig comparisonConfig, string fullName = null)
        {
            if (comparisonConfig?.TypeExceptions?.Any(t => typeof(Mesh3D).IsAssignableFrom(t)) ?? false)
                return default;

            translationFactor += (int)TypeTranslationFactor.Mesh3D;

            var dic = new Dictionary<int, int>();
            List<double> result = new List<double>();

            if (!comparisonConfig?.TypeExceptions?.Any(t => typeof(Face).IsAssignableFrom(t)) ?? true)
                for (int i = 0; i < obj.Faces.Count; i++)
                {
                    // If Points are excluded from the HashArray, include at least "topological" information i.e. the faces
                    if (comparisonConfig?.TypeExceptions?.Any(t => typeof(Point).IsAssignableFrom(t)) ?? false)
                        result.AddRange(obj.Faces[i].FaceIndices().Select<int, double>(n => n));

                    foreach (var faceIndex in obj.Faces[i].FaceIndices())
                    {
                        if (dic.ContainsKey(faceIndex))
                            dic[faceIndex] += i;
                        else
                            dic[faceIndex] = i;
                    }
                }

            for (int i = 0; i < obj.Vertices.Count; i++)
            {
                int pointTranslationFactor;
                if (!dic.TryGetValue(i, out pointTranslationFactor))
                    pointTranslationFactor = 0;

                result.AddRange(
                    obj.Vertices[i].HashArray(
                        pointTranslationFactor + translationFactor,
                        comparisonConfig,
                        fullName.AppendPropertyName($"{nameof(obj.Vertices)}[{i}]")
                        ) ?? new double[0]
                    );
            }

            return result.ToArray();
        }

        /***************************************************/

        private static double[] HashArray(this IEnumerable<Point> points, double typeTranslationFactor, BaseComparisonConfig comparisonConfig, string fullName = null)
        {
            if (comparisonConfig?.TypeExceptions?.Any(t => typeof(IEnumerable<Point>).IsAssignableFrom(t)) ?? false)
                return default;

            return points.SelectMany((p, i) => p.HashArray(typeTranslationFactor, comparisonConfig, fullName == null ? null : $"{fullName}[{i}]")).ToArray();
        }

        /***************************************************/

        [Description("The GeometryHash for a CompositeGeometry is given as the concatenated GeometryHash of the single elements composing it.")]
        private static double[] HashArray(this CompositeGeometry obj, double translationFactor, BaseComparisonConfig comparisonConfig, string fullName = null)
        {
            if (comparisonConfig?.TypeExceptions?.Any(t => typeof(CompositeGeometry).IsAssignableFrom(t)) ?? false)
                return default;

            return obj.Elements.SelectMany((c, i) => c.IHashArray(comparisonConfig, fullName.AppendPropertyName($"{nameof(obj.Elements)}[{i}]"))).ToArray();
        }

        /***************************************************/

        [Description("The GeometryHash for a Point is simply an array of 3 numbers composed by the Point X, Y and Z coordinates.")]
        private static double[] HashArray(this Point p, double translationFactor, BaseComparisonConfig comparisonConfig, string fullName = null)
        {
            if (comparisonConfig?.TypeExceptions?.Any(t => typeof(Point).IsAssignableFrom(t)) ?? false)
                return default;

            if (comparisonConfig == null)
                return new double[]
                {
                    p.X + translationFactor,
                    p.Y + translationFactor,
                    p.Z + translationFactor
                };

            return new double[]
            {
                (p.X + translationFactor).ValueToInclude(fullName.AppendPropertyName($"{nameof(p.X)}"), comparisonConfig),
                (p.Y + translationFactor).ValueToInclude(fullName.AppendPropertyName($"{nameof(p.Y)}"), comparisonConfig),
                (p.Z + translationFactor).ValueToInclude(fullName.AppendPropertyName($"{nameof(p.Z)}"), comparisonConfig)
            };
        }

        /***************************************************/

        // Fallback
        private static double[] HashArray(this object obj, double translationFactor, BaseComparisonConfig comparisonConfig, string fullName = null)
        {
            throw new NotImplementedException($"Could not find a {nameof(HashArray)} method for type {obj.GetType().FullName}.");
        }

        /***************************************************/
        /****  Other methods for "conceptual" geometry  ****/
        /***************************************************/

        [Description("The GeometryHash for a Vector is given as the concatenated GeometryHash of the single elements composing it.")]
        private static double[] HashArray(this Vector obj, double translationFactor, BaseComparisonConfig comparisonConfig, string fullName = null)
        {
            if (comparisonConfig?.TypeExceptions?.Any(t => typeof(Vector).IsAssignableFrom(t)) ?? false)
                return default;

            if (translationFactor == (double)TypeTranslationFactor.Point)
                translationFactor = (double)TypeTranslationFactor.Vector;

            if (comparisonConfig == null)
                return new double[]
                {
                    obj.X + translationFactor,
                    obj.Y + translationFactor,
                    obj.Z + translationFactor
                };

            return new double[]
            {
                (obj.X + translationFactor).ValueToInclude(fullName.AppendPropertyName($"{nameof(obj.X)}"), comparisonConfig),
                (obj.Y + translationFactor).ValueToInclude(fullName.AppendPropertyName($"{nameof(obj.Y)}"), comparisonConfig),
                (obj.Z + translationFactor).ValueToInclude(fullName.AppendPropertyName($"{nameof(obj.Z)}"), comparisonConfig)
            };
        }

        /***************************************************/

        [Description("The GeometryHash for a Basis is given as the concatenated GeometryHash of the single elements composing it.")]
        private static double[] HashArray(this Basis obj, double translationFactor, BaseComparisonConfig comparisonConfig, string fullName = null)
        {
            if (comparisonConfig?.TypeExceptions?.Any(t => typeof(Basis).IsAssignableFrom(t)) ?? false)
                return default;

            translationFactor = (double)TypeTranslationFactor.Basis;

            var x = obj.X.HashArray(translationFactor, comparisonConfig, fullName.AppendPropertyName($"{nameof(obj.X)}"));
            var y = obj.Y.HashArray(translationFactor, comparisonConfig, fullName.AppendPropertyName($"{nameof(obj.Y)}"));
            var z = obj.Z.HashArray(translationFactor, comparisonConfig, fullName.AppendPropertyName($"{nameof(obj.Z)}"));
            return x.Concat(y).Concat(z).ToArray();
        }

        /***************************************************/

        [Description("The GeometryHash for a Basis is given as the concatenated GeometryHash of the single elements composing it.")]
        private static double[] HashArray(this Cartesian obj, double translationFactor, BaseComparisonConfig comparisonConfig, string fullName = null)
        {
            if (comparisonConfig?.TypeExceptions?.Any(t => typeof(Cartesian).IsAssignableFrom(t)) ?? false)
                return default;

            translationFactor = (double)TypeTranslationFactor.Cartesian;

            var x = obj.X.HashArray(translationFactor, comparisonConfig, fullName.AppendPropertyName($"{nameof(obj.X)}"));
            var y = obj.Y.HashArray(translationFactor, comparisonConfig, fullName.AppendPropertyName($"{nameof(obj.Y)}"));
            var z = obj.Z.HashArray(translationFactor, comparisonConfig, fullName.AppendPropertyName($"{nameof(obj.Z)}"));
            var o = obj.Origin.HashArray(translationFactor, comparisonConfig, fullName.AppendPropertyName($"{nameof(obj.Origin)}"));

            return x.Concat(y).Concat(z).Concat(o).ToArray();
        }

        /***************************************************/

        [Description("The GeometryHash for a TransformMatrix is given as the concatenated numbers of the matrix.")]
        private static double[] HashArray(this TransformMatrix obj, double translationFactor, BaseComparisonConfig comparisonConfig, string fullName = null)
        {
            if (comparisonConfig?.TypeExceptions?.Any(t => typeof(TransformMatrix).IsAssignableFrom(t)) ?? false)
                return default;

            if (comparisonConfig == null)
                return obj.Matrix.Cast<double>().ToArray();

            return obj.Matrix.Cast<double>()
                .Select(n => n.ValueToInclude(fullName.AppendPropertyName($"{nameof(obj.Matrix)}"), comparisonConfig))
                .ToArray();
        }

        /***************************************************/
        /****  Other private methods                    ****/
        /***************************************************/

        [Description("The GeometryHash for a TransformMatrix is given as the concatenated numbers of the matrix.")]
        private static string AppendPropertyName(this string fullName, string toAppend)
        {
            if (fullName == null)
                return fullName;

            return $"{fullName}.{toAppend}";
        }

        /***************************************************/

        [Description("Determine whether a certain object property should be included in the Hash computation. " +
            "This is based on the property full name and the settings in the ComparisonConfig.")]
        private static bool IsPropertyIncluded(string propFullName, BaseComparisonConfig cc)
        {
            if (cc == null || string.IsNullOrWhiteSpace(propFullName))
                return true;

            // Skip if the property is among the PropertyExceptions.
            if ((cc.PropertyExceptions?.Any(pe => propFullName.EndsWith(pe) || propFullName.WildcardMatch(pe)) ?? false))
                return false;

            // If the PropertiesToConsider contains at least a value, ensure that this property is "compatible" with at least one of them.
            // Compatible means to check not only that the current propFullName is among the propertiesToInclude;
            // we need to consider this propFullName ALSO IF there is at least one PropertyToInclude that specifies a property that is a child of the current propFullName.
            if ((cc.PropertiesToConsider?.Any() ?? false) &&
                !cc.PropertiesToConsider.Any(ptc => ptc.StartsWith(propFullName) || propFullName.StartsWith(ptc))) // we want to make sure that we do not exclude sub-properties to include, hence the OR condition.
                return false;

            return true;
        }

        /***************************************************/

        [Description("Determine whether a certain object property should be included in the Hash computation. " +
            "This is based on the property full name and the settings in the ComparisonConfig.")]
        private static double ValueToInclude(this double number, string propFullName, BaseComparisonConfig cc)
        {
            if (cc == null || string.IsNullOrWhiteSpace(propFullName))
                return number;

            if (!IsPropertyIncluded(propFullName, cc))
                return 0;

            return BH.Engine.Base.Query.NumericalApproximation(number, propFullName, cc);
        }

        /***************************************************/
        /****  Private fields                           ****/
        /***************************************************/

        [Description("Multiplier used to define the TypeTranslationFactors.")]
        private const int m_ToleranceMultiplier = (int)(1e9 * Tolerance.Distance);

        [Description("Translation factors per each type of geometry." +
            "The translation is needed in order to obtain different HashArrays for geometries that may share the same defining points," +
            "like e.g. a 3-point Polyline and an Arc that passes through the same points.")]
        private enum TypeTranslationFactor
        {
            Cartesian = -3 * m_ToleranceMultiplier,
            Basis = -2 * m_ToleranceMultiplier,
            Vector = -1 * m_ToleranceMultiplier,
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