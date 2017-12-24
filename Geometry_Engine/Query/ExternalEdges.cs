using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

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
                ICurve other = curve.IGetClone();
                edges.Add(other.IGetTranslated(direction));
            }
            if (!curve.IIsClosed())
            {
                Point start = curve.IGetStartPoint();
                Point end = curve.IGetEndPoint();
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
                    new Circle(curve.IGetStartPoint(), curve.IGetStartDir(), surface.Radius),
                    new Circle(curve.IGetEndPoint(), curve.IGetEndDir(), surface.Radius)
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
            return surface.Surfaces.SelectMany(x => x.IGetExternalEdges()).ToList();
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static List<ICurve> IGetExternalEdges(this ISurface surface)
        {
            return GetExternalEdges(surface as dynamic);
        }
    }
}
