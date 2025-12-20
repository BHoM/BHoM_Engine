/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2026, the respective contributors. All rights reserved.
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

using BH.oM.Base.Attributes;
using BH.oM.Geometry;
using BH.oM.Quantities.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace BH.Engine.Geometry
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods - interfaces               ****/
        /***************************************************/

        [Description("Integrates a closed region with x to the specified power on the XY-Plane.")]
        [Input("curve", "Defined counter clockwise on the XY-Plane.")]
        [Input("powX", "The region will be evaluated under the function: x^(powX).")]
        [Input("tol", "The tolerance for considering a line segment horizontal or vertical. /n" +
                      "i.e. (value at endpoint - value at startpoint) < tol.")]
        [Output("V", "Calculated value.")]
        public static double IIntegrateRegion(this ICurve curve, int powX, double tol = Tolerance.Distance)
        {
            // Add tests (?)

            return IntegrateRegion(curve as dynamic, powX, tol);
        }


        /***************************************************/
        /**** Private Fallback  Methods                 ****/
        /***************************************************/

        private static double IntegrateRegion(ICurve curve, int powX, double tol = Tolerance.Distance)
        {
            Base.Compute.RecordError($"IntegrateRegion is not implemented for a ICurves of type: {curve.GetType().Name}.");
            return double.NaN;
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static double IntegrateRegion(PolyCurve curve, int powX, double tol = Tolerance.Distance)
        {
            if (curve.Curves.Count == 0)
                return 0;

            double result = 0;

            foreach (ICurve c in curve.Curves)
            {
                result += IntegrateRegion(c as dynamic, powX, tol);
            }

            return result;
        }

        /***************************************************/

        private static double IntegrateRegion(Line line, int powX, double tol = Tolerance.Distance)
        {
            return IntSurfLine(line.Start, line.End, powX, tol);
        }

        /***************************************************/

        private static double IntegrateRegion(Polyline pLine, int powX, double tol = Tolerance.Distance)
        {
            List<Point> pts = pLine.ControlPoints;

            double result = 0;

            for (int i = 0; i < pts.Count - 1; i++)
                result += IntSurfLine(pts[i], pts[i + 1], powX, tol);

            return result;
        }

        /***************************************************/

        private static double IntegrateRegion(Arc arc, int powX, double tol = Tolerance.Distance)
        {
            Point centre = arc.CoordinateSystem.Origin;
            double r = arc.Radius;
            Point start = arc.StartPoint();
            Point end = arc.EndPoint();

            double a = Vector.XAxis.Angle(start - centre, Plane.XY);

            double k = Math.Abs(arc.Angle());
            if ((start - centre).CrossProduct(arc.StartDir()).Z < 0)
                k *= -1;

            switch (powX)
            {
                case 0:
                    return (
                                centre.X * r * (-Math.Sin(a) + Math.Sin(a + k)) + 
                                (r * r * (2 * k - Math.Sin(2 * a) + Math.Sin(2 * (a + k)))
                            ) / 4);
                /********************/
                case 1:
                    return (r * (
                                (Math.Sin(3 * (k + a)) + 9 * Math.Sin(k + a) - 
                                 Math.Sin(3 * a) - 9 * Math.Sin(a)) * r * r + 
                                6 * (Math.Sin(2 * (k + a)) + 2 * k - Math.Sin(2 * a)) * centre.X * r + 
                                12 * (Math.Sin(k + a) - Math.Sin(a)) * centre.X * centre.X
                            )) / 24;
                /********************/
                case 2:
                    return (r * (
                                (Math.Sin(4 * (k + a)) + 8 * Math.Sin(2 * (k + a)) + 12 * k - Math.Sin(4 * a) - 
                                8 * Math.Sin(2 * a)) * r * r * r - 
                                32 * (Math.Sin(k + a) * (Math.Sin(k + a) * Math.Sin(k + a) - 3) + 
                                (Math.Cos(a) * Math.Cos(a) + 2) * Math.Sin(a)) * centre.X * r * r + 
                                24 * (Math.Sin(2 * (k + a)) + 2 * k - Math.Sin(2 * a)) * centre.X * centre.X * r + 
                                32 * (Math.Sin(k + a) - Math.Sin(a)) * centre.X * centre.X * centre.X
                            )) / 96;
                /********************/
                default:
                    return IntegrateRegion(arc.CollapseToPolyline(0.01), powX, tol); //TODO is this good value??
            }
        }

        /***************************************************/

        private static double IntegrateRegion(Circle circle, int powX, double tol = Tolerance.Distance)
        {
            double r = circle.Radius;
            Point centre = circle.Centre;
            int flip = (int)(circle.Normal.Z / Math.Abs(circle.Normal.Z));

            switch (powX)
            {
                case 0:
                    return Math.PI * r * r * flip;
                /********************/
                case 1:
                    return r * r * Math.PI * centre.X * flip;
                /********************/
                case 2:
                    return 0.25 * r * r * Math.PI * (4 * centre.X * centre.X + r * r) * flip;
                /********************/
                default:
                    return IntegrateRegion(circle.CollapseToPolyline(0.01), powX, tol); //TODO is this good value??
            }
        }

        /***************************************************/
    }
}
