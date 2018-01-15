using BH.oM.Acoustic;

namespace BH.Engine.Acoustic
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public  Methods                           ****/
        /***************************************************/

        public static double SabineTime(this Room room, double roomAbsorbtion)
        {
            return Constants.SabineTimeCoefficient * room.Volume  / roomAbsorbtion;
        }

        /***************************************************/
    }
}
