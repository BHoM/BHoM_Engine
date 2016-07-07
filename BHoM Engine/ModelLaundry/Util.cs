using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BHoM.Geometry;

namespace BHoM_Engine.ModelLaundry
{
    public static class Util
    {
        public static Curve HorizontalExtend(Curve contour, double dist)
        {
            List<Point> oldPoints = contour.ControlPoints;
            List<Point> newPoints = new List<Point>();

            int nb = oldPoints.Count();
            if (oldPoints.Last().DistanceTo(oldPoints[0]) < 0.01)
                nb = oldPoints.Count() - 1;

            for (int i = 0; i < nb; i++)
            {
                Point pt = oldPoints[i];
                Point prev = (i == 0) ? oldPoints[nb - 1] : oldPoints[i - 1];
                Point next = (i == nb - 1) ? oldPoints[0] : oldPoints[i + 1];

                Vector pDir = pt - prev; pDir.Unitize();
                Vector nDir = pt - next; nDir.Unitize();

                if (Math.Abs(pDir.Z) < 0.01 && Math.Abs(nDir.Z) < 0.01)
                {
                    newPoints.Add(pt);
                }
                else if (Math.Abs(pDir.Z) < 0.01)
                {
                    pDir.Z = 0; pDir.Unitize();
                    newPoints.Add(pt + dist * pDir);
                }
                else if (Math.Abs(nDir.Z) < 0.01)
                {
                    nDir.Z = 0; nDir.Unitize();
                    newPoints.Add(pt + dist * nDir);
                }
                else
                {
                    newPoints.Add(pt);
                }
            }

            if (nb < oldPoints.Count())
                newPoints.Add(newPoints[0]);

            return new Polyline(newPoints);
        }


        public static Line HorizontalExtend(Line line, double dist)
        {
            Vector dir = line.Direction;
            dir.Z = 0;
            dir.Unitize();
            dir = dist * dir;

            return new Line(line.StartPoint - dir, line.EndPoint + dir);
        }

        public static Group<Curve> HorizontalExtend(Group<Curve> group, double dist)
        {
            Group<Curve> newGroup = new Group<Curve>();
            foreach ( Curve curve in Curve.Join(group) )
            {
                newGroup.Add(HorizontalExtend(curve, dist));
            }
            return newGroup;
        }

        public static List<GeometryBase> FilterByBoundingBox(List<GeometryBase> elements, List<BoundingBox> boxes, out List<GeometryBase> outsiders)
        {
            List<GeometryBase> insiders = new List<GeometryBase>();
            outsiders = new List<GeometryBase>();
            foreach (GeometryBase element in elements)
            {
                if (IsInside(element, boxes))
                    insiders.Add(element);
                else
                    outsiders.Add(element);
            }

            return insiders;
        }

        public static List<BHoM.Global.BHoMObject> FilterByBoundingBox(List<BHoM.Global.BHoMObject> elements, List<BoundingBox> boxes, out List<BHoM.Global.BHoMObject> outsiders)
        {
            List<BHoM.Global.BHoMObject> insiders = new List<BHoM.Global.BHoMObject>();
            outsiders = new List<BHoM.Global.BHoMObject>();
            foreach (BHoM.Global.BHoMObject element in elements)
            {
                if (IsInside(element.GetGeometry(), boxes))
                    insiders.Add(element);
                else
                    outsiders.Add(element);
            }

            return insiders;
        }

        public static bool IsInside(GeometryBase geometry, List<BoundingBox> boxes)
        {
            bool inside = false;
            BoundingBox eBox = geometry.Bounds();
            foreach (BoundingBox box in boxes)
            {
                if (box.Contains(eBox))
                {
                    inside = true;
                    break;
                }
            }
            return inside;
        }

        public static List<Curve> GetNearContours(Curve refContour, List<Curve> contours, double tolerance)
        {
            BoundingBox ROI = refContour.Bounds();
            ROI.Inflate(tolerance);

            List<Curve> nearContours = new List<Curve>();
            foreach (Curve refC in contours)
            {
                BoundingBox cBox = refC.Bounds();
                if (ROI.Contains(cBox))
                    continue;

                if (BoundingBox.InRange(ROI, cBox))
                    nearContours.Add(refC);
            }

            return nearContours;
        }

        public static Group<Curve> RemoveSmallContours(Group<Curve> contours, double maxLength, out Group<Curve> removed)
        {
            Group<Curve> remaining = new Group<Curve>();
            removed = new Group<Curve>();

            foreach( Curve contour in contours)
            {
                if (contour.Length < maxLength)
                    removed.Add(contour);
                else
                    remaining.Add(contour);
            }

            return remaining;
        }
    }
}
