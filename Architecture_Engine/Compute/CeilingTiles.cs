using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Architecture.Elements;

using BH.oM.Geometry;
using BH.Engine.Geometry;
using BH.oM.Reflection;
using System.Runtime.InteropServices;

namespace BH.Engine.Architecture
{
    public static partial class Compute
    {
        //public static List<CeilingTile> CeilingTiles(Polyline perimeterCurve, List<Line> ceilingTileLines)
        /*public static Output<List<Line>, List<Line>, List<CeilingTile>> CeilingTiles(Polyline perimeterCurve, List<Line> ceilingTileLines)
        {
            List<Point> intersectingPoints = BH.Engine.Geometry.Query.LineIntersections(ceilingTileLines);

            List<Line> splitCurves = ceilingTileLines.SelectMany(x => x.ISplitAtPoints(intersectingPoints)).Cast<Line>().ToList();

            List<Line> perimeterLines = new List<Line>();
            List<Line> perimeterCurveLines = perimeterCurve.ISubParts().Cast<Line>().ToList();

            foreach(Line l in splitCurves)
            {
                foreach(Line l2 in perimeterCurveLines)
                {
                    Line intersect = l.BooleanIntersection(l2);
                    if(intersect != null && intersect.Length() == l.Length())
                    {
                        //This line is part of the outer perimeter
                        perimeterLines.Add(l);
                    }
                }
            }

            foreach (Line l in perimeterLines)
                splitCurves.Remove(l); //Remove the perimeter from the splits

            List<Line> splitCurves2 = new List<Line>(splitCurves);
            foreach (Line l in splitCurves2)
                splitCurves.Add(l); //Refactor this!!!!

            foreach (Line l in perimeterLines)
                splitCurves.Add(l);

            List<CeilingTile> tiles = new List<CeilingTile>();

            List<Line> usedPerimeterLines = new List<Line>();

            int count2 = 0;

            while (perimeterLines.Count > 0 && count2 < 1000)
            {
                count2++;
                foreach (Line l in perimeterLines)
                {
                    if (usedPerimeterLines.Contains(l))
                        continue;

                    List<Line> outlines = new List<Line>();
                    outlines.Add(l);

                    usedPerimeterLines.Add(l);

                    Point pt1 = l.Start.RoundCoordinates(6);
                    Point pt2 = l.End.RoundCoordinates(6);

                    Point checkPt = pt2.RoundCoordinates(6);

                    int count = 0; //Aim to remove this
                    while (!outlines.Join()[0].IsClosed() && count < 100)
                    {
                        List<Line> connectedLines = splitCurves.Where(x => x.Start.RoundCoordinates(6) == checkPt || x.End.RoundCoordinates(6) == checkPt).ToList();
                        connectedLines = connectedLines.Where(x => x != outlines.Last()).ToList();

                        Line smallestAngleLine = null;
                        double smallestAngle = 1e10;

                        foreach (Line check in connectedLines)
                        {
                            Point pt3 = check.Start.RoundCoordinates(6);
                            if (pt3 == checkPt)
                                pt3 = check.End.RoundCoordinates(6);

                            if (pt3 == pt1)
                            {
                                smallestAngleLine = check;
                                break; //We've reached our starting point
                            }

                            double angle = BH.Engine.Geometry.Query.Angle(pt1, pt2, pt3);
                            angle = Math.PI - angle; //x - y = z, x - z = y

                            if (angle < smallestAngle)
                            {
                                smallestAngle = angle;
                                smallestAngleLine = check;
                            }
                        }

                        count++;

                        if (smallestAngleLine == null)
                            break; //Error somewhere

                        outlines.Add(smallestAngleLine);
                        if (smallestAngleLine.Start.RoundCoordinates(6) == checkPt)
                            checkPt = smallestAngleLine.End.RoundCoordinates(6);
                        else
                            checkPt = smallestAngleLine.Start.RoundCoordinates(6);
                    }

                    tiles.Add(new CeilingTile
                    {
                        Perimeter = outlines.Join()[0]
                    });

                    foreach (Line remove in outlines)
                    {
                        splitCurves.Remove(remove);
                        if (perimeterLines.Contains(remove))
                            usedPerimeterLines.Add(remove);
                    }

                    splitCurves.Remove(l);
                }

                perimeterLines = new List<Line>();
                foreach(Line add in splitCurves)
                {
                    if (splitCurves.Where(x => x == add).Count() == 1)
                        perimeterLines.Add(add);
                }
            }




            return new Output<List<Line>, List<Line>, List<CeilingTile>>
            {
                Item1 = perimeterLines,
                Item2 = splitCurves,
                Item3 = tiles,
            };
            //return new List<CeilingTile>();
        }*/

