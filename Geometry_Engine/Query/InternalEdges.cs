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

        public static List<ICurve> GetInternalEdges(this Extrusion surface)
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
                Point end = curve.IGetEndPoint();
                edges.Add(new Line { Start = end, End = end + direction });
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
                    new Circle { Centre = curve.IStartPoint(), Normal = curve.IStartDir(), Radius = surface.Radius },
                    new Circle { Centre = curve.IGetEndPoint(), Normal = curve.IGetEndDir(), Radius = surface.Radius }
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
