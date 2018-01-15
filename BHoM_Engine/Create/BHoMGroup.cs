using BH.oM.Base;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Base
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static BHoMGroup BHoMGroup(IEnumerable<BHoMObject> elements)
        {
            return new BHoMGroup
            {
                Elements = elements.ToList()
            };
        }

        /***************************************************/
    }
}
