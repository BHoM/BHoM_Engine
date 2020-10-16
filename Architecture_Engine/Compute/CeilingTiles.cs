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
        public static Output<List<Line>, List<Line>> CeilingTiles(Polyline perimeterCurve, List<Line> ceilingTileLines)
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





            return new Output<List<Line>, List<Line>>
            {
                Item1 = perimeterLines,
                Item2 = splitCurves,
            };
            //return new List<CeilingTile>();
        }
    }
}
