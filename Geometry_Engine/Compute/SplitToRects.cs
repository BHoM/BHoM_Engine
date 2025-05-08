//using BH.oM.Geometry;
//using System;
//using System.Collections.Generic;
//using System.Linq;

//namespace BH.Engine.Geometry
//{
//    public static partial class Compute
//    {
//        public static List<List<Polyline>> SplitToRects(this Polyline polygon, double minSize, double tol = Tolerance.Distance)
//        {
//            polygon = polygon.CleanPolyline(tol, tol);

//            List<double> xs = new List<double>();
//            List<double> ys = new List<double>();
//            for (int i = 0; i < polygon.ControlPoints.Count; i++)
//            {
//                Point p1 = polygon.ControlPoints[i];
//                Point p2 = polygon.ControlPoints[(i + 1) % polygon.ControlPoints.Count];
//                if (Math.Abs(p1.X - p2.X) <= tol)
//                {
//                    double meanX = (p1.X + p2.X) / 2;
//                    double y1 = Math.Min(p1.Y, p2.Y);
//                    double y2 = Math.Max(p1.Y, p2.Y);
//                    if (xs.All(x => Math.Abs(x - meanX) > tol))
//                        xs.Add(meanX);
//                }
//                else if (Math.Abs(p1.Y - p2.Y) <= tol)
//                {
//                    double meanY = (p1.Y + p2.Y) / 2;
//                    double x1 = Math.Min(p1.X, p2.X);
//                    double x2 = Math.Max(p1.X, p2.X);
//                    if (x2 - x1 + tol >= minSize && ys.All(x => Math.Abs(x - meanY) > tol))
//                        ys.Add(meanY);
//                }
//            }

//            return SubDivide(polygon, xs, ys, minSize, tol);
//        }

//        private static List<List<Polyline>> SubDivide(Polyline outline, List<double> xs, List<double> ys, double minSize, double tol)
//        {
//            xs.Sort();
//            ys.Sort();

//            //todo: can be a dictionary
//            Dictionary<BoundingBox, Polyline> toPermute = new Dictionary<BoundingBox, Polyline>();
//            Dictionary<BoundingBox, Polyline> bboxOutlines = new Dictionary<BoundingBox, Polyline>();
//            for (int i1 = 0; i1 < xs.Count - 1; i1++)
//            {
//                for (int i2 = xs.Count - 1; i2 > i1; i2--)
//                {
//                    double x1 = xs[i1];
//                    double x2 = xs[i2];
//                    double dimX = x2 - x1;
//                    if (dimX + tol < minSize)
//                        break;

//                    for (int j1 = 0; j1 < ys.Count - 1; j1++)
//                    {
//                        for (int j2 = ys.Count - 1; j2 > j1; j2--)
//                        {
//                            double y1 = ys[j1];
//                            double y2 = ys[j2];
//                            double dimY = y2 - y1;
//                            if (dimY + tol < minSize)
//                                break;

//                            Point p1 = new Point { X = x1, Y = y1 };
//                            Point p2 = new Point { X = x2, Y = y1 };
//                            Point p3 = new Point { X = x2, Y = y2 };
//                            Point p4 = new Point { X = x1, Y = y2 };

//                            BoundingBox bbox = new BoundingBox
//                            {
//                                Min = p1,
//                                Max = p3
//                            };

//                            if (toPermute.Keys.Any(x => x.IsContaining(bbox, true, tol)))
//                                continue;

//                            Polyline bboxAsPolyline = new Polyline { ControlPoints = new List<Point> { p1, p2, p3, p4, p1 } };

//                            List<Polyline> boolInts = outline.BooleanIntersection(bboxAsPolyline, tol);
//                            if (boolInts.Count != 1)
//                                continue;

//                            Polyline boolInt = boolInts[0];

//                            double area = boolInt.Area();
//                            double bboxArea = dimX * dimY;
//                            //TODO: make 0.8 a parameter
//                            if (area < bboxArea * 0.8)
//                                continue;

//                            double perim = boolInt.Length();
//                            double bboxPerim = (dimX + dimY) * 2;
//                            //TODO: make 0.8 a parameter
//                            if (perim < bboxPerim * 0.8)
//                                continue;

//                            toPermute.Add(bbox, boolInt);
//                            bboxOutlines.Add(bbox, bboxAsPolyline);
//                        }
//                    }
//                }
//            }

//            List<List<BoundingBox>> permutations = FindPermutationsOfBboxesThatDoNotOverlap(toPermute.Keys.ToList(), new List<BoundingBox>(), tol).ToList();
//            List<(List<Polyline>, List<BoundingBox>)> result = new List<(List<Polyline>, List<BoundingBox>)>();
//            ToleranceComparer tolComp = new ToleranceComparer(tol);
//            foreach (List<BoundingBox> permut in permutations)
//            {
//                List<Polyline> boolInts = outline.BooleanDifference(permut.Select(x => bboxOutlines[x]).ToList(), tol);
//                boolInts.AddRange(permut.Select(x => toPermute[x]));

//                List<BoundingBox> bboxes = permut.ToList();
//                bboxes.AddRange(boolInts.Select(x => x.Bounds()));
//                bboxes = bboxes.OrderBy(x => x.Min.X, tolComp).ThenBy(x => x.Min.Y, tolComp).ToList();

//                if (result.Any(x => x.Item2.Count == bboxes.Count && x.Item2.Zip(bboxes, (y, z) => y.IsSame(z, tol)).All(b => b)))
//                    continue;

//                result.Add((boolInts, bboxes));
//            }

//            return result.Select(x => x.Item1).ToList();
//        }

//        private static bool IsSame(this BoundingBox bbox1, BoundingBox bbox2, double tol)
//        {
//            double sqTol = tol * tol;
//            return bbox1.Min.SquareDistance(bbox2.Min) <= sqTol &&
//                   bbox1.Max.SquareDistance(bbox2.Max) <= sqTol;
//        }

//        private class ToleranceComparer : IComparer<double>
//        {
//            private readonly double _tolerance;
//            public ToleranceComparer(double tolerance)
//            {
//                _tolerance = tolerance;
//            }
//            public int Compare(double x, double y)
//            {
//                if (Math.Abs(x - y) <= _tolerance)
//                    return 0;
//                return x < y ? -1 : 1;
//            }
//        }


//        public static List<List<BoundingBox>> FindPermutationsOfBboxesThatDoNotOverlap(List<BoundingBox> toPermute, List<BoundingBox> current, double tol)
//        {
//            List<List<BoundingBox>> result = new List<List<BoundingBox>>();
//            for (int i = 0; i < toPermute.Count; i++)
//            {
//                BoundingBox bbox = toPermute[i];
//                if (current.Any(x => bbox.IsInRange2(x, tol)))
//                    continue;

//                List<BoundingBox> withNew = current.ToList();
//                withNew.Add(bbox);
//                result.Add(withNew);

//                List<BoundingBox> cl = toPermute.Skip(i + 1).ToList();
//                result.AddRange(FindPermutationsOfBboxesThatDoNotOverlap(cl, withNew, tol));
//            }

//            return result;
//        }

//        public static bool IsInRange2(this BoundingBox box1, BoundingBox box2, double tolerance = Tolerance.Distance)
//        {
//            return (box1.Min.X <= box2.Max.X - tolerance && box2.Min.X <= box1.Max.X - tolerance &&
//                     box1.Min.Y <= box2.Max.Y - tolerance && box2.Min.Y <= box1.Max.Y - tolerance);
//        }



//    }
//}
