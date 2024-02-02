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
using System;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Compute
    {
        /***************************************************/
        /**** public Methods - Vectors                  ****/
        /***************************************************/

        public static Line FitLine(this IEnumerable<Point> points, double tolerance = Tolerance.Distance)
        {
            List<Point> asList = points.ToList();
            int n = asList.Count;
            if (n < 2)
                return null;

            // Three cases: collinear, coplanar in XYZ and general covered separately
            // Required due to the fact that general solution degrades to zero length line for edge cases
            Point C = points.Average();
            if (asList.IsCollinear(tolerance))
            {
                List<Point> sorted = asList.SortCollinear(tolerance);
                return new Line { Start = sorted[0], End = sorted[sorted.Count - 1] };
            }
            else if (points.All(x => Math.Abs(C.Z - x.Z) < tolerance))
            {
                // Based on https://iojes.net/Makaleler/386bbbe6-b98d-4766-a717-2c945a97f267.pdf
                double sumX = points.Sum(x => x.X);
                double sumY = points.Sum(x => x.Y);
                double sumXsq = points.Sum(x => x.X * x.X);
                double sumYsq = points.Sum(x => x.Y * x.Y);
                double sumXY = points.Sum(x => x.X * x.Y);
                double p1 = sumX * sumX - sumY * sumY - n * (sumXsq - sumYsq);
                double p2 = n * sumXY - sumX * sumY;
                double b = (p1 + Math.Sqrt(p1 * p1 + 4 * p2 * p2)) / (2 * p2);
                Vector dir = new Vector { X = 1, Y = b };
                return new Line { Start = C, End = C + dir };
            }
            else
            {
                // Based on https://www.scribd.com/doc/31477970/Regressions-et-trajectoires-3D
                double xx = 0.0; double xy = 0.0; double xz = 0.0;
                double yy = 0.0; double yz = 0.0; double zz = 0.0;

                foreach (Point P in points)
                {
                    xx += P.X * P.X;
                    xy += P.X * P.Y;
                    xz += P.X * P.Z;
                    yy += P.Y * P.Y;
                    yz += P.Y * P.Z;
                    zz += P.Z * P.Z;
                }

                double Sxx = xx / n - C.X * C.X;
                double Sxy = xy / n - C.X * C.Y;
                double Sxz = xz / n - C.X * C.Z;
                double Syy = yy / n - C.Y * C.Y;
                double Syz = yz / n - C.Y * C.Z;
                double Szz = zz / n - C.Z * C.Z;

                double theta = Math.Atan(2 * Sxy / (Sxx - Syy)) * 0.5;
                double stheta = Math.Sin(theta);
                double ctheta = Math.Cos(theta);
                double K11 = (Syy + Szz) * ctheta * ctheta + (Sxx + Szz) * stheta * stheta - 2 * Sxy * ctheta * stheta;
                double K22 = (Syy + Szz) * stheta * stheta + (Sxx + Szz) * ctheta * ctheta + 2 * Sxy * ctheta * stheta;
                double K12 = -Sxy * (ctheta * ctheta - stheta * stheta) + (Sxx - Syy) * ctheta * stheta;
                double K10 = Sxz * ctheta + Syz * stheta;
                double K01 = -Sxz * stheta + Syz * ctheta;
                double K00 = Sxx + Syy;

                double c0 = K01 * K01 * K11 + K10 * K10 * K22 - K00 * K11 * K22;
                double c1 = K00 * K11 + K00 * K22 + K11 * K22 - K01 * K01 - K10 * K10;
                double c2 = -K00 - K11 - K22;

                double p = c1 - c2 * c2 / 3;
                double q = c2 * c2 * c2 * 2 / 27 - c1 * c2 / 3 + c0;
                double R = q * q * 0.25 + p * p * p / 27;

                double sqrDeltaM;
                double cc = -c2 / 3;

                if (R > tolerance)
                    sqrDeltaM = cc + Math.Pow(-q * 0.5 + Math.Sqrt(R), 1.0 / 3.0) + Math.Pow(-q * 0.5 - Math.Sqrt(R), 1.0 / 3.0);
                else
                {
                    double rho = Math.Sqrt(-p * p * p / 27);
                    double fi = Math.Acos(-q / rho * 0.5);
                    double doubleRhoRoot = 2 * Math.Pow(rho, 1.0 / 3.0);
                    double minCos = Math.Min(Math.Cos(fi / 3), Math.Min(Math.Cos((fi + 2 * Math.PI) / 3), Math.Cos((fi + 4 * Math.PI))));
                    sqrDeltaM = cc + doubleRhoRoot * minCos;
                }

                double a = -K10 / (K11 - sqrDeltaM) * ctheta + K01 / (K22 - sqrDeltaM) * stheta;
                double b = -K10 / (K11 - sqrDeltaM) * stheta - K01 / (K22 - sqrDeltaM) * ctheta;
                double u = ((1 + b * b) * C.X - a * b * C.Y + a * C.Z) / (1 + a * a + b * b);
                double v = (-a * b * C.X + (1 + a * a) * C.Y + b * C.Z) / (1 + a * a + b * b);
                double w = (a * C.X + b * C.Y + (a * a + b * b) * C.Z) / (1 + a * a + b * b);

                Point H = new Point { X = u, Y = v, Z = w };
                return new Line { Start = C + (C - H), End = H };
            }
        }

        /***************************************************/
    }
}
