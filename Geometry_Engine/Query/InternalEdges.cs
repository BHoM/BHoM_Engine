using BH.oM.Geometry;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods - Surfaces                 ****/
        /***************************************************/

        public static List<ICurve> InternalEdges(this Extrusion surface)
        {
            ICurve curve = surface.Curve;
            Vector direction = surface.Direction;

            List<ICurve> edges = new List<ICurve>();
            if (surface.Capped)
            {
                edges.Add(curve.IClone());
                edges.Add(surface.Curve.ITranslate(surface.Direction));
            }

            if (curve.IIsClosed())
            {
                Point start = curve.IStartPoint();
                edges.Add(new Line { Start = start, End = start + direction });
            }

            if (curve.IIsClosed())
            {
                Point end = curve.IEndPoint();
                edges.Add(new Line { Start = end, End = end + direction });
            }

            return edges;
        }

        /***************************************************/

        public static List<ICurve> InternalEdges(this Pipe surface)
        {
            if (surface.Capped)
            {
                ICurve curve = surface.Centreline;
                return new List<ICurve>()
                {
                    new Circle { Centre = curve.IStartPoint(), Normal = curve.IStartDir(), Radius = surface.Radius },
                    new Circle { Centre = curve.IEndPoint(), Normal = curve.IEndDir(), Radius = surface.Radius }
                };
            }
            else
            {
                return new List<ICurve>();
            }
        }

        /***************************************************/

        public static List<ICurve> InternalEdges(this PolySurface surface)
        {
            return surface.Surfaces.SelectMany(x => x.IInternalEdges()).ToList();
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static List<ICurve> IInternalEdges(this ISurface surface)
        {
            return InternalEdges(surface as dynamic);
        }

        /***************************************************/
    }
}
