using BH.oM.Base;
using BH.oM.Geometry;

namespace BH.Engine.Base
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static IBHoMObject ISetGeometry(this IBHoMObject obj, IBHoMGeometry geometry)
        {
            return SetGeometry(obj as dynamic, geometry);
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static IBHoMObject SetGeometry(this IBHoMObject obj, IBHoMGeometry geometry)
        {
            return obj;
        }

        /***************************************************/
    }
}
