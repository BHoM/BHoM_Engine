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
using BH.oM.Geometry;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography.Xml;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /****               Public Methods              ****/
        /***************************************************/

        [Description("All geometry hash methods are implemented to be translational.")]
        public static double[] IGeometryHash(this IBHoMObject bhomObj)
        {
            IGeometry igeom = bhomObj.IGeometry();

            return GeometryHash(igeom as dynamic, 0);
        }

        private enum TypeTranslationFactor
        {
            Point = 0,
            Arc,
            Circle,
            Ellipse,
            Line,
            NurbsCurve,
            PlanarSurface,
            Extrusion,
            Loft,
            Pipe,
            PolySurface,
            NurbsSurface,
            SurfaceTrim,
            Mesh,
            Mesh3D,
            Plane
        }

        /***************************************************/
        /****              Private Methods              ****/
        /***************************************************/

        /***************************************************/
        /****  Curves                                   ****/
        /***************************************************/

        private static double[] GeometryHash(this ICurve curve, double translationFactor)
        {
            IEnumerable<ICurve> subParts = curve.ISubParts();

            return subParts.GeometryHash(translationFactor);
        }

        /***************************************************/

        private static double[] GeometryHash(this IEnumerable<ICurve> curves, double translationFactor)
        {
            return curves.SelectMany(c => (double[])GeometryHash(c as dynamic, translationFactor)).ToArray();
        }

        /***************************************************/

        private static double[] GeometryHash(this Arc curve, double translationFactor)
        {
            translationFactor += (int)TypeTranslationFactor.Arc;

            return curve.StartPoint().ToDoubleArray(translationFactor)
                .Concat(curve.EndPoint().ToDoubleArray(translationFactor))
                .Concat(curve.PointAtParameter(0.5).ToDoubleArray(translationFactor))
                .ToArray();
        }

        /***************************************************/

        private static double[] GeometryHash(this Circle curve, double translationFactor)
        {
            translationFactor += (int)TypeTranslationFactor.Circle;

            return curve.StartPoint().ToDoubleArray(translationFactor)
               .Concat(curve.PointAtParameter(0.33).ToDoubleArray(translationFactor))
               .Concat(curve.PointAtParameter(0.66).ToDoubleArray(translationFactor))
               .ToArray();
        }

        /***************************************************/

        private static double[] GeometryHash(this Ellipse curve, double translationFactor)
        {
            translationFactor += (int)TypeTranslationFactor.Ellipse;

            return curve.StartPoint().ToDoubleArray(translationFactor)
               .Concat(curve.PointAtParameter(0.33).ToDoubleArray(translationFactor))
               .Concat(curve.PointAtParameter(0.66).ToDoubleArray(translationFactor))
               .ToArray();
        }

        /***************************************************/

        [Description("")]
        private static double[] GeometryHash(this Line curve, double translationFactor)
        {
            translationFactor += (int)TypeTranslationFactor.Line;

            return curve.StartPoint().ToDoubleArray(translationFactor)
               .Concat(curve.EndPoint().ToDoubleArray(translationFactor))
               .ToArray();
        }

        /***************************************************/

        [Description("Moving control points by a translation factor composed by the weights " +
            "and a subarray of the knot vector. " +
            "The subarray is made by picking as many elements from the knot vector as the curve degree value.")]
        private static double[] GeometryHash(this NurbsCurve curve, double translationFactor)
        {
            int curveDegree = curve.Degree();

            if (curveDegree == 1)
                return BH.Engine.Geometry.Create.Polyline(curve.ControlPoints).GeometryHash(translationFactor);

            translationFactor += (int)TypeTranslationFactor.NurbsCurve;

            List<double> concatenated = new List<double>();
            for (int i = 0; i < curve.ControlPoints.Count(); i++)
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

        private static double[] GeometryHash(this PlanarSurface obj, double translationFactor)
        {
            translationFactor += (int)TypeTranslationFactor.PlanarSurface;

            return obj.ExternalBoundary.GeometryHash(translationFactor)
                .Concat(obj.InternalBoundaries.SelectMany(ib => ib.GeometryHash(translationFactor))).ToArray();
        }

        /***************************************************/

        private static double[] GeometryHash(this Extrusion obj, double translationFactor)
        {
            translationFactor += (int)TypeTranslationFactor.Extrusion;

            return obj.Curve.ITranslate(obj.Direction).GeometryHash(translationFactor)
                .Concat(obj.Curve.GeometryHash(translationFactor)).ToArray();
        }

        /***************************************************/

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

            List<double> concatenated = new List<double>();
            for (int i = 0; i < obj.ControlPoints.Count(); i++)
            {
                double UKnotsSum = obj.UKnots.ToList().GetRange(i, obj.UDegree).Sum();
                double VKnotsSum = obj.VKnots.ToList().GetRange(i, obj.VDegree).Sum();

                double[] doubles = obj.ControlPoints[i].GeometryHash(UKnotsSum + VKnotsSum + obj.Weights[i] + translationFactor);
                concatenated.AddRange(doubles);
            }

            return obj.ControlPoints.ToDoubleArray(translationFactor);
        }

        /***************************************************/

        private static double[] GeometryHash(this Pipe obj, double translationFactor)
        {
            translationFactor += (int)TypeTranslationFactor.Pipe;

            double[] result = obj.Centreline.GeometryHash(translationFactor + obj.Radius);

            if (obj.Capped)
                result.Concat(obj.Centreline.StartPoint().GeometryHash(translationFactor + obj.Radius));

            return result;
        }

        /***************************************************/

        private static double[] GeometryHash(this PolySurface obj, double translationFactor)
        {
            translationFactor += (int)TypeTranslationFactor.PolySurface;

            return obj.Surfaces.SelectMany(s => s.GeometryHash(translationFactor)).ToArray();
        }

        /***************************************************/

        private static double[] GeometryHash(this SurfaceTrim obj, double translationFactor)
        {
            translationFactor += (int)TypeTranslationFactor.SurfaceTrim;

            return obj.Curve3d.GeometryHash(translationFactor)
                .Concat(obj.Curve2d.GeometryHash(translationFactor)).ToArray();
        }


        /***************************************************/
        /****  Mesh                                     ****/
        /***************************************************/

        [Description("Get the number of faces that are attached to each control point, " +
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

        private static double[] GeometryHash(this Point obj, double translationFactor)
        {
            return obj.ToDoubleArray(translationFactor);
        }

        /***************************************************/

        private static double[] GeometryHash(this Plane obj, double translationFactor)
        {
            translationFactor += (int)TypeTranslationFactor.Plane;

            return obj.Origin.Translate(obj.Normal).ToDoubleArray(translationFactor);
        }

        /***************************************************/
        /****  Other methods                            ****/
        /***************************************************/

        private static double[] GeometryHash(this object obj, double translationFactor)
        {
            BH.Engine.Base.Compute.RecordError($"Could not find a {nameof(GeometryHash)} method for type {obj.GetType().FullName}.");
            return new double[] { };
        }

        /***************************************************/

        private static double[] ToDoubleArray(this Point p, double typeTranslationFactor, double[] translationArray = null)
        {
            return new double[]
            {
                p.X + typeTranslationFactor + translationArray?.ElementAtOrDefault(0) ?? 0,
                p.Y + typeTranslationFactor + translationArray?.ElementAtOrDefault(1) ?? 0,
                p.Z + typeTranslationFactor + translationArray?.ElementAtOrDefault(2) ?? 0
            };
        }

        /***************************************************/

        private static double[] ToDoubleArray(this IEnumerable<Point> points, double typeTranslationFactor, double[] translationArray = null)
        {
            return points.SelectMany(p => p.ToDoubleArray(typeTranslationFactor, translationArray)).ToArray();
        }
    }
}



