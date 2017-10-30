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

        public static List<double> ComputeSTI(this Room room, List<Speaker> m_speakers, List<double> speech = null, List<double> envNoise = null, List<double> revTimes = null)
        {
            // Defaulting inputs
            List<Frequency> octaves = new List<Frequency> { Frequency.Hz500, Frequency.Hz2000 };
            Dictionary<Frequency, double> gains = new Dictionary<Frequency, double> { { Frequency.Hz500, 1.6 }, { Frequency.Hz2000, 5.3 } }; // To be deleted in the future and moved into speakers input
            if (speech == null) { speech = new List<double> { 85, 85 }; }
            if (envNoise == null) { envNoise = new List<double> { 53.5, 53.5 }; }
            if (revTimes == null) { revTimes = new List<double> { 0.001, 0.001 }; }
            List<Receiver> receivers = room.Samples;

            List<double> timeConstants = new List<double>();
            List<double> revDistances = new List<double>();
            for (int i = 0; i < receivers.Count; i++)
            {
                timeConstants.Add(room.TimeConstant(revTimes[i]));
                revDistances.Add(room.ReverbDistance(revTimes[i]));
            }

            List<Speaker> speakers = new List<Speaker>();
            for (int j = 0; j < m_speakers.Count; j++)  // Setting new gains
                speakers.Add(new Speaker(m_speakers[j].Geometry, m_speakers[j].Direction, m_speakers[j].Category, m_speakers[j].SpeakerID, gains));
            double closestDist = speakers.Select(x => x.Geometry).ToList().ClosestDist(receivers.Select(x => x.Geometry).ToList());

            // Outputs
            List<RASTI> RASTI = new List<RASTI>();
            List<SPL> spl = new List<SPL>();
            List<SNRatio> snRatio = new List<SNRatio>();

            for (int k = 0; k < octaves.Count; k++)
            {
                spl.AddRange(ComputeSPL(room, speakers, revTimes, octaves[k]));
                snRatio(AddRange(SoundNoiseRatio()))
                for (int j = 0; j < speakers.Count; j++)
                {

                    for (int i = 0; i < receivers.Count; i++)
                    {
                        totalSN += CalcSoundToNoiseRatio(envNoise[i], closestDist, amb_pascals, levelSum, revDistances[i], timeConstants[i], octaves[k], gain);
                    }
                }
                STI_oct = ((totalSN / octaves.Count) +15.0) / 30.0;

                STI_OCT[octaves[k]].Add(STI_oct);
                STI.Add(STI_oct);
            }

            for (int i = 0; i < STI_OCT[Frequency.Hz500].Count; i++)
            {
                double rasti = ((4d / 9d) * STI_OCT[Frequency.Hz500][i]) + ((5d / 9d) * STI_OCT[Frequency.Hz500][i]);
                RASTI.Add(rasti);
            }
            return RASTI;
        }
    }
}