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
        public static List<double> ComputeSTI(this Room room, List<Speaker> _speakers, List<double> speech, List<double> envNoise, List<double> revTime)
        {
            List<Frequency> octaves = new List<Frequency> { Frequency.Hz500, Frequency.Hz2000 };
            //speech = new List<double> { 85, 85 };     //Defaulting speech variable if not instanciated
            //envNoise = new List<double> { 53.5, 53.5 };    //Defaulting noise variable if not instanciated
            //RT = RT == null ? new List<double> { 0.001, 0.001 } : RT;           //Defaulting RT variable if not instanciated

            List<Speaker> speakers = new List<Speaker>();
            for (int i = 0; i < _speakers.Count; i++)
                speakers.Add(new Speaker(_speakers[i].Geometry, _speakers[i].Direction, _speakers[i].Category, _speakers[i].SpeakerID, _speakers[i].Gains));
            List<Receiver> receivers = room.Samples;

            List<double> STI = new List<double>();
            List<double> RASTI = new List<double>();
            Dictionary<Frequency, List<double>> STI_OCT = new Dictionary<Frequency, List<double>>();


            double closestDist = speakers.Select(x => x.Geometry).ToList().GetClosestDist(receivers.Select(x => x.Geometry).ToList());

            for (int k = 0; k < octaves.Count; k++)
            {
                double STI_oct = 0;
                double totalSN = 0;
                for (int j = 0; j < speakers.Count; j++)
                {
                    for (int i = 0; i < receivers.Count; i++)
                    {
                        double timeConstant = room.GetTimeConstant(revTime[i]);
                        double revDistance = room.GetReverbDistance(revTime[i]);

                        double totalLevel = 0;
                        double amb_pascals = 0;
                        double levelSum = 0;

                        double gain = 0;

                        {
                            totalLevel += CalcSoundLevel(receivers[i], speakers[i], room, speech[i], revTime[i], octaves[k]).Value; // TODO : Acoustic -S witch to Room without receivers as argument
                            amb_pascals = totalLevel;
                            levelSum = (speech.Count == 2 ? speech[0] : speech[i]) + 10 * Math.Log10(totalLevel);
                            gain =
                        }
                        totalSN += CalcSoundToNoiseRatio(envNoise[i], speech[i], closestDist, amb_pascals, levelSum, revDistance, timeConstant, octaves[k], gain);
                    }
                }
                STI_oct = CalcSTI(totalSN);

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


        private static SPL CalcSoundLevel(this Receiver receiver, Speaker speaker, Room room, double speech, double revTime, Frequency octave)
        {
            Vector deltaPos = receiver.Geometry - speaker.Geometry;
            double recieverAngle = deltaPos.GetAngle(speaker.Direction) * (180 / Math.PI);
            double distance = deltaPos.GetLength();
            double revDist = room.GetReverbDistance(revTime);

            if (distance < revDist) { return new SPL() { Value = 0, ReceiverID = receiver.ReceiverID, Octave = octave }; }

            double orientationFactor = speaker.GetGain(recieverAngle, octave);
            Dictionary<Frequency, double> gains = new Dictionary<Frequency, double> { { Frequency.Hz500, 1.6 }, { Frequency.Hz2000, 5.3 } };
            speaker = new Speaker(speaker.Geometry, speaker.Direction, speaker.Category, speaker.SpeakerID, gains);

            double gain = speaker.GetGain(recieverAngle, octave) * Math.Pow(10, orientationFactor / 10);

            double roomConstant = room.GetRoomConstant(revTime);

            double level = (gain / (4.0 * Math.PI * distance * distance)) + (4.0 / roomConstant);
            return new SPL(level, receiver.ReceiverID, octave);
        }


        private static double CalcSoundToNoiseRatio(double noise, double speech, double closestDist, double amb_pascals, double levelSum, double revDist, double timeConstant, Frequency octave, double gain)
        {
            Dictionary<Frequency, double> frequencies = new Dictionary<Frequency, double> {
                { Octaves.Hz63, 1.0    },
                { Octaves.Hz125, 2.0   },
                { Octaves.Hz250, 4.0   },
                { Octaves.Hz500, 8.0   },
                { Octaves.Hz1000, 0.7  },
                { Octaves.Hz2000, 1.4  },
                { Octaves.Hz4000, 2.8  },
                { Octaves.Hz8000, 5.6  },
                { Octaves.Hz8000, 11.2 },
            };

            double amb_noise = noise;

            if (amb_pascals > 0)
            {
                double amb_sum = speech + 10 * Math.Log10(amb_pascals);
                amb_noise = 10 * Math.Log10(Math.Pow(10, noise / 10) + Math.Pow(10, amb_sum / 10));
            }

            double i_n = Math.Pow(10, (amb_noise - levelSum) / 10);

            double reciever_q = 1.5; // What is reciever_q?

            double cap_a = ((gain * reciever_q) / (closestDist * closestDist)) + ((1.0 / (revDist * revDist)) / (1.0 + Math.Pow(2.0 * Math.PI * timeConstant * frequencies[octave], 2.0)));
            double cap_b = ((2.0 * Math.PI * timeConstant * frequencies[octave]) / (revDist * revDist)) / (1.0 + Math.Pow(2.0 * Math.PI * timeConstant * frequencies[octave], 2.0));
            double cap_c = ((gain * reciever_q) / (closestDist * closestDist)) + (1.0 / (revDist * revDist)) + (gain * i_n);


            double m_f1 = Math.Sqrt(cap_a * cap_a + cap_b * cap_b) / cap_c;

            double m_f = m_f1; // ask Mathew H.

            int freqCount = 2;
            double snApp = 10.0 * Math.Log10(m_f / (1.0 - m_f));

            if (snApp > 15.0)
            {
                snApp = 15.0;
            }
            if (snApp < -15.0)
            {
                snApp = -15.0;
            }
            return ((snApp / freqCount) + 15.0) / 30.0;
        }

        private static double CalcSTI(this double totalSN) // double highest_level & double amb_noise not included yet + levelSum
        {
            int freqCount = 2;
            return ((totalSN / freqCount) + 15.0) / 30.0;
        }
    }
}