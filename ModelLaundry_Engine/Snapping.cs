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

        public static object HorizontalSnapToShape(object element, List<object> refElements, double tolerance)
        {
            // Get the geometry of the element
            GeometryBase geometry = Util.GetGeometry(element);
            BoundingBox ROI = geometry.Bounds().Inflate(tolerance);
            List<Curve> refGeom = Util.GetGeometries(refElements, ROI);

            // Do the actal snapping
            GeometryBase output = null;
            if (geometry is Point)
            {
                output = Snapping.HorizontalSnapToShape((Point)geometry, refGeom, tolerance);
            }
            else if (geometry is Line)
            {
                output = Snapping.HorizontalSnapToShape((Line)geometry, refGeom, tolerance);
            }
            else if (geometry is Curve)
            {
                output = Snapping.HorizontalSnapToShape((Curve)geometry, refGeom, tolerance);
            }
            else if (geometry is Group<Curve>)
            {
                output = Snapping.HorizontalSnapToShape((Group<Curve>)geometry, refGeom, tolerance);
            }

            // Return the final result
            return Util.SetGeometry(element, output);
        }

        /******************************************/

        public static Point HorizontalSnapToShape(Point point, List<Curve> refContours, double tolerance)
        {
            foreach (Curve refC in refContours)
            {
                Vector dir = refC.ClosestPoint(point) - point;
                if (dir.Length < tolerance)
                {
                    dir.Z = 0;
                    return point + dir;
                }  
            }

            return point;
        }

        /******************************************/

        public static Line HorizontalSnapToShape(Line line, List<Curve> refContours, double tolerance)
        {
            return new Line(HorizontalSnapToShape(line.StartPoint, refContours, tolerance), HorizontalSnapToShape(line.EndPoint, refContours, tolerance));
        }

        /******************************************/

        public static Curve HorizontalSnapToShape(Curve contour, List<Curve> refContours, double tolerance)
        {
            // Get the refContours that are close enought to matter
            List<Curve> nearContours = Util.GetNearContours(contour, refContours, tolerance);

            // Get the horizontal perpendicular direction of the contour
            List<Point> oldPoints = contour.ControlPoints;
            Vector hDir = new Vector(0, 0, 0);
            for (int i = 1; i < oldPoints.Count; i++)
            {
                Vector dir = oldPoints[i] - oldPoints[i - 1];
                if (Math.Abs(dir.Z) < 1e-3)
                {
                    hDir = new Vector(-dir.Y, dir.X, 0);
                    hDir.Unitize();
                    break;
                }
            }

            // Create snapping propositions per horizontal position
            Dictionary<string, Vector> snapDirections = new Dictionary<string, Vector>();
            foreach (Point pt in oldPoints)
            {
                string code = getPointCode(pt);

                foreach (Curve refC in nearContours)
                {
                    Vector dir = refC.ClosestPoint(pt) - pt;
                    if (dir.Length > 1e-3 && dir.Length < tolerance && Math.Abs(dir * hDir) < 1e-3)
                        snapDirections[code] = dir;
                }
            }

            // Move points according to snapping propositions
            List<Point> newPoints = new List<Point>();
            foreach (Point pt in oldPoints)
            {
                string code = getPointCode(pt);
                if (snapDirections.ContainsKey(code))
                    newPoints.Add(pt + snapDirections[code]);
                else
                    newPoints.Add(pt);
            }

            return new Polyline(newPoints);
        }

        /******************************************/

        public static Group<Curve> HorizontalSnapToShape(Group<Curve> group, List<Curve> refContours, double tolerance)
        {
            Group<Curve> newGroup = new Group<Curve>();
            foreach (Curve curve in Curve.Join(group))
            {
                if (curve is Line)
                    newGroup.Add(HorizontalSnapToShape((Line)curve, refContours, tolerance));
                else
                    newGroup.Add(HorizontalSnapToShape(curve, refContours, tolerance));
            }
            return newGroup;
        }


        /******************************************/
        /****  Horizontal Parrallel Snapping   ****/
        /******************************************/

        public static object HorizontalParallelSnap(object element, List<object> refElements, double tolerance)
        {
            // Get the geometry of the element
            GeometryBase geometry = Util.GetGeometry(element);
            BoundingBox ROI = geometry.Bounds().Inflate(tolerance);
            List<Curve> refGeom = Util.GetGeometries(refElements, ROI);

            // Do the actal snapping
            GeometryBase output = null;
            if (geometry is Curve)
            {
                output = Snapping.HorizontalParallelSnap((Curve)geometry, refGeom, tolerance);
            }
            else if (geometry is Group<Curve>)
            {
                output = Snapping.HorizontalParallelSnap((Group<Curve>)geometry, refGeom, tolerance);
            }

            // Return the final result
            return Util.SetGeometry(element, output);
        }

        /******************************************/

        public static Curve HorizontalParallelSnap(Curve contour, List<Curve> refContours, double tolerance)
        {
            // Get the refContours that are close enought to matter
            List<Curve> nearContours = Util.GetNearContours(contour, refContours, tolerance);

            // Create snapping proposition list per horizontal position
            List<Point> oldPoints = contour.ControlPoints;
            Dictionary<string, List<Snap>> snapDirections = new Dictionary<string, List<Snap>>();
            foreach (Point pt in oldPoints)
            {
                snapDirections[getPointCode(pt)] = new List<Snap>();
            }

            // Get the lines to vote for points
            foreach (Line line in contour.Explode())
            {
                // Only work with horizontal lines
                if (line.Direction.Z > 1e-3) continue;

                // Get directions of the line
                Vector hDir = line.Direction;
                hDir.Unitize();
                Vector pDir = new Vector(-hDir.Y, hDir.X, 0);

                // Add snap propositions
                List<Snap> snaps = new List<Snap>();
                foreach (Curve refC in nearContours)
                {
                    foreach (Line refL in refC.Explode())
                    {
                        if (line.Direction.IsParallel(refL.Direction))
                        {
                            Vector dir = (pDir * (refL.EndPoint - line.EndPoint)) * pDir;
                            if (dir.Length < tolerance && dir.Length > 0)
                                snaps.Add(new Snap(dir, refL));
                        }
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
                    snapDirections[getPointCode(line.StartPoint)].Add(bestSnap);
                    snapDirections[getPointCode(line.StartPoint)].Add(bestSnap);
                }
            }

            // Move points according to snapping propositions
            List<Point> newPoints = new List<Point>();
            foreach (Point pt in oldPoints)
            {
                List<Snap> snaps = snapDirections[getPointCode(pt)];
                if (snaps.Count > 0)
                {
                    Vector finalDir = snaps[0].dir;
                    Vector refDir = snaps[0].dir;
                    refDir.Unitize();

                    for (int i = 0; i < snaps.Count; i++)
                        finalDir += snaps[i].dir * Math.Sin(Vector.VectorAngle(refDir, snaps[i].dir));
                    newPoints.Add(pt + finalDir);
                }
                else
                    newPoints.Add(pt);
            }

            return new Polyline(newPoints);
        }

        /******************************************/

        public static Group<Curve> HorizontalParallelSnap(Group<Curve> group, List<Curve> refContours, double tolerance)
        {
            Group<Curve> newGroup = new Group<Curve>();
            foreach (Curve curve in Curve.Join(group))
            {
                if (curve is Line)
                    newGroup.Add(HorizontalParallelSnap((Line)curve, refContours, tolerance));
                else
                    newGroup.Add(HorizontalParallelSnap(curve, refContours, tolerance));
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
            return Math.Round(pt.X, 3).ToString() + '-' + Math.Round(pt.Y, 3).ToString();
        }

        
    }
}
