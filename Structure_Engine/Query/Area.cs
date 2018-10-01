using BH.Engine.Geometry;
using BH.oM.Geometry;
using BH.oM.Structure.Elements;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static double Area(this PanelPlanar panel)
        {
            List<PolyCurve> externalEdges = panel.ExternalEdgeCurves().IJoin();
            List<PolyCurve> internalEdges = panel.InternalEdgeCurves().IJoin();

            return externalEdges.Select(x => x.Area()).Sum() - internalEdges.Select(x => x.Area()).Sum();
        }

        /***************************************************/

        public static double Area(this FEMesh mesh)
        {
            return mesh.Geometry().Area();
        }

        /***************************************************/

        public static double Area(this MeshFace face)
        {
            return face.Geometry().Area();
        }

        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static double IArea(this IAreaElement element)
        {
            return Area(element as dynamic);
        }

        /***************************************************/
    }

}
