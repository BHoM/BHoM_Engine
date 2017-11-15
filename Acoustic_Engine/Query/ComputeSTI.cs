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

        public static List<RASTI> ComputeSTI(this Room room, List<Speaker> m_speakers, List<double> speech = null, List<double> envNoise = null, List<double> revTimes = null)
        {
            // Defaulting inputs
            List<Frequency> octaves = new List<Frequency> { Frequency.Hz500, Frequency.Hz2000 };
            Dictionary<Frequency, double> gains = new Dictionary<Frequency, double> { { Frequency.Hz500, 1.6 }, { Frequency.Hz2000, 5.3 } };
            if (speech == null) { speech = new List<double> { 85, 85 }; }
            if (envNoise == null) { envNoise = new List<double> { 53.5, 53.5 }; }
            if (revTimes == null) { revTimes = new List<double> { 0.001, 0.001 }; }

            List<Receiver> receivers = room.Samples;
            List<double> timeConstants = revTimes.Select(x => Query.TimeConstant(x)).ToList();
            List<double> revDistances = revTimes.Select(x => room.ReverbDistance(x)).ToList();
            List<Speaker> speakers = m_speakers.Select(x=> new Speaker(x.Geometry, x.Direction, x.Category, x.SpeakerID, gains)).ToList();
            double closestDist = speakers.Select(x => x.Geometry).ToList().ClosestDist(receivers.Select(x => x.Geometry).ToList());

            // Outputs
            List<RASTI> rasti = new List<RASTI>();
            List<SPL> spl = new List<SPL>();
            List<SNRatio> snRatio = new List<SNRatio>();

            for (int k = 0; k < octaves.Count; k++)
            {
                spl.AddRange(ComputeSPL(room, speakers, revTimes, octaves[k]));
                for (int j = 0; j < speakers.Count; j++)
                    for (int i = 0; i < receivers.Count; i++)
                        totalSN += CalcSoundToNoiseRatio(envNoise[i], closestDist, amb_pascals, levelSum, revDistances[i], timeConstants[i], octaves[k], gain);
                STI_oct = ((totalSN / octaves.Count) +15.0) / 30.0;

                STI_OCT[octaves[k]].Add(STI_oct);
                STI.Add(STI_oct);
            }

            for (int i = 0; i < STI_OCT[Frequency.Hz500].Count; i++)
                rasti.Add(((4d / 9d) * STI_OCT[Frequency.Hz500][i]) + ((5d / 9d) * STI_OCT[Frequency.Hz500][i]));

            return rasti;
        }
    }
}