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
using System.ComponentModel;
using BH.oM.Quantities.Attributes;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        [Description("Gets out the Tangent Vector at the normalised angle parameter t on the curve. t should be between 0 and 1 where 0 corresponds to StartAngle and 1 corresponds to EndAngle.\n" +
                     "For a circular Arc this is equivalent to the point at the normalised length paramter where 0 is the StartTangent and 1 is EndTangent.")]
        [Input("curve", "The Arc to evaluate.")]
        [Input("t", "The normalised length/angle parameter to evaluate. Should be a value between 0 and 1.")]
        [Input("tolerance", "Distance tolerance to be used int he method.", typeof(Length))]
        [Output("tan", "The tangent vector at the provided parameter.")]
        [PreviousInputNames("t", "parameter")]
        public static Vector TangentAtParameter(this Arc curve, double t, double tolerance = Tolerance.Distance)
        {
            double paramTol = tolerance / curve.Length();
            if (t > 1 + paramTol || t < 0 - paramTol)
                return null;

            return curve.CoordinateSystem.Y.Rotate(curve.StartAngle + (curve.EndAngle - curve.StartAngle) * t, curve.CoordinateSystem.Z);
        }

        /***************************************************/

        [Description("Gets out the Tangent Vector at the normalised angle parameter t on the curve. t should be between 0 and 1 where 0 corresponds to 0 angle and 1 corresponds to a full lap of 2*PI radians.\n" +
             "For a Circle this is equivalent to the point at the normalised length paramter.")]
        [Input("curve", "The Arc to evaluate.")]
        [Input("t", "The normalised length/angle parameter to evaluate. Should be a value between 0 and 1.")]
        [Input("tolerance", "Distance tolerance to be used int he method.", typeof(Length))]
        [Output("tan", "The tangent vector at the provided parameter.")]
        [PreviousInputNames("t", "parameter")]
        public static Vector TangentAtParameter(this Circle curve, double t, double tolerance = Tolerance.Distance)
        {
            double paramTol = tolerance / curve.Length();
            if (t > 1 + paramTol || t < 0 - paramTol)
                return null;

            Vector n = curve.Normal;
            Vector refVector = 1 - Math.Abs(n.DotProduct(Vector.XAxis)) > Tolerance.Angle ? Vector.XAxis : Vector.ZAxis;
            Vector localX = n.CrossProduct(refVector).Normalise();
            return n.CrossProduct(localX).Rotate(t * 2 * Math.PI, n);
        }

        /***************************************************/

        [Description("Gets out the Tangent Vector at the normalised angle parameter t on the curve. t should be between 0 and 1 where 0 corresponds to 0 angle and 1 corresponds to a full lap of 2*PI radians.\n" +
             "Note that for a general case this does not correspond to a normalised length parameter along the curve, i.e. t value 1/3 does not (for the general case) give the point at 1/3 length around the perimiter but rather the point at the angle parameter corresponding to 1/3 of a full lap.")]
        [Input("curve", "The Ellipse to evaluate.")]
        [Input("t", "The normalised angle parameter to evaluate. Should be a value between 0 and 1.")]
        [Input("tolerance", "Distance tolerance to be used int he method.", typeof(Length))]
        [Output("tan", "The tangent vector at the provided parameter.")]
        [PreviousInputNames("t", "parameter")]
        public static Vector TangentAtParameter(this Ellipse ellipse, double t, double tolerance = Tolerance.Distance)
        {
            if (ellipse.IsNull())
                return null;

            if (t < 0)
                t = 0;
            if (t > 1)
                t = 1;

            double angle = t * 2 * Math.PI;

            return (ellipse.Axis2 * (Math.Cos(angle) / ellipse.Radius1) - ellipse.Axis1 * (Math.Sin(angle) / ellipse.Radius2)).Normalise();
        }

        /***************************************************/

        [Description("Gets out the Tangent Vector at the normalised length parameter t on the curve. t should be between 0 and 1, where  0 is the Start Tangent and 1 is End Tangent.")]
        [Input("curve", "The Line to evaluate.")]
        [Input("t", "The normalised length parameter to evaluate. Should be a value between 0 and 1.")]
        [Input("tolerance", "Distance tolerance to be used int he method.", typeof(Length))]
        [Output("tan", "The tangent vector at the provided parameter.")]
        [PreviousInputNames("t", "parameter")]
        public static Vector TangentAtParameter(this Line curve, double t, double tolerance = Tolerance.Distance)
        {
            double paramTol = tolerance / curve.Length();
            if (t > 1 + paramTol || t < 0 - paramTol)
                return null;

            return curve.Direction();
        }

        /***************************************************/

        [Description("Gets out the Tangent Vector at the parameter t on the curve.\n" +
             "Note that for a general case this does not correspond to a normalised length parameter along the curve.")]
        [Input("curve", "The NurbsCurve to evaluate.")]
        [Input("t", "The parameter to evaluate.")]
        [Input("tolerance", "Distance tolerance to be used int he method.", typeof(Length))]
        [Output("tan", "The tangent vector at the provided parameter.")]
        [PreviousInputNames("t", "parameter")]
        public static Vector TangentAtParameter(this NurbsCurve curve, double t, double tolerance = Tolerance.Distance)
        {
            return DerivativeAtParameter(curve, t, 1)?.Normalise();
        }

        /***************************************************/

        [Description("Gets out the Tangent Vector at the parameter t on the curve, where t generally is the normalised length parameter t on the curve. t should be between 0 and 1, where  0 is the Start Tangent and 1 is End Tangent." +
             "Note that for PolyCurve consisting of a single ellipse the parameter will correspond to the normalised angle parameter rather than the normalised length parameter.")]
        [Input("curve", "The PolyCurve to evaluate.")]
        [Input("t", "The parameter to evaluate.")]
        [Input("tolerance", "Distance tolerance to be used int he method.", typeof(Length))]
        [Output("tan", "The tangent vector at the provided parameter.")]
        [PreviousInputNames("t", "parameter")]
        public static Vector TangentAtParameter(this PolyCurve curve, double t, double tolerance = Tolerance.Distance)
        {
            double length = curve.Length();
            double paramTol = tolerance / length;
            if (t > 1 + paramTol || t < 0 - paramTol)
                return null;

            double cLength = t * length;
            foreach (ICurve c in curve.SubParts())
            {
                double l = c.ILength();
                if (l >= cLength) return c.ITangentAtParameter(cLength / l);
                cLength -= l;
            }

            return curve.EndDir();
        }

        /***************************************************/

        [Description("Gets out the Tangent Vector at the parameter t on the curve, where t is the normalised length parameter t on the curve. t should be between 0 and 1, where 0 is the Start Tangent and 1 is End Tangent.")]
        [Input("curve", "The Polyline to evaluate.")]
        [Input("t", "The parameter to evaluate.")]
        [Input("tolerance", "Distance tolerance to be used int he method.", typeof(Length))]
        [Output("tan", "The tangent vector at the provided parameter.")]
        [PreviousInputNames("t", "parameter")]
        public static Vector TangentAtParameter(this Polyline curve, double t, double tolerance = Tolerance.Distance)
        {
            double length = curve.Length();
            double paramTol = tolerance / length;
            if (t > 1 + paramTol || t < 0 - paramTol)
                return null;

            double cLength = t * length;
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

        [Description("Gets out the Tangent Vector at the parameter t on the curve, where t for most curves is the normalised length parameter t on the curve. t should for those cases be between 0 and 1, where  0 is the Start Tangent and 1 is End Tangent.\n" +
             "Note that the parameter does not correspond to a normalised length for Ellipses and NurbsCurves or PolyCurves consisting of any of these.")]
        [Input("curve", "The ICurve to evaluate.")]
        [Input("t", "The parameter to evaluate.")]
        [Input("tolerance", "Distance tolerance to be used int he method.", typeof(Length))]
        [Output("tan", "The tangent vector at the provided parameter.")]
        [PreviousInputNames("t", "parameter")]
        public static Vector ITangentAtParameter(this ICurve curve, double t, double tolerance = Tolerance.Distance)
        {
            return TangentAtParameter(curve as dynamic, t, tolerance);
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


