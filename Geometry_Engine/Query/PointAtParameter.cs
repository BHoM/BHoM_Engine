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
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        [Description("Gets out the Point at the normalised angle parameter t on the curve. t should be between 0 and 1 where 0 corresponds to StartAngle and 1 corresponds to EndAngle.\n" + 
                     "For a circular Arc this is equivalent to the point at the normalised length paramter where 0 is the StartPoint and 1 is EndPoint.")]
        [Input("curve", "The Arc to evaluate.")]
        [Input("t", "The normalised length/angle parameter to evaluate. Should be a value between 0 and 1.")]
        [Output("pt", "The point at the provided parameter.")]
        public static Point PointAtParameter(this Arc curve, double t)
        {
            if (curve.IsNull())
                return null;

            if (t < 0)
                t = 0;
            if (t > 1)
                t = 1;

            double alfa = curve.Angle() * t + curve.StartAngle;
            Vector localX = curve.CoordinateSystem.X;
            return curve.CoordinateSystem.Origin + localX.Rotate(alfa, curve.FitPlane().Normal) * curve.Radius;
        }

        /***************************************************/

        [Description("Gets out the Point at the normalised angle parameter t on the curve. t should be between 0 and 1 where 0 corresponds to 0 angle and 1 corresponds to a full lap of 2*PI radians.\n" +
                     "For a Circle this is equivalent to the point at the normalised length paramter.")]
        [Input("curve", "The Arc to evaluate.")]
        [Input("t", "The normalised length/angle parameter to evaluate. Should be a value between 0 and 1.")]
        [Output("pt", "The point at the provided parameter.")]
        public static Point PointAtParameter(this Circle curve, double t)
        {
            if (curve.IsNull())
                return null;

            if (t < 0)
                t = 0;
            if (t > 1)
                t = 1;

            return PointAtLength(curve, t * curve.Length());
        }

        /***************************************************/

        [Description("Gets out the Point at the normalised angle parameter t on the curve. t should be between 0 and 1 where 0 corresponds to 0 angle and 1 corresponds to a full lap of 2*PI radians.\n" +
                     "Note that for a general case this does not correspond to a normalised length parameter along the curve, i.e. t value 1/3 does not (for the general case) give the point at 1/3 length around the perimiter but rather the point at the angle parameter corresponding to 1/3 of a full lap.")]
        [Input("curve", "The Ellipse to evaluate.")]
        [Input("t", "The normalised angle parameter to evaluate. Should be a value between 0 and 1.")]
        [Output("pt", "The point at the provided parameter.")]
        public static Point PointAtParameter(this Ellipse curve, double t)
        {
            if (curve.IsNull())
                return null;

            if (t < 0)
                t = 0;
            if (t > 1)
                t = 1;

            double angleParameter = t * 2 * Math.PI;
            double axis1Factor = curve.Radius1 * Math.Cos(angleParameter);
            double axis2Factor = curve.Radius2 * Math.Sin(angleParameter);

            return curve.CoordinateSystem.Origin + curve.CoordinateSystem.X * axis1Factor + curve.CoordinateSystem.Y * axis2Factor;
        }

        /***************************************************/

        [Description("Gets out the Point at the normalised length parameter t on the curve. t should be between 0 and 1, where  0 is the StartPoint and 1 is EndPoint.")]
        [Input("curve", "The Line to evaluate.")]
        [Input("t", "The normalised length parameter to evaluate. Should be a value between 0 and 1.")]
        [Output("pt", "The point at the provided parameter.")]
        public static Point PointAtParameter(this Line curve, double t)
        {
            if (curve.IsNull())
                return null;

            if (t < 0)
                t = 0;
            if (t > 1)
                t = 1;

            Vector vector = curve.End - curve.Start;
            return (curve.Start + vector * t);
        }

        /***************************************************/

        [Description("Gets out the Point at the parameter t on the curve.\n" +
                     "Note that for a general case this does not correspond to a normalised length parameter along the curve.")]
        [Input("curve", "The NurbsCurve to evaluate.")]
        [Input("t", "The parameter to evaluate.")]
        [Output("pt", "The point at the provided parameter.")]
        public static Point PointAtParameter(this NurbsCurve curve, double t)
        {
            if (curve.IsNull())
                return null;

            int n = curve.Degree();
            double a = 0;
            Point result = new Point();

            var knots = curve.Knots;

            for (int i = 0; i < curve.Weights.Count; i++)
            {
                double basis = BasisFunction(knots, i - 1, n, t) * curve.Weights[i];
                a += basis;

                Point pt = curve.ControlPoints[i];

                result += basis * pt;
            }

            return result / a;
        }

        /***************************************************/

        [Description("Gets out the Point at the parameter t on the curve, where t generally is the normalised length parameter t on the curve. t should be between 0 and 1, where  0 is the StartPoint and 1 is EndPoint." +
                     "Note that for PolyCurve consisting of a single ellipse the parameter will correspond to the normalised length parameter rather than the normalised length parameter..")]
        [Input("curve", "The PolyCurve to evaluate.")]
        [Input("t", "The parameter to evaluate.")]
        [Output("pt", "The point at the provided parameter.")]
        [PreviousInputNames("t", "parameter")]
        public static Point PointAtParameter(this PolyCurve curve, double t)
        {
            if (curve.IsNull())
                return null;

            if (t == 0)
                return curve.StartPoint();
            else if (t == 1)
                return curve.EndPoint();

            double cLength = t * curve.Length();
            foreach (ICurve c in curve.SubParts())
            {
                double l = c.ILength();
                if (l >= cLength)
                    return c.IPointAtParameter(cLength / l);

                cLength -= l;
            }

            return null;
        }

        /***************************************************/

        [Description("Gets out the Point at the parameter t on the curve, where t is the normalised length parameter t on the curve. t should be between 0 and 1, where 0 is the StartPoint and 1 is EndPoint.")]
        [Input("curve", "The Polyline to evaluate.")]
        [Input("t", "The parameter to evaluate.")]
        [Output("pt", "The point at the provided parameter.")]
        [PreviousInputNames("t", "parameter")]
        public static Point PointAtParameter(this Polyline curve, double t)
        {
            if (curve.IsNull())
                return null;

            if (t == 0)
                return curve.StartPoint();
            else if (t == 1)
                return curve.EndPoint();

            double cLength = t * curve.Length();
            foreach (Line line in curve.SubParts())
            {
                double l = line.Length();
                if (l >= cLength)
                    return line.PointAtParameter(cLength / l);

                cLength -= l;
            }

            return null;
        }


        /***************************************************/
        /**** Public Methods - Surfaces                 ****/
        /***************************************************/

        [Description("Gets out the Point at the parameters u and v on the surface.")]
        [Input("surface", "The NurbsSurface to evaluate.")]
        [Input("u", "The parameter to evaluate along the u domain.")]
        [Input("v", "The parameter to evaluate along the v domain.")]
        [Output("pt", "The point at the provided parameters.")]
        public static Point PointAtParameter(this NurbsSurface surface, double u, double v)
        {
            if (surface.IsNull())
                return null;

            double a = 0;
            Point result = new Point();

            var uv = surface.UVCount();

            List<double> uKnots = surface.UKnots.ToList();
            List<double> vKnots = surface.VKnots.ToList();

            Func<int, int, int> ind = (i,j) => i * uv[1] + j;

            for (int i = 0; i < uv[0]; i++)
            {
                for (int j = 0; j < uv[1]; j++)
                {
                    double basis = BasisFunction(uKnots, i - 1, surface.UDegree, u) *
                                   BasisFunction(vKnots, j - 1, surface.VDegree, v) *
                                   surface.Weights[ind(i, j)];

                    a += basis;
                    result += basis * surface.ControlPoints[ind(i, j)];
                }
            }

            return result / a;
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        [Description("Gets out the Point at the parameter t on the curve, where t for most curves is the normalised length parameter t on the curve. t should for those cases be between 0 and 1, where  0 is the StartPoint and 1 is EndPoint.\n" +
                     "Note that the parameter does not correspond to a normalised length for Ellipses and NurbsCurves or PolyCurves consisting of any of these.")]
        [Input("curve", "The ICurve to evaluate.")]
        [Input("t", "The parameter to evaluate.")]
        [Output("pt", "The point at the provided parameter.")]
        public static Point IPointAtParameter(this ICurve curve, double t)
        {
            if (curve.IsNull())
                return null;

            return PointAtParameter(curve as dynamic, t);
        }


        /***************************************************/
        /**** Private Fallback Methods                  ****/
        /***************************************************/

        private static Point PointAtParameter(this ICurve curve, double t)
        {
            Base.Compute.RecordError($"PointAtParameter is not implemented for ICurves of type: {curve.GetType().Name}.");
            return null;
        }

        /***************************************************/
    }
}



