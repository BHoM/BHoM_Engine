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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Structure.Elements;
using BH.oM.Geometry;
using BH.Engine.Geometry;
using System.ComponentModel;
using BH.oM.Reflection.Attributes;

namespace BH.Engine.Structure
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static double PlasticModulus(this Polyline pLine, IEnumerable<ICurve> curves, double trueArea)
        {
            // the Polyline should have the upper side along the x-axis and the rest of the lines should be defineble as a function of x apart for veritcal segments
            // The last LineSegment should be the upper one
            // use WetBlanketInterpertation()

            double area = BH.Engine.Geometry.Compute.IIntegrateRegion(pLine, 0);   //Should be calculated here for consistency
            
            double halfArea = area * 0.5;
            double halfTrueArea = trueArea * 0.5;

            double neutralAxis = PlasticNeutralAxis(pLine, halfArea);

            if (double.IsNaN(neutralAxis))
            {
                Engine.Reflection.Compute.RecordError("NeutralAxis not found, PlasticModulus set to 0");
                return 0;
            }

            // Create the half regions to find their pivot point
            Line splitLine = new Line() { Start = new Point() { X = neutralAxis }, End = new Point() { X = neutralAxis, Y = 1 } };
            List<ICurve> splitCurve = new List<ICurve>();
            foreach (ICurve curve in curves)
                splitCurve.AddRange(curve.ISplitAtPoints(curve.ILineIntersections(splitLine, true, Tolerance.MicroDistance), Tolerance.Distance));

            double lowerCenter = 0;
            double upperCenter = 0;
            foreach (ICurve curve in splitCurve)
            {
                if (curve.IPointAtParameter(0.5).X < neutralAxis)
                    lowerCenter += curve.Close().IIntegrateRegion(1);
                else
                    upperCenter += curve.Close().IIntegrateRegion(1);
            }

            lowerCenter /= halfTrueArea;
            upperCenter /= halfTrueArea;

            return halfTrueArea * Math.Abs(upperCenter - neutralAxis) + halfTrueArea * Math.Abs(lowerCenter - neutralAxis);
        }

        /***************************************************/

        private static ICurve Close(this ICurve curve, double tol = Tolerance.Distance)
        {
            Point start = curve.IStartPoint();
            Point end = curve.IEndPoint();

            if (start.SquareDistance(end) > tol * tol)
                return new PolyCurve() { Curves = new List<ICurve>() { curve, new Line() { Start = end, End = start } } };

            return curve.IClone();
        }

        /***************************************************/

        private static double PlasticNeutralAxis(this Polyline pLine, double halfArea)
        {
            double partialArea = 0;

            for (int i = 0; i < pLine.ControlPoints.Count - 2; i++)
            {
                Point first = pLine.ControlPoints[i];
                Point second = pLine.ControlPoints[i + 1];

                double currentLineArea = Engine.Geometry.Compute.IntSurfLine(first, second, 0);
                double currentCapArea = Engine.Geometry.Compute.IntSurfLine(second, new Point() { X = second.X }, 0);

                if (partialArea + currentLineArea + currentCapArea < halfArea)
                {
                    partialArea += currentLineArea;
                }
                else
                {
                    double x = first.X;
                    double z = second.X;

                    double zx = z - x;
                    if (Math.Abs(zx) < Tolerance.Distance)  //Vertical Line
                        return x;

                    double y = first.Y;
                    double u = second.Y;

                    double uy = u - y;
                    if (Math.Abs(uy) < Tolerance.Distance)  //Horizontal Line
                        return ((halfArea - partialArea) / -y);

                    double a = -(uy) / (2 * (zx));
                    double b = ((uy) * x - y * (zx)) / (zx);
                    double c = -Math.Pow(x, 2) * (uy) / (2 * (zx)) - (halfArea - partialArea);

                    // quadratic formula
                    double sqrt = Math.Sqrt(Math.Pow(b, 2) - 4 * a * c);
                    double x1 = (-b + sqrt) / (2 * a);
                    double x2 = (-b - sqrt) / (2 * a);

                    bool between1 = (x1 >= x - Tolerance.MicroDistance && x1 < z + Tolerance.MicroDistance);
                    bool between2 = (x2 >= x - Tolerance.MicroDistance && x2 < z + Tolerance.MicroDistance);

                    if (between1 && between2)           //TODO test if its always x1 or x2 or whatever (99% that it's x1)
                    {
                        Engine.Reflection.Compute.RecordWarning("two solutions: x1 = " + x1 + " x2 = " + x2);
                        return double.NaN;
                    }
                    else if (between1)
                        return x1;
                    else if (between2)
                        return x2;
                    else
                    {
                        Engine.Reflection.Compute.RecordWarning("both solutions invalid, x1 = " + x1 + " x2 = " + x2);
                        return double.NaN;
                    }
                }
            }
            return double.NaN;
        }

        /***************************************************/

    }
}
