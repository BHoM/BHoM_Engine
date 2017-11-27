using BH.oM.Geometry;
using BH.Engine.Geometry;
using BH.oM.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            List<IBHoMGeometry> geometry = svg.Geometry;

            for (int i = 0; i < svg.Geometry.Count; i++)
                bb += Geometry.Query.IGetBounds(svg.Geometry[i]);

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
