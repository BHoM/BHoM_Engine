using BH.oM.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
