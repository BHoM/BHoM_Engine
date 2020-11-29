using BH.Engine.Geometry;
using BH.oM.Geometry;
using BH.oM.Graphics.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Graphics
{
    public static partial class Create
    {
        public static List<ICurve> IArrowMarker(this IMarker marker, Point end, Vector direction)
        {
            return ArrowMarker(marker as dynamic, end , direction);
        }

        public static List<ICurve> ArrowMarker(this BasicArrowMarker marker, Point end, Vector direction)
        {
            direction.Normalise();
            Vector back = direction.Reverse() * marker.HeadLength;
            Vector perp = back.CrossProduct(Vector.ZAxis);
            if (perp.Length() == 0)
                perp = back.CrossProduct(Vector.YAxis);
            perp = perp.Normalise();

            perp = perp * marker.BaseWidth;

            Point p1 = end + (back + perp);
            Point p2 = end + (back - perp);

            List<ICurve> head = new List<ICurve>();
            head.Add(Geometry.Create.Line(p1, end));
            head.Add(Geometry.Create.Line(p2, end));

            if (marker.Closed)
                head.Add(Geometry.Create.Line(p1, p2));

            return head;
        }
        public static List<ICurve> ArrowMarker(this IMarker markerr, Point end, Vector direction)
        {
            return new List<ICurve>();
        }
    }
}
