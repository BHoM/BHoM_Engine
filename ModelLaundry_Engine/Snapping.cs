using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Geometry;
using BH.Engine.Geometry;
using System.Collections;

namespace BH.Engine.Geometry
{

    // GENERAL: SPLIT INTO SEPARATE .cs FILES

        // method for checking if the panel is planar? e.g. after point to point? add to query

    public static partial class Modify
    {
        /******************************************/
        /****  Vertical Snapping to Height     ****/
        /******************************************/

        public static Point VerticalSnapToHeight(this Point point, List<double> refHeights, double tolerance)
        {
            Point newPoint = new Point(point.X, point.Y, point.Z);

            foreach (double height in refHeights)
            {
                if (Math.Abs(newPoint.Z - height) < tolerance)
                {
                    newPoint.Z = height;
                    break;
                }
            }
            return newPoint;
        }

        /******************************************/

            // TODO: Is this one necessary?
        /*
        public static Line VerticalSnapToHeight(Line line, List<double> refHeights, double tolerance)
        {
            return new Line(VerticalSnapToHeight(line.Start, refHeights, tolerance), VerticalSnapToHeight(line.End, refHeights, tolerance));
        }

        /******************************************/

        public static ICurve VerticalSnapToHeight(this ICurve contour, List<double> refHeights, double tolerance)
        {
            List<Point> oldPoints = contour.IGetControlPoints();
            List<Point> newPoints = new List<Point>();

            foreach (Point pt in oldPoints)
            {
                newPoints.Add(VerticalSnapToHeight(pt, refHeights, tolerance));
            }
            return new Polyline(newPoints);
        }

        /******************************************/

        // TODO: Is this one necessary?

            /*
        public static List<ICurve> VerticalSnapToHeight(List<ICurve> group, List<double> refHeights, double tolerance)
        {
            List<ICurve> newGroup = new List<ICurve>();
            foreach (ICurve curve in group)
            {
                if (curve is Line)
                    newGroup.Add(VerticalSnapToHeight((Line)curve, refHeights, tolerance));
                else
                    newGroup.Add(VerticalSnapToHeight(curve, refHeights, tolerance));
            }
            return newGroup;
        }


        /******************************************/
        /****  Horizontal Snap to Shapes      ****/
        /******************************************/

