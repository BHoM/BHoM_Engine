using BH.oM.Geometry;
using BH.oM.Graphics;
using System.Collections.Generic;

namespace BH.Engine.Graphics
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static SVGDocument SVGDocument(this List<SVGObject> svgObjects)
        {
            return new SVGDocument { SVGObjects = svgObjects, Canvas = Query.Bounds(svgObjects) };
        }

        /***************************************************/

        public static SVGDocument SVGDocument(List<SVGObject> svgObjects, BoundingBox canvas)
        {
            return new SVGDocument { SVGObjects = svgObjects, Canvas = canvas };
        }

        /***************************************************/
    }
}
