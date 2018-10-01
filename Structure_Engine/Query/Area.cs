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

            if (face.Nodes.Count == 3)
            {
                Point pA = face.Nodes[0].Position;
                Point pB = face.Nodes[1].Position;
                Point pC = face.Nodes[2].Position;
                Vector ab = pB - pA;
                Vector ac = pC - pA;
                return ab.CrossProduct(ac).Length() / 2;
            }
            else if (face.Nodes.Count == 4)
            {
                Point pA = face.Nodes[0].Position;
                Point pB = face.Nodes[1].Position;
                Point pC = face.Nodes[2].Position;
                Point pD = face.Nodes[3].Position;
                Vector ab = pB - pA;
                Vector ac = pC - pA;

                Vector cb = pB - pC;
                Vector cd = pD - pC;
                return (ab.CrossProduct(ac).Length() + cb.CrossProduct(cd).Length()) / 2;
            }
            else
            {
                Reflection.Compute.RecordError("Can only calculate area for triangles and quads");
                return 0;
            }
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
