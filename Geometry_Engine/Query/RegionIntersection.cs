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

using BH.oM.Base.Attributes;
using BH.oM.Geometry;
using System.Collections.Generic;
using System.ComponentModel;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Find segments of a line lying inside the region defined by a polyline.")]
        [Input("region", "A polyline defining a closed region that the input line potentially intersects.")]
        [Input("line", "A line to check for intersections with a region defined by the input polylines.")]
        [Input("minLineLength", "Minimum length required of new intersection lines.")]
        [Input("acceptOnEdge", "Whether a point lying on the region's perimeter is considered contained by that region.")]
        [Input("tolerance", "Numerical tolerance for the operation.")]
        [Output("intersectionLines", "Segments of the input line that lie inside the region defined by the input polyline.")]
        public static List<Line> RegionIntersection(this Polyline region, Line line, double minLineLength = Tolerance.Distance, bool acceptOnEdge = true, double tolerance = Tolerance.Distance)
        {
            List<Point> intPnts = region.ILineIntersections(line, true, tolerance);
            intPnts.Add(line.Start);
            intPnts.Add(line.End);

            intPnts = intPnts.CullDuplicates(tolerance);
            intPnts = intPnts.SortCollinear(tolerance);
            List<Line> intersectionLines = new List<Line>();

            for (int i = 0; i < intPnts.Count - 1; i++)
            {
                Point pnt1 = intPnts[i];
                Point pnt2 = intPnts[i + 1];
                Point midPnt = (pnt1 + pnt2) / 2;
                bool midPntContained = region.IIsContaining(new List<Point> { midPnt }, acceptOnEdge, tolerance);

                if (midPntContained && pnt1.Distance(pnt2) >= minLineLength)
                {
                    intersectionLines.Add(new Line { Start = pnt1, End = pnt2 });
                }
            }

            return intersectionLines;
        }

        /***************************************************/
    }
}