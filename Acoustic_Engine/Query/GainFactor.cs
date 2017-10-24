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

        public static double GetGainFactor(this Speaker speaker, double angle, Frequency octave)
        {
            double gains = speaker.Gains[octave];
            switch (octave)
            {
                case Frequency.Hz63:
                    return (-2 * angle / 90 - 8);
                case Frequency.Hz125:
                    return (-2 * angle / 90 - 8);
                case Frequency.Hz250:
                    return (-2 * angle / 90 - 8);
                case Frequency.Hz500:
                    return (-2 * angle / 90 - 8);
                case Frequency.Hz1000:
                    return (-2 * angle / 90 - 8);
                case Frequency.Hz2000:
                    return (-18 * angle / 150 - 2);
                case Frequency.Hz4000:
                    return (-18 * angle / 150 - 2);
                case Frequency.Hz8000:
                    return (-18 * angle / 150 - 2);
                case Frequency.Hz16000:
                    return (-18 * angle / 150 - 2);
                default:
                    return 0;
            }
        }
    }
}
