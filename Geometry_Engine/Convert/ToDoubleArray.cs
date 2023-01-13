/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Converts the list of Points and corresponding weights to a list of double[]. If rational (any weight not equal to 1.0) the returned arrays are 4d in homogenous coordinates, if non rational the arrays are 3d corresponding to the point coordinates.")]
        [Input("points", "The poitns to convert.")]
        [Input("weights", "Corresponding weights. SHould be a list of the same length as the points.")]
        [MultiOutput(0, "arrs", "THe points converted to double[], either in cartesian or homogenous coordinates, depending on if the curve is rational or not.")]
        [MultiOutput(1, "isRational", "Returns true if the points are rational, i.e. any of the weights are not equal to one.")]
        public static Output<List<double[]>, bool> ToDoubleArray(this IReadOnlyList<Point> points, IReadOnlyList<double> weights)
        {
            if (points == null || weights == null)
            {
                return new Output<List<double[]>, bool>();
            }

            if (points.Count != weights.Count)
            {
                return new Output<List<double[]>, bool>();
            }

            List<double[]> result = new List<double[]>();
            bool isRational = weights.Any(x => x != 1.0);

            if (isRational)
            {
                for (int i = 0; i < points.Count; i++)
                {
                    Point p = points[i];
                    double w = weights[i];
                    result.Add(new double[] { p.X / w, p.Y / w, p.Z / w, w });
                }
            }
            else
            {
                for (int i = 0; i < points.Count; i++)
                {
                    Point p = points[i];
                    result.Add(new double[] { p.X, p.Y, p.Z });
                }
            }
            return new Output<List<double[]>, bool> { Item1= result, Item2 = isRational };
        }

        /***************************************************/
    }
}
