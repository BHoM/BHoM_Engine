using BH.oM.Acoustic;

namespace BH.Engine.Acoustic
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public  Methods                           ****/
        /***************************************************/

        public static int Order(this Ray ray)
        {
            return ray.PanelsID.Count;
        }

        /***************************************************/
    }
}
