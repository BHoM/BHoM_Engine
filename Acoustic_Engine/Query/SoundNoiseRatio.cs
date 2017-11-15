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
                { Frequency.Hz63, 1.0    },
                { Frequency.Hz125, 2.0   },
                { Frequency.Hz250, 4.0   },
                { Frequency.Hz500, 8.0   },
                { Frequency.Hz1000, 0.7  },
                { Frequency.Hz2000, 1.4  },
                { Frequency.Hz4000, 2.8  },
                { Frequency.Hz8000, 5.6  },
                { Frequency.Hz8000, 11.2 },
            };

            if (amb_pascals > 0)
                noise = 10 * Math.Log10(Math.Pow(10, noise / 10) + Math.Pow(10, (speech + 10 * Math.Log10(amb_pascals)) / 10));

            double i_n = Math.Pow(10, (noise - levelSum) / 10);

            double reciever_q = 1.5;

            double cap_a = ((gain * reciever_q) / (closestDist * closestDist)) + ((1.0 / (revDist * revDist)) / (1.0 + Math.Pow(2.0 * Math.PI * timeConstant * frequencies[octave], 2.0)));
            double cap_b = ((2.0 * Math.PI * timeConstant * frequencies[octave]) / (revDist * revDist)) / (1.0 + Math.Pow(2.0 * Math.PI * timeConstant * frequencies[octave], 2.0));
            double cap_c = ((gain * reciever_q) / (closestDist * closestDist)) + (1.0 / (revDist * revDist)) + (gain * i_n);

            double modulation = Math.Sqrt(cap_a * cap_a + cap_b * cap_b) / cap_c;

            double apparentSoundNoise = 10.0 * Math.Log10(modulation / (1.0 - modulation));
            apparentSoundNoise = apparentSoundNoise > 15 ? 15 : (apparentSoundNoise < -15 ? -15 : apparentSoundNoise);

            int freqCount = 2;
            return ((apparentSoundNoise / freqCount) + 15.0) / 30.0;
        }

        public static double SNRatio(double sound, double noise)
        {
            return 10 * Math.Log10(Math.Pow(10, noise / 10) + Math.Pow(10, (sound + 10 * Math.Log10(noise)) / 10));
        }
    }
}
