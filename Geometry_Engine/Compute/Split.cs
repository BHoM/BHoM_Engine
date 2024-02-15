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

        [PreviousVersion("7.1", "BH.Engine.Geometry.Compute.Split(BH.oM.Geometry.Polyline, System.Collections.Generic.List<BH.oM.Geometry.Line>, System.Double, System.Int32)")]
        [Description("Split an outer region by the cutting lines into a collection of closed contained regions within the outer region.")]
        [Input("outerRegion", "An outer region that will contain the closed regions generated.")]
        [Input("cuttingLines", "The lines to cut the outer region by.")]
        [Input("distanceTolerance", "Tolerance to use for distance measurment operations, default to BH.oM.Geometry.Tolerance.Distance.")]
        [Output("regions", "Closed polygon regions contained within the outer region cut by the cutting lines.")]
        public static List<Polyline> Split(this Polyline outerRegion, List<Line> cuttingLines, double distanceTolerance = Tolerance.Distance)
        {
            //Preproc the cutting lines to take only the parts inside the outer region
            cuttingLines = cuttingLines.BooleanUnion(distanceTolerance, true);
            cuttingLines = cuttingLines.SelectMany(x => x.SplitAtPoints(outerRegion.LineIntersections(x, false, distanceTolerance).CullDuplicates(distanceTolerance), distanceTolerance)).ToList();
            cuttingLines = cuttingLines.Where(x => outerRegion.IsContaining(new List<Point> { (x.Start + x.End) / 2 }, false, distanceTolerance)).ToList();
            if (cuttingLines.Count == 0)
                return new List<Polyline> { outerRegion };

            List<Point> intersectingPoints = cuttingLines.LineIntersections(false, distanceTolerance).CullDuplicates(distanceTolerance); //Get the points to split the lines by
            List<Line> splitCurves = cuttingLines.SelectMany(x => x.ISplitAtPoints(intersectingPoints, distanceTolerance)).Cast<Line>().ToList(); //Split the cutting lines at their points
            List<Line> perimeterLines = outerRegion.SubParts().SelectMany(x => x.SplitAtPoints(cuttingLines.SelectMany(y => y.LineIntersections(x, false, distanceTolerance)).ToList())).ToList();

            // Make sure nodes of outline and splitting curves perfectly overlap
            Polyline outline = perimeterLines.Join(distanceTolerance).First();
            List<Point> outlinePoint = outline.ControlPoints.Skip(1).ToList();
            double sqTol = distanceTolerance * distanceTolerance;
            foreach (Line l in splitCurves)
            {
                Point cpStart = l.Start.ClosestPoint(outlinePoint);
                if (l.Start.SquareDistance(cpStart) <= sqTol)
                    l.Start = cpStart;

                Point cpEnd = l.End.ClosestPoint(outlinePoint);
                if (l.End.SquareDistance(cpEnd) <= sqTol)
                    l.End = cpEnd;
            }

            return OutlinesFromPreprocessedLines(outline, splitCurves, distanceTolerance);
        }

        /***************************************************/
    }
}