            // Todo: this one (and all its derivatives) does not correct!
        /*
        public static Point HorizontalSnapToShape(Point point, List<ICurve> refContours, double tolerance, bool anyHeight = false)
        {
            foreach (ICurve refC in refContours)
            {
                Vector dir = refC.IGetClosestPoint(point) - point;
                if (anyHeight) dir.Z = 0;
                if (dir.GetLength() < tolerance)
                {
                    dir.Z = 0;
                    return point + dir;
                }  
            }

            return point;
        }

        /******************************************/
        /*
        public static Line HorizontalSnapToShape(Line line, List<ICurve> refContours, double tolerance, bool anyHeight = false)
        {
            return new Line(HorizontalSnapToShape(line.Start, refContours, tolerance, anyHeight), HorizontalSnapToShape(line.End, refContours, tolerance, anyHeight));
        }

        /******************************************/
        /*
        public static ICurve HorizontalSnapToShape(ICurve contour, List<ICurve> refContours, double tolerance, bool anyHeight = false)
        {
            // Get the refContours that are close enought to matter
            List<ICurve> nearContours = Util.GetNearContours(contour, refContours, tolerance, anyHeight);

            // Get all reference lines
            Plane ground = Plane.XY;
            List<Line> refLines = new List<Line>();
            foreach (ICurve refC in nearContours)
            {
                foreach (Line refL in refC.IGetExploded())
                {
                    if (Math.Abs(refL.GetDirection().Z) < 1e-3)
                    {
                        Transform.GetProjected(refL, ground); // This only works because refL is a new line. Project should create a new line, not modify the existing one
                        refLines.Add(refL);
                    }
                        
                }

            }

            // Create snapping proposition list per horizontal position
            List<Point> oldPoints = contour.IGetControlPoints();
            Dictionary<string, List<Snap>> snapDirections = new Dictionary<string, List<Snap>>();
            foreach (Point pt in oldPoints)
            {
                snapDirections[getPointCode(pt)] = new List<Snap>();
            }

            // Get the lines to vote for points
            foreach (Line cLine in contour.IGetExploded())
            {
                // Only work with horizontal lines
                if (Math.Abs(cLine.GetDirection().Z) > 1e-3) continue;
                Transform.GetProjected(cLine, ground); // This only works because refL is a new line. Project should create a new line, not modify the existing one
                // Get directions of the line
                Vector hDir = cLine.GetDirection();
                hDir.GetNormalised();
                Vector pDir = new Vector(-hDir.Y, hDir.X, 0);

                // Add snap propositions
                List<Snap> snaps = new List<Snap>();
                foreach (Line refL in refLines)
                {
                    if (cLine.GetDirection().IsParallel(refL.GetDirection(), 0.02)==1)
                        continue;
                    Point intersection = Query.GetIntersection(cLine, refL);
                    if (intersection != null)
                    {
                        Vector startDir = intersection - cLine.Start;
                        if (startDir.GetLength() < tolerance && startDir.GetLength() > 0)
                            snapDirections[getPointCode(cLine.Start)].Add(new Snap(startDir, refL));

                        Vector endDir = intersection - cLine.End;
                        if (endDir.GetLength() < tolerance && endDir.GetLength() > 0)
                            snapDirections[getPointCode(cLine.End)].Add(new Snap(endDir, refL));
                    }
                } 
            }

            // Move points according to snapping propositions
            List<Point> newPoints = new List<Point>();
            foreach (Point pt in oldPoints)
            {
                List<Snap> snaps = snapDirections[getPointCode(pt)];
                if (snaps.Count > 0)
                {
                    Vector finalDir = snaps[0].dir.GetClone() as Vector;
                    Vector refDir = snaps[0].dir.GetClone() as Vector;
                    refDir.GetNormalised();

                    for (int i = 1; i < snaps.Count; i++)
                    {
                        double sin = Math.Sin(Query.GetAngle(refDir, snaps[i].dir));
                        if (!Double.IsNaN(sin))
                            finalDir += snaps[i].dir * sin;
                    }
                    newPoints.Add(pt + finalDir);
                }
                else
                    newPoints.Add(pt);
            }

            return new Polyline(newPoints);
        }

        /******************************************/
        /*
        public static List<ICurve> HorizontalSnapToShape(List<ICurve> group, List<ICurve> refContours, double tolerance, bool anyHeight = false)
        {
            List<ICurve> newGroup = new List<ICurve>();
            foreach (ICurve curve in group)
            {
                if (curve is Line)
                    newGroup.Add(HorizontalSnapToShape((Line)curve, refContours, tolerance, anyHeight));
                else
                    newGroup.Add(HorizontalSnapToShape(curve, refContours, tolerance, anyHeight));
            }
            return newGroup;
        }


        /******************************************/
        /****  Horizontal Parrallel Snapping   ****/
        /******************************************/

            // Todo: SnapFloorToGrid better?

            /*
        public static object HorizontalParallelSnap(object element, List<object> refElements, double tolerance, bool anyHeight = false, double angleTol = 0.035)
        {
            // Get the geometry of the element
            IBHoMGeometry geometry = Util.GetGeometry(element);
            BoundingBox ROI = geometry.IGetBounds().GetInflated(tolerance);
            if (anyHeight) ROI.GetExtents().Z = 1e12;
            List<ICurve> refGeom = Util.GetGeometries(refElements, ROI);

            // Do the actal snapping
            IBHoMGeometry output = null;
            if (geometry is ICurve)
            {
                output = Snapping.HorizontalParallelSnap((ICurve)geometry, refGeom, tolerance, anyHeight, angleTol);
            }
            if (geometry is List<ICurve>)
            {
                foreach (ICurve curve in geometry as List<ICurve>)
                {
                    output = Snapping.HorizontalParallelSnap(curve, refGeom, tolerance, anyHeight, angleTol);
                }
            }

            // Return the final result
            return Util.SetGeometry(element, output);
        }

        /******************************************/

