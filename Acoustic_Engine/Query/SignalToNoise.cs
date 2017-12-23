using BH.Engine.Geometry;
using BH.oM.Acoustic;
using BH.oM.Geometry;
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

        public static SnRatio SignalToNoise(this Receiver receiver, List<Speaker> speakers, Room room, double sabineTime, double envNoise, Frequency f)
        {
            Dictionary<Frequency, double> gains = speakers.Select(x => x.Gains).First();
            Dictionary<Frequency, double> modFactors = new Dictionary<Frequency, double> { { Frequency.Hz500, 8.0 }, { Frequency.Hz2000, 1.4 } };
            double receiverDirectivity = 1.5;

            double revDistance = Query.ReverbDistance(room, sabineTime);
            double timeConstant = Query.TimeConstant(sabineTime);
            double closestDist = Engine.Geometry.Query.ClosestDistance(speakers.Select(x => x.Location), room.Samples.Select(x => x.Location));

            double soundLevel = receiver.DirectSound(speakers, room, sabineTime, f).Value;
            double FT = 2.0 * Math.PI * timeConstant * modFactors[f];
            double sqrdRevDist = revDistance * revDistance;
            double sqrdClosDist = closestDist * closestDist;
            double totalNoise = Math.Pow(10, (envNoise - soundLevel) / 10);

            double cap_a = ((gains[f] * receiverDirectivity) / sqrdClosDist) + ((1.0 / sqrdRevDist) / (1.0 + Math.Pow(FT, 2.0)));
            double cap_b = (FT / sqrdRevDist) / (1.0 + Math.Pow(FT, 2.0));
            double cap_c = ((gains[f] * receiverDirectivity) / sqrdClosDist) + (1.0 / sqrdRevDist) + (gains[f] * totalNoise);
            double modulationF = Math.Sqrt(cap_a * cap_a + cap_b * cap_b) / cap_c;

            double appSoundNoise = 10.0 * Math.Log10(modulationF / (1.0 - modulationF));
            appSoundNoise = appSoundNoise > 15 ? 15 : (appSoundNoise < -15 ? -15 : appSoundNoise);
            return Create.SnRatio(appSoundNoise, receiver.ReceiverID, -1, f);
        }

        /***************************************************/
    }
}
