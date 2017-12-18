using BH.oM.Acoustic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Acoustic
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public  Methods                           ****/
        /***************************************************/

        public static double RoomConstant(this Room room, double sabineTime)
        {
            if (sabineTime < 0.01) { return Double.PositiveInfinity; }

            double volume = room.Volume;
            double roomAbsorbtion = (Constants.SabineTimeCoefficient * volume / sabineTime) - (4.0 * 2.6 * volume / 1000);  // It would be good to clarify all those constants
            double avgAbsorbtionCoeff = roomAbsorbtion / room.Area;
            return roomAbsorbtion / (1 - avgAbsorbtionCoeff);
        }
    }
}
