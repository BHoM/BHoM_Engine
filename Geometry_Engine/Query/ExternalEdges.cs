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
        /**** Public Methods - Surfaces                 ****/
        /***************************************************/

        public static List<ICurve> GetExternalEdges(this Extrusion surface)
        {
            ICurve curve = surface.Curve;
            Vector direction = surface.Direction;

            List<ICurve> edges = new List<ICurve>();
            if (!surface.Capped)
            {
                edges.Add(curve);
                ICurve other = curve._GetClone();
                edges.Add(other._GetTranslated(direction));
            }
            if (!curve._IsClosed())
            {
                Point start = curve._GetStartPoint();
                Point end = curve._GetEndPoint();
                edges.Add(new Line(start, start + direction));
                edges.Add(new Line(end, end + direction));
            }

            return edges;
        }

        /***************************************************/

        public static List<ICurve> GetExternalEdges(this Loft surface)
        {
            return surface.Curves; //TODO: Is that always correct?
        }

        /***************************************************/

        public static List<ICurve> GetExternalEdges(this NurbSurface surface)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        public static List<ICurve> GetExternalEdges(this Pipe surface)
        {
            if (!surface.Capped)
            {
                ICurve curve = surface.Centreline;
                return new List<ICurve>()
                {
                    new Circle(curve._GetStartPoint(), curve._GetStartDir(), surface.Radius),
                    new Circle(curve._GetEndPoint(), curve._GetEndDir(), surface.Radius)
                };
            }
            else
            {
                return new List<ICurve>();
            }
        }

        /***************************************************/

        public static List<ICurve> GetExternalEdges(this PolySurface surface)
        {
            return surface.Surfaces.SelectMany(x => x._GetExternalEdges()).ToList();
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static List<ICurve> _GetExternalEdges(this ISurface surface)
        {
            return GetExternalEdges(surface as dynamic);
        }
    }
}
