using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Compute
    {
        public static List<Point> CoverWithCircles(List<Polyline> coverageOutlines, List<Polyline> availableOutlines, double radius, int gridDensity, List<Polyline> coverageHoles = null, List<Polyline> availableHoles = null)
        {
            // Density needs to be an odd number
            if (gridDensity % 2 != 1)
                throw new Exception();

            coverageHoles = coverageHoles ?? new List<Polyline>();
            availableHoles = availableHoles ?? new List<Polyline>();

            Dictionary<Vector, double> dirSignificance = new Dictionary<Vector, double>();
            foreach (Line l in coverageOutlines.Concat(coverageHoles).SelectMany(x => x.SubParts()))
            {
                Vector dir = l.Direction();
                double len = l.Length();
                var key = dirSignificance.Keys.FirstOrDefault(x => 1 - Math.Abs(x.DotProduct(dir)) <= 1e-3);
                if (key == null)
                {
                    dirSignificance.Add(dir, len);
                    key = dir;
                }

                dirSignificance[key] += len;
            }

            double totalOutline = dirSignificance.Values.Sum();
            foreach(var key in dirSignificance.Keys.ToList())
            {
                double normalised = dirSignificance[key] / totalOutline;
                if (normalised <= 0.1)
                    dirSignificance.Remove(key);
                else
                    dirSignificance[key] = normalised;
            }

            dirSignificance = dirSignificance.Where(x => x.Value >= totalOutline * 0.1).ToDictionary(x => x.Key, x => x.Value);


            foreach (Point point in coverageOutlines.Concat(coverageHoles).Concat(availableOutlines).Concat(availableHoles).SelectMany(x => x.ControlPoints))
                point.Z = 0;

            m_Range = CircleRange(gridDensity);

            //TODO: try alternative ratios to fit edge case widths
            double horizontalGridSize = 2 * radius / Math.Sqrt(2) / gridDensity;
            double verticalGridSize = 2 * radius / Math.Sqrt(2) / gridDensity;
            double diagonal = Math.Sqrt(verticalGridSize * verticalGridSize + horizontalGridSize * horizontalGridSize);

            //TODO: orient to primary direction

            var bbox = coverageOutlines.SelectMany(x => x.ControlPoints()).ToList().Bounds();
            int horizontalSteps = (int)Math.Ceiling((bbox.Max.X - bbox.Min.X) / horizontalGridSize);
            int verticalSteps = (int)Math.Ceiling((bbox.Max.Y - bbox.Min.Y) / verticalGridSize);

            bool[,] toCover = new bool[horizontalSteps, verticalSteps];
            bool[,] available = new bool[horizontalSteps, verticalSteps];
            double[,] distances = new double[horizontalSteps, verticalSteps];
            int[,] depths = new int[horizontalSteps, verticalSteps];
            //List<(int, int, double)> availableByDistance = new List<(int, int, double)>();

            List<(int, int)> outlineCells = new List<(int, int)>();
            for (int m = 0; m < horizontalSteps; m++)
            {
                for (int n = 0; n < verticalSteps; n++)
                {
                    var corners = Corners(bbox.Min.X, bbox.Min.Y, m, n, horizontalGridSize, verticalGridSize);
                    toCover[m, n] = corners.IsAtLeastPartlyInside(coverageOutlines, coverageHoles);
                    if (toCover[m, n])
                    {
                        double dist = coverageOutlines.Select(x => corners.Center().ToPoint().Distance(x)).Min();
                        distances[m, n] = dist;

                        if (dist <= diagonal / 2 + 1e-6)
                        {
                            depths[m, n] = 1;
                            outlineCells.Add((m, n));
                        }
                    }

                    available[m, n] = corners.IsAtLeastPartlyInside(availableOutlines, availableHoles);
                }
            }

            for (int m = 0; m < horizontalSteps; m++)
            {
                for (int n = 0; n < verticalSteps; n++)
                {
                    if (toCover[m, n] && depths[m, n] == 0)
                        depths[m, n] = outlineCells.Select(x => Distance(x.Item1, x.Item2, m, n)).Min() + 1;
                }
            }


            //Print(toCover, "To cover:");
            //Print(available, "Available:");
            //Print(depths, "Depths:");

            List<(int, int)> result = new List<(int, int)>();


            //TODO: really weird results with circ shape in cells
            //int[,] allSpaceCoverage = CalculateAllSpaceCoverage(toCover, available, gridDensity);
            //double[,] candidateCoverage = CalculateCandidateCoverage2(toCover, available, depths, gridDensity);
            //(int, int) lowest = allSpaceCoverage.Lowest();
            //while (lowest != (-1, -1))
            //{
            //    Print(allSpaceCoverage, $"To cover, iteration {result.Count}:");
            //    Print(candidateCoverage, $"Candidates, iteration {result.Count}:");

            //    (int, int) next = NextBasedOnCoverage(candidateCoverage, lowest.Item1, lowest.Item2);
            //    if (next == (-1, -1))
            //        break;

            //    result.Add(next);

            //    SetAsCovered(toCover, next.Item1, next.Item2, gridDensity);
            //    //SetAsCovered(available, lowest.Item1, lowest.Item2, gridDensity);
            //    //foreach (var cell in result)
            //    //{
            //    //    available[cell.Item1, cell.Item2] = true;
            //    //}


            //    allSpaceCoverage = CalculateAllSpaceCoverage(toCover, available, gridDensity);
            //    candidateCoverage = CalculateCandidateCoverage2(toCover, available, depths, gridDensity);
            //    lowest = allSpaceCoverage.Lowest();
            //}

            //int[,] coverage = CalculateCandidateCoverage(toCover, available, gridDensity);
            //(int, int) lowest = coverage.Lowest();
            //while (lowest != (-1, -1))
            //{
            //    Print(coverage, $"Coverage, iteration {result.Count}:");

            //    if (IsOnlyParentForAnyCell(available, toCover, lowest.Item1, lowest.Item2, gridDensity))
            //    {
            //        result.Add(lowest);
            //        SetAsCovered(toCover, lowest.Item1, lowest.Item2, gridDensity);
            //    }

            //    available[lowest.Item1, lowest.Item2] = false;

            //    coverage = CalculateCandidateCoverage(toCover, available, gridDensity);
            //    lowest = coverage.Lowest();
            //}

            //(int, int) next;
            //while (true)
            //{
            //    next = NextPlacement(toCover, available, distances, depths, result, gridDensity);
            //    if (next != (-1, -1))
            //    {
            //        SetAsCovered(toCover, next.Item1, next.Item2, gridDensity);
            //        result.Add(next);
            //    }
            //    else
            //        break;
            //}

            //double[,] candidateScores = ScoreCandidates(toCover, available, distances, depths, result, gridDensity);
            //int topScorersCount = 10;
            //(int, int)[] topScorers = candidateScores.TopScorers(topScorersCount);
            //while (topScorers.Length != 0)
            //{
            //    (int, int) top = (-1, -1);
            //    double bestRatio = 0;
            //    bool end = false;
            //    foreach (var candidate in topScorers)
            //    {
            //        bool[,] clone = toCover.MyClone();
            //        SetAsCovered(clone, candidate.Item1, candidate.Item2, gridDensity);
            //        double ratio = ShapeRatio(clone);
            //        if (double.IsNaN(ratio) || ratio == 0)
            //        {
            //            top = candidate;
            //            end = true;
            //            break;
            //        }
            //        else if (ratio > bestRatio)
            //        {
            //            top = candidate;
            //            bestRatio = ratio;
            //        }
            //    }

            //    Print(candidateScores, $"Candidate scores, iteration {result.Count}:");

            //    result.Add(top);
            //    SetAsCovered(toCover, top.Item1, top.Item2, gridDensity);
            //    if (end)
            //        break;

            //    candidateScores = ScoreCandidates(toCover, available, distances, depths, result, gridDensity);
            //    topScorers = candidateScores.TopScorers(topScorersCount);
            //}

            double[,] candidateScores = ScoreCandidates(toCover, available, distances, depths, result, dirSignificance, gridDensity);
            (int, int) topScorer = candidateScores.TopScorer();
            while (topScorer != (-1, -1))
            {

                Print(candidateScores, $"Candidate scores, iteration {result.Count}:");

                result.Add(topScorer);
                SetAsCovered(toCover, topScorer.Item1, topScorer.Item2, gridDensity);

                candidateScores = ScoreCandidates(toCover, available, distances, depths, result, dirSignificance, gridDensity);
                topScorer = candidateScores.TopScorer();
            }


            ////TODO: introduce tolerance comparer, also thenby x, y to keep order
            //availableByDistance.OrderBy(x => x.Item3);

            //foreach (var cell in availableByDistance)
            //{
            //    int m = cell.Item1;
            //    int n = cell.Item2;

            //    bool keep = IsNeeded(available, toCover, m, n, gridDensity);
            //    if (keep)
            //    {
            //        SetAsCovered(toCover, m, n, gridDensity);
            //        result.Add((m, n));
            //    }

            //    available[m, n] = keep;
            //}

            return result.Select(x => Corners(bbox.Min.X, bbox.Min.Y, x.Item1, x.Item2, horizontalGridSize, verticalGridSize).Center().ToPoint()).ToList();
        }

        private static T[,] MyClone<T>(this T[,] original)
        {
            int rows = original.GetLength(0);
            int columns = original.GetLength(1);
            T[,] clone = new T[rows, columns];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    clone[i, j] = original[i, j];
                }
            }

            return clone;
        }

        private static List<(int, int)> m_Range = null;

        private static IEnumerable<(int, int)> CircleCells(int m, int n)
        {
            foreach (var cell in m_Range)
            {
                yield return (cell.Item1 + m, cell.Item2 + n);
            }
        }

        private static List<(int, int)> SquareRange(int gridDensity)
        {
            List<(int, int)> result = new List<(int, int)>();
            int range = gridDensity / 2;
            for (int m = -range; m <= range; m++)
            {
                for (int n = -range; n <= range; n++)
                {
                    result.Add((m, n));
                }
            }

            return result;
        }

        private static List<(int, int)> CircleRange(int gridDensity)
        {
            List<(int, int)> result = new List<(int, int)>();
            double r = ((double)gridDensity) / 2 * Math.Sqrt(2);
            double sqR = r * r;
            int range = (int)(r - 0.5);
            for (int m = -range; m <= 0; m++)
            {
                for (int n = -range; n <= 0; n++)
                {
                    double mm = m - 0.5;
                    double nn = n - 0.5;
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

            return result;
        }

        private static List<(int, int)> CircleOrSquareRange(int gridDensity)
        {
            // square edge if none of the cells on the given side is at the edge
            // otherwise circle
            throw new NotImplementedException();
        }

        private static (int, int) NextBasedOnCoverage(double[,] candidateCoverage, int m, int n)
        {
            int rangeM = candidateCoverage.GetLength(0);
            int rangeN = candidateCoverage.GetLength(1);

            (int, int) result = (-1, -1);
            double maxCoverage = 0;
            double maxDistance = 0;
            foreach (var cell in CircleCells(m, n))
            {
                int m1 = cell.Item1;
                int n1 = cell.Item2;

                if (m1 < 0 || m1 >= rangeM)
                    continue;

                if (n1 < 0 || n1 >= rangeN)
                    continue;

                if (candidateCoverage[m1, n1] == 0)
                    continue;

                if (candidateCoverage[m1, n1] == maxCoverage)
                {
                    double distance = RealDistance(m, n, m1, n1);
                    if (distance > maxDistance)
                    {
                        maxDistance = distance;
                        result = (m1, n1);
                    }
                }
                else if (candidateCoverage[m1, n1] > maxCoverage)
                {
                    maxCoverage = candidateCoverage[m1, n1];
                    result = (m1, n1);
                }
            }

            return result;
        }

        private static (int, int) NextPlacement(bool[,] toCover, bool[,] available, double[,] distances, int[,] depths, List<(int, int)> alreadyFound, int gridDensity)
        {
            int dimM = toCover.GetLength(0);
            int dimN = toCover.GetLength(1);

            int range = gridDensity / 2;

            for (int m = 0; m < dimM; m++)
            {
                for (int n = 0; n < dimN; n++)
                {
                    if (toCover[m, n])
                    {
                        (int, int) result = (-1, -1);
                        
                        double maxScore = 0;

                        for (int m1 = m - range; m1 <= m + range; m1++)
                        {
                            if (m1 < 0 || m1 >= dimM)
                                continue;

                            for (int n1 = n - range; n1 <= n + range; n1++)
                            {
                                if (n1 < 0 || n1 >= dimN)
                                    continue;

                                if (!toCover[m1, n1])
                                    continue;

                                double depth = (double)depths[m1, n1];
                                double distance = RealDistance(m, n, m1, n1);
                                double score = distance * Math.Sqrt(depth);
                                if (score > maxScore)
                                {
                                    maxScore = score;
                                    result = (m1, n1);
                                }
                            }
                        }

                        //int maxDepth = 0;
                        //double maxDistance = 0;

                        //for (int m1 = m - range; m1 <= m + range; m1++)
                        //{
                        //    if (m1 < 0 || m1 >= dimM)
                        //        continue;

                        //    for (int n1 = n - range; n1 <= n + range; n1++)
                        //    {
                        //        if (n1 < 0 || n1 >= dimN)
                        //            continue;

                        //        if (!toCover[m1, n1])
                        //            continue;

                        //        int depth = depths[m1, n1];
                        //        if (depth == maxDepth)
                        //        {
                        //            double distance = RealDistance(m, n, m1, n1);
                        //            if (distance > maxDistance)
                        //            {
                        //                maxDistance = distance;
                        //                result = (m1, n1);
                        //            }
                        //        }
                        //        else if (depth > maxDepth)
                        //        {
                        //            maxDepth = depth;
                        //            result = (m1, n1);
                        //        }
                        //    }
                        //}

                        return result;
                    }
                }
            }

            return (-1, -1);
        }

        private static double[,] ScoreCandidates(bool[,] toCover, bool[,] available, double[,] distances, int[,] depths, List<(int, int)> alreadyFound, Dictionary<Vector, double> dirSignificance, int gridDensity)
        {
            Dictionary<Line, double> gridPromoters = new Dictionary<Line, double>();
            foreach (var found in alreadyFound)
            {
                foreach (var dirs in dirSignificance)
                {
                    Point start = new Point { X = found.Item1, Y = found.Item2 };
                    Line grid = new Line { Start=start, End = start + dirs.Key };
                    if (gridPromoters.Keys.All(x => !(x.IsParallel(grid) == 0)))
                        gridPromoters.Add(grid, dirs.Value);
                }
            }

            int dimM = toCover.GetLength(0);
            int dimN = toCover.GetLength(1);

            int range = gridDensity / 2;

            double[,] result = new double[dimM, dimN];
            for (int m = 0; m < dimM; m++)
            {
                for (int n = 0; n < dimN; n++)
                {
                    if (!available[m, n])
                        continue;


                    bool[,] clone = toCover.MyClone();
                    SetAsCovered(clone, m, n, gridDensity);
                    //double ratio = ShapeRatio(clone);
                    //if (double.IsNaN(ratio))
                    //    ratio = 100;

                    double perimeter = (double)Perimeter(clone);
                    perimeter = perimeter == 0 ? 1e-6 : perimeter;

                    //TODO:
                    //promote candidates on same axis as others

                    Dictionary<int, int> byDepth = new Dictionary<int, int>();
                    for (int m1 = m - range; m1 <= m + range; m1++)
                    {
                        if (m1 < 0 || m1 >= dimM)
                            continue;

                        for (int n1 = n - range; n1 <= n + range; n1++)
                        {
                            if (n1 < 0 || n1 >= dimN)
                                continue;

                            if (!toCover[m1, n1])
                                continue;

                            int depth = depths[m1, n1];
                            if (!byDepth.ContainsKey(depth))
                                byDepth[depth] = 0;

                            byDepth[depth]++;
                        }
                    }

                    double promotion = gridPromoters.Where(x => IsOnLine(m, n, x.Key)).Sum(x => x.Value);

                    double neighbourDepthFactor = byDepth.Sum(x => ((double)x.Value) / ((double)x.Key));
                    int depthFactor = depths[m, n];
                    //double distanceToFoundFactor = alreadyFound.Count == 0 ? 1 : alreadyFound.Sum(x => Distance(x.Item1, x.Item2, m, n));

                    //result[m, n] = neighbourDepthFactor / (distanceToFoundFactor * perimeter);
                    result[m, n] = (neighbourDepthFactor / perimeter) * (1 + promotion);
                }
            }

            return result;
        }

        private static bool IsOnLine(int m, int n, Line l)
        {
            return new Point { X = m, Y = n }.Distance(l, true) <= 0.5;
        }

        private static int Perimeter(this bool[,] matrix)
        {
            int dimM = matrix.GetLength(0);
            int dimN = matrix.GetLength(1);

            int c = 0;
            for (int m = 0; m < dimM; m++)
            {
                for (int n = 0; n < dimN; n++)
                {
                    if (matrix[m, n] && IsOnEdge(matrix, m, n))
                        c++;
                }
            }

            return c;
        }

        private static int Area(this bool[,] matrix)
        {
            int c = 0;
            foreach (bool value in matrix.Values())
            {
                if (value)
                    c++;
            }

            return c;
        }

        private static IEnumerable<T> Values<T>(this T[,] matrix)
        {
            int dimM = matrix.GetLength(0);
            int dimN = matrix.GetLength(1);

            for (int m = 0; m < dimM; m++)
            {
                for (int n = 0; n < dimN; n++)
                {
                    yield return matrix[m, n];
                }
            }
        }

        private static bool IsOnEdge(bool[,] matrix, int m, int n)
        {
            int dimM = matrix.GetLength(0);
            int dimN = matrix.GetLength(1);

            for (int m1 = m - 1; m1 <= m + 1; m1++)
            {
                if (m1 == -1 || m1 == dimM)
                    continue;

                for (int n1 = n - 1; n1 <= n + 1; n1++)
                {
                    if (n1 == -1 || n1 == dimN)
                        continue;

                    if (matrix[m1, n1] == false)
                        return true;
                }
            }

            return false;
        }

        private static int[,] CalculateCandidateCoverage(bool[,] toCover, bool[,] available, int gridDensity)
        {
            int dimM = toCover.GetLength(0);
            int dimN = toCover.GetLength(1);

            int range = gridDensity / 2;

            int[,] result = new int[dimM, dimN];
            for (int m = 0; m < dimM; m++)
            {
                for (int n = 0; n < dimN; n++)
                {
                    if (!available[m, n])
                        continue;

                    int c = 0;
                    foreach (var cell in CircleCells(m, n))
                    {
                        int m1 = cell.Item1;
                        if (m1 < 0 || m1 >= dimM)
                            continue;

                        int n1 = cell.Item2;
                        if (n1 < 0 || n1 >= dimN)
                            continue;

                        if (toCover[m1, n1])
                            c++;
                    }

                    result[m, n] = c;
                }
            }

            return result;
        }

        private static double[,] CalculateCandidateCoverage2(bool[,] toCover, bool[,] available, int[,] depths, int gridDensity)
        {
            int dimM = toCover.GetLength(0);
            int dimN = toCover.GetLength(1);

            int range = gridDensity / 2;

            double[,] result = new double[dimM, dimN];
            for (int m = 0; m < dimM; m++)
            {
                for (int n = 0; n < dimN; n++)
                {
                    if (!available[m, n])
                        continue;

                    double c = 0;
                    foreach (var cell in CircleCells(m, n))
                    {
                        int m1 = cell.Item1;
                        if (m1 < 0 || m1 >= dimM)
                            continue;

                        int n1 = cell.Item2;
                        if (n1 < 0 || n1 >= dimN)
                            continue;

                        if (toCover[m1, n1])
                            c += 1.0 / (double)depths[m1, n1];
                    }

                    result[m, n] = c;
                }
            }

            return result;
        }

        private static int[,] CalculateAllSpaceCoverage(bool[,] toCover, bool[,] available, int gridDensity)
        {
            int dimM = toCover.GetLength(0);
            int dimN = toCover.GetLength(1);

            int range = gridDensity / 2;

            int[,] result = new int[dimM, dimN];
            for (int m = 0; m < dimM; m++)
            {
                for (int n = 0; n < dimN; n++)
                {
                    if (!toCover[m, n])
                        continue;

                    int c = 0;
                    foreach(var cell in CircleCells(m, n))
                    {
                        int m1 = cell.Item1;
                        if (m1 < 0 || m1 >= dimM)
                            continue;

                        int n1 = cell.Item2;
                        if (n1 < 0 || n1 >= dimN)
                            continue;

                        if (available[m1, n1])
                            c++;
                    }

                    result[m, n] = c;
                }
            }

            return result;
        }

        private static int Distance(int m, int n, int refM, int refN)
        {
            int distM = m - refM;
            if (distM < 0)
                distM = -distM;

            int distN = n - refN;
            if (distN < 0)
                distN = -distN;

            return distM > distN ? distM : distN;
        }

        private static double RealDistance(int m, int n, int refM, int refN)
        {
            int distM = m - refM;
            int distN = n - refN;

            return Math.Sqrt(distM * distM + distN * distN);
        }


        private static int CountToCover(bool[,] toCover, int m, int n, int gridDensity)
        {
            int c = 0;
            int rangeM = toCover.GetLength(0);
            int rangeN = toCover.GetLength(1);

            int range = gridDensity / 2;
            for (int m1 = m - range; m1 <= m + range; m1++)
            {
                if (m1 < 0 || m1 >= rangeM)
                    continue;

                for (int n1 = n - range; n1 <= n + range; n1++)
                {
                    if (n1 < 0 || n1 >= rangeN)
                        continue;

                    if (toCover[m1, n1])
                        c++;
                }
            }

            return c;
        }

        private static (int, int) TopScorer(this double[,] array)
        {
            double maxValue = 0;
            (int, int) result = (-1, -1);
            for (int m = 0; m < array.GetLength(0); m++)
            {
                for (int n = 0; n < array.GetLength(1); n++)
                {
                    if (array[m, n] > maxValue)
                    {
                        maxValue = array[m, n];
                        result = (m, n);
                    }
                }
            }

            return result;
        }

        private static (int, int)[] TopScorers(this double[,] array, int count)
        {
            Dictionary<double, List<(int, int)>> results = new Dictionary<double, List<(int, int)>>();
            for (int m = 0; m < array.GetLength(0); m++)
            {
                for (int n = 0; n < array.GetLength(1); n++)
                {
                    if (!results.ContainsKey(array[m, n]))
                        results.Add(array[m, n], new List<(int, int)>());

                    results[array[m, n]].Add((m, n));
                }
            }

            return results.OrderByDescending(x => x.Key).SelectMany(x => x.Value).Take(count).ToArray();
        }

        private static (int, int) Lowest(this int[,] array)
        {
            int minValue = int.MaxValue;
            (int, int) result = (-1, -1);
            for (int m = 0; m < array.GetLength(0); m++)
            {
                for (int n = 0; n < array.GetLength(1); n++)
                {
                    if (array[m, n] == 0)
                        continue;

                    if (array[m, n] < minValue)
                    {
                        minValue = array[m, n];
                        result = (m, n);
                    }
                }
            }

            return result;
        }

        private static bool IsNeeded(bool[,] available, bool[,] toCover, int m, int n, int gridDensity)
        {
            int rangeM = toCover.GetLength(0);
            int rangeN = toCover.GetLength(1);

            int range = gridDensity / 2;

            int m1, n1;

            m1 = m - range;
            if (m1 >= 0)
            {
                for (n1 = n - range; n1 <= n + range; n1++)
                {
                    if (n1 < 0 || n1 >= rangeN)
                        continue;

                    //if (m1 == m && n1 == n)
                    //    continue;

                    if (!toCover[m1, n1])
                        continue;

                    if (!HasMoreThanOneParents(available, m1, n1, gridDensity))
                        return true;
                }
            }

            m1 = m + range;
            if (m1 < rangeM)
            {
                for (n1 = n - range; n1 <= n + range; n1++)
                {
                    if (n1 < 0 || n1 >= rangeN)
                        continue;

                    //if (m1 == m && n1 == n)
                    //    continue;

                    if (!toCover[m1, n1])
                        continue;

                    if (!HasMoreThanOneParents(available, m1, n1, gridDensity))
                        return true;
                }
            }

            n1 = n - range;
            if (n1 >= 0)
            {
                for (m1 = m - range + 1; m1 <= m + range - 1; m1++)
                {
                    if (m1 < 0 || m1 >= rangeM)
                        continue;

                    //if (m1 == m && n1 == n)
                    //    continue;

                    if (!toCover[m1, n1])
                        continue;

                    if (!HasMoreThanOneParents(available, m1, n1, gridDensity))
                        return true;
                }
            }

            n1 = n + range;
            if (n1 < rangeN)
            {
                for (m1 = m - range + 1; m1 <= m + range - 1; m1++)
                {
                    if (m1 < 0 || m1 >= rangeM)
                        continue;

                    //if (m1 == m && n1 == n)
                    //    continue;

                    if (!toCover[m1, n1])
                        continue;

                    if (!HasMoreThanOneParents(available, m1, n1, gridDensity))
                        return true;
                }
            }

            return false;
        }

        private static bool IsOnlyParentForAnyCell(bool[,] available, bool[,] toCover, int m, int n, int gridDensity)
        {
            int dimM = toCover.GetLength(0);
            int dimN = toCover.GetLength(1);

            int range = gridDensity / 2;

            for (int m1 = m - range; m1 <= m + range; m1++)
            {
                if (m1 < 0 || m1 >= dimM)
                    continue;

                for (int n1 = n - range; n1 <= n + range; n1++)
                {
                    if (n1 < 0 || n1 >= dimN)
                        continue;

                    if (!toCover[m1, n1])
                        continue;

                    if (!HasMoreThanOneParents(available, m1, n1, gridDensity))
                        return true;
                }
            }

            return false;
        }

        private static bool HasMoreThanOneParents(bool[,] available, int m, int n, int gridDensity)
        {
            bool firstFound = false;
            int rangeM = available.GetLength(0);
            int rangeN = available.GetLength(1);

            int range = gridDensity / 2;
            for (int m1 = m - range; m1 <= m + range; m1++)
            {
                if (m1 < 0 || m1 >= rangeM)
                    continue;

                for (int n1 = n - range; n1 <= n + range; n1++)
                {
                    if (n1 < 0 || n1 >= rangeN)
                        continue;

                    if (m1 == m && n1 == n)
                        continue;

                    if (available[m1, n1])
                    {
                        if (firstFound)
                            return true;
                        else
                            firstFound = true;
                    }
                }
            }

            return false;
        }

        private static void SetAsCovered(bool[,] toCover, int m, int n, int gridDensity)
        {
            int rangeM = toCover.GetLength(0);
            int rangeN = toCover.GetLength(1);

            //int range = gridDensity / 2;
            //for (int m1 = m - range; m1 <= m + range; m1++)
            //{
            //    if (m1 < 0 || m1 >= rangeM)
            //        continue;

            //    for (int n1 = n - range; n1 <= n + range; n1++)
            //    {
            //        if (n1 < 0 || n1 >= rangeN)
            //            continue;

            //        toCover[m1, n1] = false;
            //    }
            //}

            foreach (var cell in CircleCells(m, n))
            {
                int m1 = cell.Item1;
                int n1 = cell.Item2;

                if (m1 < 0 || m1 >= rangeM)
                    continue;

                if (n1 < 0 || n1 >= rangeN)
                    continue;

                toCover[m1, n1] = false;
            }
        }

        private static void Print(this bool[,] matrix, string header)
        {
            System.Diagnostics.Debug.WriteLine(header);
            for (int m = 0; m < matrix.GetLength(0); m++)
            {
                bool[] row = new bool[matrix.GetLength(1)];
                for (int n = 0; n < matrix.GetLength(1); n++)
                {
                    row[n] = matrix[m, n];
                }

                System.Diagnostics.Debug.WriteLine(string.Join(" ", row.Select(x => x ? "x" : " ")));
            }
        }

        private static void Print(this int[,] matrix, string header)
        {
            System.Diagnostics.Debug.WriteLine(header);
            List<int[]> rows = new List<int[]>();
            for (int m = 0; m < matrix.GetLength(0); m++)
            {
                int[] row = new int[matrix.GetLength(1)];
                for (int n = 0; n < matrix.GetLength(1); n++)
                {
                    row[n] = matrix[m, n];
                }

                rows.Add(row);
            }

            int max = rows.SelectMany(x => x).Max();
            int len = max.ToString().Length;

            foreach (int[] row in rows)
            {
                string[] rowStrings = new string[row.Length];
                for (int i = 0; i < row.Length; i++)
                {
                    string intAsString = row[i].ToString();
                    while (intAsString.Length < len)
                    {
                        intAsString += " ";
                    }

                    rowStrings[i] = intAsString;
                }

                System.Diagnostics.Debug.WriteLine(string.Join(" ", rowStrings));
            }
        }

        private static void Print(this double[,] matrix, string header)
        {
            System.Diagnostics.Debug.WriteLine(header);
            List<double[]> rows = new List<double[]>();
            for (int m = 0; m < matrix.GetLength(0); m++)
            {
                double[] row = new double[matrix.GetLength(1)];
                for (int n = 0; n < matrix.GetLength(1); n++)
                {
                    row[n] = matrix[m, n];
                }

                rows.Add(row);
            }

            int len = 4;
            foreach (double[] row in rows)
            {
                string[] rowStrings = new string[row.Length];
                for (int i = 0; i < row.Length; i++)
                {
                    string doubleAsString = Math.Round(row[i], 2).ToString();
                    while (doubleAsString.Length < len)
                    {
                        doubleAsString += " ";
                    }

                    rowStrings[i] = doubleAsString;
                }

                System.Diagnostics.Debug.WriteLine(string.Join(" ", rowStrings));
            }
        }

        private static PointXY[] Corners(double startX, double startY, int m, int n, double horD, double vertD)
        {
            return new PointXY[]
            {
                new PointXY(startX + m * horD, startY + n * vertD),
                new PointXY(startX + (m + 1) * horD, startY + n * vertD),
                new PointXY(startX + (m + 1) * horD, startY + (n + 1) * vertD),
                new PointXY(startX + m * horD, startY + (n + 1) * vertD)
            };
        }

        private static bool IsAtLeastPartlyInside(this PointXY[] corners, List<Polyline> outlines, List<Polyline> exclusions)
        {
            //TODO: could take center only instead?
            return corners.Any(x => outlines.Any(y => x.IsInside(y))) && !exclusions.Any(x => corners.All(y => y.IsInside(x)));
        }

        private static bool Intersects(this PointXY pt, Vector dir, Line line2, double angleTolerance = Tolerance.Angle)
        {
            Vector v2 = line2.End - line2.Start;
            Vector v2N = v2.Normalise();

            if (1 - Math.Abs(dir.DotProduct(v2N)) <= angleTolerance)
                return false;

            Point p1 = pt.ToPoint();
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

        private static bool IsInside(this PointXY pt, Polyline outline, double tolerance = 1e-6)
        {
            //TODO: make sure right vector is picked - no intersections etc.
            Point asbhom = pt.ToPoint();
            double sqTol = tolerance * tolerance;
            if (outline.ControlPoints.Any(x => x.SquareDistance(asbhom) <= sqTol))
                return true;

            Vector dir = Vector.XAxis;
            while(IntersectsWithCorner(asbhom, dir, outline.ControlPoints, tolerance))
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

        private static bool IntersectsWithCorner(Point pt, Vector dir, List<Point> controlPoints, double tolerance)
        {
            return controlPoints.Any(x => 1 - (x - pt).Normalise().DotProduct(dir) <= tolerance);
        }

        private static List<Polyline> MyOffset(this Polyline curve, double offset, Vector normal = null, bool tangentExtensions = false, double distTol = Tolerance.Distance, double angleTol = Tolerance.Angle)
        {
            if (curve == null || curve.Length() < distTol)
                return null;

            if (offset == 0)
                return new List<Polyline> { curve };

            OffsetOptions options = new OffsetOptions();
            return Compute.MultiOffset(curve, new List<double> { offset }, normal, options, true, distTol, angleTol);
        }

        private static PointXY Center(this PointXY[] corners)
        {
            return new PointXY(corners.Average(x => x.X), corners.Average(x => x.Y));
        }
    }

    struct PointXY
    {
        public double X { get; set; }
        public double Y { get; set; }
    public PointXY(double x, double y)
    {
        X = x; Y = y;
    }
    public PointXY(Point point)
    {
        X = point.X; Y = point.Y;
    }

    public Point ToPoint()
        {
            return new Point { X = this.X, Y = this.Y };
        }
    }
}
