using BH.oM.Geometry;
using BH.oM.Graphics;
using System.Collections.Generic;

namespace BH.Engine.Graphics
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static BoundingBox GetBounds(SVGObject svg)
        {
            BoundingBox bb = new BoundingBox();
            List<IBHoMGeometry> geometry = svg.Shapes;

            for (int i = 0; i < svg.Shapes.Count; i++)
                bb += Engine.Geometry.Query.IGetBounds(svg.Shapes[i]);

            return bb;
        }

        /***************************************************/

        public static BoundingBox GetBounds(List<SVGObject> svg)
        {
            BoundingBox bb = new BoundingBox();

            for (int i = 0; i < svg.Count; i++)
                bb += GetBounds(svg[i]);

            return bb;
        }

        /***************************************************/

        public static BoundingBox GetBounds(SVGDocument svg)
        {
            return svg.Canvas;
        }

        /***************************************************/

        public static BoundingBox GetBounds(List<SVGDocument> svg)
        {
            BoundingBox bb = new BoundingBox();

            for (int i = 0; i < svg.Count; i++)
                bb += GetBounds(svg[i]);

            return bb;
        }
    }
}
