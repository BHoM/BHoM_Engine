using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Geometry
{
    public static partial class Measure
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<ICurve> GetInternalEdges(this ISurface surface)
        {
            return _GetInternalEdges(surface as dynamic);
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static List<ICurve> _GetInternalEdges(this ISurface surface)
        {
            return new List<ICurve>();
        }

        /***************************************************/

        private static List<ICurve> _GetInternalEdges(this Extrusion surface)
        {
            ICurve curve = surface.Curve;
            Vector direction = surface.Direction;

            List<ICurve> edges = new List<ICurve>();
            if (surface.Capped)
            {
                edges.Add(curve.GetClone() as ICurve);
                edges.Add(surface.Curve.GetTranslated(surface.Direction) as ICurve);
            }

            if (curve.IsClosed())
            {
                Point start = curve.GetStartPoint();
                edges.Add(new Line(start, start + direction));
            }

            if (curve.IsClosed())
            {
                Point end = curve.GetEndPoint();
                edges.Add(new Line(end, end + direction));
            }

            return edges;
        }

        /***************************************************/

        private static List<ICurve> _GetInternalEdges(this Pipe surface)
        {
            if (surface.Capped)
            {
                ICurve curve = surface.Centreline;
                return new List<ICurve>()
                {
                    new Circle(curve.GetStartPoint(), curve.GetStartDir(), surface.Radius),
                    new Circle(curve.GetEndPoint(), curve.GetEndDir(), surface.Radius)
                };
            }
            else
            {
                return new List<ICurve>();
            }
        }

        /***************************************************/

        private static List<ICurve> _GetInternalEdges(this PolySurface surface)
        {
            return surface.Surfaces.SelectMany(x => x.GetInternalEdges()).ToList();
        }
    }
}
