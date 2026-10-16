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

        [Description("Checks if a polyline is a closed, globally aligned rectangle with edges parallel to the specified X and Y vectors.")]
        [Input("pLine", "The polyline to check if it is a globally aligned rectangle.")]
        [Input("xVector", "The X direction vector that one pair of parallel edges should be parallel to.")]
        [Input("yVector", "The Y direction vector that the other pair of parallel edges should be parallel to.")]
        [Output("isAlignedRectangle", "True if the polyline is a closed rectangle with edges aligned to the specified vectors.")]
        public static bool IsAlignedRectangle(this Polyline pLine, Vector xVector, Vector yVector)
        {
            if (pLine == null || xVector == null || yVector == null)
                return false;

            if (!pLine.IsClosed())
                return false;

            List<Point> controlPoints = pLine.ControlPoints;
            if (controlPoints.Count != 5)
                return false;

            List<Line> edges = pLine.SubParts();
            if (edges.Count != 4)
                return false;

            Vector firstDirection = edges[0].Direction();
            List<Line> group1 = edges.Where(x => x.Direction().IsParallel(firstDirection) != 0).ToList();
            List<Line> group2 = edges.Where(x => x.Direction().IsParallel(firstDirection) == 0).ToList();

            if (group1.Count != 2 || group2.Count != 2)
                return false;

            Vector group2Direction = group2[0].Direction();
            double angle = firstDirection.Angle(group2Direction);
            if (Math.Abs(angle - Math.PI / 2) > Tolerance.Angle)
                return false;

            bool group1Aligned = firstDirection.IsParallel(xVector) != 0 || firstDirection.IsParallel(yVector) != 0;
            bool group2Aligned = group2Direction.IsParallel(xVector) != 0 || group2Direction.IsParallel(yVector) != 0;
            return group1Aligned && group2Aligned;
        }

        /***************************************************/
    }
}
