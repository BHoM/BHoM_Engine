/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
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
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** public Methods - Vector                   ****/
        /***************************************************/

        [Description("Checks if a given set of points is collinear within tolerance.")]
        [Input("pts", "Points to check for collinearity.")]
        [Input("tolerance", "Tolerance used for checking collinearity.", typeof(Length))]
        [Output("isCollinear", "True if the input points are collinear, otherwise false.")]
        public static bool IsCollinear(this List<Point> pts, double tolerance = Tolerance.Distance)
        {
            if (pts.Count < 3)
                return true;

            Line fitLine = pts.FitLine(tolerance);

            // Coincident points can be considered collinear
            if (fitLine == null)
                return true;

            double sqTol = tolerance * tolerance;
            return pts.All(x => x.SquareDistance(fitLine, true) <= sqTol);
        }


        /***************************************************/
        /**** public Methods - Curves                   ****/
        /***************************************************/

        [Description("Checks if a given pair of lines is collinear within tolerance.")]
        [Input("line1", "First line to check for collinearity.")]
        [Input("line2", "Second line to check for collinearity.")]
        [Input("tolerance", "Tolerance used for checking collinearity.", typeof(Length))]
        [Output("isCollinear", "True if the input lines are collinear, otherwise false.")]
        public static bool IsCollinear(this Line line1, Line line2, double tolerance = Tolerance.Distance)
        {
            // Check if the lines are unidirectional first to catch cases of short curves with different dirs
            if (1 - Math.Abs(line1.Direction().DotProduct(line2.Direction())) > tolerance)
                return false;

            // Check if points are collinear
            List<Point> cPts = new List<Point> { line1.Start, line1.End, line2.Start, line2.End };
            return cPts.IsCollinear(tolerance);
        }

        /***************************************************/

        [Description("Checks if a given set of lines is collinear within tolerance.")]
        [Input("lines", "Lines to check for collinearity.")]
        [Input("tolerance", "Tolerance used for checking collinearity.", typeof(Length))]
        [Output("isCollinear", "True if the input lines are collinear, otherwise false.")]
        public static bool IsCollinear(this List<Line> lines, double tolerance = Tolerance.Distance)
        {
            // Check if the lines are unidirectional first to catch cases of short curves with different dirs
            List<Vector> dirs = lines.Select(x => x.Direction()).ToList();
            Vector avgDir = dirs.Average();
            if (dirs.Any(x => 1 - Math.Abs(avgDir.DotProduct(x)) > tolerance))
                return false;

            // Check if points are collinear
            return lines.Select(x => x.Start).Union(lines.Select(x => x.End)).ToList().IsCollinear(tolerance);
        }

        /***************************************************/
    }
}






