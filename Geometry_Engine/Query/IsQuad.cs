using BH.oM.Geometry;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static bool IsQuad(this Face face)
        {
            return face.D != -1;
        }

        /***************************************************/
    }
}
