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

        public static double GetGain(this Speaker speaker, double frequency, double octave)
        {
            return (frequency < 5) ? speaker.Gains[0] : speaker.Gains[1];
        }

        /***************************************************/

        public static double GetGainAngleFactor(this Speaker speaker, double angle, double octave) // take out octave
        {
            return (octave < 1000) ? (-2 * angle / 90 - 8) : (-18 * angle / 150 - 2);
        }
    }
}
