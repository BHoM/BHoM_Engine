using System;
using BH.oM.Graphics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
