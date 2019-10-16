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
            switch (powX)
            {
                case 0:
                    return -((a.X + b.X) * (a.Y - b.Y)) * 0.5;
                /********************/
                case 1:
                    return ((a.X * a.X + a.X * b.X + b.X * b.X) * (-a.Y + b.Y)) / 6;
                /********************/
                case 2:
                    return -((a.X + b.X) * (a.X * a.X + b.X * b.X) * (a.Y - b.Y)) / 12;
                /********************/
                default:
                    return 0;
            }
        }

        /***************************************************/

    }
}