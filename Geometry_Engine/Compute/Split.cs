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

using BH.oM.Geometry;
using BH.oM.Base.Attributes;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Compute
    {
        [Description("Split an outer region by the cutting lines into a collection of closed contained regions within the outer region.")]
        [Input("outerRegion", "An outer region that will contain the closed regions generated.")]
        [Input("cuttingLines", "The lines to cut the outer region by.")]
        [Input("distanceTolerance", "Tolerance to use for distance measurment operations, default to BH.oM.Geometry.Tolerance.Distance.")]
        [Input("decimalPlaces", "All coordinates of the geometry will be rounded to the number of decimal places specified. Default 6.")]
        [Output("regions", "Closed polygon regions contained within the outer region cut by the cutting lines.")]
        public static List<Polyline> Split(this Polyline outerRegion, List<Line> cuttingLines, double distanceTolerance = BH.oM.Geometry.Tolerance.Distance, int decimalPlaces = 6)
        {
            //Preproc the cutting lines to take only the parts inside the outer region
            cuttingLines = cuttingLines.BooleanUnion(distanceTolerance);
            cuttingLines = cuttingLines.SelectMany(x => x.SplitAtPoints(outerRegion.LineIntersections(x, false, distanceTolerance), distanceTolerance)).ToList();
            cuttingLines = cuttingLines.Where(x => outerRegion.IsContaining(new List<Point> { (x.Start + x.End) / 2 }, false, distanceTolerance)).ToList();
            if (cuttingLines.Count == 0)
                return new List<Polyline> { outerRegion };

            List<Point> intersectingPoints = BH.Engine.Geometry.Query.LineIntersections(cuttingLines); //Get the points to split the lines by
            List<Line> splitCurves = cuttingLines.SelectMany(x => x.ISplitAtPoints(intersectingPoints)).Cast<Line>().ToList(); //Split the cutting lines at their points
            List<Line> perimeterLines = outerRegion.SubParts().SelectMany(x => x.SplitAtPoints(cuttingLines.SelectMany(y => y.LineIntersections(x)).ToList())).ToList();

            splitCurves.AddRange(splitCurves); //Duplicate the inner curves within the list, each line will only be used twice one two adjacent regions
            splitCurves.AddRange(perimeterLines); //Add the perimeters back in as single lines

            List<Polyline> cutRegions = new List<Polyline>(); //Return object to store the resulting regions

            //The actual magic starts now
            while (perimeterLines.Count > 0)
            {
                Line start = perimeterLines[0];

                List<Line> searchLines = splitCurves.Distinct().ToList(); //Each iteration will reduce the search space
                searchLines.Remove(start); //Do not need the current line being evaluated in the search criteria

                Point startPt = start.Start.RoundCoordinates(decimalPlaces);
                Point endPt = start.End.RoundCoordinates(decimalPlaces);

                List<LineTree> children = searchLines.Where(x => x.Start.RoundCoordinates(decimalPlaces) == endPt || x.End.RoundCoordinates(decimalPlaces) == endPt).Select(x =>
                {
                    Point uPt = (x.Start.RoundCoordinates(decimalPlaces) == endPt ? x.End.RoundCoordinates(decimalPlaces) : x.Start.RoundCoordinates(decimalPlaces));
                    return new LineTree
                    {
                        Parent = start,
                        ThisLine = x,
                        UnconnectedPoint = uPt,
                    };
                }).ToList();

                LineTree last = null; //The last line in the path, once we find it then we can traverse up the tree to the start
                List<LineTree> master = new List<LineTree>(); //As we're only generating the LineTree on each iteration, we need to keep track of what we've created this iteration for traversing. We're generating the LineTrees on each iteration between the children is dependent on the directionality of the start, so cannot be preprocessed (until someone refactors this to enable such a thing!)
                
                while (children.Count > 0 && last == null) //while(last == null) risks an infinite loop if we never find the last node, we should eventually run out of children though
                {
                    master.AddRange(children);
                    List<LineTree> grandchildren = new List<LineTree>();

                    foreach (LineTree lt in children)
                    {
                        //If the search line already has a LineTree object in `master` then it means we've already looked at it in some capacity, and didn't find what we're looking for - so this is to limit the search space to narrow in on the solution
                        List<LineTree> ltChildren = searchLines.Where(x => master.FirstOrDefault(y => y.ThisLine == x) == null && (x.Start.RoundCoordinates(decimalPlaces) == lt.UnconnectedPoint || x.End.RoundCoordinates(decimalPlaces) == lt.UnconnectedPoint)).Select(x =>
                        {
                            Point uPt = (x.Start.RoundCoordinates(decimalPlaces) == lt.UnconnectedPoint ? x.End.RoundCoordinates(decimalPlaces) : x.Start.RoundCoordinates(decimalPlaces));
                            return new LineTree
                            {
                                Parent = lt.ThisLine,
                                ThisLine = x,
                                UnconnectedPoint = uPt,
                            };
                        }).ToList();
                        
                        //Take distinct only
                        ltChildren = ltChildren.GroupBy(p => p.ThisLine).Select(g => g.First()).ToList();

                        LineTree foundStart = ltChildren.FirstOrDefault(x => x.ThisLine.Start.RoundCoordinates(decimalPlaces) == startPt || x.ThisLine.End.RoundCoordinates(decimalPlaces) == startPt);

                        if (foundStart != null)
                        {
                            last = foundStart;
                            break;
                        }

                        grandchildren.AddRange(ltChildren);
                    }

                    children = new List<LineTree>(grandchildren.GroupBy(p => p.ThisLine).Select(g => g.First())); //Add distinct grandchildren only
                }

                if (last == null)
                    continue; //Probably means we had an error and didn't find the last node, so let's try the next perimeter line

                //Generate the closed region
                List<Line> outlines = new List<Line>();
                outlines.Add(last.ThisLine);

                while (last.Parent != start)
                {
                    last = master.FirstOrDefault(x => x.ThisLine == last.Parent);
                    outlines.Add(last.ThisLine);
                }
                outlines.Add(start); //Close the region

                //Tidy up the search space for the next iteration
                foreach (Line l in outlines)
                {
                    if (perimeterLines.Contains(l))
                        perimeterLines.Remove(l);

                    splitCurves.Remove(l);
                }

                //Set up the perimeter lines to include any new 'perimeter lines' - lines which only have one instance in the list
                foreach (Line add in splitCurves)
                {
                    if (splitCurves.Count(x => x == add) == 1)
                        perimeterLines.Add(add);
                }

                perimeterLines = perimeterLines.Distinct().ToList();

                cutRegions.Add(outlines.Join()[0]);
            }

            return cutRegions;
        }
    }
}



