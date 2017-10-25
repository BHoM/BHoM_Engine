using BH.oM.Base;
using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Base
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static IBHoMGeometry IGetGeometry(this BHoMObject obj)
        {
            return GetGeometry(obj as dynamic);
        }

        /***************************************************/

        private static IBHoMGeometry GetGeometry(this BHoMObject obj)
        {
            return null;
        }
    }
}
