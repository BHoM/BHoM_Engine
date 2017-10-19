using BH.oM.Geometry;
using BH.Engine.Geometry;
using BH.oM.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.SVG
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static BoundingBox GetBounds(this SVGObject svg)
        {
            return Geometry.Query.IGetBounds(svg.Geometry);
        }

        public static BoundingBox GetBounds(this List<SVGObject> svg)
        {
            BoundingBox box = new BoundingBox();
            for (int i = 0; i < svg.Count; i++)
                box += svg[i].GetBounds();

            return box;
        }

        public static BoundingBox GetBounds(this SVGDocument svg)
        {
            return svg.Canvas;
        }

        public static BoundingBox GetBounds(this List<SVGDocument> svg)
        {
            BoundingBox box = new BoundingBox();
            for (int i = 0; i < svg.Count; i++)
                box += svg[i].GetBounds();

            return box;
        }
    }
}
