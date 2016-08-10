using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BHoM.Geometry;

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
            GeometryBase geometry = null;
            if (element is BHoM.Base.BHoMObject)
                geometry = ((BHoM.Base.BHoMObject)element).GetGeometry();
            else if (element is GeometryBase)
                geometry = element as GeometryBase;

            GeometryBase output = null;
            if (geometry is Line)
            {
                output = Util.HorizontalExtend((Line)geometry, dist);
            }
            else if (geometry is Curve)
            {
                output = Util.HorizontalExtend((Curve)geometry, dist);
            }
            else if (geometry is Group<Curve>)
            {
                output = Util.HorizontalExtend((Group<Curve>)geometry, dist);
            }

            // Prepare the result
            object result = element;
            if (element is BHoM.Base.BHoMObject)
            {
                result = (BHoM.Base.BHoMObject)((BHoM.Base.BHoMObject)element).ShallowClone();
                ((BHoM.Base.BHoMObject)result).SetGeometry(output);
            }
            else if (element is GeometryBase)
            {
                result = output;
            }

            // Return the result
            return result;

        }

        /*************************************/

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

        /*************************************/

        public static Line HorizontalExtend(Line line, double dist)
        {
            Vector dir = line.Direction;
            dir.Z = 0;
            dir.Unitize();
            dir = dist * dir;

            return new Line(line.StartPoint - dir, line.EndPoint + dir);
        }

        /*************************************/

        public static Group<Curve> HorizontalExtend(Group<Curve> group, double dist)
        {
            Group<Curve> newGroup = new Group<Curve>();
            foreach ( Curve curve in Curve.Join(group) )
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
                if (element is BHoM.Base.BHoMObject && Util.IsInside(((BHoM.Base.BHoMObject)element).GetGeometry(), boxes))
                    insiders.Add(element);
                else if (element is GeometryBase && Util.IsInside((GeometryBase)element, boxes))
                    insiders.Add(element);
                else
                    outsiders.Add(element);
            }

            return insiders;
        }

        /*************************************/

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

        /*************************************/

        public static List<BHoM.Base.BHoMObject> FilterByBoundingBox(List<BHoM.Base.BHoMObject> elements, List<BoundingBox> boxes, out List<BHoM.Base.BHoMObject> outsiders)
        {
            List<BHoM.Base.BHoMObject> insiders = new List<BHoM.Base.BHoMObject>();
            outsiders = new List<BHoM.Base.BHoMObject>();
            foreach (BHoM.Base.BHoMObject element in elements)
            {
                if (IsInside(element.GetGeometry(), boxes))
                    insiders.Add(element);
                else
                    outsiders.Add(element);
            }

            return insiders;
        }

        /*************************************/

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

        /*************************************/
        /****  Remove Small Contours      ****/
        /*************************************/

        public static object RemoveSmallContours(object element, double maxLength, out Group<Curve> removed)
        {
            // Get the geometry
            GeometryBase geometry = null;
            if (element is BHoM.Base.BHoMObject)
                geometry = ((BHoM.Base.BHoMObject)element).GetGeometry();
            else if (element is GeometryBase)
                geometry = element as GeometryBase;

            removed = new Group<Curve>();
            GeometryBase output = null;
            if (geometry is Curve)
            {
                Group<Curve> group = new Group<Curve>();
                group.Add((Curve)geometry);
                output = Util.RemoveSmallContours(group, maxLength, out removed);
            }
            else if (geometry is Group<Curve>)
            {
                output = Util.RemoveSmallContours((Group<Curve>)geometry, maxLength, out removed);
            }

            // Prepare the result
            object result = element;
            if (element is BHoM.Base.BHoMObject)
            {
                result = (BHoM.Base.BHoMObject)((BHoM.Base.BHoMObject)element).ShallowClone();
                ((BHoM.Base.BHoMObject)result).SetGeometry(output);
            }
            else if (element is GeometryBase)
            {
                result = output;
            }

            // Return the result
            return result;

        }

        /*************************************/

        public static Group<Curve> RemoveSmallContours(Group<Curve> contours, double maxLength, out Group<Curve> removed)
        {
            Group<Curve> remaining = new Group<Curve>();
            removed = new Group<Curve>();

            foreach (Curve contour in contours)
            {
                if (contour.Length < maxLength)
                    removed.Add(contour);
                else
                    remaining.Add(contour);
            }

            return remaining;
        }


        /*************************************/
        /****  Get Near Contours          ****/
        /*************************************/

        public static List<Curve> GetNearContours(Curve refContour, List<Curve> contours, double tolerance)
        {
            BoundingBox ROI = refContour.Bounds().Inflate(tolerance);

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


        /*************************************/
        /****  Geometry accessors         ****/
        /*************************************/

        internal static GeometryBase GetGeometry(object element)
        {
            GeometryBase geometry = null;
            if (element is BHoM.Base.BHoMObject)
                geometry = ((BHoM.Base.BHoMObject)element).GetGeometry();
            else if (element is GeometryBase)
                geometry = element as GeometryBase;
            return geometry;
        }

        /******************************************/

        internal static object SetGeometry(object element, GeometryBase geometry)
        {
            object result = element;
            if (element is BHoM.Base.BHoMObject)
            {
                result = (BHoM.Base.BHoMObject)((BHoM.Base.BHoMObject)element).ShallowClone();
                ((BHoM.Base.BHoMObject)result).SetGeometry(geometry);
            }
            else if (element is GeometryBase)
            {
                result = geometry;
            }

            return result;
        }

        /******************************************/

        internal static List<Curve> GetGeometries(List<object> elements)
        {
            // Get the geometry of the ref elements
            List<Curve> geometries = new List<Curve>();
            foreach (object element in elements)
            {
                GeometryBase geometry = GetGeometry(element);

                if (geometry is Curve)
                    geometries.Add((Curve)geometry);
                else if (geometry is Group<Curve>)
                {
                    List<Curve> list = Curve.Join((Group<Curve>)geometry);
                    geometries.Add(list[0]);
                }
            }

            return geometries;
        }

        /******************************************/

        internal static List<Curve> GetGeometries(List<object> elements, BoundingBox ROI)
        {
            // Get the geometry of the ref elements
            List<Curve> geometries = new List<Curve>();
            foreach (object element in elements)
            {
                GeometryBase geometry = GetGeometry(element);

                if (BoundingBox.InRange(ROI, geometry.Bounds()))
                {
                    if (geometry is Curve)
                        geometries.Add((Curve)geometry);
                    else if (geometry is Group<Curve>)
                    {
                        foreach (Curve curve in Curve.Join((Group<Curve>)geometry))
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
                GeometryBase geometry = GetGeometry(element);

                if (BoundingBox.InRange(ROI, geometry.Bounds()))
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

            GeometryBase geometry = GetGeometry(element);

            if (geometry is Point)
            {
                points.Add(geometry as Point);
            }
            else if (geometry is Curve)
            {
                foreach (Point pt in ((Curve)geometry).ControlPoints)
                    points.Add(pt);
            }
            else if (geometry is Group<Curve>)
            {
                foreach (Curve curve in Curve.Join((Group<Curve>)geometry))
                {
                    foreach (Point pt in curve.ControlPoints)
                        points.Add(pt);
                }
            }

            return points;
        }

    }
}