        public static Output<List<Line>, List<Line>, List<CeilingTile>> CeilingTiles(Polyline perimeterCurve, List<Line> ceilingTileLines)
        {
            List<Point> intersectingPoints = BH.Engine.Geometry.Query.LineIntersections(ceilingTileLines);

            List<Line> splitCurves = ceilingTileLines.SelectMany(x => x.ISplitAtPoints(intersectingPoints)).Cast<Line>().ToList();

            List<Line> perimeterLines = new List<Line>();
            List<Line> perimeterCurveLines = perimeterCurve.ISubParts().Cast<Line>().ToList();

            foreach (Line l in splitCurves)
            {
                foreach (Line l2 in perimeterCurveLines)
                {
                    Line intersect = l.BooleanIntersection(l2);
                    if (intersect != null && intersect.Length() == l.Length())
                    {
                        //This line is part of the outer perimeter
                        perimeterLines.Add(l);
                    }
                }
            }

            foreach (Line l in perimeterLines)
                splitCurves.Remove(l); //Remove the perimeter from the splits

            List<Line> splitCurves2 = new List<Line>(splitCurves);
            foreach (Line l in splitCurves2)
                splitCurves.Add(l); //Refactor this!!!!

            foreach (Line l in perimeterLines)
                splitCurves.Add(l);

            

            List<CeilingTile> tiles = new List<CeilingTile>();

            List<Line> usedPerimeterLines = new List<Line>();

            int count2 = 0;

           while(perimeterLines.Count > 0 && count2 < 1000)
            {
                count2++; //Refactor out

                Line start = perimeterLines[0];

                List<Line> searchLines = splitCurves.Distinct().ToList();

                searchLines.Remove(start);

                List<LineTree> children = searchLines.Where(x => x.Start.RoundCoordinates(6) == start.End.RoundCoordinates(6) || x.End.RoundCoordinates(6) == start.End.RoundCoordinates(6)).Select(x =>
                        {
                            Point uPt = (x.Start.RoundCoordinates(6) == start.End.RoundCoordinates(6) ? x.End.RoundCoordinates(6) : x.Start.RoundCoordinates(6));
                            return new LineTree
                            {
                                Parent = start,
                                ThisLine = x,
                                UnconnectedPoint = uPt,
                            };
                        }).ToList();

                LineTree last = null;
                List<LineTree> master = new List<LineTree>();

                int count = 0;

                while(children.Count > 0 && last == null && count < 1000)
                {
                    count++; //Refactor out

                    master.AddRange(children);
                    List<LineTree> grandchildren = new List<LineTree>();

                    foreach(LineTree lt in children)
                    {
                        List<LineTree> ltChildren = searchLines.Where(x => master.Where(y => y.ThisLine == x).FirstOrDefault() == null && (x.Start.RoundCoordinates(6) == lt.UnconnectedPoint || x.End.RoundCoordinates(6) == lt.UnconnectedPoint)).Select(x =>
                        {
                            Point uPt = (x.Start.RoundCoordinates(6) == lt.UnconnectedPoint ? x.End.RoundCoordinates(6) : x.Start.RoundCoordinates(6));
                            return new LineTree
                            {
                                Parent = lt.ThisLine,
                                ThisLine = x,
                                UnconnectedPoint = uPt,
                            };
                        }).ToList();

                        ltChildren = ltChildren.Distinct().ToList();

                        if(ltChildren.Where(x => x.ThisLine.Start.RoundCoordinates(6) == start.Start.RoundCoordinates(6) || x.ThisLine.End.RoundCoordinates(6) == start.Start.RoundCoordinates(6)).FirstOrDefault() != null)
                        {
                            last = ltChildren.Where(x => x.ThisLine.Start.RoundCoordinates(6) == start.Start.RoundCoordinates(6) || x.ThisLine.End.RoundCoordinates(6) == start.Start.RoundCoordinates(6)).FirstOrDefault();
                            break;
                        }

                        grandchildren.AddRange(ltChildren);
                    }

                    children = new List<LineTree>(grandchildren);
                }

                if (last == null)
                    continue;

                List<Line> outlines = new List<Line>();
                outlines.Add(last.ThisLine);
                int count3 = 0;
                while(last.Parent != start && count3 < 1000)
                {
                    count3++; //Refactor out

                    last = master.Where(x => x.ThisLine == last.Parent).FirstOrDefault();
                    outlines.Add(last.ThisLine);
                }

                outlines.Add(start);

                foreach (Line l in outlines)
                {
                    if (perimeterLines.Contains(l))
                        perimeterLines.Remove(l);

                    splitCurves.Remove(l);
                }

                if(perimeterLines.Count > 0)
                    perimeterLines.RemoveAt(0);

                foreach (Line add in splitCurves)
                {
                    if (splitCurves.Where(x => x == add).Count() == 1)
                        perimeterLines.Add(add);
                }

                perimeterLines = perimeterLines.Distinct().ToList();

                tiles.Add(new CeilingTile { Perimeter = outlines.Join()[0] });
            }





            return new Output<List<Line>, List<Line>, List<CeilingTile>>
            {
                Item1 = new List<Line>(),
                Item2 = new List<Line>(),
                Item3 = tiles,
            };
        }

        
    }

    public class LineTree
    {
        public Line ThisLine { get; set; } = null;
        public Line Parent { get; set; } = null;
        public Point UnconnectedPoint { get; set; } = null;
    }
}
