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

        public static double Gain(this Speaker speaker, double angle, Frequency frequency)
        {
            double gains = speaker.Gains[frequency];
            switch (frequency)
            {
                case Frequency.Hz63:
                    return gains * (-2 * angle / 90 - 8);
                case Frequency.Hz125:
                    return gains * (-2 * angle / 90 - 8);
                case Frequency.Hz250:
                    return gains * (-2 * angle / 90 - 8);
                case Frequency.Hz500:
                    return gains * (-2 * angle / 90 - 8);
                case Frequency.Hz1000:
                    return gains * (-2 * angle / 90 - 8);
                case Frequency.Hz2000:
                    return gains * (-18 * angle / 150 - 2);
                case Frequency.Hz4000:
                    return gains * (-18 * angle / 150 - 2);
                case Frequency.Hz8000:
                    return gains * (-18 * angle / 150 - 2);
                case Frequency.Hz16000:
                    return gains * (-18 * angle / 150 - 2);
                default:
                    return 0;
            }
        }
    }
}