            /*
        public static ICurve HorizontalParallelSnap(ICurve contour, List<ICurve> refContours, double tolerance, bool anyHeight = false, double angleTol = 0.035)
        {
            // Get the refContours that are close enought to matter
            List<ICurve> nearContours = Util.GetNearContours(contour, refContours, tolerance, anyHeight);

            // Create snapping proposition list per horizontal position
            List<Point> oldPoints = contour.IGetControlPoints();
            Dictionary<string, List<Snap>> snapDirections = new Dictionary<string, List<Snap>>();
            foreach (Point pt in oldPoints)
            {
                snapDirections[getPointCode(pt)] = new List<Snap>();
            }

            // Get all reference lines
            Plane ground = new Plane(new Point(0, 0, 0), new Vector(0, 0, 1)); //TODO: Update to XY plane once one a constructor is added to BH.Engine.Geometry
            List<Line> refLines = new List<Line>();
            foreach (ICurve refC in nearContours)
            {
                foreach (Line refL in refC.IGetExploded())
                {
                    if (Math.Abs(refL.GetDirection().Z) < 1e-3)
                    {
                        Transform.GetProjected(refL, ground);
                        refLines.Add(refL);
                    }
                }
                    
            }

            // Get the lines to vote for points
            foreach (Line cLine in contour.IGetExploded())
            {
                // Only work with horizontal lines
                if (Math.Abs(cLine.GetDirection().Z) > 1e-3) continue;
                Transform.GetProjected(cLine, ground); // This only works because refL is a new line. Project should create a new line, not modify the existing one
                // Get directions of the line
                Vector hDir = cLine.GetDirection();
                hDir.GetNormalised();
                Vector pDir = new Vector(-hDir.Y, hDir.X, 0);

                // Add snap propositions
                List<Snap> snaps = new List<Snap>();
                foreach (Line refL in refLines)
                {
                    if (cLine.GetDirection().IsParallel(refL.GetDirection(), angleTol) == 1 && cLine.GetDistance(refL) < tolerance)
                    {
                        Vector startDir = (pDir * (refL.Start - cLine.Start)) * pDir;
                        Vector endDir = (pDir * (refL.End - cLine.End)) * pDir;
                        Vector dir = (startDir.GetLength() < endDir.GetLength()) ? startDir : endDir;
                        if (dir.GetLength() < tolerance)
                            snaps.Add(new Snap(dir, refL));
                    }
                }

                // Pick the shortest snap if any and add to points' list
                if (snaps.Count > 0)
                {
                    Snap bestSnap = snaps[0];
                    for (int i = 1; i < snaps.Count; i++)
                    {
                        if (snaps[i].dir.GetLength() < bestSnap.dir.GetLength())
                            bestSnap = snaps[i];
                    }
                        
                    Snap startSnap = new Snap(Transform.GetProjected(cLine.Start, bestSnap.target) - cLine.Start, bestSnap.target);
                    Snap endSnap = new Snap(Transform.GetProjected(cLine.End, bestSnap.target) - cLine.End, bestSnap.target);

                    snapDirections[getPointCode(cLine.Start)].Add(startSnap);
                    snapDirections[getPointCode(cLine.End)].Add(endSnap);
                }
            }

            // Move points according to snapping propositions
            List<Point> newPoints = new List<Point>();
            foreach (Point pt in oldPoints)
            {
                List<Snap> snaps = snapDirections[getPointCode(pt)];

                if (snaps.Count == 0)
                {
                    newPoints.Add(pt);
                }
                else
                {
                    List<Snap> cleanSnaps = new List<Snap>();
                    cleanSnaps.Add(snaps[0]);
                    for (int i = 1; i < snaps.Count; i++)
                    {
                        bool match = false;
                        for (int j = 0; j < cleanSnaps.Count; j++)
                        {
                            if (snaps[i].target.GetDirection().IsParallel(cleanSnaps[j].target.GetDirection())==1)
                            {
                                if ((cleanSnaps[j].dir.GetLength() < snaps[i].dir.GetLength()))
                                    cleanSnaps[j] = snaps[i];
                                match = true;
                                break;
                            }
                        }
                        if (!match)
                            cleanSnaps.Add(snaps[i]);
                    }

                    if (cleanSnaps.Count == 1)
                    {
                        newPoints.Add(pt + (cleanSnaps[0].dir.GetClone() as Vector));
                    }
                    else
                    {
                        cleanSnaps = cleanSnaps.OrderBy(x => x.dir.GetLength()).ToList();
                        Point target = Query.GetIntersection(snaps[0].target, snaps[1].target);
                        target.Z = pt.Z;
                        newPoints.Add(target);
                    }
                }

                
            }

            return new Polyline(newPoints);
        }

        /******************************************/

            /*
        public static List<ICurve> HorizontalParallelSnap(List<ICurve> group, List<ICurve> refContours, double tolerance, bool anyHeight = false, double angleTol = 0.035)
        {
            List<ICurve> newGroup = new List<ICurve>();
            foreach (ICurve curve in group)
            {
                if (curve is Line)
                    newGroup.Add(HorizontalParallelSnap((Line)curve, refContours, tolerance, anyHeight, angleTol));
                else
                    newGroup.Add(HorizontalParallelSnap(curve, refContours, tolerance, anyHeight, angleTol));
            }
            return newGroup;
        }


        /******************************************/
        /****  Snapping to reference points    ****/
        /******************************************/

