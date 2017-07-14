using BH.oM.Base;
using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Base
{
    public static partial class Geometry
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static IBHoMGeometry GetGeometry(this BHoMObject obj)
        {
            return _GetGeometry(obj as dynamic);
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        public static IBHoMGeometry _GetGeometry(this BHoMObject obj)
        {
            return null;
        }
    }
}
