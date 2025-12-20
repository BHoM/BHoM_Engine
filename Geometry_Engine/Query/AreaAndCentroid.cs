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

using BH.oM.Base;
using BH.oM.Base.Attributes;
using BH.oM.Geometry;
using BH.oM.Geometry.CoordinateSystem;
using BH.oM.Quantities.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Calculates both the area and centroid of a closed, planar NurbsCurve using numerical integration.")]
        [Input("curve", "The NurbsCurve to calculate area and centroid for. Must be closed and planar.")]
        [Input("tolerance", "The tolerance used for geometric calculations.")]
        [MultiOutput(0, "centroid", "The centroid point of the curve.")]
        [MultiOutput(1, "area", "The area enclosed by the curve.", typeof(Area))]
        public static Output<Point, double> AreaAndCentroid(this NurbsCurve curve, double tolerance = Tolerance.Distance)
        {
            if (curve == null)
            {
                BH.Engine.Base.Compute.RecordError("Can't compute area and centroid of a null curve.");
                return new Output<Point, double> { Item1 = null, Item2 = double.NaN };
            }

            if (!curve.IsClosed(tolerance))
            {
                Base.Compute.RecordError("Curve is not closed, cannot evaluate area and centroid.");
                return new Output<Point, double> { Item1 = null, Item2 = double.NaN };
            }

            Plane plane = curve.ControlPoints.FitPlane(tolerance);
            if (curve.ControlPoints.Any(p => p.Distance(plane) > tolerance))
            {
                Base.Compute.RecordError("Curve is not planar, cannot evaluate area and centroid.");
                return new Output<Point, double> { Item1 = null, Item2 = double.NaN };
            }

            // Orient the curve to the XY plane
            Vector localX = Vector.XAxis;
            if (1 - Math.Abs(localX.DotProduct(plane.Normal)) < tolerance)
                localX = Vector.YAxis;

            localX = localX.CrossProduct(plane.Normal);
            Vector localY = plane.Normal.CrossProduct(localX);
            Cartesian cs = new Cartesian(plane.Origin, localX, localY, plane.Normal);
            TransformMatrix orientationToXY = cs.OrientationMatrix(new Cartesian());

            curve = curve.Transform(orientationToXY);

            Vector x;
            Point origin = curve.ControlPoints[0];
            int n = 1;
            do
            {
                x = curve.ControlPoints[n] - origin;
                n++;
            } while (x.SquareLength() < tolerance * tolerance && n < curve.ControlPoints.Count);

            // Level equal to 100 based on empirical testing and discussion with @isaknaslundbh
            int level = 100;

            double totalA = 0;
            double totalX = 0;
            double totalY = 0;

            Output<List<double>, List<double>> gausPairs = curve.Knots.GaussPairs(curve.Degree(), level);
            List<double> values = gausPairs.Item1;
            List<double> weights = gausPairs.Item2;

            List<List<Vector>> ders = curve.DerivativesAtParameters(1, values, false);

            for (int i = 0; i < weights.Count; i++)
            {
                List<Vector> der = ders[i];
                Vector v = der[0];
                Vector v_p = der[1];
                double w = weights[i];

                totalA += 0.5 * (v.X * v_p.Y - v.Y * v_p.X) * w;
                totalX += 0.5 * (v.X * v.X * v_p.Y) * w;
                totalY += -0.5 * (v.Y * v.Y * v_p.X) * w;
            }

            Point centroid = new Point { X = totalX / totalA, Y = totalY / totalA }.Transform(orientationToXY.Invert());
            return new Output<Point, double> { Item1 = centroid, Item2 = Math.Abs(totalA) };
        }

        /***************************************************/

        [Description("Calculates both the area and centroid of a NurbsSurface using numerical integration. Trimmed surfaces are not supported.")]
        [Input("surface", "The NurbsSurface to calculate area and centroid for. Must not have trim curves.")]
        [MultiOutput(0, "centroid", "The centroid point of the surface.")]
        [MultiOutput(1, "area", "The area of the surface.", typeof(Area))]
        public static Output<Point, double> AreaAndCentroid(this NurbsSurface surface)
        {
            if (surface == null)
            {
                BH.Engine.Base.Compute.RecordError("Can't compute area and centroid of a null surface.");
                return new Output<Point, double> { Item1 = null, Item2 = double.NaN };
            }

            if (surface.InnerTrims.Count != 0 || surface.OuterTrims.Count != 0)
            {
                Base.Compute.RecordError("Trimmed surfaces are not supported, cannot evaluate area and centroid.");
                return new Output<Point, double> { Item1 = null, Item2 = double.NaN };
            }

            // Level equal to 100 based on empirical testing and discussion with @isaknaslundbh
            int level = 100;

            Output<List<double>, List<double>> uGausPairs = surface.UKnots.GaussPairs(surface.UDegree, level);
            Output<List<double>, List<double>> vGausPairs = surface.VKnots.GaussPairs(surface.VDegree, level);

            double total = 0;
            Point centroid = new Point();

            for (int i = 0; i < uGausPairs.Item1.Count; i++)
            {
                for (int j = 0; j < vGausPairs.Item1.Count; j++)
                {
                    List<List<Vector>> der = surface.DerivativesAtParameter(1, uGausPairs.Item1[i], vGausPairs.Item1[j], false);
                    Vector cross = der[0][1].CrossProduct(der[1][0]);
                    double area = cross.Length() * uGausPairs.Item2[i] * vGausPairs.Item2[j];
                    centroid += der[0][0] * area;
                    total += area;
                }
            }

            return new Output<Point, double> { Item1 = centroid / total, Item2 = total };
        }
    }
}

