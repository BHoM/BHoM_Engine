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

        public static SVGObject SVGObject(List<IBHoMGeometry> shapes, SVGStyle style = null)
        {
            return new SVGObject { Shapes = shapes, Style = style };
        }

        /***************************************************/
    }
}
