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

        public static List<ICurve> GetExternalEdges(this ISurface surface)
        {
            return _GetExternalEdges(surface as dynamic);
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        public static List<ICurve> _GetExternalEdges(this Extrusion surface)
        {
            return new List<ICurve>
            {
                surface.Curve.GetClone() as ICurve,
                surface.Curve.GetTranslated(surface.Direction) as ICurve
            };
        }

        /***************************************************/

        public static List<ICurve> _GetExternalEdges(this Loft surface)
        {
            return surface.Curves; //TODO: Is that always correct?
        }

        /***************************************************/

        public static List<ICurve> _GetExternalEdges(this NurbSurface surface)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        public static List<ICurve> _GetExternalEdges(this Pipe surface)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        public static List<ICurve> _GetExternalEdges(this PolySurface surface)
        {
            return surface.Surfaces.SelectMany(x => x.GetExternalEdges()).ToList();
        }
    }
}
