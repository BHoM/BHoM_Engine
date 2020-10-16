using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Architecture.Elements;

using BH.oM.Geometry;
using BH.Engine.Geometry;
using BH.oM.Reflection;

namespace BH.Engine.Architecture
{
    public static partial class Compute
    {
        //public static List<CeilingTile> CeilingTiles(Polyline perimeterCurve, List<Line> ceilingTileLines)
        public static Output<List<Line>, List<Line>, List<CeilingTile>> CeilingTiles(Polyline perimeterCurve, List<Line> ceilingTileLines)
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

            //foreach (Line l in perimeterLines)
                //splitCurves.Remove(l); //Remove the perimeter from the splits

            List<Line> splitCurves2 = new List<Line>(splitCurves);
            foreach (Line l in splitCurves2)
                splitCurves.Add(l); //Refactor this!!!!


            List<CeilingTile> tiles = new List<CeilingTile>();

            foreach(Line l in perimeterLines)
            {
                List<Line> outlines = new List<Line>();
                outlines.Add(l);

                Point pt1 = l.Start;
                Point pt2 = l.End;

                Point checkPt = pt2;

                int count = 0; //Aim to remove this
                while(!outlines.Join()[0].IsClosed() && count < 100)
                {
                    List<Line> connectedLines = splitCurves.Where(x => x.Start == checkPt || x.End == checkPt).ToList();
                    connectedLines = connectedLines.Where(x => x != l).ToList();

                    Line smallestAngleLine = null;
                    double smallestAngle = 1e10;

                    foreach (Line check in connectedLines)
                    {
                        Point pt3 = check.Start;
                        if (pt3 == checkPt)
                            pt3 = check.End;

                        if(pt3 == pt1)
                        {
                            outlines.Add(check);
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

                    if (smallestAngleLine == null)
                        break; //Error somewhere

                    outlines.Add(smallestAngleLine);
                    if (smallestAngleLine.Start == checkPt)
                        checkPt = smallestAngleLine.End;
                    else
                        checkPt = smallestAngleLine.Start;
                }

                tiles.Add(new CeilingTile
                {
                    Perimeter = outlines.Join()[0]
                });

                foreach (Line remove in outlines)
                    splitCurves.Remove(remove);

                splitCurves.Remove(l);
            }




            return new Output<List<Line>, List<Line>, List<CeilingTile>>
            {
                Item1 = perimeterLines,
                Item2 = splitCurves,
                Item3 = tiles,
            };
            //return new List<CeilingTile>();
        }
    }
}
