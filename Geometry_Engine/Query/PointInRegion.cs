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

using BH.oM.Geometry;
using BH.oM.Base.Attributes;
using BH.oM.Quantities.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        [Description("Finds a semi-arbitrary Point that lies within the region defined by a closed curve.")]
        [Input("curve", "The closed ICurve defining the region.")]
        [Input("acceptOnEdge", "Whether a point that lies on the edge of the region is considered valid.")]
        [Input("tolerance", "The tolerance for containment checks.", typeof(Length))]
        [Output("interiorPoint", "A Point within the region, or null if no valid point is found.")]
        public static Point PointInRegion(this ICurve curve, bool acceptOnEdge = false, double tolerance = Tolerance.Distance)
        {
            Point point = curve.ICentroid(tolerance);
            if (point != null && curve.IIsContaining(new List<Point> { point }, acceptOnEdge, tolerance))
                return point;

            List<Point> controlPoints = curve.IControlPoints();
            if (controlPoints.Count < 3)
                return null;

            double maxSqLength = 0;
            for (int i = 0; i < controlPoints.Count - 1; i++)
            {
                for (int j = i + 2; j < controlPoints.Count; j++)
                {
                    double sqLength = controlPoints[i].SquareDistance(controlPoints[j]);
                    if (sqLength > maxSqLength)
                    {
                        Point midPoint = (controlPoints[i] + controlPoints[j]) * 0.5;
                        if (curve.IIsContaining(new List<Point> { midPoint }, acceptOnEdge, tolerance))
                        {
                            point = midPoint;
                            maxSqLength = sqLength;
                        }
                    }
                }
            }

            return point;
        }

        /***************************************************/
    }
}