        public static Point PointToPointSnap(this Point point, List<Point> refPoints, double tolerance)
        {
            
            foreach(Point refPt in refPoints)
            {
                if (refPt.GetDistance(point) < tolerance)
                    return (Point)refPt.GetClone();
            }

            return (Point)point.GetClone();
        }

        /******************************************/

            // Todo: is it needed?
            /*
        public static Line PointToPointSnap(Line line, List<Point> refPoints, double tolerance)
        {
            return new Line(PointToPointSnap(line.Start, refPoints, tolerance), PointToPointSnap(line.End, refPoints, tolerance));
        }

        /******************************************/

        public static Polyline PointToPointSnap(this ICurve curve, List<Point> refPoints, double tolerance)
        {
            List<Point> points = new List<Point>();
            foreach(Point pt in curve.IGetControlPoints())
            {
                points.Add(PointToPointSnap(pt, refPoints, tolerance));
            }
            return new Polyline(points);
        }

        /******************************************/

        // Todo: is it needed?
        /*

        public static List<ICurve> PointToPointSnap(List<ICurve> group, List<Point> refPoints, double tolerance)
        {
            List<ICurve> newGroup = new List<ICurve>();
            foreach (ICurve curve in group)
            {
                if (curve is Line)
                    newGroup.Add(PointToPointSnap((Line)curve, refPoints, tolerance));
                else
                    newGroup.Add(PointToPointSnap(curve, refPoints, tolerance));
            }
            return newGroup;
        }


        /******************************************/
        /****     Floor snapping to grids      ****/
        /******************************************/

            // Todo: check if panel is horizontal, make sure it works with inclined/vertical panels

        public static Polyline SnapFloorContourToGrids(this Polyline contour, List<Line> grids, double tolerance, double angleTol)
        {
            double dottol = Math.Cos(angleTol);
            List<Line> edges = contour.GetExploded();
            Line[] refgrids = new Line[edges.Count];
            for (int j = 0; j < edges.Count; j++)
            {
                double mindist = tolerance;
                Vector v = edges[j].GetDirection();
                foreach (Line grid in grids)
                {
                    Vector gv = grid.GetDirection();
                    if (Math.Abs(v.X * gv.X + v.Y * gv.Y) >= dottol)
                    {
                        Point startPt = new Point(edges[j].Start.X, edges[j].Start.Y, 0);
                        Point endPt = new Point(edges[j].End.X, edges[j].End.Y, 0);
                        double sdist = grid.GetClosestPoint(startPt).GetDistance(startPt);
                        double edist = grid.GetClosestPoint(endPt).GetDistance(endPt);
                        if (sdist <= tolerance && edist <= tolerance)
                        {
                            double dist = (sdist + edist) * 0.5;
                            if (dist <= mindist)
                            {
                                mindist = dist;
                                refgrids[j] = grid;
                            }
                        }
                    }
                }
            }
            for (int j = 0; j < refgrids.Length; j++)
            {
                if (refgrids[j] != null)
                {
                    Line grid = refgrids[j];
                    edges = SnapFloorEdgeToGrid(edges, j, grid, dottol);
                }
            }
            
            return Create.Polyline(edges);
        }

        /******************************************/

        private static List<Line> SnapFloorEdgeToGrid(List<Line> edges, int id, Line grid, double dottol)
        {
            int lc = edges.Count;
            Point[] verts = new Point[4];
            verts[0] = edges[(id + lc - 1) % lc].Start;
            verts[1] = IntersectLineGrid(edges[(id + lc - 1) % lc].End, edges[(id + lc - 1) % lc].GetDirection(), grid.Start, grid.GetDirection(), dottol);
            verts[2] = IntersectLineGrid(edges[(id + 1) % lc].Start, edges[(id + 1) % lc].GetDirection(), grid.Start, grid.GetDirection(), dottol);
            verts[3] = edges[(id + 1) % lc].End;

            for (int i = -1; i < 2; i++)
            {
                edges[(id + i + lc) % lc] = new Line(verts[i + 1], verts[i + 2]);
            }
            return edges;
        }


        /******************************************/
        /****   Wall plane snapping to grids   ****/
        /******************************************/

        // Todo: check if the wall is vertical and handle inclined ones?

