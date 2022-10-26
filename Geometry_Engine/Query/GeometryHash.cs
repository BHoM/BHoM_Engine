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
using System.Linq;
using System.Security.Cryptography.Xml;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /****               Public Methods              ****/
        /***************************************************/

        public static double[] IGeometryHash(this IBHoMObject bhomObj)
        {
            IGeometry igeom = bhomObj.IGeometry();

            return GeometryHash(igeom as dynamic);
        }

        /***************************************************/
        /****              Private Methods              ****/
        /***************************************************/

        /***************************************************/
        /****  Curves                                   ****/
        /***************************************************/

        private static double[] GeometryHash(this ICurve curve)
        {
            IEnumerable<ICurve> subParts = curve.ISubParts();

            return subParts.GeometryHash();
        }

        /***************************************************/

        private static double[] GeometryHash(this IEnumerable<ICurve> curves)
        {
            return curves.SelectMany(c => (double[])GeometryHash(c as dynamic)).ToArray();
        }

        /***************************************************/

        private static double[] GeometryHash(this Arc curve, double typeTranslationFactor = 1)
        {
            return curve.StartPoint().ToDoubleArray(typeTranslationFactor)
                .Concat(curve.EndPoint().ToDoubleArray(typeTranslationFactor))
                .Concat(curve.PointAtParameter(0.5).ToDoubleArray(typeTranslationFactor))
                .ToArray();
        }

        /***************************************************/

        private static double[] GeometryHash(this Circle curve, double typeTranslationFactor = 1)
        {
            return curve.StartPoint().ToDoubleArray(typeTranslationFactor)
               .Concat(curve.PointAtParameter(0.33).ToDoubleArray(typeTranslationFactor))
               .Concat(curve.PointAtParameter(0.66).ToDoubleArray(typeTranslationFactor))
               .ToArray();
        }

        /***************************************************/

        private static double[] GeometryHash(this Ellipse curve, double typeTranslationFactor = 1)
        {
            return curve.StartPoint().ToDoubleArray(typeTranslationFactor)
               .Concat(curve.PointAtParameter(0.33).ToDoubleArray(typeTranslationFactor))
               .Concat(curve.PointAtParameter(0.66).ToDoubleArray(typeTranslationFactor))
               .ToArray();
        }

        /***************************************************/

        private static double[] GeometryHash(this Line curve, double typeTranslationFactor = 1)
        {
            return curve.StartPoint().ToDoubleArray(typeTranslationFactor)
               .Concat(curve.EndPoint().ToDoubleArray(typeTranslationFactor))
               .ToArray();
        }

        /***************************************************/

        private static double[] GeometryHash(this NurbsCurve curve, double typeTranslationFactor = 1)
        {
            double[] translationArray = curve.Weights.Zip(curve.Knots, (w, n) => w * n).ToArray();
            int curveDegree = curve.Degree();

            List<Point> pointWeights = curve.ControlPoints.Zip(curve.Weights, (p, w) => p * w).ToList();
            List<double> concatenated = new List<double>();
            for (int i = 0; i < pointWeights.Count(); i++)
            {
                double avg = curve.Knots.GetRange(i, curveDegree).Average();
                double[] doubles = pointWeights[i].GeometryHash(avg + typeTranslationFactor);
                concatenated.AddRange(doubles);
            }

            return concatenated.ToArray();

            // Simpler:
            //return curve.Knots.Take(pointWeights.Count())
            //    .Zip(curve.ControlPoints, (k, p) => new double[]{ p.X + k, p.Y + k, p.Z + k })
            //    .SelectMany(arr => arr).ToArray();
        }

        /***************************************************/
        /****  Surfaces                                 ****/
        /***************************************************/

        private static double[] GeometryHash(this ISurface obj, double typeTranslationFactor = 3)
        {
            return GeometryHash(obj as dynamic, typeTranslationFactor);
        }

        /***************************************************/

        private static double[] GeometryHash(this PlanarSurface obj, double typeTranslationFactor = 3)
        {
            return obj.ExternalBoundary.GeometryHash().Concat(obj.InternalBoundaries.SelectMany(ib => ib.GeometryHash())).ToArray();
        }

        /***************************************************/

        private static double[] GeometryHash(this Extrusion obj, double typeTranslationFactor = 3)
        {
            return obj.Curve.ITranslate(obj.Direction).GeometryHash().Concat(obj.Curve.GeometryHash()).ToArray();
        }

        /***************************************************/

        private static double[] GeometryHash(this Loft obj, double typeTranslationFactor = 3)
        {
            return obj.Curves.GeometryHash();
        }

        /***************************************************/

        private static double[] GeometryHash(this NurbsSurface obj, double typeTranslationFactor = 3)
        {
            return obj.ControlPoints.ToDoubleArray(typeTranslationFactor);
        }

        /***************************************************/

        private static double[] GeometryHash(this Pipe obj, double typeTranslationFactor = 3)
        {
            return obj.Centreline.GeometryHash(); // radius
        }

        /***************************************************/

        private static double[] GeometryHash(this PolySurface obj, double typeTranslationFactor = 3)
        {
            return obj.Surfaces.SelectMany(s => s.GeometryHash()).ToArray();
        }

        /***************************************************/

        private static double[] GeometryHash(this SurfaceTrim obj, double typeTranslationFactor = 3)
        {
            return obj.Curve3d.GeometryHash().Concat(obj.Curve2d.GeometryHash()).ToArray();
        }


        /***************************************************/
        /****  Mesh                                     ****/
        /***************************************************/

        private static double[] GeometryHash(this Mesh obj, double typeTranslationFactor = 3)
        {
            // TODO faces?
            Dictionary<int, int> asd = obj.Faces.SelectMany(f => new List<int> { f.A, f.B, f.C, f.D })
                .GroupBy(i => i)
                .ToDictionary(g => g.Key, g => g.Count());

            List<double> all = new List<double>();

            for (int i = 0; i < obj.Vertices.Count; i++)
            {
                int pointTranslationFactor;
                if (!asd.TryGetValue(i, out pointTranslationFactor))
                    pointTranslationFactor = 0;

                all.AddRange(obj.Vertices[i].ToDoubleArray(pointTranslationFactor + typeTranslationFactor));
            }

            return all.ToArray();
        }

        /***************************************************/

        private static double[] GeometryHash(this Mesh3D obj, double typeTranslationFactor = 3)
        {
            // TODO faces?
            return obj.Vertices.SelectMany(v => v.ToDoubleArray(typeTranslationFactor)).ToArray();
        }

        /***************************************************/
        /****  Vector                                   ****/
        /***************************************************/

        private static double[] GeometryHash(this Point obj, double typeTranslationFactor = 0)
        {
            return obj.ToDoubleArray(typeTranslationFactor);
        }

        /***************************************************/

        private static double[] GeometryHash(this Plane obj, double typeTranslationFactor = 3)
        {
            return obj.Origin.Translate(obj.Normal).ToDoubleArray(typeTranslationFactor);
        }

        /***************************************************/
        /****  Other methods                            ****/
        /***************************************************/

        private static double[] GeometryHash(this object obj)
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



