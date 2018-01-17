using BH.oM.Base;
using BH.oM.Geometry;

namespace BH.Engine.Base
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static IObject ISetGeometry(this IObject obj, IBHoMGeometry geometry)
        {
            return SetGeometry(obj as dynamic, geometry);
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static IObject SetGeometry(this IObject obj, IBHoMGeometry geometry)
        {
            return obj;
        }

        /***************************************************/
    }
}
