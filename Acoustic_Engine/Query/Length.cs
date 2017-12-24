using BH.oM.Acoustic;

namespace BH.Engine.Acoustic
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public  Methods                           ****/
        /***************************************************/

        public static double Length(this Ray ray)
        {
            return Engine.Geometry.Query.GetLength(ray.Path);
        }

        /***************************************************/
    }
}
