using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Geometry;
using BH.Engine.Base;
using BH.Engine.Geometry;

namespace ModelLaundry_Engine
{
    public static class Util
    {
        /*************************************/
        /****  Horizontal Extend          ****/
        /*************************************/

        public static object HorizontalExtend(object element, double dist)
        {
            // Get the geometry
            IBHoMGeometry geometry = null;
            if (element is BH.oM.Base.BHoMObject)
                geometry = ((BH.oM.Base.BHoMObject)element).IGetGeometry();
            else if (element is IBHoMGeometry)
                geometry = element as IBHoMGeometry;

            IBHoMGeometry output = null;
            if (geometry is Line)
            {
                output = Util.HorizontalExtend((Line)geometry, dist);
            }
            else if (geometry is ICurve)
            {
                output = Util.HorizontalExtend((ICurve)geometry, dist) as ICurve;
            }
            else if (geometry is List<ICurve>)
            {
                output = Util.HorizontalExtend((List<ICurve>)geometry, dist) as ICurve;
            }
            //else if (geometry is CompositeGeometry)
            //{
            //    List<ICurve> curves = geometry.GetExploded();
            //    output = Util.HorizontalExtend(geometry.GetExploded(), dist) as CompositeGeometry;
            //}

            // Prepare the result
            object result = element;
            if (element is BH.oM.Base.BHoMObject)
            {
                result = (BH.oM.Base.BHoMObject)((BH.oM.Base.BHoMObject)element).GetShallowClone();
                ((BH.oM.Base.BHoMObject)result).ISetGeometry(output);
            }
            else if (element is IBHoMGeometry)
            {
                result = output;
            }

            // Return the result
            return result;

        }

        /*************************************/

        public static ICurve HorizontalExtend(ICurve contour, double dist)
        {
            List<Point> oldPoints = contour.IGetControlPoints();
            List<Point> newPoints = new List<Point>();

            int nb = oldPoints.Count();
            if (oldPoints.Last().GetDistance(oldPoints[0]) < 0.01)
                nb = oldPoints.Count() - 1;

            for (int i = 0; i < nb; i++)
            {
                Point pt = oldPoints[i];
                Point prev = (i == 0) ? oldPoints[nb - 1] : oldPoints[i - 1];
                Point next = (i == nb - 1) ? oldPoints[0] : oldPoints[i + 1];

                Vector pDir = pt - prev; pDir.GetNormalised();
                Vector nDir = pt - next; nDir.GetNormalised();

                if (Math.Abs(pDir.Z) < 0.01 && Math.Abs(nDir.Z) < 0.01)
                {
                    newPoints.Add(pt);
                }
                else if (Math.Abs(pDir.Z) < 0.01)
                {
                    pDir.Z = 0; pDir.GetNormalised();
                    newPoints.Add(pt + dist * pDir);
                }
                else if (Math.Abs(nDir.Z) < 0.01)
                {
                    nDir.Z = 0; nDir.GetNormalised();
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

        /*************************************/

        public static Line HorizontalExtend(Line line, double dist)
        {
            Vector dir = line.GetDirection();
            dir.Z = 0;
            dir.GetNormalised();
            dir = dist * dir;

            return new Line(line.Start - dir, line.End + dir);
        }

        /*************************************/

        public static List<ICurve> HorizontalExtend(List<ICurve> group, double dist)
        {
            List<ICurve> newGroup = new List<ICurve>();
            foreach ( ICurve curve in group)
            {
                newGroup.Add(HorizontalExtend(curve, dist));
            }
            return newGroup;
        }


        /*************************************/
        /****  Filter By Bounding Box     ****/
        /*************************************/

        public static List<object> FilterByBoundingBox(List<object> elements, List<BoundingBox> boxes, out List<object> outsiders)
        {
            // Separate the objects between insiders and outsiders
            List<object> insiders = new List<object>();
            outsiders = new List<object>();
            foreach (object element in elements)
            {
                if (element is BH.oM.Base.BHoMObject && Util.IsInside(((BH.oM.Base.BHoMObject)element).IGetGeometry(), boxes))
                    insiders.Add(element);
                else if (element is IBHoMGeometry && Util.IsInside((IBHoMGeometry)element, boxes))
                    insiders.Add(element);
                else
                    outsiders.Add(element);
            }

            return insiders;
        }

        /*************************************/

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

        public static List<BH.oM.Base.BHoMObject> FilterByBoundingBox(List<BH.oM.Base.BHoMObject> elements, List<BoundingBox> boxes, out List<BH.oM.Base.BHoMObject> outsiders)
        {
            List<BH.oM.Base.BHoMObject> insiders = new List<BH.oM.Base.BHoMObject>();
            outsiders = new List<BH.oM.Base.BHoMObject>();
            foreach (BH.oM.Base.BHoMObject element in elements)
            {
                if (IsInside(element.IGetGeometry(), boxes))
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
        /****  Get Near Contours          ****/
        /*************************************/

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
    }
}
