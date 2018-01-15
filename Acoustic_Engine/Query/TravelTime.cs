using BH.oM.Acoustic;

namespace BH.Engine.Acoustic
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public  Methods                           ****/
        /***************************************************/

        public static double TravelTime(this Ray ray)
        {
            return ray.Length() / Constants.SpeedOfSound;
        }

        /***************************************************/
    }
}
