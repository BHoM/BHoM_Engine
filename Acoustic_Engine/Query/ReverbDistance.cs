using BH.oM.Acoustic;
using System;

namespace BH.Engine.Acoustic
{
    public static  partial class Query
    {
        /***************************************************/
        /**** Public  Methods                           ****/
        /***************************************************/

        public static double ReverbDistance(this Room room, double revTime)
        {
            double q = 0.0032; // directivity factor of the source
            return Math.Sqrt(q * room.Volume / revTime); // using approximated Sabine's reverberation formula
        }

        /***************************************************/
    }
}
