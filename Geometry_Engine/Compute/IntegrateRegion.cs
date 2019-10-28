/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2018, the respective contributors. All rights reserved.
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
using BH.oM.Geometry;
using BH.oM.Reflection;
using System.Collections.Generic;
using BH.oM.Reflection.Attributes;
using System.ComponentModel;

namespace BH.Engine.Geometry
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Integrates a closed region with x to the specified power")]
        [Input("pLine", "defined counter clockwise")]
        [Input("powX", "")]
        [Output("V", "Calculated value")]
        public static double IIntegrateRegion(ICurve curve, int powX)
        {
            // Add tests (?)

            List<int> valid = new List<int>() { 0, 1, 2 };
            if (!valid.Contains(powX))
            {
                Engine.Reflection.Compute.RecordError("unsupporeted value for powX");
                return 0;
            }

            return IntegrateRegion(curve as dynamic, powX);
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static double IntegrateRegion(PolyCurve curve, int powX)
        {
            // Add tests (?)
            if (curve.Curves.Count == 0)
                return 0;

            double result = 0;

            double sqTol = Tolerance.Distance * Tolerance.Distance;

            Point last = curve.Curves[0].IStartPoint();

            foreach (ICurve c in curve.Curves)
            {

                if (c.IStartPoint().SquareDistance(last) > sqTol)
                {
                    if (c.IEndPoint().SquareDistance(last) > sqTol)
                    {
                        Engine.Reflection.Compute.RecordError("Curve not closed");
                        return 0;
                    }
                    else
                    {
                        result += IntegrateRegion(c.IFlip() as dynamic, powX);
                        last = c.IStartPoint();
                    }
                }
                else
                {
                    result += IntegrateRegion(c as dynamic, powX);
                    last = c.IEndPoint();
                }
            }

            return result;
        }

        /***************************************************/

        private static double IntegrateRegion(Line line, int powX)
        {
            // Add tests (?)

            return IntSurfLine(line.Start, line.End, powX);
        }

        /***************************************************/

        private static double IntegrateRegion(Polyline pLine, int powX)
        {
            // Add tests (?)

            List<Point> pts = new List<Point>(pLine.ControlPoints);

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
            // Add tests (?)
            Point O = arc.CoordinateSystem.Origin; //--
            double r = arc.Radius;  //--
            //double add = Vector.XAxis.Angle(arc.CoordinateSystem.X, Plane.XY);
            Point start = arc.StartPoint();
            //Point mid = arc.ControlPoints()[2];
            Point end = arc.EndPoint();

            double a = Vector.XAxis.Angle(start - O, Plane.XY);

            double k = Math.Abs(arc.Angle()); //--    * factor
            if ((start - O).Angle(end - O, Plane.XY) - k > Tolerance.Angle)
                k *= -1;

            //double factor = arc.CoordinateSystem.Z.Normalise().Z;


            //double a = (arc.StartAngle + add) * factor;

            switch (powX)
            {
                case 0:
                    return (O.X * r * (-Math.Sin(a) + Math.Sin(a + k)) + (r * r * (2 * k - Math.Sin(2 * a) + Math.Sin(2 * (a + k)))) / 4);
                /********************/
                case 1:
                    return (r * ((Math.Sin(3 * (k + a)) + 9 * Math.Sin(k + a) - Math.Sin(3 * a) - 9 * Math.Sin(a)) * r * r + 6 * (Math.Sin(2 * (k + a)) + 2 * k - Math.Sin(2 * a)) * O.X * r + 12 * (Math.Sin(k + a) - Math.Sin(a)) * O.X * O.X)) / 24;
                /********************/
                case 2:
                    return (r * ((Math.Sin(4 * (k + a)) + 8 * Math.Sin(2 * (k + a)) + 12 * k - Math.Sin(4 * a) - 8 * Math.Sin(2 * a)) * r * r * r - 32 * (Math.Sin(k + a) * (Math.Sin(k + a) * Math.Sin(k + a) - 3) + (Math.Cos(a) * Math.Cos(a) + 2) * Math.Sin(a)) * O.X * r * r + 24 * (Math.Sin(2 * (k + a)) + 2 * k - Math.Sin(2 * a)) * O.X * O.X * r + 32 * (Math.Sin(k + a) - Math.Sin(a)) * O.X * O.X * O.X)) / 96;
                /********************/
                default:
                    return 0;
            }

            /**********/
            /*
             * 
             * 
             * 
            (r(12 k r(4 O ^ 2 + r ^ 2) Cos[a] + 16 O(2 O ^ 2 + 3 r ^ 2) Sin[k] + r(24 O r Sin[2 a + k] + 6(4 O ^ 2 + r ^ 2) Sin[a + 2 k] + r(2 r Sin[3 a + 2 k] + 8 O Sin[2 a + 3 k] + r Sin[3 a + 4 k])))) / 32 -
                (r(24 O r Sin[2 a] + 6(4 O ^ 2 + r ^ 2) Sin[a] + r(2 r Sin[3 a] + 8 O Sin[2 a] + r Sin[3 a])))) / 32;

            (r * (12 * k * r * (4 * O.X*O.X + r*r) * Math.Cos(a) + 16 * O.X * (2 * O.X*O.X + 3 * r*r) * Math.Sin(k) + r * (24 * O.X * r * Math.Sin(2 * a + k) + 6 * (4 * O.X*O.X + r*r) * Math.Sin(a + 2 * k) +r * (2 * r * Math.Sin(3 * a + 2 * k) + 8 * O.X * Math.Sin(2 * a + 3 * k) + r * Math.Sin(3 * a + 4 * k))))) / 32 - 
                (r * (24 * O.X * r * Math.Sin(2 * a) + 6 * (4 * O.X*O.X + r*r) * Math.Sin(a) + r * (2 * r * Math.Sin(3 * a) + 8 * O.X * Math.Sin(2 * a) + r * Math.Sin(3 * a))))) / 32;
                */
            /***********************************/
            // make into a pLine
            List<Point> pts = new List<Point>();
            double res = (10 * arc.Angle());
            if (res < 1)
                res = 1;
            res = Math.Floor(res);
            for (double i = 0; i <= res; i++)
                pts.Add(arc.PointAtParameter((i / res)));

            Polyline pLine = new Polyline() { ControlPoints = pts };

            // call IntegrateRegion(pLine);
            return IntegrateRegion(pLine, powX);
        }

        /***************************************************/

        private static double IntegrateRegion(Circle circle, int powX)
        {
            // Add tests (?)

            double r = circle.Radius;
            Point O = circle.Centre;

            switch (powX)
            {
                case 0:
                    return Math.PI * r * r;
                /********************/
                case 1:
                    return r * r * Math.PI * O.X;
                /********************/
                case 2:
                    return 0.25 * r * r * Math.PI * (4 * O.X * O.X + r * r);
                /********************/
                default:
                    return 0;
            }
        }

        /***************************************************/

        private static double IntSurfLine(Point a, Point b, int powX)
        {
            //TODO Should do some checks if these are good Tolerances
            //TODO powX could be a double, but that might slow thing down somewhat

            double Y = (a.Y - b.Y);
            /***************/
            if (Math.Abs(Y) < Tolerance.Distance)
                return 0;
            /***************/
            double X = (a.X - b.X);
            if (Math.Abs(X) < Tolerance.Distance)
                return -(Math.Pow(a.X, powX + 1) * Y) / (powX + 1);
            /***************/
            double N = (powX + 1) * (powX + 2);
            double bigX = (Math.Pow(b.X, powX + 2) - Math.Pow(a.X, powX + 2));

            return Y * bigX / (N * X);
            /***************/

            // Still use these for speed??
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
                default:
                    return 0;
            }
        }

        /***************************************************/

        public static double ShearAreaPolyline(Polyline pLine)
        {
            // the Polyline should have the upper side along the x-axis and the rest of the lines should be defineble as a function of x apart for veritcal segments
            // The last LineSegment should be the upper one
            double Sy = 0;
            double ShearArea = 0;
            // Sy = 0
            List<Point> controllPoints = new List<Point>(pLine.ControlPoints);  // Should the formatting happen here or earlier??
            // foreach (linesegement) except for the upper one, must begin at the line after the upper one (on the left side)
            for (int i = 0; i < controllPoints.Count - 2; i++)
            {
                ShearArea += ShearAreaLine(controllPoints[i], controllPoints[i + 1], Sy);
                Sy += IntSurfLine(controllPoints[i], controllPoints[i + 1], 1);
            }
            // ShearAreaLine()
            // Calculate Sy for the linesegment (by IntSurfLine()) and add to Sy +=

            return ShearArea;
        }

        /***************************************************/

        private static double ShearAreaLine(Point a, Point b, double s)
        {
            //TODO Should do some checks if these are good Tolerances


            // a.Y & b.Y can't be 0.    (due to some log expresssions, which comes from the expression being divided by the width (can't divide by zero))

            double axbx = a.X - b.X;
            if (Math.Abs(axbx) < Tolerance.Distance)  // The solution is zero
                return 0;

            double byay = b.Y - a.Y;
            double ax3 = Math.Pow(a.X, 3);
            double ax2 = Math.Pow(a.X, 2);
            double ay2 = Math.Pow(a.Y, 2);

            if (Math.Abs(byay) < Tolerance.Distance)  // The solution for a "constant" integral, i.e. horizontal line
            {
                //$$ \frac{(x-z)(10 y (x-z)^2 (3 x^2 y-2 s)-30 x y (x-z) (x^2 y-2 s)+15 (x^2 y-2 s)^2+3 y^2 (x-z)^4-15 x y^2 (x-z)^3)}{60 y} $$
                return (axbx) * (
                        10 * a.Y * Math.Pow(axbx, 2) * (3 * ax2 * a.Y - 2 * s)
                        - 30 * a.X * a.Y * axbx * (ax2 * a.Y - 2 * s)
                        + 15 * Math.Pow(ax2 * a.Y - 2 * s, 2)
                        + 3 * ay2 * Math.Pow(axbx, 4)
                        - 15 * a.X * ay2 * Math.Pow(axbx, 3)
                    ) / (60 * a.Y);
            }

            double ax4 = Math.Pow(a.X, 4);
            double bx2 = Math.Pow(b.X, 2);
            double bx3 = Math.Pow(b.X, 3);
            double bx4 = Math.Pow(b.X, 4);

            double ay3 = Math.Pow(a.Y, 3);
            double ay4 = Math.Pow(a.Y, 4);
            double by2 = Math.Pow(b.Y, 2);
            double by3 = Math.Pow(b.Y, 3);
            double by4 = Math.Pow(b.Y, 4);

            //...
            double byay2 = Math.Pow(byay, 2);

            // WIP below here

            double A =
                    -(20 * Math.Pow(axbx, 2) * (24 * s * byay2 + a.Y * (-39 * by2 * ax2 + 6 * b.Y * a.X * a.Y * (14 * a.X - b.X) + ay2 * (-44 * ax2 + 4 * a.X * b.X + bx2))))
                    / (byay2);
            double B =
                    -(30 * axbx * (a.Y * (18 * by3 * ax3 + 3 * by2 * ax2 * a.Y * (5 * b.X - 23 * a.X) + 6 * b.Y * a.X * ay2 * (13 * ax2 - 3 * a.X * b.X - bx2) + ay3 * (-28 * ax3 + 6 * ax2 * b.X + 3 * a.X * bx2 + bx3)) - 12 * s * byay2 * (3 * b.Y * a.X + a.Y * (b.X - 4 * a.X))))
                    / (Math.Pow(byay, 3));
            double C =
                    (60 * a.Y * axbx * (a.Y * (2 * a.X + b.X) - 3 * b.Y * a.X) * (a.Y * (6 * by2 * ax2 - 3 * b.Y * a.X * a.Y * (3 * a.X + b.X) + ay2 * (4 * ax2 + a.X * b.X + bx2)) - 12 * s * byay2))
                    / (Math.Pow(byay, 4));
            double D =
                    (60 * Math.Log(b.Y / a.Y) * Math.Pow(a.Y * (3 * by2 * ax2 - 3 * b.Y * a.X * a.Y * (a.X + b.X) + ay2 * (ax2 + a.X * b.X + bx2)) - 6 * s * byay2, 2))
                    / (Math.Pow(byay, 5));
            double E =
                    40 * byay * Math.Pow(axbx, 4) - 48 * Math.Pow(axbx, 3) * (3 * b.Y * a.X - 5 * a.X * a.Y + 2 * a.Y * b.X)

                    + (15 * Math.Pow(axbx, 2) * (9 * by2 * ax2 - 6 * b.Y * a.X * a.Y * (8 * a.X - 5 * b.X) + ay2 * (40 * ax2 - 32 * a.X * b.X + bx2)))
                    / (byay);

            double factor = (axbx / 2160);

            return (A + B + C + D + E) * factor;

            // (You could perhaps simplyfy this further), think it's as good as it's gonna get apart form spliting it up into varibles
            //return axbx * (40 * Math.Pow(b.Y - 2 * a.Y, 2) * Math.Pow(axbx, 4) * Math.Pow(byay, 6) - 48 * (b.Y - 2 * a.Y) * Math.Pow(axbx, 2) * (3 * a.X * axbx * by2 - 2 * a.Y * (7 * ax2 - 5 * b.X * a.X + bx2) * b.Y + ay2 * (10 * ax2 - 5 * b.X * a.X + bx2)) * Math.Pow(byay, 5) + 15 * (9 * ax2 * Math.Pow(axbx, 2) * by2 - 6 * a.X * a.Y * (17 * ax3 - (29 * b.X + 4) * ax2 + b.X * (13 * b.X + 8) * a.X - bx2 * (b.X + 4)) * by3 + ay2 * (313 * ax4 - 2 * (227 * b.X + 48) * ax3 + 6 * b.X * (31 * b.X + 32) * ax2 - 2 * bx2 * (5 * b.X + 48) * a.X + bx4) * by2 - 2 * ay3 * (164 * ax4 - (197 * b.X + 60) * ax3 + 3 * b.X * (21 * b.X + 40) * ax2 + bx2 * (7 * b.X - 60) * a.X - bx4) * b.Y + ay4 * (112 * ax4 - 16 * (7 * b.X + 3) * ax3 + 3 * b.X * (11 * b.X + 32) * ax2 + 2 * (b.X - 24) * bx2 * a.X + bx4)) * Math.Pow(byay, 4) + 20 * (a.Y * (3 * ax2 * (17 * ax2 - 2 * (11 * b.X + 6) * a.X + b.X * (5 * b.X + 12)) * by4 - 6 * a.X * a.Y * (45 * ax3 - (47 * b.X + 38) * ax2 + b.X * (13 * b.X + 28) * a.X + (b.X - 2) * bx2) * by3 + ay2 * (443 * ax4 - 2 * (205 * b.X + 222) * ax3 + 6 * b.X * (23 * b.X + 40) * ax2 + 2 * bx2 * (5 * b.X - 6) * a.X - bx4) * by2 - 2 * ay3 * (154 * ax4 - (151 * b.X + 174) * ax3 + 15 * b.X * (5 * b.X + 4) * ax2 + (6 - 7 * b.X) * bx2 * a.X + bx4) * b.Y + ay4 * (80 * ax4 - 4 * (23 * b.X + 24) * ax3 + 3 * b.X * (17 * b.X + 4) * ax2 - 2 * (b.X - 6) * bx2 * a.X - bx4)) - 24 * s * (b.Y - 2 * a.Y) * Math.Pow(byay, 3) * Math.Pow(axbx, 2)) * Math.Pow(byay, 3) - 30 * (-12 * s * (3 * a.X * axbx * by2 - a.Y * (13 * ax2 - 8 * b.X * a.X + bx2) * b.Y + ay2 * (8 * ax2 - b.X * a.X - bx2)) * Math.Pow(byay, 3) - a.Y * (-36 * ax3 * axbx * Math.Pow(b.Y, 5) + 3 * ax2 * a.Y * (91 * ax2 - 2 * (31 * b.X + 6) * a.X + 7 * bx2 - 12 * b.X + 12) * by4 - 6 * a.X * ay2 * (111 * ax3 - (61 * b.X + 10) * ax2 + (11 * bx2 - 28 * b.X + 24) * a.X - (b.X - 2) * bx2) * by3 + ay3 * (781 * ax4 + (12 - 382 * b.X) * ax3 + 6 * (13 * bx2 - 40 * b.X + 36) * ax2 - 2 * bx2 * (5 * b.X - 6) * a.X + bx4) * by2 - 2 * ay4 * (224 * ax4 + (30 - 83 * b.X) * ax3 - 3 * (bx2 + 20 * b.X - 24) * ax2 + bx2 * (7 * b.X - 6) * a.X - bx4) * b.Y + Math.Pow(a.Y, 5) * (100 * ax4 - 8 * (2 * b.X - 3) * ax3 - 3 * (5 * bx2 + 4 * b.X - 12) * ax2 + 2 * (b.X - 6) * bx2 * a.X + bx4))) * Math.Pow(byay, 2) - 60 * a.Y * (3 * a.X * (a.X + b.X - 2) * by2 + a.Y * (ax2 + (12 - 8 * b.X) * a.X + bx2) * b.Y + ay2 * (-2 * ax2 + (b.X - 6) * a.X + bx2)) * (a.Y * (12 * ax2 * by3 + 3 * a.X * a.Y * (-11 * a.X + b.X - 2) * by2 + ay2 * (37 * ax2 + (12 - 8 * b.X) * a.X + bx2) * b.Y + ay3 * (-14 * ax2 + (b.X - 6) * a.X + bx2)) - 12 * s * Math.Pow(byay, 3)) * (byay) + (60 * Math.Pow(6 * s * Math.Pow(byay, 3) + a.Y * (-6 * ax2 * by3 + 3 * a.X * a.Y * (5 * axbx + 2) * by2 - ay2 * (19 * ax2 - 8 * b.X * a.X + 12 * a.X + bx2) * b.Y + ay3 * (8 * ax2 - b.X * a.X + 6 * a.X - bx2)), 2)) * Math.Log(Math.Abs(b.Y/a.Y))) / (60 * Math.Pow(byay, 7));
            // double check which kind of log to use (99% it is the natural one, and adding the abs might be unnessecary)
        }

        /***************************************************/

    }
}