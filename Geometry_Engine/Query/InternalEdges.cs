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

        public static List<ICurve> GetInternalEdges(this Extrusion surface)
        {
            ICurve curve = surface.Curve;
            Vector direction = surface.Direction;

            List<ICurve> edges = new List<ICurve>();
            if (surface.Capped)
            {
                edges.Add(curve.IGetClone());
                edges.Add(surface.Curve.IGetTranslated(surface.Direction));
            }

            if (curve.IIsClosed())
            {
                Point start = curve.IGetStartPoint();
                edges.Add(new Line(start, start + direction));
            }

            if (curve.IIsClosed())
            {
                Point end = curve.IGetEndPoint();
                edges.Add(new Line(end, end + direction));
            }

            return edges;
        }

        /***************************************************/

        public static List<ICurve> GetInternalEdges(this Pipe surface)
        {
            if (surface.Capped)
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

        public static List<ICurve> GetInternalEdges(this PolySurface surface)
        {
            return surface.Surfaces.SelectMany(x => x.IGetInternalEdges()).ToList();
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static List<ICurve> IGetInternalEdges(this ISurface surface)
        {
            return GetInternalEdges(surface as dynamic);
        }


    }
}
