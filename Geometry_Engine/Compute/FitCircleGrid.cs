using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Compute
    {
        public static List<List<Point>> FitCircleGrid(List<Polyline> coverageOutlines, List<Polyline> availableOutlines, double radius, List<Polyline> coverageHoles = null, List<Polyline> availableHoles = null, double tol = Tolerance.Distance)
        {
            coverageHoles = coverageHoles ?? new List<Polyline>();
            availableHoles = availableHoles ?? new List<Polyline>();

            coverageOutlines = coverageOutlines.Select(o => o.CleanPolyline(tol, tol)).ToList();
            availableOutlines = availableOutlines.Select(o => o.CleanPolyline(tol, tol)).ToList();
            coverageHoles = coverageHoles.Select(o => o.CleanPolyline(tol, tol)).ToList();
            availableHoles = availableHoles.Select(o => o.CleanPolyline(tol, tol)).ToList();

            throw new NotImplementedException();
        }

        private static bool?[] ContainmentGrid(List<Polyline> outlines, List<Polyline> holes, double cellSize, double offset)
        {

        }

        // cellRatio 0.8 to 1.25
        // shiftX, shiftY 0.0 to 1.0
        public static List<Point> CreateGrid(List<Polyline> outlines, List<Polyline> holes, bool?[] containmentGrid, double radius, double cellRatio, double shiftX, double shiftY, double tol = Tolerance.Distance)
        {
            BoundingBox bbox = outlines.Select(x => x.Bounds()).ToList().Bounds();
            bbox = bbox.Inflate(radius);
            //Point centre = (bbox.Min + bbox.Max) / 2;

            double hor = 2 * radius / Math.Sqrt(1 + cellRatio * cellRatio);
            double ver = cellRatio * hor;

            double horShift = hor * shiftX;
            double verShift = ver * shiftY;

            int horSteps = (int)Math.Ceiling((bbox.Max.X - bbox.Min.X) / hor);
            int verSteps = (int)Math.Ceiling((bbox.Max.Y - bbox.Min.Y) / ver);
            List<Point> grid = new List<Point>();
            for (int i = 0; i < horSteps; i++)
            {
                for (int j = 0; j < verSteps; j++)
                {
                    double x = bbox.Min.X + hor * i + horShift;
                    double y = bbox.Min.Y + ver * j + verShift;
                    Point p = new Point { X = x, Y = y };

                    //TODO: rewrite IsContaining to 2d for performance reasons
                    if (outlines.Any(o => o.IsContaining(new List<Point> { p }, true, tol)) && holes.All(h => !h.IsContaining(new List<Point> { p }, false, tol)))
                        grid.Add(p);
                }
            }

            return grid;
        }

        public static List<Line> UncoveredEdges(Polyline outline, List<Polyline> holes, List<Point> grid, double radius, double tol = Tolerance.Distance)
        {
            List<Line> uncoveredEdges = new List<Line>();

            //TODO: use R-trees here based on bboxes of circles etc.

            foreach (Line l in holes.Concat(new Polyline[] { outline }).SelectMany(x => x.SubParts()))
            {
                Point start = l.Start;
                Vector dir = l.Direction();
                double len = l.Length();

                List<(double, double)> coveredParams = new List<(double, double)>();
                foreach (Point p in grid)
                {
                    double t = dir * (p - start);

                    Point closest = start + t * dir;
                    double dist = p.Distance(closest);
                    if (dist + tol >= radius)
                        continue;

                    double along = Math.Sqrt(radius * radius - dist * dist);
                    //double par = along / len;
                    double tMax = t + along;
                    double tMin = t - along;
                    coveredParams.Add((tMin / len, tMax / len));
                }

                coveredParams = coveredParams.Where(x => x.Item1 < 1 - tol && x.Item2 > tol).OrderBy(x => x.Item1).ToList();
                if (coveredParams.Count == 0)
                {
                    uncoveredEdges.Add(l);
                    continue;
                }

                // join coveredparams that overlap, include tol
                List<(double, double)> joined = new List<(double, double)>();
                (double, double) current = coveredParams[0];
                for (int i = 1; i < coveredParams.Count; i++)
                {
                    if (current.Item2 + tol >= coveredParams[i].Item1)
                        current.Item2 = Math.Max(current.Item2, coveredParams[i].Item2);
                    else
                    {
                        joined.Add(current);
                        current = coveredParams[i];
                    }
                }
                joined.Add(current);

                // create uncovered edges

                if (joined[0].Item1 > tol)
                {
                    Line uncovered = new Line { Start = start, End = start + joined[0].Item1 * dir * len };
                    uncoveredEdges.Add(uncovered);
                }

                if (joined[joined.Count - 1].Item2 < 1 - tol)
                {
                    Line uncovered = new Line { Start = start + joined[joined.Count - 1].Item2 * dir * len, End = l.End };
                    uncoveredEdges.Add(uncovered);
                }

                for (int i = 0; i < joined.Count - 1; i++)
                {
                    Line uncovered = new Line { Start = start + joined[i].Item2 * dir * len, End = start + joined[i + 1].Item1 * dir * len };
                    uncoveredEdges.Add(uncovered);
                }
            }

            return uncoveredEdges;
        }
    }
}
