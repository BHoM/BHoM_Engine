using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<ICurve> GetExternalEdges(this ISurface surface)
        {
            return _GetExternalEdges(surface as dynamic);
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static List<ICurve> _GetExternalEdges(this Extrusion surface)
        {
            ICurve curve = surface.Curve;
            Vector direction = surface.Direction;

            List<ICurve> edges = new List<ICurve>();
            if (!surface.Capped)
            {
                edges.Add(curve);
                ICurve other = curve.GetClone() as ICurve;
                edges.Add(other.GetTranslated(direction) as ICurve);
            }
            if (!curve.IsClosed())
            {
                Point start = curve.GetStartPoint();
                Point end = curve.GetEndPoint();
                edges.Add(new Line(start, start + direction));
                edges.Add(new Line(end, end + direction));
            }

            return edges;
        }

        /***************************************************/

        private static List<ICurve> _GetExternalEdges(this Loft surface)
        {
            return surface.Curves; //TODO: Is that always correct?
        }

        /***************************************************/

        private static List<ICurve> _GetExternalEdges(this NurbSurface surface)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        private static List<ICurve> _GetExternalEdges(this Pipe surface)
        {
            if (!surface.Capped)
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

        private static List<ICurve> _GetExternalEdges(this PolySurface surface)
        {
            return surface.Surfaces.SelectMany(x => x.GetExternalEdges()).ToList();
        }
    }
}
