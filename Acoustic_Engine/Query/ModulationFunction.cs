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
        public static ModulationFunction(this Room room, Receiver receiver, List<Speaker> speakers, double revTime, Frequency frequency)
        {
            Dictionary<Frequency, double> gains = new Dictionary<Frequency, double> { { Frequency.Hz500, 1.6 }, { Frequency.Hz2000, 5.3 } };


            double revDistance =

            double soundLevel = receiver.DirectSound(room, speakers, revTime, frequency).Value;
            double i_n = Math.Pow(10, (envNoise - soundLevel) / 10);
            double FT = 2.0 * Math.PI * timeConstants[i] * modFactor;
            double sqrdRevDist = revDistances[i] * revDistance;
            double sqrdClosDist = closestDist * closestDist;
            double cap_a = ((gains[f] * reciever_q) / sqrdClosDist) + ((1.0 / sqrdRevDist) / (1.0 + Math.Pow(FT, 2.0)));
            double cap_b = (FT / sqrdRevDist) / (1.0 + Math.Pow(FT, 2.0));
            double cap_c = ((gains[f] * reciever_q) / sqrdClosDist) + (1.0 / sqrdRevDist) + (gains[f] * i_n);
            double modulationF = Math.Sqrt(cap_a * cap_a + cap_b * cap_b) / cap_c;
        }
    }
}
