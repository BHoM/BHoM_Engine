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

        public static List<ICurve> _GetInternalEdges(this ISurface surface)
        {
            return new List<ICurve>();
        }

        /***************************************************/

        public static List<ICurve> _GetInternalEdges(this PolySurface surface)
        {
            return surface.Surfaces.SelectMany(x => x.GetInternalEdges()).ToList();
        }
    }
}
