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
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        public static List<Line> GetExploded(this Polyline curve)
        {
            List<Line> result = new List<Line>();

            List<Point> pts = curve.ControlPoints;

            for (int i = 1; i < pts.Count; i++)
                result.Add(new Line(pts[i - 1], pts[i]));

            return result;
        }

        /***************************************************/

        public static List<ICurve> GetExploded(this PolyCurve curve)
        {
            List<ICurve> exploded = new List<ICurve>();
            List<ICurve> curves = curve.Curves;

            for (int i = 0; i < curves.Count; i++)
                exploded.AddRange(curves[i]._GetExploded());

            return exploded;
        }

        /***************************************************/
        /**** Public Methods - Surfaces                 ****/
        /***************************************************/

        public static List<ISurface> GetExploded(this PolySurface surface)
        {
            List<ISurface> exploded = new List<ISurface>();
            List<ISurface> surfaces = surface.Surfaces;

            for (int i = 0; i < surfaces.Count; i++)
                exploded.AddRange(surfaces[i]._GetExploded());

            return exploded;
        }


        /***************************************************/
        /**** Public Methods - Others                   ****/
        /***************************************************/

        public static List<IBHoMGeometry> GetExploded(this CompositeGeometry group)
        {
            List<IBHoMGeometry> exploded = new List<IBHoMGeometry>();
            List<IBHoMGeometry> elements = group.Elements;

            for (int i = 0; i < elements.Count; i++)
                exploded.AddRange(elements[i]._GetExploded());

            return exploded;
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static List<IBHoMGeometry> _GetExploded(this IBHoMGeometry geometry)
        {
            return GetExploded(geometry as dynamic);
        }

        /***************************************************/

        public static List<ICurve> _GetExploded(this ICurve geometry)
        {
            return GetExploded(geometry as dynamic);
        }

        /***************************************************/

        public static List<ISurface> _GetExploded(this ISurface geometry)
        {
            return GetExploded(geometry as dynamic);
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static List<IBHoMGeometry> GetExploded(this IBHoMGeometry geometry)
        {
            return new List<IBHoMGeometry> { geometry };
        }
    }
}
