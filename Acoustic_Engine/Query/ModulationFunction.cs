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

        public static double ModulationFunction(this Receiver receiver, List<Speaker> speakers, Room room, double revTime, double envNoise, Frequency f)
        {
            Dictionary<Frequency, double> gains = speakers.Select(x => x.Gains).First();
            Dictionary<Frequency, double> modFactors = new Dictionary<Frequency, double> { { Frequency.Hz500, 8.0 }, { Frequency.Hz2000, 1.4 } };
            double receiverDirectivity = 1.5;

            double revDistance = Query.ReverbDistance(room, revTime);
            double timeConstant = Query.TimeConstant(revTime);
            double closestDist = Engine.Geometry.Query.ClosestDistance(speakers.Select(x => x.Location), room.Samples.Select(x => x.Location));

            double soundLevel = receiver.DirectSound(speakers, room, revTime, f).Value;
            double totalNoise = Query.SoundLevelsSum(new List<double>(){envNoise, soundLevel});
            double FT = 2.0 * Math.PI * timeConstant * modFactors[f];
            double sqrdRevDist = revDistance * revDistance;
            double sqrdClosDist = closestDist * closestDist;
            double cap_a = ((gains[f] * receiverDirectivity) / sqrdClosDist) + ((1.0 / sqrdRevDist) / (1.0 + Math.Pow(FT, 2.0)));
            double cap_b = (FT / sqrdRevDist) / (1.0 + Math.Pow(FT, 2.0));
            double cap_c = ((gains[f] * receiverDirectivity) / sqrdClosDist) + (1.0 / sqrdRevDist) + (gains[f] * totalNoise);
            return Math.Sqrt(cap_a * cap_a + cap_b * cap_b) / cap_c;
        }
    }
}
