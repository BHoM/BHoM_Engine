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

using BH.oM.Base;
using BH.oM.Base.Attributes;
using BH.oM.Geometry;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Compute
    {
        /***************************************************/
        /****              Public Methods               ****/
        /***************************************************/

        [Description("Fits a line into a set of points using Orthogonal Least Squares algorithm. Returns null if the number of non-duplicate points is less than two.")]
        [Input("points", "Points into which the line is meant to be fit.")]
        [Input("tolerance", "Distance tolerance to be used to check whether a line can be fit into the input points (i.e. whether the points are not coincident).")]
        [Output("fitLine", "Line fit into the input set of points based on the Orthogonal Least Squares algorithm.")]
        public static Line FitLine(this IEnumerable<Point> points, double tolerance = Tolerance.Distance)
        {
            List<Point> asList = points.ToList();
            int n = asList.Count;
            if (asList.CullDuplicates(tolerance).Count < 2)
                return null;

            Point C = points.Average();

            double[,] A = new double[n,3];
            for (int i = 0; i < n; i++)
            {
                A[i, 0] = asList[i].X - C.X;
                A[i, 1] = asList[i].Y - C.Y;
                A[i, 2] = asList[i].Z - C.Z;
            }

            Output<double[,], double[], double[,]> svd = A.SingularValueDecomposition();
            double[,] Vh = svd.Item3;
            Vector dir = new Vector { X = Vh[0, 0], Y = Vh[0, 1], Z = Vh[0, 2] };
            return new Line { Start = C - dir, End = C + dir };
        }

        /***************************************************/
    }
}
