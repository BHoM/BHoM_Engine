using BH.Engine.Geometry;
using BH.oM.Geometry;
using BH.oM.Graphics.Components;
using BH.oM.Reflection.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Graphics
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/
        [Description("Create a marker object.")]
        [Input("end", "Point where the marker is attached.")]
        [Input("direction", "Vector representing the direction of the marker.")]
        [Output("marker curves", "COllection of curves to represent the marker.")]
        public static List<ICurve> IMarker(this IMarker marker, Point end, Vector direction)
        {
            return Marker(marker as dynamic, end , direction);
        }

        /***************************************************/

        [Description("Create a basic arrow marker object.")]
        [Input("end", "Point where the marker is attached.")]
        [Input("direction", "Vector representing the direction of the marker.")]
        [Output("marker curves", "COllection of curves to represent the marker.")]
        public static List<ICurve> Marker(this BasicArrowMarker marker, Point end, Vector direction)
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

        /***************************************************/

        public static List<ICurve> Marker(this IMarker markerr, Point end, Vector direction)
        {
            return new List<ICurve>();
        }

        /***************************************************/
        /****           Private Methods                 ****/
        /***************************************************/
    }
}
