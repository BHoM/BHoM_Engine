using BH.oM.Graphics;
using System.Collections.Generic;

namespace BH.Engine.Graphics
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static SVGDocument SVGDocument(this List<SVGObject> svg)
        {
            return new SVGDocument(svg, Query.GetBounds(svg));
        }
    }
}