        public static Polyline SnapWallPlaneToGrids(this Polyline contour, List<Line> grids, double tolerance, double angleTol)
        {
            double dottol = Math.Cos(angleTol);
            List<Point> verts = contour.ControlPoints;
            Line wline = GetWallLine(verts, tolerance * 0.001);
            if (wline != null)
            {
                Vector v = wline.GetDirection();
                Point startPt = wline.Start;
                Point endPt = wline.End;

                Line refgrid = null;
                double mindist = tolerance;
                foreach (Line grid in grids)
                {
                    Vector gv = grid.GetDirection();
                    if (Math.Abs(v.X * gv.X + v.Y * gv.Y) >= dottol)
                    {
                        double sdist = grid.GetClosestPoint(startPt).GetDistance(startPt);
                        double edist = grid.GetClosestPoint(endPt).GetDistance(endPt);
                        if (sdist <= tolerance && edist <= tolerance)
                        {
                            double dist = (sdist + edist) * 0.5;
                            if (dist <= mindist)
                            {
                                mindist = dist;
                                refgrid = grid;
                            }
                        }
                    }
                }

                if (refgrid != null)
                {
                    Vector gv = refgrid.GetDirection();
                    for (int i = 0; i < verts.Count; i++)
                    {
                        verts[i] = IntersectLineGrid(verts[i], v, refgrid.Start, gv, dottol);
                    }
                    return new Polyline(verts);
                }
                return contour;
            }
            return contour;
        }


        /******************************************/
        /****    Wall end snapping to grids    ****/
        /******************************************/

            // Todo: check if the wall is vertical and handle inclined ones?

        public static Polyline SnapWallEndToGrids(this Polyline contour, List<Line> grids, double tolerance, double angleTol)
        {
            double dottol = Math.Cos(angleTol);
            List<Point> verts = contour.ControlPoints;
            Line wline = GetWallLine(verts, tolerance * 0.001);
            if (wline != null)
            {
                Vector v = wline.GetDirection();
                Point startPt = wline.Start;
                Point endPt = wline.End;

                for (int i = 0; i < verts.Count; i++)
                {
                    Line refgrid = null;
                    double mindist = tolerance;
                    Point fvert = new Point(verts[i].X, verts[i].Y, 0);
                    foreach (Line grid in grids)
                    {
                        Vector gv = grid.GetDirection();
                        if (Math.Abs(v.X * gv.X + v.Y * gv.Y) < dottol)
                        {
                            double dist = grid.GetClosestPoint(fvert).GetDistance(fvert);
                            if (dist <= mindist)
                            {
                                mindist = dist;
                                refgrid = grid;
                            }
                        }
                    }
                    if (refgrid != null)
                    {
                        Vector gv = refgrid.GetDirection();
                        verts[i] = IntersectLineGrid(verts[i], v, refgrid.Start, gv, dottol);
                    }
                }
                return new Polyline(verts);
            }
            return contour;
        }


        /******************************************/
        /****    Beam end snapping to grids    ****/
        /******************************************/

            // Todo: make sure it works for inclined elements

        public static Line SnapBeamEndToGrids(this Line beam, List<Line> grids, double tolerance, double angleTol)
        {
            if (beam.GetLength() > 0)
            {
                double dottol = Math.Cos(angleTol);
                Point[] ends = new Point[] { beam.Start, beam.End };
                Vector v = new Vector(ends[1].X - ends[0].X, ends[1].Y - ends[0].Y, 0);
                v = v.GetNormalised();

                for (int i = 0; i < ends.Length; i++)
                {
                    Line refgrid = null;
                    double mindist = tolerance;
                    Point fvert = new Point(ends[i].X, ends[i].Y, 0);
                    foreach (Line grid in grids)
                    {
                        Vector gv = grid.GetDirection();
                        if (Math.Abs(v.X * gv.X + v.Y * gv.Y) < dottol)
                        {
                            double dist = grid.GetClosestPoint(fvert).GetDistance(fvert);
                            if (dist <= mindist)
                            {
                                mindist = dist;
                                refgrid = grid;
                            }
                        }
                    }
                    if (refgrid != null)
                    {
                        Vector gv = refgrid.GetDirection();
                        ends[i] = IntersectLineGrid(ends[i], v, refgrid.Start, gv, dottol);
                    }
                }
                return new Line(ends[0], ends[1]);
            }
            return beam;
        }


        /******************************************/
        /****   Beam plane snapping to grids   ****/
        /******************************************/

