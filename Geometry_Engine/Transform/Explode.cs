using BH.oM.Geometry;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Geometry
{
    public static partial class Transform
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static IList GetExploded(this IBHoMGeometry geometry)
        {
            return _GetExploded(geometry as dynamic);
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static IList _GetExploded(this IBHoMGeometry geometry)
        {
            return new List<IBHoMGeometry> { geometry };
        }

        /***************************************************/

        private static List<Line> _GetExploded(this Polyline curve)
        {
            List<Line> result = new List<Line>();

            List<Point> pts = curve.ControlPoints;

            for (int i = 1; i < pts.Count; i++)
                result.Add(new Line(pts[i - 1], pts[i]));

            return result;
        }

        /***************************************************/

        private static IList _GetExploded(this PolyCurve curve)
        {
            List<PolyCurve> exploded = new List<PolyCurve>();
            List<ICurve> curves = curve.Curves;

            for (int i = 0; i < curves.Count; i++)
                exploded.AddRange(curves[i].GetExploded() as List<PolyCurve>);

            return exploded;
        }

        /***************************************************/

        private static IList _GetExploded(this PolySurface surface)
        {
            List<PolySurface> exploded = new List<PolySurface>();
            List<ISurface> surfaces = surface.Surfaces;

            for (int i = 0; i < surfaces.Count; i++)
                exploded.AddRange(surfaces[i].GetExploded() as List<PolySurface>);

            return exploded;
        }

        /***************************************************/

        private static IList _GetExploded(this GeometryGroup group)
        {
            List<GeometryGroup> exploded = new List<GeometryGroup>();
            List<IBHoMGeometry> elements = group.Elements;

            for (int i = 0; i < elements.Count; i++)
                exploded.AddRange(elements[i].GetExploded() as List<GeometryGroup>);

            return exploded;
        }
    }
}
