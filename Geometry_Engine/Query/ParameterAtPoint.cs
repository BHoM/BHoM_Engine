/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
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
using BH.oM.Geometry;
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

        [Description("Computes and returns the parameter at the curve that corresponds to the provided point.\n" +
                    "This is the reverse of PointAtParameter, i.e. providing the resulting parameter from this method to PointAtParamter should result in the provided Point.\n" +
                    "Method returns -1 for points with a distance larger than the provided tolerance from the curve.")]
        [Input("curve", "The Arc from which to get the parameter.")]
        [Input("point", "The point to get the parameter at.")]
        [Input("tolerance", "Distance tolerance to be used in the method. Method returns -1 for Points with a distance larger than this value from the curve.", typeof(Length))]
        [Output("t", "The curve parameter at the provided point.")]
        public static double ParameterAtPoint(this Arc curve, Point point, double tolerance = Tolerance.Distance)
        {
            if (curve.ClosestPoint(point).SquareDistance(point) > tolerance * tolerance)
                return -1;

            Point centre = curve.CoordinateSystem.Origin;
            Vector normal = curve.CoordinateSystem.Z;
            Vector v1 = curve.CoordinateSystem.X;
            Vector v2 = point - centre;

            double angle = v1.SignedAngle(v2, normal) - curve.StartAngle;
            angle = (Math.Abs(angle) < Tolerance.Angle) || Math.Abs(angle - 2 * Math.PI) < Tolerance.Angle ? 0 : angle;  //Really small negative angles gives wrong result. This solves that problem.
            double parameter = ((angle + 2 * Math.PI) % (2 * Math.PI)) / curve.Angle();
            return parameter > 1 ? 1 : parameter < 0 ? 0 : parameter;
        }

        /***************************************************/

        [Description("Computes and returns the parameter at the curve that corresponds to the provided point.\n" +
            "This is the reverse of PointAtParameter, i.e. providing the resulting parameter from this method to PointAtParamter should result in the provided Point.\n" +
            "Method returns -1 for points with a distance larger than the provided tolerance from the curve.")]
        [Input("curve", "The Circle from which to get the parameter.")]
        [Input("point", "The point to get the parameter at.")]
        [Input("tolerance", "Distance tolerance to be used in the method. Method returns -1 for Points with a distance larger than this value from the curve.", typeof(Length))]
        [Output("t", "The curve parameter at the provided point.")]
        public static double ParameterAtPoint(this Circle curve, Point point, double tolerance = Tolerance.Distance)
        {
            if (curve.ClosestPoint(point).SquareDistance(point) > tolerance * tolerance)
                return -1;

            Vector v1 = curve.StartPoint() - curve.Centre;
            Vector v2 = point - curve.Centre;
            return ((v1.SignedAngle(v2, curve.Normal) + 2 * Math.PI) % (2 * Math.PI)) / (2 * Math.PI);
        }

        /***************************************************/

        [Description("Computes and returns the parameter at the curve that corresponds to the provided point.\n" +
            "This is the reverse of PointAtParameter, i.e. providing the resulting parameter from this method to PointAtParamter should result in the provided Point.\n" +
            "Method returns -1 for points with a distance larger than the provided tolerance from the curve.")]
        [Input("curve", "The Ellipse from which to get the parameter.")]
        [Input("point", "The point to get the parameter at.")]
        [Input("tolerance", "Distance tolerance to be used in the method. Method returns -1 for Points with a distance larger than this value from the curve.", typeof(Length))]
        [Output("t", "The curve parameter at the provided point.")]
        public static double ParameterAtPoint(this Ellipse curve, Point point, double tolerance = Tolerance.Distance)
        {
            if (curve.IsNull() || point.IsNull())
                return double.NaN;

            if (curve.ClosestPoint(point).SquareDistance(point) > tolerance * tolerance)
                return -1;

            Vector v = point - curve.Centre;

            Vector vProj1 = curve.Axis1 * v.DotProduct(curve.Axis1) / curve.Radius1;
            Vector vProj2 = curve.Axis2 * v.DotProduct(curve.Axis2) / curve.Radius2;
            v = vProj1 + vProj2;

            return ((curve.Axis1.SignedAngle(v, curve.Normal()) + 2 * Math.PI) % (2 * Math.PI)) / (2 * Math.PI);
        }

        /***************************************************/

        [Description("Computes and returns the parameter at the curve that corresponds to the provided point.\n" +
            "This is the reverse of PointAtParameter, i.e. providing the resulting parameter from this method to PointAtParamter should result in the provided Point.\n" +
            "Method returns -1 for points with a distance larger than the provided tolerance from the curve.")]
        [Input("curve", "The Line from which to get the parameter.")]
        [Input("point", "The point to get the parameter at.")]
        [Input("tolerance", "Distance tolerance to be used in the method. Method returns -1 for Points with a distance larger than this value from the curve.", typeof(Length))]
        [Output("t", "The curve parameter at the provided point.")]
        public static double ParameterAtPoint(this Line curve, Point point, double tolerance = Tolerance.Distance)
        {
            if (curve.ClosestPoint(point).SquareDistance(point) > tolerance * tolerance)
                return -1;

            double parameter = point.Distance(curve.Start) / curve.Length();
            if (curve.Infinite)
                return parameter;
            return parameter > 1 ? 1 : parameter < 0 ? 0 : parameter;
        }

        /***************************************************/

        [Description("Computes and returns the parameter at the curve that corresponds to the provided point.\n" +
            "This is the reverse of PointAtParameter, i.e. providing the resulting parameter from this method to PointAtParamter should result in the provided Point.\n" +
            "Method returns -1 for points with a distance larger than the provided tolerance from the curve.")]
        [Input("curve", "The PolyCurve from which to get the parameter.")]
        [Input("point", "The point to get the parameter at.")]
        [Input("tolerance", "Distance tolerance to be used in the method. Method returns -1 for Points with a distance larger than this value from the curve.", typeof(Length))]
        [Output("t", "The curve parameter at the provided point.")]
        public static double ParameterAtPoint(this PolyCurve curve, Point point, double tolerance = Tolerance.Distance)
        {
            double sqTol = tolerance * tolerance;
            double length = 0;

            foreach (ICurve c in curve.SubParts())
            {
                if (c.IClosestPoint(point).SquareDistance(point) <= sqTol)
                {
                    double parameter = (length + c.IParameterAtPoint(point, tolerance) * c.ILength()) / curve.ILength();
                    return parameter > 1 ? 1 : parameter < 0 ? 0 : parameter;
                }
                else
                    length += c.ILength();
            }

            return -1;
        }

        /***************************************************/

        [Description("Computes and returns the parameter at the curve that corresponds to the provided point.\n" +
            "This is the reverse of PointAtParameter, i.e. providing the resulting parameter from this method to PointAtParamter should result in the provided Point.\n" +
            "Method returns -1 for points with a distance larger than the provided tolerance from the curve.")]
        [Input("curve", "The Polyline from which to get the parameter.")]
        [Input("point", "The point to get the parameter at.")]
        [Input("tolerance", "Distance tolerance to be used in the method. Method returns -1 for Points with a distance larger than this value from the curve.", typeof(Length))]
        [Output("t", "The curve parameter at the provided point.")]
        public static double ParameterAtPoint(this Polyline curve, Point point, double tolerance = Tolerance.Distance)
        {
            double sqTol = tolerance * tolerance;
            double param = 0;

            foreach (Line l in curve.SubParts())
            {
                if (l.ClosestPoint(point).SquareDistance(point) <= sqTol)
                {
                    double parameter = (param + l.ParameterAtPoint(point, tolerance) * l.Length()) / curve.Length();
                    return parameter > 1 ? 1 : parameter < 0 ? 0 : parameter;
                }
                else
                    param += l.Length();
            }

            return -1;
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        [Description("Computes and returns the parameter at the curve that corresponds to the provided point.\n" +
            "This is the reverse of PointAtParameter, i.e. providing the resulting parameter from this method to PointAtParamter should result in the provided Point.\n" +
            "Method returns -1 for points with a distance larger than the provided tolerance from the curve.")]
        [Input("curve", "The curve from which to get the parameter.")]
        [Input("point", "The point to get the parameter at.")]
        [Input("tolerance", "Distance tolerance to be used in the method. Method returns -1 for Points with a distance larger than this value from the curve.", typeof(Length))]
        [Output("t", "The curve parameter at the provided point.")]
        public static double IParameterAtPoint(this ICurve curve, Point point, double tolerance = Tolerance.Distance)
        {
            return ParameterAtPoint(curve as dynamic, point, tolerance);
        }


        /***************************************************/
        /**** Private Fallback Methods                  ****/
        /***************************************************/

        private static double ParameterAtPoint(this ICurve curve, Point point, double tolerance = Tolerance.Distance)
        {
            Base.Compute.RecordError($"ParameterAtPoint is not implemented for ICurves of type: {curve.GetType().Name}.");
            return double.NaN;
        }

        /***************************************************/
    }
}