        public static Line SnapBeamPlaneToGrids(this Line beam, List<Line> grids, double tolerance, double angleTol)
        {
            if (beam.GetLength() > 0)
            {
                double dottol = Math.Cos(angleTol);
                Point[] ends = new Point[] { beam.Start, beam.End };
                Point fSPt = new Point(ends[0].X, ends[0].Y, 0);
                Point fEPt = new Point(ends[1].X, ends[1].Y, 0);
                Vector v = fEPt - fSPt;
                v = v.GetNormalised();

                Line refgrid = null;
                double mindist = tolerance;
                foreach (Line grid in grids)
                {
                    Vector gv = grid.GetDirection();
                    if (Math.Abs(v.X * gv.X + v.Y * gv.Y) >= dottol)
                    {
                        double sdist = grid.GetClosestPoint(fSPt).GetDistance(fSPt);
                        double edist = grid.GetClosestPoint(fEPt).GetDistance(fEPt);
                        if (sdist <= tolerance && edist <= tolerance)
                        {
                            double dist = (sdist + edist) * 0.5;
                            if (dist <= mindist)
                            {
                                mindist = dist;
                                refgrid = grid;
                            }
                        }
                    }
                }
                if (refgrid != null)
                {
                    Vector gv = refgrid.GetDirection();
                    for (int i = 0; i < ends.Length; i++)
                    {
                        ends[i] = IntersectLineGrid(ends[i], v, refgrid.Start, gv, dottol);
                    }
                }
                return new Line(ends[0], ends[1]);
            }
            return beam;
        }


        /******************************************/
        /****     Column snapping to grids     ****/
        /******************************************/

        public static Line SnapColumnToGrids(this Line col, List<Line> grids, double tolerance)
        {
            Point[] ends = new Point[] { col.Start, col.End };

            for (int i = 0; i < ends.Length; i++)
            {
                Point fpt = new Point(ends[i].X, ends[i].Y, 0);

                Line refgrid = null;
                double mindist = tolerance;
                foreach (Line grid in grids)
                {
                    double dist = grid.GetClosestPoint(fpt).GetDistance(fpt);
                    if (dist <= mindist)
                    {
                        mindist = dist;
                        refgrid = grid;
                    }
                }
                if (refgrid != null)
                {
                    Point rpt = refgrid.GetClosestPoint(fpt);
                    ends[i] = new Point(rpt.X, rpt.Y, ends[i].Z);
                }
            }
            return new Line(ends[0], ends[1]);
        }


        /******************************************/
        /*** Bar snapping to grid intersections ***/
        /******************************************/

        public static Line SnapBarToGridIntersections(this Line bar, List<Point> intersections, double tolerance)
        {
            Point[] ends = new Point[] { bar.Start, bar.End };

            for (int i = 0; i < ends.Length; i++)
            {
                Point pt = new Point(ends[i].X, ends[i].Y, 0);
                Point refpt = null;
                double mindist = tolerance;

                foreach (Point ipt in intersections)
                {
                    double dist = ipt.GetDistance(pt);
                    if (dist <= mindist)
                    {
                        mindist = dist;
                        refpt = ipt;
                    }
                }
                if (refpt != null)
                {
                    ends[i] = new Point(refpt.X, refpt.Y, ends[i].Z);
                }
            }
            return new Line(ends[0], ends[1]);
        }


        /******************************************/
        /***      Remove floor protrusions      ***/
        /******************************************/

        // TODO: Does not work because of the GetIntersection method issue.

        public static Polyline RemoveFloorProtrusions(this Polyline contour, double minArea)
        {
            Polyline ccontour = contour.RemoveZeroSegments(0.001);
            ccontour = ccontour.MergeColinearSegments(0.001, false);
            Polyline output = null;
            List<Polyline> cntrs = new List<Polyline>();
            bool running = true;

            while (running)
            {
                List<Line> crvs = ccontour.GetExploded();
                int lc = crvs.Count;
                List<SIVertice> mvertices = new List<SIVertice> { new SIVertice(crvs[0].Start, false) };
                for (int i = 0; i < crvs.Count; i++)
                {
                    List<SIVertice> ints = new List<SIVertice>();
                    for (int j = 0; j < crvs.Count; j++)
                    {
                        if (Math.Abs(i - j) % (lc - 1) > 1)
                        {
                            Point ipt = crvs[i].GetIntersection(crvs[j]);
                            if (ipt != null)
                            {
                                ints.Add(new SIVertice(ipt, true));
                            }
                        }
                    }
                    ints.Sort(delegate (SIVertice v1, SIVertice v2)
                    {
                        return crvs[i].Start.GetDistance(v1.Location).CompareTo(crvs[i].Start.GetDistance(v2.Location));
                    });
                    mvertices.AddRange(ints);
                    mvertices.Add(new SIVertice(crvs[i].End, false));
                }
                if (mvertices.Count == lc + 1)
                {
                    return ccontour;
                }

                List<List<Point>> plns = new List<List<Point>>();
                int vc = mvertices.Count;
                List<Point> pln = new List<Point>();
                foreach (SIVertice mvert in mvertices)
                {
                    pln.Add(mvert.Location);
                    if (mvert.SelfIntersection && pln.Count != 1)
                    {
                        plns.Add(pln);
                        pln = new List<Point> { mvert.Location };
                    }
                }

                if (pln.Count > 1)
                {
                    plns.Add(pln);
                }

                List<Point> remaining = new List<Point>();
                foreach (List<Point> npln in plns)
                {
                    if (npln[0].GetDistance(npln[npln.Count - 1]) < 0.000001)
                    {
                        cntrs.Add(new Polyline(npln));
                    }
                    else
                    {
                        remaining.AddRange(npln);
                    }
                }
                if (remaining.Count == 0)
                {
                    running = false;
                }
                else
                {
                    ccontour = new Polyline(remaining);
                    ccontour = ccontour.RemoveZeroSegments(0.001);
                }
            }

            double maxArea = minArea;
            foreach (Polyline cntr in cntrs)
            {
                double area = cntr.GetFloorSignedArea();
                if (area >= maxArea)
                {
                    maxArea = area;
                    output = cntr;
                }
            }

            return output;
        }


