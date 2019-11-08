/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2019, the respective contributors. All rights reserved.
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
using System.ComponentModel;
using BH.oM.Reflection.Attributes;

namespace BH.Engine.Geometry
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Integrates a closed region with x to the specified power on the XY-Plane")]
        [Input("curve", "defined counter clockwise on the XY-Plane")]
        [Input("powX", "the region will be evaluated under the function: x^(powX)")]
        [Output("V", "Calculated value")]
        public static double IIntegrateRegion(this ICurve curve, int powX)
        {
            // Add tests (?)

            return IntegrateRegion(curve as dynamic, powX);
        }

        /***************************************************/

        [Description("Integrates a according to Green's Therom between two points with x to the specified power on the XY-Plane")]
        [Input("a", "the point to begin from")]
        [Input("b", "the point to end at")]
        [Input("powX", "the region will be evaluated under the function: x^(powX)")]
        [Output("V", "Calculated value, a 'meningless' value unless combined with other values to enclose a region")]
        public static double IntSurfLine(Point a, Point b, int powX)
        {
            //TODO Should do some checks if these are good Tolerances
            //TODO powX could be a double, but that might slow thing down somewhat
            double tol = Tolerance.Distance;

            double X;
            double Y = (a.Y - b.Y);
            /***************/
            if (Math.Abs(Y) < tol)
                return 0;
            /***************/
            switch (powX)
            {
                case 0:
                    return -((a.X + b.X) * Y) * 0.5;
                /********************/
                case 1:
                    return ((a.X * a.X + a.X * b.X + b.X * b.X) * (-Y)) / 6;
                /********************/
                case 2:
                    return -((a.X + b.X) * (a.X * a.X + b.X * b.X) * Y) / 12;
                /********************/
                case -1:
                    if (a.X < tol || b.X < tol)
                    {
                        Engine.Reflection.Compute.RecordError("powX = -1 is not defined left of the Y-axis");
                        return 0;
                    }
                    X = (a.X - b.X);
                    if (Math.Abs(X) < tol)
                        return -Math.Log(a.X) * Y;

                    return (-Y * (X * Math.Log(b.X)) + a.X * Math.Log(a.X / b.X)) / X;
                /********************/
                case -2:
                    if ((a.X < 0 ^ b.X < 0) || Math.Abs(a.X) < tol || Math.Abs(b.X) < tol)
                    {
                        Engine.Reflection.Compute.RecordError("powX = -2 is not defined on the Y-axis");
                        return 0;
                    }
                    X = (a.X - b.X);
                    if (Math.Abs(X) < tol)
                        return Y / a.X;

                    return -Y * Math.Log(b.X / a.X) / X;
                /********************/
                default:
                    X = (a.X - b.X);
                    if (Math.Abs(X) < tol)
                        return -(Math.Pow(a.X, powX + 1) * Y) / (powX + 1);
                    /***************/
                    double N = (powX + 1) * (powX + 2);
                    double bigX = (Math.Pow(b.X, powX + 2) - Math.Pow(a.X, powX + 2));

                    return Y * bigX / (N * X);
                    /********************/
            }
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static double IntegrateRegion(PolyCurve curve, int powX)
        {
            if (curve.Curves.Count == 0)
                return 0;

            double result = 0;

            foreach (ICurve c in curve.Curves)
            {
                result += IntegrateRegion(c as dynamic, powX);
            }

            return result;
        }

        /***************************************************/

        private static double IntegrateRegion(Line line, int powX)
        {
            return IntSurfLine(line.Start, line.End, powX);
        }

        /***************************************************/

        private static double IntegrateRegion(Polyline pLine, int powX)
        {
            List<Point> pts = pLine.ControlPoints;

            double result = 0;

            for (int i = 0; i < pts.Count - 1; i++)
                result += IntSurfLine(pts[i], pts[i + 1], powX);

            return result;
        }

        /***************************************************/

        private static double IntegrateRegion(NurbsCurve pLine, int powX)
        {
            throw new NotImplementedException("NurbsCurve is not imlemented yet so this cannot be calculated");
        }

        /***************************************************/

        private static double IntegrateRegion(Arc arc, int powX)
        {
            Point O = arc.CoordinateSystem.Origin;
            double r = arc.Radius;
            Point start = arc.StartPoint();
            Point end = arc.EndPoint();

            double a = Vector.XAxis.Angle(start - O, Plane.XY);

            double k = Math.Abs(arc.Angle());
            if ((start - O).Angle(end - O, Plane.XY) - k > Tolerance.Angle)
                k *= -1;

            switch (powX)
            {
                case 0:
                    return (
                                O.X * r * (-Math.Sin(a) + Math.Sin(a + k)) + 
                                (r * r * (2 * k - Math.Sin(2 * a) + Math.Sin(2 * (a + k)))
                            ) / 4);
                /********************/
                case 1:
                    return (r * (
                                (Math.Sin(3 * (k + a)) + 9 * Math.Sin(k + a) - 
                                 Math.Sin(3 * a) - 9 * Math.Sin(a)) * r * r + 
                                6 * (Math.Sin(2 * (k + a)) + 2 * k - Math.Sin(2 * a)) * O.X * r + 
                                12 * (Math.Sin(k + a) - Math.Sin(a)) * O.X * O.X
                            )) / 24;
                /********************/
                case 2:
                    return (r * (
                                (Math.Sin(4 * (k + a)) + 8 * Math.Sin(2 * (k + a)) + 12 * k - Math.Sin(4 * a) - 
                                8 * Math.Sin(2 * a)) * r * r * r - 
                                32 * (Math.Sin(k + a) * (Math.Sin(k + a) * Math.Sin(k + a) - 3) + 
                                (Math.Cos(a) * Math.Cos(a) + 2) * Math.Sin(a)) * O.X * r * r + 
                                24 * (Math.Sin(2 * (k + a)) + 2 * k - Math.Sin(2 * a)) * O.X * O.X * r + 
                                32 * (Math.Sin(k + a) - Math.Sin(a)) * O.X * O.X * O.X
                            )) / 96;
                /********************/
                default:
                    return IntegrateRegion(arc.CollapseToPolyline(0.01), powX); //TODO is this good value??
            }
        }

        /***************************************************/

        private static double IntegrateRegion(Circle circle, int powX)
        {
            double r = circle.Radius;
            Point O = circle.Centre;
            int flip = (int)(circle.Normal.Z / Math.Abs(circle.Normal.Z));

            switch (powX)
            {
                case 0:
                    return Math.PI * r * r * flip;
                /********************/
                case 1:
                    return r * r * Math.PI * O.X * flip;
                /********************/
                case 2:
                    return 0.25 * r * r * Math.PI * (4 * O.X * O.X + r * r) * flip;
                /********************/
                default:
                    return IntegrateRegion(circle.CollapseToPolyline(0.01), powX); //TODO is this good value??
            }
        }

        /***************************************************/
    }
}
