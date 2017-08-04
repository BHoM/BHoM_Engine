using BH.oM.Geometry;
using System;
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

        public static List<IBHoMGeometry> GetExploded(this IBHoMGeometry geometry)
        {
            return _GetExploded(geometry as dynamic);
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static List<IBHoMGeometry> _GetExploded(this IBHoMGeometry geometry)
        {
            return new List<IBHoMGeometry> { geometry };
        }

        /***************************************************/

        private static List<IBHoMGeometry> _GetExploded(this Polyline curve)
        {
            List<IBHoMGeometry> result = new List<IBHoMGeometry>();

            List<Point> pts = curve.ControlPoints;

            for (int i = 1; i < pts.Count; i++)
                result.Add(new Line(pts[i - 1], pts[i]));

            return result;
        }

        /***************************************************/

        private static List<IBHoMGeometry> _GetExploded(this PolyCurve curve)
        {
            List<IBHoMGeometry> exploded = new List<IBHoMGeometry>();
            List<ICurve> curves = curve.Curves;

            for (int i = 0; i < curves.Count; i++)
                exploded.AddRange(curves[i].GetExploded());

            return exploded;
        }

        /***************************************************/

        private static List<IBHoMGeometry> _GetExploded(this PolySurface surface)
        {
            List<IBHoMGeometry> exploded = new List<IBHoMGeometry>();
            List<ISurface> surfaces = surface.Surfaces;

            for (int i = 0; i < surfaces.Count; i++)
                exploded.AddRange(surfaces[i].GetExploded());

            return exploded;
        }
    }
}