        /******************************************/
        /***            Scale element           ***/
        /******************************************/

            /*
        public static object ScaleElement(object element, object refElement, double factorX, double factorY, double factorZ)
        {
            // Get the geometry of the element
            IBHoMGeometry geometry = Util.GetGeometry(element);
            IBHoMGeometry refGeometry = Util.GetGeometry(refElement);
            Vector factorV = new Vector(factorX, factorY, factorZ);

            // Do the actal snapping
            IBHoMGeometry output = null;
            if (refGeometry is Point)
            {
                output = geometry.IGetScaled((Point)refGeometry, factorV);
            }

            // Return the final result
            return Util.SetGeometry(element, output);
        }

        /******************************************/
        /****  Utility classes and functions   ****/
        /******************************************/

            /*
        private class Snap
        {
            public Vector dir;
            public Line target;

            public Snap()
            {
                dir = new Vector(0, 0, 0);
                target = new Line(new Point(), new Point());
            }

            public Snap(Vector d, Line t)
            {
                dir = d;
                target = t;
            }
        }

        /******************************************/

            /*
        private static string getPointCode(Point pt)
        {
            return Math.Round(pt.X, 3).ToString() + ';' + Math.Round(pt.Y, 3).ToString();
        }

        /******************************************/
        
        private static Point IntersectLineGrid(Point p1, Vector v1, Point p2, Vector v2, double dottol)
        {
            v1 = v1.GetNormalised();
            v2 = v2.GetNormalised();
            double a1, b1, a2, b2, x, y;
            a1 = a2 = b1 = b2 = x = y = double.NaN;
            if (Math.Abs(v1.X * v2.X + v1.Y * v2.Y) > dottol)
            {
                v1 = new Vector(-v2.Y, v2.X, 0);
            }
            if (Math.Abs(v1.X) < 0.000001)
            {
                x = p1.X;
                a2 = v2.Y / v2.X;
                b2 = p2.Y - p2.X * a2;
                y = a2 * x + b2;
                return new Point(x, y, p1.Z);
            }
            else if (Math.Abs(v2.X) < 0.000001)
            {
                x = p2.X;
                a1 = v1.Y / v1.X;
                b1 = p1.Y - p1.X * a1;
                y = a1 * x + b1;
                return new Point(x, y, p1.Z);
            }
            else
            {
                a1 = v1.Y / v1.X;
                b1 = p1.Y - p1.X * a1;
                a2 = v2.Y / v2.X;
                b2 = p2.Y - p2.X * a2;
                x = (b1 - b2) / (a2 - a1);
                y = b1 + a1 * x;
                return new Point(x, y, p1.Z);
            }
        }

        /******************************************/

        private static Line GetWallLine(List<Point> pts, double tol)
        {
            Point pS, pE;
            List<double> Xs = new List<double>();
            List<double> Ys = new List<double>();
            foreach (Point pt in pts)
            {
                Xs.Add(pt.X);
                Ys.Add(pt.Y);
            }
            if (Math.Abs(Xs.Max() - Xs.Min()) >= tol)
            {
                pS = pts[Xs.IndexOf(Xs.Min())];
                pE = pts[Xs.IndexOf(Xs.Max())];
            }
            else if (Math.Abs(Ys.Max() - Ys.Min()) >= tol)
            {
                pS = pts[Ys.IndexOf(Ys.Min())];
                pE = pts[Ys.IndexOf(Ys.Max())];
            }
            else
            {
                return null;
            }
            pS = new Point(pS.X, pS.Y, 0);
            pE = new Point(pE.X, pE.Y, 0);
            return new Line(pS, pE);
        }

