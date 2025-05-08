using BH.oM.Base;
using BH.oM.Data.Genetic;
using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Geometry
{
    [Description("Object representing a region in a form of a matrix of square cells where each cell has a boolean value indicating if the cell is inside or outside of the region. Null means on edge.")]
    public class ContainmentGrid
    {
        [Description("Bottom left corner of the grid.")]
        public Point Origin { get; set; }

        [Description("Size of the grid cell.")]
        public double CellSize { get; set; }

        [Description("Number of cells in the X direction.")]
        public int CellCountX { get; set; }

        [Description("Number of cells in the Y direction.")]
        public int CellCountY { get; set; }

        [Description("Matrix of boolean values indicating if the cell is inside (true), outside (false) or on edge (null) of the region.")]
        public bool?[,] ContainmentMatrix { get; set; }
    }

    public static partial class Compute
    {
        public static Output<List<Line>, List<Point>, List<Point>> ContainmentGridTest(List<Polyline> outlines, List<Polyline> holes, double cellSize, double offset, double tol)
        {
            List<Point> inside = new List<Point>();
            List<Point> edge = new List<Point>();
            ContainmentGrid grid = ContainmentGrid(outlines, holes, cellSize, offset, tol);
            int cellCountX = grid.CellCountX;
            int cellCountY = grid.CellCountY;
            double dx = cellCountX * cellSize;
            double dy = cellCountY * cellSize;
            Point origin = grid.Origin;
            bool?[,] matrix = grid.ContainmentMatrix;

            for (int m = 0; m < cellCountX; m++)
            {
                for (int n = 0; n < cellCountY; n++)
                {
                    if (matrix[m, n] == null)
                        edge.Add(CellCenter(origin, cellSize, m, n));
                    else if (matrix[m, n] == true)
                        inside.Add(CellCenter(origin, cellSize, m, n));
                }
            }

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

            return new Output<List<Line>, List<Point>, List<Point>>
            {
                Item1 = lns,
                Item2 = inside,
                Item3 = edge
            };
        }

        public static List<List<int>> GetIntersectedCellsTest(Line line, Point origin, double cellSize, double tol = 1e-6)
        {
            return GetIntersectedCells(line.Start, line.End, origin, cellSize, tol).Select(x => new List<int> { x.Item1, x.Item2 }).ToList();
        }

        public static List<Point> FillInRemainderTest2(List<Polyline> availableOutlines, List<Polyline> availableHoles, List<Polyline> toCoverOutlines, List<Polyline> toCoverHoles, List<Point> grid, double cellSize, double offset, double radius, double existingGridPremium = 2, double tol = Tolerance.Distance)
        {
            ContainmentGrid containmentGrid = ContainmentGrid(availableOutlines, availableHoles, cellSize, offset, tol);
            return FillInRemainder(toCoverOutlines, toCoverHoles, containmentGrid, grid, radius, existingGridPremium);
        }

        public static IGeneticAlgorithmResult GeneticSumTest(IGeneticAlgorithmSettings settings, List<DoubleParameter> parameters)
        {
            Func<double[], double> fitnessFunction = SumFunction;
            return IRunGeneticAlgorithm(settings, parameters.ToArray(), fitnessFunction);
        }

        public static IGeneticAlgorithmResult GeneticStDevTest(IGeneticAlgorithmSettings settings, List<DoubleParameter> parameters)
        {
            Func<double[], double> fitnessFunction = StDevFunction;
            return IRunGeneticAlgorithm(settings, parameters.ToArray(), fitnessFunction);
        }

        private static double SumFunction(double[] chromosome)
        {
            // Example fitness function: sum of the chromosome values
            return chromosome.Sum();
        }

        private static double StDevFunction(double[] chromosome)
        {
            // Example fitness function: standard deviation of the chromosome values
            double mean = chromosome.Average();
            double sumOfSquares = chromosome.Sum(x => Math.Pow(x - mean, 2));
            return Math.Sqrt(sumOfSquares / chromosome.Length);
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

        [Description("Creates containment grid for a given region.")]
        private static ContainmentGrid ContainmentGrid(List<Polyline> outlines, List<Polyline> holes, double cellSize, double offset, double tol)
        {
            // Offset the room outline, and map a square grid on it
            BoundingBox bbox = outlines.Select(x => x.Bounds()).ToList().Bounds();
            double dx = bbox.Max.X - bbox.Min.X + offset * 2;
            double dy = bbox.Max.Y - bbox.Min.Y + offset * 2;
            int cellCountX = (int)Math.Ceiling(dx / cellSize) + 1;
            int cellCountY = (int)Math.Ceiling(dy / cellSize) + 1;
            Point origin = (bbox.Min + bbox.Max) / 2 - new Vector { X = cellCountX * cellSize / 2, Y = cellCountY * cellSize / 2 };

            // Initiate the containment matrix
            bool?[,] containment = new bool?[cellCountX, cellCountY];
            for (int m = 0; m < cellCountX; m++)
            {
                for (int n = 0; n < cellCountY; n++)
                {
                    containment[m, n] = false;
                }
            }

            // Mark the cells that are on the edge of the outlines
            foreach (Polyline outline in outlines.Concat(holes))
            {
                for (int i = 0; i < outline.ControlPoints.Count; i++)
                {
                    Point start = outline.ControlPoints[i];
                    Point end = outline.ControlPoints[(i + 1) % outline.ControlPoints.Count];
                    foreach ((int, int) coords in GetIntersectedCells(start, end, origin, cellSize, tol))
                    {
                        containment[coords.Item1, coords.Item2] = null;
                    }
                }
            }

            // Mark the cells that are inside the outlines
            // Going row by row check 1st cell after edge and repeat until edge met again
            for (int m = 0; m < containment.GetLength(0); m++)
            {
                bool inside = false;
                bool onEdge = false;
                for (int n = 0; n < containment.GetLength(1); n++)
                {
                    if (containment[m, n] == null)
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

                    containment[m, n] = inside;
                }
            }

            // Return the object
            return new Geometry.ContainmentGrid
            {
                CellCountX = cellCountX,
                CellCountY = cellCountY,
                CellSize = cellSize,
                Origin = origin,
                ContainmentMatrix = containment
            };
        }

        [Description("Finds the center of a cell at given coordinates.")]
        private static Point CellCenter(Point origin, double cellSize, int cellX, int cellY)
        {
            return new Point
            {
                X = origin.X + (cellX + 0.5) * cellSize,
                Y = origin.Y + (cellY + 0.5) * cellSize
            };
        }

        [Description("Optimised 2d version of IsContaining method.")]
        private static bool IsInside(this Point pt, Polyline outline, double tolerance = 1e-6)
        {
            // Check if point on the edge
            double sqTol = tolerance * tolerance;
            if (outline.ControlPoints.Any(x => x.SquareDistance(pt) <= sqTol))
                return true;

            // Get arbitrary vector that does not point at any corner of the polygon
            Vector dir = Vector.XAxis;
            while (IntersectsWithCorner(pt, dir, outline.ControlPoints, tolerance))
            {
                dir = dir.Rotate(Math.PI / 180, Vector.ZAxis);
                if (1 - dir.DotProduct(Vector.XAxis) <= tolerance)
                    throw new Exception("is inside failed");
            }

            // Count intersectios of the ray with the outline
            int c = 0;
            foreach (Line ln in outline.SubParts())
            {
                if (Intersects(pt, dir, ln))
                    c++;
            }

            // If odd number of intersections, point is inside
            return c % 2 == 1;
        }

        [Description("Checks whether a ray hits any point in the provided set.")]
        private static bool IntersectsWithCorner(Point pt, Vector dir, List<Point> controlPoints, double tolerance)
        {
            return controlPoints.Any(x => 1 - (x - pt).Normalise().DotProduct(dir) <= tolerance);
        }

        [Description("Checks if a ray intersects with a line.")]
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

        [Description("Lists out ids of cells of a containment matrix that are intersected by a line.")]
        private static HashSet<(int, int)> GetIntersectedCells(Point lineStart, Point lineEnd, Point origin, double cellSize, double tol)
        {
            double startX = lineStart.X;
            double startY = lineStart.Y;
            double endX = lineEnd.X;
            double endY = lineEnd.Y;
            double originX = origin.X;
            double originY = origin.Y;

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

            List<(int, int)> startCells = new List<(int, int)> { (startCellX, startCellY) };

            // Edge case when a line start is at grid edge
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

            List<(int, int)> endCells = new List<(int, int)> { (endCellX, endCellY) };

            // Edge case when a line end is at grid edge
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

        [Description("Creates a grid of points inside a given region defined by outlines and holes. Containment grid is used as an optimisation to avoid calling containment checks too often.")]
        private static List<Point> CreatePointGrid(List<Polyline> outlines, List<Polyline> holes, ContainmentGrid containment, double radius, double cellRatio, double shiftX, double shiftY, double tol = Tolerance.Distance)
        {
            BoundingBox bbox = outlines.Select(x => x.Bounds()).ToList().Bounds();
            bbox = bbox.Inflate(radius);

            // Find horizontal cell size (horizontal distance between centers of circles)
            double hor = 2 * radius / Math.Sqrt(1 + cellRatio * cellRatio);

            // Same with vertical
            double ver = cellRatio * hor;

            // Proportional shift
            double horShift = hor * shiftX;
            double verShift = ver * shiftY;

            // Number of cells in each direction
            int horSteps = (int)Math.Ceiling((bbox.Max.X - bbox.Min.X) / hor);
            int verSteps = (int)Math.Ceiling((bbox.Max.Y - bbox.Min.Y) / ver);

            // Generate the grid
            List<Point> grid = new List<Point>();
            for (int i = 0; i < horSteps; i++)
            {
                for (int j = 0; j < verSteps; j++)
                {
                    double x = bbox.Min.X + hor * i + horShift;
                    double y = bbox.Min.Y + ver * j + verShift;
                    Point p = new Point { X = x, Y = y };

                    // Check if the point is in the region, containment grid used to speed things up
                    if (IsInside(p, containment, outlines, holes, tol))
                        grid.Add(p);
                }
            }

            return grid;
        }

        [Description("Checks if a point is in the region, containment grid used for performance purposes.")]
        private static bool IsInside(Point pt, ContainmentGrid grid, List<Polyline> outlines, List<Polyline> holes, double tol)
        {
            int cellX = (int)((pt.X - grid.Origin.X) / grid.CellSize);
            int cellY = (int)((pt.Y - grid.Origin.Y) / grid.CellSize);

            bool? containment = grid.ContainmentMatrix[cellX, cellY];
            if (containment != null)
                return (bool)containment;
            else
                return outlines.Any(o => IsInside(pt, o, tol)) && holes.All(h => !IsInside(pt, h, tol));
        }

        [Description("Finds segments of edges of an outline that are not inside any circle defined by given grid.")]
        private static Output<List<Line>, List<double>> UncoveredEdges(Polyline outline, List<Point> grid, double radius, double tol = Tolerance.Distance)
        {
            List<Line> uncoveredEdges = new List<Line>();
            double[] minDists = Enumerable.Repeat(double.MaxValue, grid.Count).ToArray();

            //TODO: profiling shows that hyper-optimisation of maths could help a lot here
            // Per each line find the parameter ranges that are covered by circles
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
                    double tMax = t + along;
                    double tMin = t - along;
                    coveredParams.Add((tMin / len, tMax / len));
                }

                // Cull out irrelevant values
                coveredParams = coveredParams.Where(x => x.Item1 < 1 - tol && x.Item2 > tol).OrderBy(x => x.Item1).ToList();
                if (coveredParams.Count == 0)
                {
                    uncoveredEdges.Add(l);
                    continue;
                }

                // Join covered params that overlap, include tol
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

                // Create uncovered edges
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

        [Description("Fills in the uncovered area of a region with circles of given radius. Containment grid used to speed up maths. Existing grid premium is a multiplier applied to candidates that align with the main grid provided.")]
        private static List<Point> FillInRemainder(List<Polyline> outlines, List<Polyline> holes, ContainmentGrid containmentGrid, List<Point> grid, double radius, double existingGridPremium = 2, double tol = Tolerance.Distance)
        {
            int cellCountX = containmentGrid.CellCountX;
            int cellCountY = containmentGrid.CellCountY;
            double cellSize = containmentGrid.CellSize;
            Point origin = containmentGrid.Origin;

            List<Point> result = new List<Point>();

            // Find boolean difference of region outlines with circles
            List<PolyCurve> diff = outlines.SelectMany(x => x.BooleanDifference(holes.Cast<ICurve>().Concat(grid.Select(y => new Circle { Centre = y, Radius = radius })), tol)).ToList();

            // Turn the outlines to lines and find the cells that are intersected by these lines
            List<Line> segments = diff.SelectMany(x => x.CollapseToPolyline(Math.PI / 32).SubParts()).ToList();
            HashSet<(int, int)> toCover = new HashSet<(int, int)>(segments.SelectMany(x => GetIntersectedCells(x.Start, x.End, containmentGrid.Origin, containmentGrid.CellSize, tol)));

            // Find relative addresses of cells that are covered by a single circle at address 0,0
            (int, int)[] circleRange = CircleRange(radius, containmentGrid.CellSize);

            // Find cells that host centres of circles
            // Also find unique X and Y coordinates of the grid
            Dictionary<int, List<int>> gridRows = new Dictionary<int, List<int>>();
            Dictionary<int, List<int>> gridCols = new Dictionary<int, List<int>>();
            List<double> gridXs = new List<double>();
            List<double> gridYs = new List<double>();
            foreach (Point p in grid)
            {
                if (gridXs.All(x => Math.Abs(x - p.X) > tol))
                    gridXs.Add(p.X);

                if (gridYs.All(x => Math.Abs(x - p.Y) > tol))
                    gridYs.Add(p.Y);

                int xInt = (int)((p.X - origin.X) / cellSize);
                int yInt = (int)((p.Y - origin.Y) / cellSize);

                if (!gridRows.ContainsKey(xInt))
                    gridRows.Add(xInt, new List<int>());

                if (!gridCols.ContainsKey(yInt))
                    gridCols.Add(yInt, new List<int>());

                gridRows[xInt].Add(yInt);
                gridCols[yInt].Add(xInt);
            }

            // Find the span of the point grid in X direction
            int cellSpanX = int.MaxValue;
            if (gridXs.Count > 1)
            {
                gridXs.Sort();
                cellSpanX = (int)((gridXs[1] - gridXs[0]) / cellSize) + 2;
            }

            // Find the span of the point grid in Y direction
            int cellSpanY = int.MaxValue;
            if (gridYs.Count > 1)
            {
                gridYs.Sort();
                cellSpanY = (int)((gridYs[1] - gridYs[0]) / cellSize) + 2;
            }

            // Find candidate points that can host the extra circles
            Dictionary<(int, int), int> cands = CoverageCandidates(containmentGrid.ContainmentMatrix, toCover, circleRange);
            while (toCover.Count != 0)
            {
                if (cands.Count == 0)
                {
                    BH.Engine.Base.Compute.RecordError("Something went wrong when filling in the remainder space. Please review the results with care.");
                    return result;
                }

                // Boost the scores based on grid alignement
                foreach ((int, int) key in cands.Keys.ToList())
                {
                    double value = cands[key];
                    if (gridRows.ContainsKey(key.Item1) && gridRows[key.Item1].Any(x => Math.Abs(x - key.Item2) <= cellSpanY))
                        value *= existingGridPremium;

                    if (gridCols.ContainsKey(key.Item2) && gridCols[key.Item2].Any(x => Math.Abs(x - key.Item1) <= cellSpanX))
                        value *= existingGridPremium;

                    cands[key] = (int)Math.Round(value);
                }

                // Find topscorer and add it to the output
                (int, int) bestCand = cands.OrderByDescending(x => x.Value).First().Key;
                result.Add(CellCenter(origin, cellSize, bestCand.Item1, bestCand.Item2));

                // Add the topscorer coordinates to cache
                if (!gridRows.ContainsKey(bestCand.Item1))
                    gridRows.Add(bestCand.Item1, new List<int>());

                gridRows[bestCand.Item1].Add(bestCand.Item2);

                if (!gridCols.ContainsKey(bestCand.Item2))
                    gridCols.Add(bestCand.Item2, new List<int>());

                gridCols[bestCand.Item2].Add(bestCand.Item1);

                // Remove the space covered by candidate from the search
                (int, int)[] coveredByCand = circleRange.Select(x => (bestCand.Item1 + x.Item1, bestCand.Item2 + x.Item2)).ToArray();
                toCover.ExceptWith(coveredByCand);

                //TODO: could speed up by reducing cand dict rather than recomputing it all
                // Recalculate the candidates
                cands = CoverageCandidates(containmentGrid.ContainmentMatrix, toCover, circleRange);
            }

            // Align newly added points with the primary grid
            foreach (Point pt in result)
            {
                int idX = gridXs.FindIndex(x => Math.Abs(x - pt.X) <= cellSize / 2 + tol);
                if (idX != -1)
                    pt.X = gridXs[idX];

                int idY = gridYs.FindIndex(x => Math.Abs(x - pt.Y) <= cellSize / 2 + tol);
                if (idY != -1)
                    pt.Y = gridYs[idY];
            }

            return result;
        }

        [Description("Find candidate points that can host the extra circles.")]
        private static Dictionary<(int, int), int> CoverageCandidates(bool?[,] containment, IEnumerable<(int, int)> toCover, (int, int)[] circleRange)
        {
            Dictionary<(int, int), int> result = new Dictionary<(int, int), int>();

            int dimM = containment.GetLength(0);
            int dimN = containment.GetLength(1);
            foreach ((int, int) cell in toCover)
            {
                foreach ((int, int) item in circleRange)
                {
                    int m = cell.Item1 + item.Item1;
                    int n = cell.Item2 + item.Item2;

                    if (m < 0 || m >= dimM || n < 0 || n >= dimN)
                        continue;

                    if (containment[m, n] == true)
                    {
                        (int m, int n) toAdd = (m, n);
                        if (result.ContainsKey(toAdd))
                            result[toAdd]++;
                        else
                            result.Add(toAdd, 1);
                    }
                }
            }

            return result;
        }

        [Description("Finds relative addresses of cells that are covered by a single circle at address 0,0")]
        private static (int, int)[] CircleRange(double radius, double cellSize)
        {
            List<(int, int)> result = new List<(int, int)>();
            double sqR = radius * radius;
            int range = (int)(radius / cellSize);
            for (int m = -range; m <= 0; m++)
            {
                for (int n = -range; n <= 0; n++)
                {
                    double mm = m * cellSize;
                    double nn = n * cellSize;
                    if (mm * mm + nn * nn <= sqR)
                    {
                        (int, int)[] a = new (int, int)[]
                        {
                            (m, n),
                            (-m, n),
                            (-m, -n),
                            (m, -n)
                        };

                        result.AddRange(a.Distinct());
                    }
                }
            }

            return result.ToArray();
        }
    }
}