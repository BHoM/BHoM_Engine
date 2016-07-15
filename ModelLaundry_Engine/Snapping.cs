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
        /****  Vertical Point Snapping         ****/
        /******************************************/

        public static Point VerticalPointSnap(Point point, List<double> refHeights, double tolerance)
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

        public static Point VerticalPointSnap(Point point, List<Curve> refContours, double tolerance)
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
        /****  Vertical End Point Snapping     ****/
        /******************************************/

        public static Line VerticalEndSnap(Line line, List<double> refHeights, double tolerance)
        {
            return new Line(VerticalPointSnap(line.StartPoint, refHeights, tolerance), VerticalPointSnap(line.EndPoint, refHeights, tolerance));
        }

        /******************************************/

        public static Line VerticalEndSnap(Line line, List<Curve> refContours, double tolerance)
        {
            return new Line(VerticalPointSnap(line.StartPoint, refContours, tolerance), VerticalPointSnap(line.EndPoint, refContours, tolerance));
        }

        /******************************************/

        public static Curve VerticalEndSnap(Curve contour, List<double> refHeights, double tolerance)
        {
            List<Point> oldPoints = contour.ControlPoints;
            List<Point> newPoints = new List<Point>();

            foreach (Point pt in oldPoints)
            {
                newPoints.Add(VerticalPointSnap(pt, refHeights, tolerance));
            }
            return new Polyline(newPoints);
        }

        /******************************************/

        public static Curve VerticalEndSnap(Curve contour, List<Curve> refContours, double tolerance)
        {
            List<Point> oldPoints = contour.ControlPoints;
            List<Point> newPoints = new List<Point>();

            foreach (Point pt in oldPoints)
            {
                newPoints.Add(VerticalPointSnap(pt, refContours, tolerance));
            }
            return new Polyline(newPoints);
        }

        /******************************************/

        public static Group<Curve> VerticalEndSnap(Group<Curve> group, List<double> refHeights, double tolerance)
        {
            Group<Curve> newGroup = new Group<Curve>();
            foreach (Curve curve in group)
            {
                newGroup.Add(VerticalEndSnap(curve, refHeights, tolerance));
            }
            return newGroup;
        }

        /******************************************/

        public static Group<Curve> VerticalEndSnap(Group<Curve> group, List<Curve> refContours, double tolerance)
        {
            Group<Curve> newGroup = new Group<Curve>();
            foreach (Curve curve in group)
            {
                newGroup.Add(VerticalEndSnap(curve, refContours, tolerance));
            }
            return newGroup;
        }


        /******************************************/
        /****  Horizontal End Point Snapping   ****/
        /******************************************/

        public static Curve HorizontalPointSnap(Curve contour, List<Curve> refContours, double tolerance)
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

        public static Group<Curve> HorizontalPointSnap(Group<Curve> group, List<Curve> refContours, double tolerance)
        {
            Group<Curve> newGroup = new Group<Curve>();
            foreach (Curve curve in Curve.Join(group))
            {
                newGroup.Add(HorizontalPointSnap(curve, refContours, tolerance));
            }
            return newGroup;
        }


        /******************************************/
        /****  Horizontal Parrallel Snapping   ****/
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
                newGroup.Add(HorizontalParallelSnap(curve, refContours, tolerance));
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
