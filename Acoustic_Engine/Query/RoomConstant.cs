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
        public static double GetRoomConstant(this Room room, double revTime)
        {
            if (revTime < 0.01) { return Double.PositiveInfinity; }

            double volume = room.Volume;
            double sAlpha = (0.163 * volume / revTime) - (4.0 * 2.6 * volume / 1000);  // It would be good to clarify all those constants
            double alpha = sAlpha / room.Area;
            return sAlpha / (1 - alpha);
        }
    }
}
