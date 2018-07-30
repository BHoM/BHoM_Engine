using BH.oM.Geometry;
using BH.oM.Reflection.Attributes;
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

        public static List<ICurve> ExternalEdges(this Extrusion surface)
        {
            ICurve curve = surface.Curve;
            Vector direction = surface.Direction;

            List<ICurve> edges = new List<ICurve>();
            if (!surface.Capped)
            {
                edges.Add(curve);
                ICurve other = curve.IClone();
                edges.Add(other.ITranslate(direction));
            }
            if (!curve.IIsClosed())
            {
                Point start = curve.IStartPoint();
                Point end = curve.IEndPoint();
                edges.Add(new Line { Start = start, End = start + direction });
                edges.Add(new Line { Start = end, End = end + direction });
            }

            return edges;
        }

        /***************************************************/

        public static List<ICurve> ExternalEdges(this Loft surface)
        {
            return surface.Curves; //TODO: Is that always correct?
        }

        /***************************************************/

        [NotImplemented]
        public static List<ICurve> ExternalEdges(this NurbSurface surface)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        public static List<ICurve> ExternalEdges(this Pipe surface)
        {
            if (!surface.Capped)
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

        public static List<ICurve> ExternalEdges(this PolySurface surface)
        {
            return surface.Surfaces.SelectMany(x => x.IExternalEdges()).ToList();
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static List<ICurve> IExternalEdges(this ISurface surface)
        {
            return ExternalEdges(surface as dynamic);
        }
    }
}
