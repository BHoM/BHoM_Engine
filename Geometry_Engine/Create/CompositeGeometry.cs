using BH.oM.Geometry;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static CompositeGeometry CompositeGeometry(IEnumerable<IBHoMGeometry> elements)
        {
            return new CompositeGeometry { Elements = elements.ToList() };
        }

        /***************************************************/
    }
}
