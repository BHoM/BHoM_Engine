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

        public static List<SNRatio> SoundNoise(this Room room, List<Speaker> speakers, List<double> revTimes, double envNoise, List<Frequency> frequencies)
        {

            List<Receiver> receivers = room.Samples;
            List<double> timeConstants = revTimes.Select(rt => Query.TimeConstant(rt)).ToList();
            List<double> revDistances = revTimes.Select(rt => Query.ReverbDistance(room, rt)).ToList();

            List<SNRatio> apparentSN = new List<SNRatio>();
            foreach (Frequency f in frequencies)
            {
                for (int i = 0; i < receivers.Count; i++)
                {
                    double modulationF = receivers[i].ModulationFunction(speakers, room, revTimes[i], envNoise, f);
                    double appSoundNoise = 10.0 * Math.Log10(modulationF / (1.0 - modulationF));
                    appSoundNoise = appSoundNoise > 15 ? 15 : (appSoundNoise < -15 ? -15 : appSoundNoise); // SNRatio always clipped at [-15, +15]
                    apparentSN.Add(new SNRatio(appSoundNoise, receivers[i].ReceiverID, -1, f));
                }
            }
            return apparentSN;
        }
    }
}
