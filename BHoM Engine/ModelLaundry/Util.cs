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
        public static Polyline HorizontalExtend(Polyline contour, double dist)
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

        public static List<GeometryBase> FilterByBoundingBox(List<GeometryBase> elements, List<BoundingBox> boxes, out List<GeometryBase> outsiders)
        {
            List<GeometryBase> insiders = new List<GeometryBase>();
            outsiders = new List<GeometryBase>();
            foreach (GeometryBase element in elements)
            {
                bool keep = false;
                BoundingBox eBox = element.Bounds();
                foreach (BoundingBox box in boxes)
                {
                    if (box.Contains(eBox))
                    {
                        keep = true;
                        break;
                    }
                }
                if (keep)
                    insiders.Add(element);
                else
                {
                    outsiders.Add(element);
                }
            }

            return insiders;
        }

        public static List<Curve> GetNearContours(Polyline refContour, List<Polyline> contours, double tolerance)
        {
            BoundingBox ROI = refContour.Bounds();
            ROI.Inflate(tolerance);

            List<Curve> nearContours = new List<Curve>();
            foreach (Polyline refC in contours)
            {
                BoundingBox cBox = refC.Bounds();
                if (ROI.Contains(cBox))
                    continue;

                if (BoundingBox.InRange(ROI, cBox))
                    nearContours.Add(refC);
            }

            return nearContours;
        }
    }
}
