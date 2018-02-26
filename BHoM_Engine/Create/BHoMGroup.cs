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

        public static BHoMGroup<BHoMObject> BHoMGroup(IEnumerable<BHoMObject> elements)
        {
            return new BHoMGroup<BHoMObject>
            {
                Elements = elements.ToList()
            };
        }

        /***************************************************/

        public static BHoMGroup<T> BHoMGroup<T>(IEnumerable<T> elements) where T:IBHoMObject
        {
            return new BHoMGroup<T>
            {
                Elements = elements.ToList()
            };
        }

        /***************************************************/
    }
}
