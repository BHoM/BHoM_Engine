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
                geometry = ((BH.oM.Base.BHoMObject)element).GetGeometry();
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
                ((BH.oM.Base.BHoMObject)result).SetGeometry(output);
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
            List<Point> oldPoints = contour.GetControlPoints();
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
                if (element is BH.oM.Base.BHoMObject && Util.IsInside(((BH.oM.Base.BHoMObject)element).GetGeometry(), boxes))
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
                if (IsInside(element.GetGeometry(), boxes))
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
            BoundingBox eBox = geometry.GetBounds();
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
        /****  Remove Small Contours      ****/
        /*************************************/

        public static object RemoveSmallContours(object element, double maxLength, out List<ICurve> removed)
        {
            // Get the geometry
            IBHoMGeometry geometry = null;
            if (element is BH.oM.Base.BHoMObject)
                geometry = ((BH.oM.Base.BHoMObject)element).GetGeometry();
            else if (element is IBHoMGeometry)
                geometry = element as IBHoMGeometry;

            removed = new List<ICurve>();
            List<ICurve> output = null;
            if (geometry is ICurve)
            {
                List<ICurve> group = new List<ICurve>();
                group.Add((ICurve)geometry);
                output = Util.RemoveSmallContours(group, maxLength, out removed);
            }
            else if (geometry is List<ICurve>)
            {
                output = Util.RemoveSmallContours((List<ICurve>)geometry, maxLength, out removed);
            }

            // Prepare the result
            object result = element;
            if (element is BH.oM.Base.BHoMObject)
            result = output;

            // Return the result
            return result;

        }

        /*************************************/

        public static List<ICurve> RemoveSmallContours(List<ICurve> contours, double maxLength, out List<ICurve> removed)
        {
            List<ICurve> remaining = new List<ICurve>();
            removed = new List<ICurve>();

            foreach (ICurve contour in contours)
            {
                if (contour.GetLength() < maxLength)
                    removed.Add(contour);
                else
                    remaining.Add(contour);
            }

            return remaining;
        }


        /*************************************/
        /****  Get Near Contours          ****/
        /*************************************/

        public static List<ICurve> GetNearContours(ICurve refContour, List<ICurve> contours, double tolerance, bool anyHeight = false)
        {
            BoundingBox bounds = refContour.GetBounds();
            BoundingBox ROI = bounds.GetInflated(tolerance);
            if (anyHeight) ROI.GetExtents().Z = 1e12;

            List<ICurve> nearContours = new List<ICurve>();
            foreach (ICurve refC in contours)
            {
                BoundingBox cBox = refC.GetBounds();
                if (cBox.GetCentre().GetDistance(bounds.GetCentre()) > 1e-5 && cBox.IsInRange(ROI))
                    nearContours.Add(refC);
            }

            return nearContours;
        }


        /*************************************/
        /****  Geometry accessors         ****/
        /*************************************/

        internal static IBHoMGeometry GetGeometry(object element)
        {
            IBHoMGeometry geometry = null;
            if (element is BH.oM.Base.BHoMObject)
                geometry = ((BH.oM.Base.BHoMObject)element).GetGeometry();
            else if (element is IBHoMGeometry)
                geometry = element as IBHoMGeometry;
            return geometry;
        }

        /******************************************/

        internal static object SetGeometry(object element, IBHoMGeometry geometry)
        {
            object result = element;
            if (element is BH.oM.Base.BHoMObject)
            {
                result = (BH.oM.Base.BHoMObject)((BH.oM.Base.BHoMObject)element).GetShallowClone();
                ((BH.oM.Base.BHoMObject)result).SetGeometry(geometry);
            }
            else if (element is IBHoMGeometry)
            {
                result = geometry;
            }

            return result;
        }

        /******************************************/

        internal static List<ICurve> GetGeometries(List<object> elements)
        {
            // Get the geometry of the ref elements
            List<ICurve> geometries = new List<ICurve>();
            foreach (object element in elements)
            {
                IBHoMGeometry geometry = GetGeometry(element);

                if (geometry is ICurve)
                    geometries.Add((ICurve)geometry);
                else if (geometry is List<ICurve>)
                {
                    geometries.AddRange((List<ICurve>)geometry);
                }
            }

            return geometries;
        }

        /******************************************/

        internal static List<ICurve> GetGeometries(List<object> elements, BoundingBox ROI)
        {
            // Get the geometry of the ref elements
            List<ICurve> geometries = new List<ICurve>();
            foreach (object element in elements)
            {
                IBHoMGeometry geometry = GetGeometry(element);
                if (geometry.GetBounds().IsInRange(ROI))
                {
                    if (geometry is ICurve)
                        geometries.Add((ICurve)geometry);
                    else if (geometry is List<ICurve>)
                    {
                        foreach (ICurve curve in (List<ICurve>)geometry)
                        {
                            geometries.Add(curve);
                        }
                    }
                }
            }

            return geometries;
        }

        /******************************************/

        internal static List<Point> GetControlPoints(List<object> elements, BoundingBox ROI)
        {
            // Get the geometry of the ref elements
            List<Point> points = new List<Point>();
            foreach (object element in elements)
            {
                IBHoMGeometry geometry = GetGeometry(element);

                if (geometry.GetBounds().IsInRange(ROI))
                {
                    foreach (Point pt in GetControlPoints(geometry))
                        points.Add(pt);
                }
            }

            return points;
        }

        /******************************************/

        internal static List<Point> GetControlPoints(List<object> elements)
        {
            // Get the geometry of the ref elements
            List<Point> points = new List<Point>();
            foreach (object element in elements)
            {
                foreach (Point pt in GetControlPoints(GetGeometry(element)))
                    points.Add(pt);
            }

            return points;
        }

        /******************************************/

        internal static List<Point> GetControlPoints(object element)
        {
            // Get the geometry of the ref elements
            List<Point> points = new List<Point>();

            IBHoMGeometry geometry = GetGeometry(element);

            if (geometry is Point)
            {
                points.Add(geometry as Point);
            }
            else if (geometry is ICurve)
            {
              foreach (Point pt in ((ICurve)geometry).GetControlPoints())
                    points.Add(pt);
            }
            else if (geometry is List<ICurve>)
            {
                //TODO: There are a lot of instances where a list of curves are passed through 'join' probably to re-order the curve list to be sequential. May need to re-add a sequential function
                foreach (ICurve curve in ((List<ICurve>)geometry))
                {
                    foreach (Point pt in curve.GetControlPoints())
                        points.Add(pt);
                }
            }

            return points;
        }

    }
}
