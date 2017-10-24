using BH.oM.Base;
using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Base
{
    public static partial class Transform
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static BHoMObject ISetGeometry(this BHoMObject obj, IBHoMGeometry geometry)
        {
            return SetGeometry(obj as dynamic, geometry);
        }

        /***************************************************/

        private static BHoMObject SetGeometry(this BHoMObject obj, IBHoMGeometry geometry)
        {
            return obj;
        }


    }
}
