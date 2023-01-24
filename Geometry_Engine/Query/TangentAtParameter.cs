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

using System;
using System.Collections.Generic;
using System.Linq;
using BH.oM.Geometry;
using BH.oM.Base;
using BH.oM.Base.Attributes;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        public static Vector TangentAtParameter(this Arc curve, double parameter, double tolerance = Tolerance.Distance)
        {
            double paramTol = tolerance / curve.Length();
            if (parameter > 1 + paramTol || parameter < 0 - paramTol)
                return null;

            return curve.CoordinateSystem.Y.Rotate(curve.StartAngle + (curve.EndAngle - curve.StartAngle) * parameter, curve.CoordinateSystem.Z);
        }

        /***************************************************/

        public static Vector TangentAtParameter(this Circle curve, double parameter, double tolerance = Tolerance.Distance)
        {
            double paramTol = tolerance / curve.Length();
            if (parameter > 1 + paramTol || parameter < 0 - paramTol)
                return null;

            Vector n = curve.Normal;
            Vector refVector = 1 - Math.Abs(n.DotProduct(Vector.XAxis)) > Tolerance.Angle ? Vector.XAxis : Vector.ZAxis;
            Vector localX = n.CrossProduct(refVector).Normalise();
            return n.CrossProduct(localX).Rotate(parameter * 2 * Math.PI, n);
        }

        /***************************************************/

        public static Vector TangentAtParameter(this Ellipse ellipse, double parameter, double tolerance = Tolerance.Distance)
        {
            if (ellipse.IsNull())
                return null;

            if (parameter < 0)
                parameter = 0;
            if (parameter > 1)
                parameter = 1;

            double angle = parameter * 2 * Math.PI;

            return (ellipse.Axis2 * (Math.Cos(angle) / ellipse.Radius1) - ellipse.Axis1 * (Math.Sin(angle) / ellipse.Radius2)).Normalise();
        }

        /***************************************************/

        public static Vector TangentAtParameter(this Line curve, double parameter, double tolerance = Tolerance.Distance)
        {
            double paramTol = tolerance / curve.Length();
            if (parameter > 1 + paramTol || parameter < 0 - paramTol)
                return null;

            return curve.Direction();
        }

        /***************************************************/

        public static Vector TangentAtParameter(this NurbsCurve curve, double t, double tolerance = Tolerance.Distance)
        {
            return DerivativeAtParameter(curve, t, 1)?.Normalise();
        }

        /***************************************************/

        public static Vector TangentAtParameter(this PolyCurve curve, double parameter, double tolerance = Tolerance.Distance)
        {
            double length = curve.Length();
            double paramTol = tolerance / length;
            if (parameter > 1 + paramTol || parameter < 0 - paramTol)
                return null;

            double cLength = parameter * length;
            foreach (ICurve c in curve.SubParts())
            {
                double l = c.ILength();
                if (l >= cLength) return c.ITangentAtParameter(cLength / l);
                cLength -= l;
            }

            return curve.EndDir();
        }

        /***************************************************/

        public static Vector TangentAtParameter(this Polyline curve, double parameter, double tolerance = Tolerance.Distance)
        {
            double length = curve.Length();
            double paramTol = tolerance / length;
            if (parameter > 1 + paramTol || parameter < 0 - paramTol)
                return null;

            double cLength = parameter * length;
            double sum = 0;
            foreach (Line line in curve.SubParts())
            {
                sum += line.Length();
                if (cLength <= sum)
                    return line.Direction();
            }

            return curve.EndDir();
        }

        /***************************************************/

        [MultiOutput(0, "uTangent", "The tangent of the surface along it's U direction.")]
        [MultiOutput(1, "vTangent", "The tangent of the surface along it's V direction.")]
        public static Output<Vector, Vector> TangentAtParameter(this NurbsSurface surface, double u, double v, double tolerance = Tolerance.Distance)
        {
            double a = 0;
            double dua = 0;
            double dva = 0;
            Point result = new Point();
            Point resultU = new Point();
            Point resultV = new Point();

            var uKnots = surface.UKnots.ToList();
            var vKnots = surface.VKnots.ToList();

            var uv = surface.UVCount();

            Func<int, int, int> ind = (i,j) => i * uv[1] + j;

            for (int i = 0; i < uv[0]; i++)
            {
                for (int j = 0; j < uv[1]; j++)
                {
                    double ubasis = BasisFunction(uKnots, i - 1, surface.UDegree, u);
                    double vbasis = BasisFunction(vKnots, j - 1, surface.VDegree, v);
                    double basis = ubasis * vbasis * surface.Weights[ind(i, j)];

                    double dubasis = DerivativeFunction(uKnots, i - 1, surface.UDegree, u) *
                                     vbasis * surface.Weights[ind(i, j)];

                    double dvbasis = DerivativeFunction(vKnots, j - 1, surface.VDegree, v) *
                                     ubasis * surface.Weights[ind(i, j)];

                    a += basis;
                    dua += dubasis;
                    dva += dvbasis;

                    Point pt = surface.ControlPoints[ind(i, j)];

                    result += basis * pt;

                    resultU += dubasis * pt;
                    resultV += dvbasis * pt;
                }
            }

            return new Output<Vector, Vector>()
            {
                Item1 = (resultU * a - result * dua).Normalise(),
                Item2 = (resultV * a - result * dva).Normalise(),
            };
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static Vector ITangentAtParameter(this ICurve curve, double parameter, double tolerance = Tolerance.Distance)
        {
            return TangentAtParameter(curve as dynamic, parameter, tolerance);
        }


        /***************************************************/
        /**** Private Methods - Fallback                ****/
        /***************************************************/

        private static Vector TangentAtParameter(this ICurve curve, double parameter, double tolerance = Tolerance.Distance)
        {
            Base.Compute.RecordError($"TangentAtParameter is not implemented for ICurves of type: {curve.GetType().Name}.");
            return null;
        }

        /***************************************************/
    }
}


