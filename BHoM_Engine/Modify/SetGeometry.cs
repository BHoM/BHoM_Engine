using BH.oM.Base;
using BH.oM.Geometry;

namespace BH.Engine.Base
{
    public static partial class Modify
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

        /***************************************************/
    }
}
