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
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Integrates according to Green's Theorem between two points with x to the specified power on the XY-Plane.")]
        [Input("a", "the point to begin from.")]
        [Input("b", "the point to end at.")]
        [Input("powX", "the region will be evaluated under the function: x^(powX).")]
        [Input("tol", "Tolerance for the integration calculation.", typeof(Length))]
        [Output("V", "Calculated value. The region intergral calculated over a boundery, the line from a to b./n" + 
                     "The solution is only defined for closed counter-clockwise oriented regions, this can be achived by a sum of solutions." +
                     "This value should only be used on its own with this in mind.")]
        public static double IntSurfLine(Point a, Point b, int powX, double tol = Tolerance.Distance)
        {
            //TODO powX could be a double, but that might slow thing down somewhat

            double diffX;
            double diffY = (a.Y - b.Y);
            /***************/
            if (Math.Abs(diffY) < tol)  // The answer is zero
                return 0;
            /***************/
            switch (powX)
            {
                case 0:
                    return -((a.X + b.X) * diffY) * 0.5;
                /********************/
                case 1:
                    return ((a.X * a.X + a.X * b.X + b.X * b.X) * (-diffY)) / 6;
                /********************/
                case 2:
                    return -((a.X + b.X) * (a.X * a.X + b.X * b.X) * diffY) / 12;
                /********************/
                case -1:
                    Engine.Base.Compute.RecordError("powX = -1 is not implemented");
                    return 0;
                    //if (a.X < tol || b.X < tol)
                    //{
                    //    Engine.Base.Compute.RecordError("powX = -1 is not defined left of the Y-axis");
                    //    return 0;
                    //}
                    //diffX = (a.X - b.X);
                    //if (Math.Abs(diffX) < tol)
                    //    return -Math.Log(a.X) * diffY;

                    //return (-diffY * (diffX * Math.Log(b.X)) + a.X * Math.Log(a.X / b.X)) / diffX;
                /********************/
                case -2:
                    if ((a.X < 0 ^ b.X < 0) || Math.Abs(a.X) < tol || Math.Abs(b.X) < tol)
                    {
                        Engine.Base.Compute.RecordError("powX = -2 is not defined on the Y-axis");
                        return 0;
                    }
                    diffX = (a.X - b.X);
                    if (Math.Abs(diffX) < tol)
                        return diffY / a.X;

                    return -diffY * Math.Log(b.X / a.X) / diffX;
                /********************/
                default:
                    diffX = (a.X - b.X);
                    if (Math.Abs(diffX) < tol)
                        return -(Math.Pow(a.X, powX + 1) * diffY) / (powX + 1);
                    /***************/
                    double N = (powX + 1) * (powX + 2);
                    double bigX = (Math.Pow(b.X, powX + 2) - Math.Pow(a.X, powX + 2));

                    return diffY * bigX / (N * diffX);
                    /********************/
            }
        }

        /***************************************************/

    }
}
