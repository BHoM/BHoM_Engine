using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Geometry;
using BH.oM.Acoustic;
using BH.Engine.Geometry;

namespace BH.Engine.Acoustic
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public  Methods                           ****/
        /***************************************************/

        public static Rasti Rasti(this Receiver receiver, List<Speaker> speakers, Room room, double sabineTime, double envNoise)
        {
            SNRatio apparentSn500 = receiver.SignalToNoise(speakers, room, sabineTime, envNoise, Frequency.Hz500);
            SNRatio apparentSn2000 = receiver.SignalToNoise(speakers, room, sabineTime, envNoise, Frequency.Hz2000);
            double rasti500 = (apparentSn500.Value + 15) / 30;
            double rasti2000 = (apparentSn2000.Value + 15) / 30;
            return new Rasti((4d / 9d) * rasti500 + (5d / 9d) * rasti2000, receiver.ReceiverID);
        }

    }
}