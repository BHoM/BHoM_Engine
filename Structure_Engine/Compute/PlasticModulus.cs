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

        public static double PlasticModulus(this Polyline pLine, double area)
        {
            // the Polyline should have the upper side along the x-axis and the rest of the lines should be defineble as a function of x apart for veritcal segments
            // The last LineSegment should be the upper one
            // use WetBlanketInterpertation()

            int index;
            double halfArea = area * 0.5;

            double neutralAxis = PlasticNeutralAxis(pLine, halfArea, out index);

            // Create the half regions to find their pivot point
            if (index == -1)
            {
                Engine.Reflection.Compute.RecordError("Index out of range, PlasticModulus set to 0");
                return 0;
            }
            Polyline lower = new Polyline() { ControlPoints = pLine.ControlPoints.GetRange(0, index) };
            // add capping points
            lower.ControlPoints.Add(Engine.Geometry.Compute.PointAtX(pLine.ControlPoints[index - 1], pLine.ControlPoints[index], neutralAxis));
            lower.ControlPoints.Add(new Point() { X = neutralAxis });

            Polyline upper = new Polyline() { ControlPoints = pLine.ControlPoints.GetRange(index, pLine.ControlPoints.Count - index) };
            // add capping points
            upper.ControlPoints.Insert(0, Engine.Geometry.Compute.PointAtX(pLine.ControlPoints[index - 1], pLine.ControlPoints[index], neutralAxis));
            upper.ControlPoints.Insert(0, new Point() { X = neutralAxis });

            double lowerCenter = Engine.Geometry.Compute.IIntegrateRegion(lower, 1) / halfArea;
            double upperCenter = Engine.Geometry.Compute.IIntegrateRegion(upper, 1) / halfArea;

            return halfArea * Math.Abs(upperCenter - neutralAxis) + halfArea * Math.Abs(lowerCenter - neutralAxis);
        }

        /***************************************************/

        private static double PlasticNeutralAxis(this Polyline pLine, double halfArea, out int index)
        {
            double partialArea = 0;

            for (int i = 0; i < pLine.ControlPoints.Count - 2; i++)
            {
                Point first = pLine.ControlPoints[i];
                Point second = pLine.ControlPoints[i + 1];

                double currentLineArea = Engine.Geometry.Compute.IIntegrateRegion(new Line() { Start = first, End = second }, 0);
                double currentCapArea = Engine.Geometry.Compute.IIntegrateRegion(new Line() { Start = second, End = new Point() { X = second.X } }, 0);

                if (partialArea + currentLineArea + currentCapArea < halfArea)
                {
                    partialArea += currentLineArea;
                }
                else
                {
                    index = i + 1;

                    double x = first.X;
                    double z = pLine.ControlPoints[i + 1].X;

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

                    bool between1 = (x1 >= x && x1 < z);
                    bool between2 = (x2 >= x && x2 < z);

                    if (between1 && between2)           //TODO test if its always x1 or x2 or whatever
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
            index = -1;
            return double.NaN;
        }

        /***************************************************/

    }
}
