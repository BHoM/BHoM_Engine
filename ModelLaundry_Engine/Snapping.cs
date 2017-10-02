using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Geometry;
using BH.Engine.Geometry;
using System.Collections;

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
            IBHoMGeometry geometry = Util.GetGeometry(element);
            IBHoMGeometry output = null;

            if (!typeof(IEnumerable).IsAssignableFrom(element.GetType()))
            {
                if (geometry is Point)
                {
                    output = Snapping.VerticalSnapToHeight((Point)geometry, refHeights, tolerance);
                }
                else if (geometry is Line)
                {
                    output = Snapping.VerticalSnapToHeight((Line)geometry, refHeights, tolerance);
                }
                else if (geometry is ICurve)
                {
                    output = Snapping.VerticalSnapToHeight((ICurve)geometry, refHeights, tolerance);
                }
            }
            else
            {
                if (geometry is List<Point>)
                {
                    foreach (Point point in geometry as List<Point>)
                    {
                        output = Snapping.VerticalSnapToHeight(point, refHeights, tolerance);
                    }
                }
                else if (geometry is List<Line>)
                {
                    foreach (Line line in geometry as List<Line>)
                    {
                        output = Snapping.VerticalSnapToHeight(line, refHeights, tolerance);
                    }
                }
                else if (geometry is List<ICurve>)
                {
                    foreach (ICurve curve in geometry as List<ICurve>)
                    {
                        output = Snapping.VerticalSnapToHeight(curve, refHeights, tolerance);
                    }
                }                
            }

            // Return the final result
            return Util.SetGeometry(element, output);
        }

        /******************************************/

        public static Point VerticalSnapToHeight(Point point, List<double> refHeights, double tolerance)
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

        public static Line VerticalSnapToHeight(Line line, List<double> refHeights, double tolerance)
        {
            return new Line(VerticalSnapToHeight(line.Start, refHeights, tolerance), VerticalSnapToHeight(line.End, refHeights, tolerance));
        }

        /******************************************/

        public static ICurve VerticalSnapToHeight(ICurve contour, List<double> refHeights, double tolerance)
        {
            List<Point> oldPoints = contour._GetControlPoints();
            List<Point> newPoints = new List<Point>();

            foreach (Point pt in oldPoints)
            {
                newPoints.Add(VerticalSnapToHeight(pt, refHeights, tolerance));
            }
            return new Polyline(newPoints);
        }

        /******************************************/

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
        /****  Vertical Snapping to Curves     ****/
        /******************************************/

        public static object VerticalSnapToShape(object element, List<object> refElements, double tolerance)
        {
            // Get the geometry of the elements
            IBHoMGeometry geometry = Util.GetGeometry(element);
            BoundingBox ROI = geometry._GetBounds().GetInflated(tolerance);
            List<ICurve> refGeom = Util.GetGeometries(refElements, ROI);

            IBHoMGeometry output = null;

            if (!typeof(IEnumerable).IsAssignableFrom(element.GetType()))
            {
                if (geometry is Point)
                {
                    output = Snapping.VerticalSnapToShape((Point)geometry, refGeom, tolerance);
                }
                else if (geometry is Line)
                {
                    output = Snapping.VerticalSnapToShape((Line)geometry, refGeom, tolerance);
                }
                else if (geometry is ICurve)
                {
                    output = Snapping.VerticalSnapToShape((ICurve)geometry, refGeom, tolerance);
                }
            }
            else
            {
                if (geometry is List<Point>)
                {
                    foreach (Point point in geometry as List<Point>)
                    {
                        output = Snapping.VerticalSnapToShape(point, refGeom, tolerance);
                    }
                }
                else if (geometry is Line)
                {
                    foreach (Line line in geometry as List<Line>)
                    {
                        output = Snapping.VerticalSnapToShape(line, refGeom, tolerance);
                    }
                }
                else if (geometry is ICurve)
                {
                    foreach (ICurve curve in geometry as List<ICurve>)
                    {
                        output = Snapping.VerticalSnapToShape(curve, refGeom, tolerance);
                    }
                }
            }

              
            // Return the final result
            return Util.SetGeometry(element, output);
        }

        /******************************************/

        public static Point VerticalSnapToShape(Point point, List<ICurve> refContours, double tolerance)
        {
            Point newPoint = new Point(point.X, point.Y, point.Z);

            foreach (ICurve contour in refContours)
            {
                double height = contour._GetBounds().GetCentre().Z;
                if (Math.Abs(newPoint.Z - height) < tolerance) // Need to add && contour.IsInside(newPoint)
                {
                    newPoint.Z = height;
                    break;
                }
            }
            return newPoint;
        }

        /******************************************/

        public static Line VerticalSnapToShape(Line line, List<ICurve> refContours, double tolerance)
        {
            return new Line(VerticalSnapToShape(line.Start, refContours, tolerance), VerticalSnapToShape(line.End, refContours, tolerance));
        }

        /******************************************/

        public static ICurve VerticalSnapToShape(ICurve contour, List<ICurve> refContours, double tolerance)
        {
            List<Point> oldPoints = contour._GetControlPoints();
            List<Point> newPoints = new List<Point>();

            foreach (Point pt in oldPoints)
            {
                newPoints.Add(VerticalSnapToShape(pt, refContours, tolerance));
            }
            return new Polyline(newPoints);
        }

        /******************************************/

        public static List<ICurve> VerticalSnapToShape(List<ICurve> group, List<ICurve> refContours, double tolerance)
        {
            List<ICurve> newGroup = new List<ICurve>();
            foreach (ICurve curve in group)
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
            IBHoMGeometry geometry = Util.GetGeometry(element);
            BoundingBox ROI = geometry._GetBounds().GetInflated(tolerance);
            if (anyHeight) ROI.GetExtents().Z = 1e12;
            List<ICurve> refGeom = Util.GetGeometries(refElements, ROI);

            // Do the actal snapping
            IBHoMGeometry output = null;
            if (geometry is Point)
            {
                output = Snapping.HorizontalSnapToShape((Point)geometry, refGeom, tolerance, anyHeight);
            }
            else if (geometry is Line)
            {
                output = Snapping.HorizontalSnapToShape((Line)geometry, refGeom, tolerance, anyHeight);
            }
            else if (geometry is ICurve)
            {
                output = Snapping.HorizontalSnapToShape((ICurve)geometry, refGeom, tolerance, anyHeight);
            }
            if (geometry is List<ICurve>)
            {
                foreach (ICurve curve in geometry as List<ICurve>)
                {
                    output = Snapping.HorizontalSnapToShape(curve, refGeom, tolerance, anyHeight);
                }
            }

            // Return the final result
            return Util.SetGeometry(element, output);
        }

        /******************************************/

        public static Point HorizontalSnapToShape(Point point, List<ICurve> refContours, double tolerance, bool anyHeight = false)
        {
            foreach (ICurve refC in refContours)
            {
                Vector dir = refC._GetClosestPoint(point) - point;
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

        public static Line HorizontalSnapToShape(Line line, List<ICurve> refContours, double tolerance, bool anyHeight = false)
        {
            return new Line(HorizontalSnapToShape(line.Start, refContours, tolerance, anyHeight), HorizontalSnapToShape(line.End, refContours, tolerance, anyHeight));
        }

        /******************************************/

        public static ICurve HorizontalSnapToShape(ICurve contour, List<ICurve> refContours, double tolerance, bool anyHeight = false)
        {
            // Get the refContours that are close enought to matter
            List<ICurve> nearContours = Util.GetNearContours(contour, refContours, tolerance, anyHeight);

            // Get all reference lines
            Plane ground = Plane.XY;
            List<Line> refLines = new List<Line>();
            foreach (ICurve refC in nearContours)
            {
                foreach (Line refL in refC.GetExploded())
                {
                    if (Math.Abs(refL.GetDirection().Z) < 1e-3)
                    {
                        Transform.GetProjected(refL, ground); // This only works because refL is a new line. Project should create a new line, not modify the existing one
                        refLines.Add(refL);
                    }
                        
                }

            }

            // Create snapping proposition list per horizontal position
            List<Point> oldPoints = contour._GetControlPoints();
            Dictionary<string, List<Snap>> snapDirections = new Dictionary<string, List<Snap>>();
            foreach (Point pt in oldPoints)
            {
                snapDirections[getPointCode(pt)] = new List<Snap>();
            }

            // Get the lines to vote for points
            foreach (Line cLine in contour.GetExploded())
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

        public static object HorizontalParallelSnap(object element, List<object> refElements, double tolerance, bool anyHeight = false, double angleTol = 0.035)
        {
            // Get the geometry of the element
            IBHoMGeometry geometry = Util.GetGeometry(element);
            BoundingBox ROI = geometry._GetBounds().GetInflated(tolerance);
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

        public static ICurve HorizontalParallelSnap(ICurve contour, List<ICurve> refContours, double tolerance, bool anyHeight = false, double angleTol = 0.035)
        {
            // Get the refContours that are close enought to matter
            List<ICurve> nearContours = Util.GetNearContours(contour, refContours, tolerance, anyHeight);

            // Create snapping proposition list per horizontal position
            List<Point> oldPoints = contour._GetControlPoints();
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
                foreach (Line refL in refC.GetExploded())
                {
                    if (Math.Abs(refL.GetDirection().Z) < 1e-3)
                    {
                        Transform.GetProjected(refL, ground);
                        refLines.Add(refL);
                    }
                }
                    
            }

            // Get the lines to vote for points
            foreach (Line cLine in contour.GetExploded())
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

        public static object PointToPointSnap(object element, List<object> refElements, double tolerance)
        {
            // Get the geometry of the elements
            IBHoMGeometry geometry = Util.GetGeometry(element);
            BoundingBox ROI = geometry._GetBounds().GetInflated(tolerance);

            // Get the reference points
            List<Point> refPoints = Util.GetControlPoints(refElements, ROI);

            // Do the actal snapping
            IBHoMGeometry output = null;
            if (geometry is Point)
            {
                output = Snapping.PointToPointSnap((Point)geometry, refPoints, tolerance);
            }
            else if (geometry is Line)
            {
                output = Snapping.PointToPointSnap((Line)geometry, refPoints, tolerance);
            }
            else if (geometry is ICurve)
            {
                output = Snapping.PointToPointSnap((ICurve)geometry, refPoints, tolerance);
            }
            if (geometry is List<ICurve>)
            {
                foreach (ICurve curve in geometry as List<ICurve>)
                {
                    output = Snapping.PointToPointSnap(curve, refPoints, tolerance);
                }
            }

            // Return the final result
            return Util.SetGeometry(element, output);
        }

        /******************************************/

        public static Point PointToPointSnap(Point point, List<Point> refPoints, double tolerance)
        {
            
            foreach(Point refPt in refPoints)
            {
                if (refPt.GetDistance(point) < tolerance)
                    return (Point)refPt.GetClone();
            }

            return (Point)point.GetClone();
        }

        /******************************************/

        public static Line PointToPointSnap(Line line, List<Point> refPoints, double tolerance)
        {
            return new Line(PointToPointSnap(line.Start, refPoints, tolerance), PointToPointSnap(line.End, refPoints, tolerance));
        }

        /******************************************/

        public static Polyline PointToPointSnap(ICurve curve, List<Point> refPoints, double tolerance)
        {
            List<Point> points = new List<Point>();
            foreach(Point pt in curve._GetControlPoints())
            {
                points.Add(PointToPointSnap(pt, refPoints, tolerance));
            }
            return new Polyline(points);
        }

        /******************************************/

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
