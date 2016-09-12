using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BHoM.Geometry;

namespace ModelLaundry_Engine
{
    public static class Snapping
    {
        /******************************************/
        /****  Vertical Snapping to Height     ****/
        /******************************************/

        public static object VerticalSnapToHeight(object element, List<double> refHeights, double tolerance)
        {
            // Get the geometry of the element
            GeometryBase geometry = Util.GetGeometry(element);

            // Do the actal snapping
            GeometryBase output = null;
            if (geometry is Point)
            {
                output = Snapping.VerticalSnapToHeight((Point)geometry, refHeights, tolerance);
            }
            else if (geometry is Line)
            {
                output = Snapping.VerticalSnapToHeight((Line)geometry, refHeights, tolerance);
            }
            else if (geometry is Curve)
            {
                output = Snapping.VerticalSnapToHeight((Curve)geometry, refHeights, tolerance);
            }
            else if (geometry is Group<Curve>)
            {
                output = Snapping.VerticalSnapToHeight((Group<Curve>)geometry, refHeights, tolerance);
            }

            // Return the final result
            return Util.SetGeometry(element, output);
        }

        /******************************************/

        public static Point VerticalSnapToHeight(Point point, List<double> refHeights, double tolerance)
        {
            Point newPoint = new Point(point);

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

        public static Line VerticalSnapToHeight(Line line, List<double> refHeights, double tolerance)
        {
            return new Line(VerticalSnapToHeight(line.StartPoint, refHeights, tolerance), VerticalSnapToHeight(line.EndPoint, refHeights, tolerance));
        }

        /******************************************/

        public static Curve VerticalSnapToHeight(Curve contour, List<double> refHeights, double tolerance)
        {
            List<Point> oldPoints = contour.ControlPoints;
            List<Point> newPoints = new List<Point>();

            foreach (Point pt in oldPoints)
            {
                newPoints.Add(VerticalSnapToHeight(pt, refHeights, tolerance));
            }
            return new Polyline(newPoints);
        }

        /******************************************/

        public static Group<Curve> VerticalSnapToHeight(Group<Curve> group, List<double> refHeights, double tolerance)
        {
            Group<Curve> newGroup = new Group<Curve>();
            foreach (Curve curve in group)
            {
                if (curve is Line)
                    newGroup.Add(VerticalSnapToHeight((Line)curve, refHeights, tolerance));
                else
                    newGroup.Add(VerticalSnapToHeight(curve, refHeights, tolerance));
            }
            return newGroup;
        }


        /******************************************/
        /****  Vertical Snapping to Curves     ****/
        /******************************************/

        public static object VerticalSnapToShape(object element, List<object> refElements, double tolerance)
        {
            // Get the geometry of the elements
            GeometryBase geometry = Util.GetGeometry(element);
            BoundingBox ROI = geometry.Bounds().Inflate(tolerance);
            List<Curve> refGeom = Util.GetGeometries(refElements, ROI);

            // Do the actal snapping
            GeometryBase output = null;
            if (geometry is Point)
            {
                output = Snapping.VerticalSnapToShape((Point)geometry, refGeom, tolerance);
            }
            else if (geometry is Line)
            {
                output = Snapping.VerticalSnapToShape((Line)geometry, refGeom, tolerance);
            }
            else if (geometry is Curve)
            {
                output = Snapping.VerticalSnapToShape((Curve)geometry, refGeom, tolerance);
            }
            else if (geometry is Group<Curve>)
            {
                output = Snapping.VerticalSnapToShape((Group<Curve>)geometry, refGeom, tolerance);
            }

            // Return the final result
            return Util.SetGeometry(element, output);
        }

        /******************************************/

        public static Point VerticalSnapToShape(Point point, List<Curve> refContours, double tolerance)
        {
            Point newPoint = new Point(point);

            foreach (Curve contour in refContours)
            {
                double height = contour.Bounds().Centre.Z;
                if (Math.Abs(newPoint.Z - height) < tolerance) // Need to add && contour.IsInside(newPoint)
                {
                    newPoint.Z = height;
                    break;
                }
            }
            return newPoint;
        }

        /******************************************/

        public static Line VerticalSnapToShape(Line line, List<Curve> refContours, double tolerance)
        {
            return new Line(VerticalSnapToShape(line.StartPoint, refContours, tolerance), VerticalSnapToShape(line.EndPoint, refContours, tolerance));
        }

        /******************************************/

        public static Curve VerticalSnapToShape(Curve contour, List<Curve> refContours, double tolerance)
        {
            List<Point> oldPoints = contour.ControlPoints;
            List<Point> newPoints = new List<Point>();

            foreach (Point pt in oldPoints)
            {
                newPoints.Add(VerticalSnapToShape(pt, refContours, tolerance));
            }
            return new Polyline(newPoints);
        }

        /******************************************/

        public static Group<Curve> VerticalSnapToShape(Group<Curve> group, List<Curve> refContours, double tolerance)
        {
            Group<Curve> newGroup = new Group<Curve>();
            foreach (Curve curve in group)
            {
                if (curve is Line)
                    newGroup.Add(VerticalSnapToShape((Line)curve, refContours, tolerance));
                else
                    newGroup.Add(VerticalSnapToShape(curve, refContours, tolerance));
            }
            return newGroup;
        }


        /******************************************/
        /****  Horizontal Spnap to Shapes      ****/
        /******************************************/

        public static object HorizontalSnapToShape(object element, List<object> refElements, double tolerance, bool anyHeight = false)
        {
            // Get the geometry of the element
            GeometryBase geometry = Util.GetGeometry(element);
            BoundingBox ROI = geometry.Bounds().Inflate(tolerance);
            if (anyHeight) ROI.Extents.Z = 1e12;
            List<Curve> refGeom = Util.GetGeometries(refElements, ROI);

            // Do the actal snapping
            GeometryBase output = null;
            if (geometry is Point)
            {
                output = Snapping.HorizontalSnapToShape((Point)geometry, refGeom, tolerance, anyHeight);
            }
            else if (geometry is Line)
            {
                output = Snapping.HorizontalSnapToShape((Line)geometry, refGeom, tolerance, anyHeight);
            }
            else if (geometry is Curve)
            {
                output = Snapping.HorizontalSnapToShape((Curve)geometry, refGeom, tolerance, anyHeight);
            }
            else if (geometry is Group<Curve>)
            {
                output = Snapping.HorizontalSnapToShape((Group<Curve>)geometry, refGeom, tolerance, anyHeight);
            }

            // Return the final result
            return Util.SetGeometry(element, output);
        }

        /******************************************/

        public static Point HorizontalSnapToShape(Point point, List<Curve> refContours, double tolerance, bool anyHeight = false)
        {
            foreach (Curve refC in refContours)
            {
                Vector dir = refC.ClosestPoint(point) - point;
                if (anyHeight) dir.Z = 0;
                if (dir.Length < tolerance)
                {
                    dir.Z = 0;
                    return point + dir;
                }  
            }

            return point;
        }

        /******************************************/

        public static Line HorizontalSnapToShape(Line line, List<Curve> refContours, double tolerance, bool anyHeight = false)
        {
            return new Line(HorizontalSnapToShape(line.StartPoint, refContours, tolerance, anyHeight), HorizontalSnapToShape(line.EndPoint, refContours, tolerance, anyHeight));
        }

        /******************************************/

        public static Curve HorizontalSnapToShape(Curve contour, List<Curve> refContours, double tolerance, bool anyHeight = false)
        {
            // Get the refContours that are close enought to matter
            List<Curve> nearContours = Util.GetNearContours(contour, refContours, tolerance, anyHeight);

            // Get all reference lines
            List<Line> refLines = new List<Line>();
            foreach (Curve refC in nearContours)
            {
                foreach (Line refL in refC.Explode())
                {
                    if (Math.Abs(refL.Direction.Z) < 1e-3)
                        refLines.Add(refL.ProjectToGround());
                }

            }

            // Create snapping proposition list per horizontal position
            List<Point> oldPoints = contour.ControlPoints;
            Dictionary<string, List<Snap>> snapDirections = new Dictionary<string, List<Snap>>();
            foreach (Point pt in oldPoints)
            {
                snapDirections[getPointCode(pt)] = new List<Snap>();
            }

            // Get the lines to vote for points
            foreach (Line cLine in contour.Explode())
            {
                // Only work with horizontal lines
                if (Math.Abs(cLine.Direction.Z) > 1e-3) continue;
                Line line = cLine.ProjectToGround();

                // Get directions of the line
                Vector hDir = line.Direction;
                hDir.Unitize();
                Vector pDir = new Vector(-hDir.Y, hDir.X, 0);

                // Add snap propositions
                List<Snap> snaps = new List<Snap>();
                foreach (Line refL in refLines)
                {
                    if (line.Direction.IsParallel(refL.Direction, 0.02))
                        continue;
                    Point intersection = Intersect.LineLine(line, refL);
                    if (intersection != null)
                    {
                        Vector startDir = intersection - line.StartPoint;
                        if (startDir.Length < tolerance && startDir.Length > 0)
                            snapDirections[getPointCode(line.StartPoint)].Add(new Snap(startDir, refL));

                        Vector endDir = intersection - line.EndPoint;
                        if (endDir.Length < tolerance && endDir.Length > 0)
                            snapDirections[getPointCode(line.EndPoint)].Add(new Snap(endDir, refL));
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
                    Vector finalDir = snaps[0].dir.DuplicateVector();
                    Vector refDir = snaps[0].dir.DuplicateVector();
                    refDir.Unitize();

                    for (int i = 1; i < snaps.Count; i++)
                    {
                        double sin = Math.Sin(Vector.VectorAngle(refDir, snaps[i].dir));
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

        public static Group<Curve> HorizontalSnapToShape(Group<Curve> group, List<Curve> refContours, double tolerance, bool anyHeight = false)
        {
            Group<Curve> newGroup = new Group<Curve>();
            foreach (Curve curve in Curve.Join(group))
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

        public static object HorizontalParallelSnap(object element, List<object> refElements, double tolerance, bool anyHeight = false, double angleTol = 0.035)
        {
            // Get the geometry of the element
            GeometryBase geometry = Util.GetGeometry(element);
            BoundingBox ROI = geometry.Bounds().Inflate(tolerance);
            if (anyHeight) ROI.Extents.Z = 1e12;
            List<Curve> refGeom = Util.GetGeometries(refElements, ROI);

            // Do the actal snapping
            GeometryBase output = null;
            if (geometry is Curve)
            {
                output = Snapping.HorizontalParallelSnap((Curve)geometry, refGeom, tolerance, anyHeight, angleTol);
            }
            else if (geometry is Group<Curve>)
            {
                output = Snapping.HorizontalParallelSnap((Group<Curve>)geometry, refGeom, tolerance, anyHeight, angleTol);
            }

            // Return the final result
            return Util.SetGeometry(element, output);
        }

        /******************************************/

        public static Curve HorizontalParallelSnap(Curve contour, List<Curve> refContours, double tolerance, bool anyHeight = false, double angleTol = 0.035)
        {
            // Get the refContours that are close enought to matter
            List<Curve> nearContours = Util.GetNearContours(contour, refContours, tolerance, anyHeight);

            // Create snapping proposition list per horizontal position
            List<Point> oldPoints = contour.ControlPoints;
            Dictionary<string, List<Snap>> snapDirections = new Dictionary<string, List<Snap>>();
            foreach (Point pt in oldPoints)
            {
                snapDirections[getPointCode(pt)] = new List<Snap>();
            }

            // Get all reference lines
            List<Line> refLines = new List<Line>();
            foreach (Curve refC in nearContours)
            {
                foreach (Line refL in refC.Explode())
                {
                    if (Math.Abs(refL.Direction.Z) < 1e-3)
                        refLines.Add(refL.ProjectToGround());
                }
                    
            }

            // Get the lines to vote for points
            foreach (Line cLine in contour.Explode())
            {
                // Only work with horizontal lines
                if (Math.Abs(cLine.Direction.Z) > 1e-3) continue;
                Line line = cLine.ProjectToGround();

                // Get directions of the line
                Vector hDir = line.Direction;
                hDir.Unitize();
                Vector pDir = new Vector(-hDir.Y, hDir.X, 0);

                // Add snap propositions
                List<Snap> snaps = new List<Snap>();
                foreach (Line refL in refLines)
                {
                    if (line.Direction.IsParallel(refL.Direction, angleTol) && line.DistanceTo(refL) < tolerance)
                    {
                        Vector startDir = (pDir * (refL.StartPoint - line.StartPoint)) * pDir;
                        Vector endDir = (pDir * (refL.EndPoint - line.EndPoint)) * pDir;
                        Vector dir = (startDir.Length < endDir.Length) ? startDir : endDir;
                        if (dir.Length < tolerance)
                            snaps.Add(new Snap(dir, refL));
                    }
                }

                // Pick the shortest snap if any and add to points' list
                if (snaps.Count > 0)
                {
                    Snap bestSnap = snaps[0];
                    for (int i = 1; i < snaps.Count; i++)
                    {
                        if (snaps[i].dir.Length < bestSnap.dir.Length)
                            bestSnap = snaps[i];
                    }
                    Snap startSnap = new Snap(bestSnap.target.ProjectOnInfiniteLine(line.StartPoint) - line.StartPoint, bestSnap.target);
                    Snap endSnap = new Snap(bestSnap.target.ProjectOnInfiniteLine(line.EndPoint) - line.EndPoint, bestSnap.target);

                    snapDirections[getPointCode(line.StartPoint)].Add(startSnap);
                    snapDirections[getPointCode(line.EndPoint)].Add(endSnap);
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
                    cleanSnaps[0] = snaps[0];
                    for (int i = 1; i < snaps.Count; i++)
                    {
                        bool match = false;
                        for (int j = 0; j < cleanSnaps.Count; j++)
                        {
                            if (snaps[i].target.Direction.IsParallel(cleanSnaps[j].target.Direction))
                            {
                                if ((cleanSnaps[j].dir.Length < snaps[i].dir.Length))
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
                        newPoints.Add(pt + cleanSnaps[0].dir.DuplicateVector());
                    }
                    else
                    {
                        cleanSnaps = cleanSnaps.OrderBy(x => x.dir.Length).ToList();
                        Point target = Intersect.LineLine(snaps[0].target, snaps[1].target);
                        target.Z = pt.Z;
                        newPoints.Add(target);
                    }
                }

                
            }

            return new Polyline(newPoints);
        }

        /******************************************/

        public static Group<Curve> HorizontalParallelSnap(Group<Curve> group, List<Curve> refContours, double tolerance, bool anyHeight = false, double angleTol = 0.035)
        {
            Group<Curve> newGroup = new Group<Curve>();
            foreach (Curve curve in Curve.Join(group))
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

        public static object PointToPointSnap(object element, List<object> refElements, double tolerance)
        {
            // Get the geometry of the elements
            GeometryBase geometry = Util.GetGeometry(element);
            BoundingBox ROI = geometry.Bounds().Inflate(tolerance);

            // Get the reference points
            List<Point> refPoints = Util.GetControlPoints(refElements, ROI);

            // Do the actal snapping
            GeometryBase output = null;
            if (geometry is Point)
            {
                output = Snapping.PointToPointSnap((Point)geometry, refPoints, tolerance);
            }
            else if (geometry is Line)
            {
                output = Snapping.PointToPointSnap((Line)geometry, refPoints, tolerance);
            }
            else if (geometry is Curve)
            {
                output = Snapping.PointToPointSnap((Curve)geometry, refPoints, tolerance);
            }
            else if (geometry is Group<Curve>)
            {
                output = Snapping.PointToPointSnap((Group<Curve>)geometry, refPoints, tolerance);
            }

            // Return the final result
            return Util.SetGeometry(element, output);
        }

        /******************************************/

        public static Point PointToPointSnap(Point point, List<Point> refPoints, double tolerance)
        {
            
            foreach(Point refPt in refPoints)
            {
                if (refPt.DistanceTo(point) < tolerance)
                    return (Point)refPt.ShallowClone();
            }

            return (Point)point.ShallowClone();
        }

        /******************************************/

        public static Line PointToPointSnap(Line line, List<Point> refPoints, double tolerance)
        {
            return new Line(PointToPointSnap(line.StartPoint, refPoints, tolerance), PointToPointSnap(line.EndPoint, refPoints, tolerance));
        }

        /******************************************/

        public static Polyline PointToPointSnap(Curve curve, List<Point> refPoints, double tolerance)
        {
            List<Point> points = new List<Point>();
            foreach(Point pt in curve.ControlPoints)
            {
                points.Add(PointToPointSnap(pt, refPoints, tolerance));
            }
            return new Polyline(points);
        }

        /******************************************/

        public static Group<Curve> PointToPointSnap(Group<Curve> group, List<Point> refPoints, double tolerance)
        {
            Group<Curve> newGroup = new Group<Curve>();
            foreach (Curve curve in group)
            {
                if (curve is Line)
                    newGroup.Add(PointToPointSnap((Line)curve, refPoints, tolerance));
                else
                    newGroup.Add(PointToPointSnap(curve, refPoints, tolerance));
            }
            return newGroup;
        }


        /******************************************/
        /****  Utility classes and functions   ****/
        /******************************************/

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

        private static string getPointCode(Point pt)
        {
            return Math.Round(pt.X, 3).ToString() + ';' + Math.Round(pt.Y, 3).ToString();
        }

        
    }
}
