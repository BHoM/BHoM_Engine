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

        public static List<SNRatio> SoundNoise(this Room room, List<Speaker> speakers, List<double> revTimes, double envNoise)
        {
            List<Frequency> frequencies = new List<Frequency>() { Frequency.Hz500, Frequency.Hz2000 };
            Dictionary<Frequency, double> modFactors = new Dictionary<Frequency, double> { { Frequency.Hz500, 8.0   }, { Frequency.Hz2000, 1.4  } };
            Dictionary<Frequency, double> gains = new Dictionary<Frequency, double> { { Frequency.Hz500, 1.6 }, { Frequency.Hz2000, 5.3 } };
            double reciever_q = 1.5;

            List<Receiver> receivers = room.Samples;
            double closestDist = ClosestDist(speakers.Select(x => x.Geometry), receivers.Select(x => x.Geometry));
            List<double> timeConstants = revTimes.Select(rt => Query.TimeConstant(rt)).ToList();
            List<double> revDistances = revTimes.Select(rt => Query.ReverbDistance(room, rt)).ToList();

            List<SNRatio> apparentSN = new List<SNRatio>();
            foreach (Frequency f in frequencies)
            {
                for (int i = 0; i < receivers.Count; i++)
                {
                    double soundLevel = receivers[i].DirectSound(room, speakers, revTimes[i], f).Value;
                    double i_n = Math.Pow(10, (envNoise - soundLevel) / 10);
                    double FT = 2.0 * Math.PI * timeConstants[i] * modFactors[f];
                    double sqrdRevDist = revDistances[i] * revDistances[i];
                    double sqrdClosDist = closestDist * closestDist;
                    double cap_a = ((gains[f] * reciever_q) / sqrdClosDist) + ((1.0 / sqrdRevDist) / (1.0 + Math.Pow(FT, 2.0)));
                    double cap_b = (FT / sqrdRevDist) / (1.0 + Math.Pow(FT, 2.0));
                    double cap_c = ((gains[f] * reciever_q) / sqrdClosDist) + (1.0 / sqrdRevDist) + (gains[f] * i_n);
                    double modulationF = Math.Sqrt(cap_a * cap_a + cap_b * cap_b) / cap_c;

                    //double modulationF = ModulationFunction();
                    double appSoundNoise = 10.0 * Math.Log10(modulationF / (1.0 - modulationF));
                    appSoundNoise = appSoundNoise > 15 ? 15 : (appSoundNoise < -15 ? -15 : appSoundNoise);
                    apparentSN.Add(new SNRatio(appSoundNoise, receivers[i].ReceiverID, -1));
                }
            }
            return apparentSN;
        }
    }
}