        /******************************************/

        private static double GetFloorSignedArea(this Polyline contour)
        {
            double area = 0;
            List<Line> edges = contour.GetExploded();
            List<Point> vertices = new List<Point> { edges[0].Start };
            for (int i = 0; i < edges.Count; i++)
            {
                Line edge = edges[i];
                vertices.Add(edge.End);
                area += Math.Abs(vertices[i].X * vertices[i + 1].Y - vertices[i + 1].X * vertices[i].Y) * 0.5;
            }
            return area;
        }

        /******************************************/

        private class SIVertice
        {
            public Point Location;
            public bool SelfIntersection;
            public SIVertice(Point location, bool SI)
            {
                Location = location;
                SelfIntersection = SI;
            }
        }

        /******************************************/

        // TODO: This does not work because of malfunctioning GetIntersection method...
        public static List<Point> GetGridIntersections(List<Line> grids)
        {
            List<Point> intersections = new List<Point>();
            for (int i = 0; i < grids.Count - 1; i++)
            {
                for (int j = i + 1; j < grids.Count; j++)
                {
                    Point ipt = grids[i].GetIntersection(grids[j]);
                    if (ipt != null)
                    {
                        intersections.Add(ipt);
                    }
                }
            }
            return intersections;
        }


        /*************************************/
        /****  Get Near Contours          ****/
        /*************************************/

            // Todo: check if works and move to the right place

        public static List<ICurve> GetNearContours(ICurve refContour, List<ICurve> contours, double tolerance, bool anyHeight = false)
        {
            BoundingBox bounds = refContour.IGetBounds();
            BoundingBox ROI = bounds.GetInflated(tolerance);
            if (anyHeight) ROI.GetExtents().Z = 1e12;

            List<ICurve> nearContours = new List<ICurve>();
            foreach (ICurve refC in contours)
            {
                BoundingBox cBox = refC.IGetBounds();
                if (cBox.GetCentre().GetDistance(bounds.GetCentre()) > 1e-5 && cBox.IsInRange(ROI))
                    nearContours.Add(refC);
            }

            return nearContours;
        }

        /*************************************/
        /****  Filter By Bounding Box     ****/
        /*************************************/

        // Todo: check if works and move to the right place

        public static List<IBHoMGeometry> FilterByBoundingBox(List<IBHoMGeometry> elements, List<BoundingBox> boxes, out List<IBHoMGeometry> outsiders)
        {
            List<IBHoMGeometry> insiders = new List<IBHoMGeometry>();
            outsiders = new List<IBHoMGeometry>();
            foreach (IBHoMGeometry element in elements)
            {
                if (IsInside(element, boxes))
                    insiders.Add(element);
                else
                    outsiders.Add(element);
            }

            return insiders;
        }

        /*************************************/

        public static bool IsInside(IBHoMGeometry geometry, List<BoundingBox> boxes)
        {
            bool inside = false;
            BoundingBox eBox = geometry.IGetBounds();
            if (eBox != null)
            {
                foreach (BoundingBox box in boxes)
                {
                    if (box.IsContaining(eBox))
                    {
                        inside = true;
                        break;
                    }
                }
            }
            return inside;
        }

        /*************************************/
        /****    Check snapped points     ****/
        /*************************************/

        // Todo: is it still relevant? How to improve it? 

            /*
        public static List<Point> CheckSnappedPoints(List<object> elements, double tolerance, double minDist = 1e-12) //TODO: do we need to re-add #min dist' in bh.om.base?
        {
            PointMatrix matrix = new PointMatrix(tolerance);

            // Get the control points in the matrix
            List<Point> refPoints = new List<Point>();
            List<ICurve> refGeom = Util.GetGeometries(elements);
            foreach (ICurve curve in refGeom)
            {
                foreach (Point pt in curve.IGetControlPoints())
                    matrix.AddPoint(pt);
            }

            // Get all the errors
            HashSet<Point> errors = new HashSet<Point>();
            foreach (Tuple<PointMatrix.CompositeValue, PointMatrix.CompositeValue> tuple in matrix.GetRelatedPairs(minDist, tolerance))
            {
                errors.Add(tuple.Item1.Point);
            }

            return errors.ToList();
        }
        */
    }
}
