using BH.oM.Geometry;
using BH.oM.Structure.Elements;
using BH.Engine.Geometry;
using System;
using System.Linq;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Vector Normal(this Bar bar)
        {

            Point p1 = bar.StartNode.Position;
            Point p2 = bar.EndNode.Position;

            Vector normal;

            if (!IsVertical(p1, p2))
                normal = Vector.ZAxis;
            else
                normal = Vector.YAxis;


            Vector tan = (p2 - p1).Normalise();

            normal = (normal - tan.DotProduct(normal) * tan).Normalise();

            return normal.Rotate(bar.OrientationAngle, tan);
        }

        /***************************************************/

        public static Vector Normal(this PanelPlanar panel)
        {
            return panel.AllEdgeCurves().SelectMany(x => x.IControlPoints()).ToList().FitPlane().Normal;
        }

        /***************************************************/

        public static Vector Normal(this MeshFace face)
        {
            return face.Nodes.Select(x => x.Position).ToList().FitPlane().Normal;
        }

        /***************************************************/
        /**** Public Methods - Interface methods        ****/
        /***************************************************/

        public static Vector INormal(this IAreaElement areaElement)
        {
            return Normal(areaElement as dynamic);
        }

        /***************************************************/

    }
}