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

        [Description("Calculates the overlap length between multiple parallel lines when moved to coincide.")]
        [Input("parallelLines", "Collection of parallel lines.")]
        [Input("distanceTolerance", "Distance tolerance for geometric operations.")]
        [Output("overlapLength", "Length of overlap when all lines are moved to coincide.")]
        public static double OverlapLength(this List<Line> parallelLines, double distanceTolerance = Tolerance.Distance)
        {
            if (parallelLines == null || parallelLines.Count < 2)
                return 0.0;

            Vector direction = parallelLines[0].Direction();

            List<(double start, double end)> projections = new List<(double start, double end)>();
            foreach (Line line in parallelLines)
                projections.Add(EndParametersOnVector(line, direction));

            double overlapStart = projections.Max(p => p.start);
            double overlapEnd = projections.Min(p => p.end);

            if (overlapEnd <= overlapStart)
                return 0.0;

            return overlapEnd - overlapStart;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static (double start, double end) EndParametersOnVector(this Line line, Vector direction)
        {
            if (line == null || direction == null)
                return (0.0, 0.0);

            Vector normalizedDir = direction.Normalise();
            double startParam = (line.Start - Point.Origin) * normalizedDir;
            double endParam = (line.End - Point.Origin) * normalizedDir;

            return (Math.Min(startParam, endParam), Math.Max(startParam, endParam));
        }

        /***************************************************/
    }
}
