using BH.oM.Base;
using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Geometry
{
    public class SquareGrid
    {
        public Point Origin { get; set; }
        public double CellSize { get; set; }
        public int CellCountX { get; set; }
        public int CellCountY { get; set; }
    }

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

        public static Output<List<Line>, List<Point>, List<Point>> ContainmentGridTest(List<Polyline> outlines, List<Polyline> holes, double cellSize, double offset, double tol)
        {
            BoundingBox bbox = outlines.Select(x => x.Bounds()).ToList().Bounds();
            double dx = bbox.Max.X - bbox.Min.X;
            double dy = bbox.Max.Y - bbox.Min.Y;
            int cellCountX = (int)Math.Ceiling(dx / cellSize);
            int cellCountY = (int)Math.Ceiling(dy / cellSize);
            Point origin = (bbox.Min + bbox.Max) / 2 - new Vector { X = cellCountX * cellSize / 2, Y = cellCountY * cellSize / 2 };
            SquareGrid grid = new SquareGrid { CellCountX = cellCountX, CellCountY = cellCountY, CellSize = cellSize, Origin = origin };

            List<Line> lns = new List<Line>();
            for (int i = 0; i <= cellCountX; i++)
            {
                lns.Add(new Line
                {
                    Start = new Point { X = origin.X + i * cellSize, Y = origin.Y },
                    End = new Point { X = origin.X + i * cellSize, Y = origin.Y + dy }
                });
            }
            for (int i = 0; i <= cellCountY; i++)
            {
                lns.Add(new Line
                {
                    Start = new Point { X = origin.X, Y = origin.Y + i * cellSize },
                    End = new Point { X = origin.X + dx, Y = origin.Y + i * cellSize }
                });
            }

            List<Point> inside = new List<Point>();
            List<Point> edge = new List<Point>();
            bool?[,] containmentGrid = ContainmentGrid(outlines, holes, grid, tol);
            for (int m = 0; m < cellCountX; m++)
            {
                for (int n = 0; n < cellCountY; n++)
                {
                    if (containmentGrid[m, n] == null)
                        edge.Add(CellCenter(origin, cellSize, m, n));
                    else if (containmentGrid[m, n] == true)
                        inside.Add(CellCenter(origin, cellSize, m, n));
                }
            }

            return new Output<List<Line>, List<Point>, List<Point>>
            {
                Item1 = lns,
                Item2 = inside,
                Item3 = edge
            };
        }

        private static SquareGrid Grid(List<Polyline> outlines, List<Polyline> holes, double cellSize, double offset)
        {
            BoundingBox bbox = outlines.Select(x => x.Bounds()).ToList().Bounds();
            double dx = bbox.Max.X - bbox.Min.X + offset * 2;
            double dy = bbox.Max.Y - bbox.Min.Y + offset * 2;
            int cellCountX = (int)Math.Ceiling(dx / cellSize) + 1;
            int cellCountY = (int)Math.Ceiling(dy / cellSize) + 1;
            Point origin = (bbox.Min + bbox.Max) / 2 - new Vector { X = cellCountX * cellSize / 2, Y = cellCountY * cellSize / 2 };
            return new SquareGrid { CellCountX = cellCountX, CellCountY = cellCountY, CellSize = cellSize, Origin = origin };
        }

        private static bool?[,] ContainmentGrid(List<Polyline> outlines, List<Polyline> holes, SquareGrid grid, double tol)
        {
            int cellCountX = grid.CellCountX;
            int cellCountY = grid.CellCountY;
            Point origin = grid.Origin;
            double cellSize = grid.CellSize;


            bool?[,] containmentGrid = new bool?[cellCountX, cellCountY];

            //TODO: check if nullable bool is false or null at start
            for (int m = 0; m < cellCountX; m++)
            {
                for (int n = 0; n < cellCountY; n++)
                {
                    containmentGrid[m, n] = false;
                }
            }

            foreach (Polyline outline in outlines.Concat(holes))
            {
                for (int i = 0; i < outline.ControlPoints.Count; i++)
                {
                    Point start = outline.ControlPoints[i];
                    Point end = outline.ControlPoints[(i + 1) % outline.ControlPoints.Count];
                    foreach ((int, int) coords in GetIntersectedCells(start, end, grid, tol))
                    {
                        containmentGrid[coords.Item1, coords.Item2] = null;
                    }
                }
            }

            for (int m = 0; m < containmentGrid.GetLength(0); m++)
            {
                bool inside = false;
                bool onEdge = false;
                for (int n = 0; n < containmentGrid.GetLength(1); n++)
                {
                    if (containmentGrid[m, n] == null)
                    {
                        onEdge = true;
                        continue;
                    }
                    else if (onEdge)
                    {
                        Point pt = CellCenter(origin, cellSize, m, n);
                        inside = outlines.Any(x => IsInside(pt, x, tol)) && !holes.Any(x => IsInside(pt, x, tol));
                        onEdge = false;
                    }

                    containmentGrid[m, n] = inside;
                }
            }

            return containmentGrid;
        }

        private static Point CellCenter(Point origin, double cellSize, int cellX, int cellY)
        {
            return new Point
            {
                X = origin.X + (cellX + 0.5) * cellSize,
                Y = origin.Y + (cellY + 0.5) * cellSize
            };
        }

        private static bool IsInside(this Point pt, Polyline outline, double tolerance = 1e-6)
        {
            //TODO: make sure right vector is picked - no intersections etc.
            double sqTol = tolerance * tolerance;
            if (outline.ControlPoints.Any(x => x.SquareDistance(pt) <= sqTol))
                return true;

            Vector dir = Vector.XAxis;
            while (IntersectsWithCorner(pt, dir, outline.ControlPoints, tolerance))
            {
                dir = dir.Rotate(Math.PI / 36, Vector.ZAxis);
                if (1 - dir.DotProduct(Vector.XAxis) <= tolerance)
                    throw new Exception("is inside failed");
            }

            int c = 0;
            foreach (Line ln in outline.SubParts())
            {
                if (Intersects(pt, dir, ln))
                    c++;
            }

            return c % 2 == 1;
        }
        private static bool Intersects(this Point pt, Vector dir, Line line2, double angleTolerance = Tolerance.Angle)
        {
            Vector v2 = line2.End - line2.Start;
            Vector v2N = v2.Normalise();

            if (1 - Math.Abs(dir.DotProduct(v2N)) <= angleTolerance)
                return false;

            Point p1 = pt;
            Point p2 = line2.Start;

            Vector cp = dir.CrossProduct(v2);
            Vector n1 = dir.CrossProduct(-cp);
            Vector n2 = v2.CrossProduct(cp);

            double t1 = (p2 - p1) * n2 / (dir * n2);
            if (t1 < 0)
                return false;

            double t2 = (p1 - p2) * n1 / (v2 * n1);
            return t2 >= 0 && t2 <= 1;
        }


        public static List<List<int>> GetIntersectedCellsTest(Line line, Point origin, double cellSize, int numCellsX, int numCellsY, double tol = 1e-6)
        {
            SquareGrid grid = new SquareGrid { CellCountX = numCellsX, CellCountY = numCellsY, CellSize = cellSize, Origin = origin };
            return GetIntersectedCells(line.Start, line.End, grid, tol).Select(x => new List<int> { x.Item1, x.Item2 }).ToList();
        }

        public static HashSet<(int, int)> GetIntersectedCells(Point lineStart, Point lineEnd, SquareGrid grid, double tol)
        {
            double startX = lineStart.X;
            double startY = lineStart.Y;
            double endX = lineEnd.X;
            double endY = lineEnd.Y;
            double originX = grid.Origin.X;
            double originY = grid.Origin.Y;
            double cellSize = grid.CellSize;

            HashSet<(int, int)> crossedCells = new HashSet<(int, int)>();

            // Find the cells to which the start and end points belong
            int startCellX = (int)((startX - originX) / cellSize);
            int startCellY = (int)((startY - originY) / cellSize);
            int endCellX = (int)((endX - originX) / cellSize);
            int endCellY = (int)((endY - originY) / cellSize);
            if (startCellX == endCellX && startCellY == endCellY)
            {
                crossedCells.Add((startCellX, startCellY));
                return crossedCells;
            }

            // Edge case when a line start is at grid edge
            List<(int, int)> startCells = new List<(int, int)> { (startCellX, startCellY) };
            if (Math.Abs(startX - originX - startCellX * cellSize) <= tol)
                startCells.Add((startCellX - 1, startCellY));
            else if (Math.Abs(startX - originX - (startCellX + 1) * cellSize) <= tol)
                startCells.Add((startCellX + 1, startCellY));

            if (Math.Abs(startY - originY - startCellY * cellSize) <= tol)
            {
                foreach ((int, int) cell in startCells)
                {
                    crossedCells.Add((cell.Item1, cell.Item2 - 1));
                }
            }
            else if (Math.Abs(startY - originY - (startCellY + 1) * cellSize) <= tol)
            {
                foreach ((int, int) cell in startCells)
                {
                    crossedCells.Add((cell.Item1, cell.Item2 + 1));
                }
            }

            crossedCells.UnionWith(startCells);

            // Edge case when a line end is at grid edge
            List<(int, int)> endCells = new List<(int, int)> { (endCellX, endCellY) };
            if (Math.Abs(endX - originX - endCellX * cellSize) <= tol)
                endCells.Add((endCellX - 1, endCellY));
            else if (Math.Abs(endX - originX - (endCellX + 1) * cellSize) <= tol)
                endCells.Add((endCellX + 1, endCellY));

            if (Math.Abs(endY - originY - endCellY * cellSize) <= tol)
            {
                foreach ((int, int) cell in endCells)
                {
                    crossedCells.Add((cell.Item1, cell.Item2 - 1));
                }
            }
            else if (Math.Abs(endY - originY - (endCellY + 1) * cellSize) <= tol)
            {
                foreach ((int, int) cell in endCells)
                {
                    crossedCells.Add((cell.Item1, cell.Item2 + 1));
                }
            }

            crossedCells.UnionWith(endCells);

            // Edge cases of vertical & horizontal lines
            if (Math.Abs(startX - endX) <= tol)
            {
                int minY = Math.Min(startCellY, endCellY);
                int maxY = Math.Max(startCellY, endCellY);
                double mean = (startX + endX) / 2;

                if (Math.Abs(mean - originX - startCellX * cellSize) <= tol)
                {
                    // Vertical line along the cell edge
                    for (int i = minY; i <= maxY; i++)
                    {
                        crossedCells.Add((startCellX - 1, i));
                        crossedCells.Add((startCellX, i));
                    }
                }
                else if (Math.Abs(mean - originX - (startCellX + 1) * cellSize) <= tol)
                {
                    // Vertical line along the cell edge
                    for (int i = minY; i <= maxY; i++)
                    {
                        crossedCells.Add((startCellX + 1, i));
                        crossedCells.Add((startCellX, i));
                    }
                }
                else
                {
                    for (int i = minY; i <= maxY; i++)
                    {
                        crossedCells.Add((startCellX, i));
                    }
                }

                return crossedCells;
            }
            else if (Math.Abs(startY - endY) <= tol)
            {
                int minX = Math.Min(startCellX, endCellX);
                int maxX = Math.Max(startCellX, endCellX);
                double mean = (startY + endY) / 2;

                if (Math.Abs(mean - originY - startCellY * cellSize) <= tol)
                {
                    // Horizontal line along the cell edge
                    for (int i = minX; i <= maxX; i++)
                    {
                        crossedCells.Add((i, startCellY - 1));
                        crossedCells.Add((i, startCellY));
                    }
                }
                else if (Math.Abs(mean - originY - (startCellY + 1) * cellSize) <= tol)
                {
                    // Horizontal line along the cell edge
                    for (int i = minX; i <= maxX; i++)
                    {
                        crossedCells.Add((i, startCellY + 1));
                        crossedCells.Add((i, startCellY));
                    }
                }
                else
                {
                    for (int i = minX; i <= maxX; i++)
                    {
                        crossedCells.Add((i, startCellY));
                    }
                }

                return crossedCells;
            }

            // Determine the direction of the line
            bool isMovingRight = endX > startX;
            bool isMovingUp = endY > startY;
            double slope = (endY - startY) / (endX - startX);

            // Initialize current cell
            int currentCellX = startCellX;
            int currentCellY = startCellY;
            (int, int) currentCell = (currentCellX, currentCellY);

            while (!endCells.Contains(currentCell))
            {
                // Calculate intersection with vertical line at the relevant cell edge
                double nextVerticalLineX = isMovingRight ? (currentCellX + 1) * cellSize + originX : currentCellX * cellSize + originX;
                double intersectY = startY + slope * (nextVerticalLineX - startX);

                // Check if cutting corners
                double yThreshold = isMovingUp ? (currentCellY + 1) * cellSize + originY : currentCellY * cellSize + originY;
                if (Math.Abs(intersectY - yThreshold) <= tol)
                {
                    int horizontalShift = isMovingRight ? 1 : -1;
                    int verticalShift = isMovingUp ? 1 : -1;
                    crossedCells.Add((currentCellX + horizontalShift, currentCellY));
                    crossedCells.Add((currentCellX, currentCellY + verticalShift));
                    currentCellX += horizontalShift;
                    currentCellY += verticalShift;
                }
                else if ((isMovingUp && intersectY < yThreshold) || (!isMovingUp && intersectY > yThreshold))
                {
                    // Move to the next cell horizontally
                    currentCellX += isMovingRight ? 1 : -1;
                }
                else
                {
                    // Move to the next cell vertically
                    currentCellY += isMovingUp ? 1 : -1;
                }

                currentCell = (currentCellX, currentCellY);
                crossedCells.Add(currentCell);
            }

            return crossedCells;
        }


        public static List<Point> CreatePointGrid(List<Polyline> outlines, List<Polyline> holes, SquareGrid sqGrid, bool?[,] containmentGrid, double radius, double cellRatio, double shiftX, double shiftY, double tol = Tolerance.Distance)
        {
            BoundingBox bbox = outlines.Select(x => x.Bounds()).ToList().Bounds();
            bbox = bbox.Inflate(radius);

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

                    if (IsInsideInclMatrix(p, sqGrid, containmentGrid, outlines, holes, tol))
                        grid.Add(p);
                }
            }

            return grid;
        }


        public static List<Point> CreateDebugGrid(List<Polyline> outlines, List<Polyline> holes, double radius, double cellRatio, double shiftX, double shiftY, double tol = Tolerance.Distance)
        {
            BoundingBox bbox = outlines.Select(x => x.Bounds()).ToList().Bounds();
            bbox = bbox.Inflate(radius);

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

                    if (outlines.Any(o => IsInside(p, o, tol)) && holes.All(h => !IsInside(p, h, tol)))
                        grid.Add(p);
                }
            }

            return grid;
        }

        private static bool IsInsideInclMatrix(Point pt, SquareGrid sqGrid, bool?[,] containmentGrid, List<Polyline> outlines, List<Polyline> holes, double tol)
        {
            int cellX = (int)((pt.X - sqGrid.Origin.X) / sqGrid.CellSize);
            int cellY = (int)((pt.Y - sqGrid.Origin.Y) / sqGrid.CellSize);

            //TODO: check of grid size etc.

            bool? containment = containmentGrid[cellX, cellY];
            if (containment != null)
                return (bool)containment;
            else
                return outlines.Any(o => IsInside(pt, o, tol)) && holes.All(h => !IsInside(pt, h, tol));
        }

        public static Output<List<Line>, List<double>> UncoveredEdges(List<Point> grid, Polyline outline, double radius, double tol = Tolerance.Distance)
        {
            List<Line> uncoveredEdges = new List<Line>();
            double[] minDists = Enumerable.Repeat(double.MaxValue, grid.Count).ToArray();

            //TODO: use R-trees here based on bboxes of circles etc.

            foreach (Line l in outline.SubParts())
            {
                Point start = l.Start;
                Point end = l.End;
                Vector dir = l.Direction();
                Vector perp = new Vector { X = -dir.Y, Y = dir.X };
                double len = l.Length();

                List<(double, double)> coveredParams = new List<(double, double)>();
                int j = 0;
                foreach (Point p in grid)
                {
                    Vector dif = p - start;
                    double t = dir * dif;
                    double dist = Math.Abs(perp * dif);

                    if (t < -tol)
                        minDists[j] = Math.Min(p.Distance(start), minDists[j]);
                    else if (t > len + tol)
                        minDists[j] = Math.Min(p.Distance(end), minDists[j]);
                    else
                        minDists[j] = Math.Min(dist, minDists[j]);

                    j++;
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

            return new Output<List<Line>, List<double>> { Item1 = uncoveredEdges, Item2 = minDists.ToList() };
        }
    }
}
