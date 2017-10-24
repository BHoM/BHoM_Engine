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
        public static double SoundNoiseRatio(double noise, double speech, double closestDist, double amb_pascals, double levelSum, double revDist, double timeConstant, Frequency octave, double gain)
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
    }
}
    
